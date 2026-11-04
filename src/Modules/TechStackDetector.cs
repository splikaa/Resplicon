using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using OsintHq.Engine;

namespace OsintHq.Modules
{
    public class TechItem
    {
        public string Name { get; set; }
        public string Category { get; set; } // Web Server, CMS, Frontend, Backend, CDN, Analytics, Security
        public string Evidence { get; set; }
        public string Confidence { get; set; }

        public TechItem(string name, string cat, string evidence, string conf = "HIGH (Verified Signature)")
        {
            Name = name;
            Category = cat;
            Evidence = evidence;
            Confidence = conf;
        }
    }

    public class TechStackProfile
    {
        public string TargetDomain { get; set; }
        public string FinalUrl { get; set; }
        public int HttpStatusCode { get; set; }
        public List<TechItem> Technologies { get; set; }
        public Dictionary<string, string> RawHeaders { get; set; }

        public TechStackProfile()
        {
            Technologies = new List<TechItem>();
            RawHeaders = new Dictionary<string, string>();
        }
    }

    public static class TechStackDetector
    {
        static TechStackDetector()
        {
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }
            catch { }
        }
        public static TechStackProfile AnalyzeDomainTechStack(string domain)
        {
            if (string.IsNullOrEmpty(domain)) domain = "google.com";
            domain = domain.Replace("https://", "").Replace("http://", "").TrimEnd('/');

            var profile = new TechStackProfile();
            profile.TargetDomain = domain;

            string targetUrl = "https://" + domain;
            string html = "";
            WebHeaderCollection headers = null;

            // Fetch headers and HTML
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(targetUrl);
                req.Timeout = 5000;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
                req.AllowAutoRedirect = true;
                req.MaximumAutomaticRedirections = 4;

                using (var resp = (HttpWebResponse)req.GetResponse())
                {
                    profile.HttpStatusCode = (int)resp.StatusCode;
                    profile.FinalUrl = resp.ResponseUri.ToString();
                    headers = resp.Headers;

                    using (var stream = resp.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        html = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException wex)
            {
                var hr = wex.Response as HttpWebResponse;
                if (hr != null)
                {
                    profile.HttpStatusCode = (int)hr.StatusCode;
                    profile.FinalUrl = hr.ResponseUri.ToString();
                    headers = hr.Headers;
                    try
                    {
                        using (var reader = new StreamReader(hr.GetResponseStream()))
                        {
                            html = reader.ReadToEnd();
                        }
                    }
                    catch { }
                }
                else
                {
                    // Fallback to HTTP
                    try
                    {
                        var reqHttp = (HttpWebRequest)WebRequest.Create("http://" + domain);
                        reqHttp.Timeout = 4000;
                        reqHttp.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Resplicon-Audit/2.0";
                        using (var respHttp = (HttpWebResponse)reqHttp.GetResponse())
                        {
                            profile.HttpStatusCode = (int)respHttp.StatusCode;
                            profile.FinalUrl = respHttp.ResponseUri.ToString();
                            headers = respHttp.Headers;
                            using (var reader = new StreamReader(respHttp.GetResponseStream()))
                            {
                                html = reader.ReadToEnd();
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }

            // Store raw headers
            if (headers != null)
            {
                foreach (string key in headers.AllKeys)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        profile.RawHeaders[key] = headers[key];
                    }
                }
            }

            string lowerHtml = (html ?? "").ToLower();
            var detected = new Dictionary<string, TechItem>(StringComparer.OrdinalIgnoreCase);

            // 1. INSPECT HEADERS
            if (headers != null)
            {
                string server = headers["Server"] ?? "";
                string xPoweredBy = headers["X-Powered-By"] ?? "";
                string cfRay = headers["CF-RAY"] ?? "";
                string cfCache = headers["cf-cache-status"] ?? "";

                // Cloudflare CDN
                if (!string.IsNullOrEmpty(cfRay) || !string.IsNullOrEmpty(cfCache) || server.IndexOf("cloudflare", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    AddTech(detected, new TechItem("Cloudflare", "CDN & Reverse Proxy", "CF-RAY / cf-cache-status header"));
                    AddTech(detected, new TechItem("Cloudflare WAF", "Security & DDoS Protection", "Cloudflare edge filtering"));
                }

                // Web Servers
                if (server.IndexOf("nginx", StringComparison.OrdinalIgnoreCase) >= 0)
                    AddTech(detected, new TechItem("Nginx", "Web Server", "Server: " + server));
                else if (server.IndexOf("apache", StringComparison.OrdinalIgnoreCase) >= 0)
                    AddTech(detected, new TechItem("Apache HTTP Server", "Web Server", "Server: " + server));
                else if (server.IndexOf("litespeed", StringComparison.OrdinalIgnoreCase) >= 0)
                    AddTech(detected, new TechItem("LiteSpeed Web Server", "Web Server", "Server: " + server));
                else if (server.IndexOf("microsoft-iis", StringComparison.OrdinalIgnoreCase) >= 0)
                    AddTech(detected, new TechItem("Microsoft IIS", "Web Server", "Server: " + server));
                else if (server.IndexOf("caddy", StringComparison.OrdinalIgnoreCase) >= 0)
                    AddTech(detected, new TechItem("Caddy Server", "Web Server", "Server: " + server));
                else if (server.IndexOf("gunicorn", StringComparison.OrdinalIgnoreCase) >= 0)
                    AddTech(detected, new TechItem("Gunicorn WSGI", "Python Application Server", "Server: " + server));
                else if (server.IndexOf("kestrel", StringComparison.OrdinalIgnoreCase) >= 0)
                    AddTech(detected, new TechItem("Kestrel", ".NET Core Web Server", "Server: " + server));
                else if (!string.IsNullOrEmpty(server))
                    AddTech(detected, new TechItem(server, "Web Server", "Server: " + server));

                // Backend Runtimes via Headers
                if (xPoweredBy.IndexOf("php", StringComparison.OrdinalIgnoreCase) >= 0)
                    AddTech(detected, new TechItem("PHP", "Programming Language", "X-Powered-By: " + xPoweredBy));
                else if (xPoweredBy.IndexOf("asp.net", StringComparison.OrdinalIgnoreCase) >= 0)
                    AddTech(detected, new TechItem("ASP.NET", "Web Framework", "X-Powered-By: " + xPoweredBy));
                else if (xPoweredBy.IndexOf("express", StringComparison.OrdinalIgnoreCase) >= 0)
                    AddTech(detected, new TechItem("Express.js", "Node.js Framework", "X-Powered-By: " + xPoweredBy));
                else if (xPoweredBy.IndexOf("next.js", StringComparison.OrdinalIgnoreCase) >= 0)
                    AddTech(detected, new TechItem("Next.js", "React Framework", "X-Powered-By: " + xPoweredBy));

                // Cloud Platforms via Headers
                if (headers["X-Vercel-Id"] != null)
                    AddTech(detected, new TechItem("Vercel", "Cloud Edge Platform", "X-Vercel-Id header"));
                if (headers["X-Netlify-Id"] != null || headers["X-NF-Request-ID"] != null)
                    AddTech(detected, new TechItem("Netlify", "Cloud Hosting Platform", "X-Netlify header"));
                if (headers["X-Amz-Cf-Id"] != null || headers["X-Amz-Id-2"] != null)
                    AddTech(detected, new TechItem("Amazon Web Services (AWS CloudFront / S3)", "Cloud Infrastructure", "X-Amz headers"));
                if (headers["X-Azure-Ref"] != null)
                    AddTech(detected, new TechItem("Microsoft Azure Front Door", "Cloud Platform", "X-Azure-Ref header"));
                if (headers["X-Fastly-Request-ID"] != null)
                    AddTech(detected, new TechItem("Fastly", "CDN & Edge Cloud", "X-Fastly headers"));

                // Security Headers
                if (headers["Strict-Transport-Security"] != null)
                    AddTech(detected, new TechItem("HSTS Policy Active", "Security Configuration", "Strict-Transport-Security"));
                if (headers["Content-Security-Policy"] != null)
                    AddTech(detected, new TechItem("Content Security Policy (CSP)", "Security Configuration", "Content-Security-Policy header"));
            }

            // 2. INSPECT DOM & HTML
            if (!string.IsNullOrEmpty(lowerHtml))
            {
                // CMS Identification
                if (lowerHtml.Contains("wp-content") || lowerHtml.Contains("wp-includes") || lowerHtml.Contains("name=\"generator\" content=\"wordpress"))
                {
                    AddTech(detected, new TechItem("WordPress", "Content Management System (CMS)", "wp-content/ / wp-includes directory structure"));
                    AddTech(detected, new TechItem("PHP", "Programming Language", "WordPress core runtime"));
                }
                if (lowerHtml.Contains("cdn.shopify.com") || lowerHtml.Contains("shopify.theme") || lowerHtml.Contains("window.shopify"))
                {
                    AddTech(detected, new TechItem("Shopify", "E-Commerce & CMS Platform", "cdn.shopify.com assets & Shopify global object"));
                    AddTech(detected, new TechItem("Ruby on Rails", "Backend Architecture", "Shopify platform infrastructure"));
                }
                if (lowerHtml.Contains("drupal.settings") || lowerHtml.Contains("name=\"generator\" content=\"drupal"))
                {
                    AddTech(detected, new TechItem("Drupal", "Content Management System (CMS)", "Drupal.settings DOM object"));
                }
                if (lowerHtml.Contains("wix.com") || lowerHtml.Contains("_wix") || lowerHtml.Contains("static.parastorage.com"))
                {
                    AddTech(detected, new TechItem("Wix", "Website Builder Platform", "Wix asset infrastructure"));
                }
                if (lowerHtml.Contains("squarespace.com") || lowerHtml.Contains("static1.squarespace.com"))
                {
                    AddTech(detected, new TechItem("Squarespace", "Website Builder & CMS", "static.squarespace.com CDN assets"));
                }
                if (lowerHtml.Contains("webflow.com") || lowerHtml.Contains("data-wf-site"))
                {
                    AddTech(detected, new TechItem("Webflow", "Visual CMS Platform", "data-wf-site tag"));
                }
                if (lowerHtml.Contains("ghost-root") || lowerHtml.Contains("name=\"generator\" content=\"ghost"))
                {
                    AddTech(detected, new TechItem("Ghost", "Publishing Platform & Headless CMS", "Ghost generator tag"));
                    AddTech(detected, new TechItem("Node.js", "Runtime Environment", "Ghost Node.js stack"));
                }

                // Frontend Frameworks
                if (lowerHtml.Contains("__next_data__") || lowerHtml.Contains("/_next/static/"))
                {
                    AddTech(detected, new TechItem("Next.js", "React Web Framework (SSR/SSG)", "__NEXT_DATA__ hydration bundle"));
                    AddTech(detected, new TechItem("React", "Frontend JavaScript Library", "Next.js dependency"));
                    AddTech(detected, new TechItem("Node.js", "Runtime Environment", "Next.js runtime"));
                }
                else if (lowerHtml.Contains("react-dom") || lowerHtml.Contains("data-reactroot") || lowerHtml.Contains("_reactrootcontainer") || lowerHtml.Contains("react.production.min.js"))
                {
                    AddTech(detected, new TechItem("React", "Frontend JavaScript Library", "react-dom bundle / data-reactroot"));
                }

                if (lowerHtml.Contains("__nuxt__") || lowerHtml.Contains("/_nuxt/"))
                {
                    AddTech(detected, new TechItem("Nuxt.js", "Vue.js Framework (SSR)", "__NUXT__ hydration bundle"));
                    AddTech(detected, new TechItem("Vue.js", "Frontend Framework", "Nuxt.js dependency"));
                }
                else if (lowerHtml.Contains("vue.min.js") || lowerHtml.Contains("data-v-") || lowerHtml.Contains("v-bind") || lowerHtml.Contains("v-if"))
                {
                    AddTech(detected, new TechItem("Vue.js", "Frontend Framework", "Vue data-v- scope attribute / vue.js bundle"));
                }

                if (lowerHtml.Contains("ng-version") || lowerHtml.Contains("ng-app") || lowerHtml.Contains("angular.min.js"))
                {
                    AddTech(detected, new TechItem("Angular", "Frontend Enterprise Framework", "ng-version / ng-app tag"));
                }

                if (lowerHtml.Contains("svelte-") || lowerHtml.Contains("class=\"svelte-"))
                {
                    AddTech(detected, new TechItem("Svelte", "Compiled UI Framework", "svelte- CSS scoping classes"));
                }

                if (lowerHtml.Contains("jquery.min.js") || lowerHtml.Contains("jquery-"))
                {
                    AddTech(detected, new TechItem("jQuery", "JavaScript Utility Library", "jquery script inclusion"));
                }

                // CSS Frameworks
                if (lowerHtml.Contains("tailwind") || lowerHtml.Contains("class=\"flex flex-") || lowerHtml.Contains("class=\"grid grid-cols-"))
                {
                    AddTech(detected, new TechItem("Tailwind CSS", "Utility-First CSS Framework", "Tailwind utility class patterns"));
                }
                if (lowerHtml.Contains("bootstrap.min.css") || lowerHtml.Contains("class=\"container-fluid") || lowerHtml.Contains("class=\"col-md-"))
                {
                    AddTech(detected, new TechItem("Bootstrap", "CSS Framework", "Bootstrap grid & class hierarchy"));
                }

                // Analytics & Tag Managers
                if (lowerHtml.Contains("googletagmanager.com/gtm.js") || lowerHtml.Contains("gtm-"))
                {
                    AddTech(detected, new TechItem("Google Tag Manager", "Tag Management System", "gtm.js script injection"));
                }
                if (lowerHtml.Contains("google-analytics.com") || lowerHtml.Contains("gtag('config'") || lowerHtml.Contains("ga('create'"))
                {
                    AddTech(detected, new TechItem("Google Analytics (GA4/Universal)", "Web Analytics", "Google Analytics gtag script"));
                }
                if (lowerHtml.Contains("connect.facebook.net/en_us/fbevents.js") || lowerHtml.Contains("fbq('init'"))
                {
                    AddTech(detected, new TechItem("Meta Pixel (Facebook)", "Advertising & Conversion Tracking", "fbevents.js script"));
                }
                if (lowerHtml.Contains("static.hotjar.com"))
                {
                    AddTech(detected, new TechItem("Hotjar", "Heatmap & User Behavior Tracking", "hotjar.com tracker script"));
                }

                // Payments & E-Commerce
                if (lowerHtml.Contains("js.stripe.com"))
                {
                    AddTech(detected, new TechItem("Stripe", "Payment Gateway", "js.stripe.com SDK"));
                }
                if (lowerHtml.Contains("paypal.com/sdk/js"))
                {
                    AddTech(detected, new TechItem("PayPal SDK", "Payment Gateway", "PayPal JS SDK integration"));
                }
                if (lowerHtml.Contains("woocommerce"))
                {
                    AddTech(detected, new TechItem("WooCommerce", "E-Commerce Solution", "WooCommerce stylesheet/classes"));
                }

                // Captcha & Bot Management
                if (lowerHtml.Contains("google.com/recaptcha") || lowerHtml.Contains("grecaptcha"))
                {
                    AddTech(detected, new TechItem("Google reCAPTCHA", "Bot Detection & Proof-of-Work", "recaptcha integration"));
                }
                if (lowerHtml.Contains("challenges.cloudflare.com/turnstile"))
                {
                    AddTech(detected, new TechItem("Cloudflare Turnstile", "Smart Captcha Alternative", "Turnstile challenge script"));
                }
            }

            foreach (var kv in detected)
            {
                profile.Technologies.Add(kv.Value);
            }

            return profile;
        }

        private static void AddTech(Dictionary<string, TechItem> dict, TechItem item)
        {
            if (!dict.ContainsKey(item.Name))
            {
                dict[item.Name] = item;
            }
        }

        public static void DisplayTechStackReport(TechStackProfile p)
        {
            AnsiConsole.SubHeader("TECH STACK FINGERPRINT & ARCHITECTURE EXPOSURE");
            AnsiConsole.KeyValue("Target Domain", p.TargetDomain, ConsoleColor.Cyan, ConsoleColor.White);
            AnsiConsole.KeyValue("Final Responding URI", string.IsNullOrEmpty(p.FinalUrl) ? "https://" + p.TargetDomain : p.FinalUrl, ConsoleColor.Cyan, ConsoleColor.White);
            AnsiConsole.KeyValue("HTTP Response Status", p.HttpStatusCode > 0 ? p.HttpStatusCode + " OK" : "Connection Refused / Filtered", p.HttpStatusCode == 200 ? ConsoleColor.Green : ConsoleColor.Yellow, ConsoleColor.White);
            AnsiConsole.KeyValue("Identified Technologies", p.Technologies.Count.ToString() + " Stack Components Detected", ConsoleColor.Green, ConsoleColor.White);
            Console.WriteLine();

            if (p.Technologies.Count == 0)
            {
                AnsiConsole.WriteLine("  [*] Technology headers stripped or protected behind edge firewall.", ConsoleColor.DarkGray);
                return;
            }

            // Group by category
            var categories = new Dictionary<string, List<TechItem>>();
            foreach (var t in p.Technologies)
            {
                if (!categories.ContainsKey(t.Category)) categories[t.Category] = new List<TechItem>();
                categories[t.Category].Add(t);
            }

            foreach (var cat in categories)
            {
                AnsiConsole.WriteLine("  [" + cat.Key.ToUpper() + "]", ConsoleColor.Yellow);
                foreach (var tech in cat.Value)
                {
                    AnsiConsole.Write("    • ", ConsoleColor.Green);
                    AnsiConsole.Write(tech.Name.PadRight(28), ConsoleColor.White);
                    AnsiConsole.Write("Evidence: ", ConsoleColor.DarkGray);
                    AnsiConsole.WriteLine(tech.Evidence, ConsoleColor.Cyan);
                }
                Console.WriteLine();
            }
        }
    }
}
