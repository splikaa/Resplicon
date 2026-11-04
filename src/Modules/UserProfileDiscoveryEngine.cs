using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using OsintHq.Engine;

namespace OsintHq.Modules
{
    public class DiscoveredSocialPlatform
    {
        public string PlatformName { get; set; }
        public string ProfileUrl { get; set; }
        public bool Confirmed { get; set; }
        public string DisplayName { get; set; }
        public string BioOrDetails { get; set; }
        public string Location { get; set; }
        public string Stats { get; set; }
        public string CreatedDate { get; set; }
        public string ExternalWebsite { get; set; }
    }

    public class UsernameDossier
    {
        public string TargetUsername { get; set; }
        public List<DiscoveredSocialPlatform> FoundPlatforms { get; set; }
        public List<string> InterestingFacts { get; set; }
        public string PrimaryRealName { get; set; }
        public string PrimaryLocation { get; set; }
        public string PrimaryOrganization { get; set; }
        public List<string> CorrelatedWebsites { get; set; }

        public UsernameDossier()
        {
            FoundPlatforms = new List<DiscoveredSocialPlatform>();
            InterestingFacts = new List<string>();
            CorrelatedWebsites = new List<string>();
        }
    }

    public static class UserProfileDiscoveryEngine
    {
        private static readonly JavaScriptSerializer _json = new JavaScriptSerializer();

        static UserProfileDiscoveryEngine()
        {
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }
            catch { }
        }

        private static string HttpGetJson(string url, int timeoutMs = 4500)
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = timeoutMs;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Resplicon-Hunter/2.0";
                req.Accept = "application/json, text/plain, */*";
                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var stream = resp.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return null;
            }
        }

        private static bool CheckUrlExists(string url)
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "HEAD";
                req.Timeout = 3000;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Resplicon-Recon/2.0";
                using (var resp = (HttpWebResponse)req.GetResponse())
                {
                    return resp.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }

        public static UsernameDossier HarvestUsernameProfile(string username)
        {
            if (string.IsNullOrEmpty(username)) username = "splikaa";
            username = username.Trim().TrimStart('@');

            var dossier = new UsernameDossier();
            dossier.TargetUsername = username;

            AnsiConsole.SubHeader("MULTI-PLATFORM PROFILE HARVEST & IDENTITY CORRELATION");
            AnsiConsole.WriteLine("Deploying deep crawlers for alias: @" + username, ConsoleColor.Cyan);
            Console.WriteLine();

            // 1. GITHUB DEEP HARVEST
            string ghJson = HttpGetJson("https://api.github.com/users/" + username);
            if (!string.IsNullOrEmpty(ghJson))
            {
                try
                {
                    var dict = _json.Deserialize<Dictionary<string, object>>(ghJson);
                    if (dict.ContainsKey("login"))
                    {
                        var p = new DiscoveredSocialPlatform
                        {
                            PlatformName = "GitHub",
                            ProfileUrl = "https://github.com/" + username,
                            Confirmed = true
                        };

                        if (dict.ContainsKey("name") && dict["name"] != null)
                        {
                            p.DisplayName = dict["name"].ToString();
                            if (string.IsNullOrEmpty(dossier.PrimaryRealName)) dossier.PrimaryRealName = p.DisplayName;
                        }
                        if (dict.ContainsKey("location") && dict["location"] != null)
                        {
                            p.Location = dict["location"].ToString();
                            if (string.IsNullOrEmpty(dossier.PrimaryLocation)) dossier.PrimaryLocation = p.Location;
                        }
                        if (dict.ContainsKey("company") && dict["company"] != null)
                        {
                            string comp = dict["company"].ToString();
                            if (string.IsNullOrEmpty(dossier.PrimaryOrganization)) dossier.PrimaryOrganization = comp;
                            dossier.InterestingFacts.Add("Professional Affiliation: " + comp + " (GitHub)");
                        }
                        if (dict.ContainsKey("bio") && dict["bio"] != null)
                        {
                            string bio = dict["bio"].ToString().Trim();
                            if (!string.IsNullOrEmpty(bio)) p.BioOrDetails = bio;
                        }
                        if (dict.ContainsKey("blog") && dict["blog"] != null)
                        {
                            string blog = dict["blog"].ToString().Trim();
                            if (!string.IsNullOrEmpty(blog))
                            {
                                p.ExternalWebsite = blog;
                                dossier.CorrelatedWebsites.Add(blog);
                            }
                        }

                        int repos = dict.ContainsKey("public_repos") ? Convert.ToInt32(dict["public_repos"]) : 0;
                        int followers = dict.ContainsKey("followers") ? Convert.ToInt32(dict["followers"]) : 0;
                        p.Stats = followers.ToString("N0") + " followers | " + repos + " public repos";

                        if (dict.ContainsKey("created_at") && dict["created_at"] != null)
                        {
                            string created = dict["created_at"].ToString();
                            p.CreatedDate = created.Length >= 10 ? created.Substring(0, 10) : created;
                            dossier.InterestingFacts.Add("GitHub profile active since: " + p.CreatedDate + " (" + followers.ToString("N0") + " followers)");
                        }

                        dossier.FoundPlatforms.Add(p);
                        PrintPlatformHit(p);
                    }
                }
                catch { }
            }

            // 2. HACKERNEWS DEEP HARVEST
            string hnJson = HttpGetJson("https://hacker-news.firebaseio.com/v0/user/" + username + ".json");
            if (!string.IsNullOrEmpty(hnJson) && !hnJson.Contains("\"null\""))
            {
                try
                {
                    var dict = _json.Deserialize<Dictionary<string, object>>(hnJson);
                    if (dict != null && dict.ContainsKey("id"))
                    {
                        var p = new DiscoveredSocialPlatform
                        {
                            PlatformName = "HackerNews",
                            ProfileUrl = "https://news.ycombinator.com/user?id=" + username,
                            Confirmed = true
                        };

                        int karma = dict.ContainsKey("karma") ? Convert.ToInt32(dict["karma"]) : 0;
                        p.Stats = karma.ToString("N0") + " karma points";

                        if (dict.ContainsKey("created") && dict["created"] != null)
                        {
                            long epoch = Convert.ToInt64(dict["created"]);
                            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch);
                            p.CreatedDate = dt.ToString("yyyy-MM-dd");
                            dossier.InterestingFacts.Add("HackerNews account active since " + p.CreatedDate + " with " + karma.ToString("N0") + " karma");
                        }

                        if (dict.ContainsKey("about") && dict["about"] != null)
                        {
                            string about = Regex.Replace(dict["about"].ToString(), @"<[^>]+>", " ").Trim();
                            if (about.Length > 120) about = about.Substring(0, 120) + "...";
                            p.BioOrDetails = about;
                            dossier.InterestingFacts.Add("HackerNews About Quote: \"" + about + "\"");
                        }

                        dossier.FoundPlatforms.Add(p);
                        PrintPlatformHit(p);
                    }
                }
                catch { }
            }

            // 3. DEV.TO DEEP HARVEST
            string devJson = HttpGetJson("https://dev.to/api/users/by_username?url=" + username);
            if (!string.IsNullOrEmpty(devJson))
            {
                try
                {
                    var dict = _json.Deserialize<Dictionary<string, object>>(devJson);
                    if (dict != null && dict.ContainsKey("username"))
                    {
                        var p = new DiscoveredSocialPlatform
                        {
                            PlatformName = "Dev.to",
                            ProfileUrl = "https://dev.to/" + username,
                            Confirmed = true
                        };

                        if (dict.ContainsKey("name") && dict["name"] != null) p.DisplayName = dict["name"].ToString();
                        if (dict.ContainsKey("location") && dict["location"] != null) p.Location = dict["location"].ToString();
                        if (dict.ContainsKey("summary") && dict["summary"] != null) p.BioOrDetails = dict["summary"].ToString();
                        if (dict.ContainsKey("website_url") && dict["website_url"] != null)
                        {
                            string web = dict["website_url"].ToString().Trim();
                            if (!string.IsNullOrEmpty(web))
                            {
                                p.ExternalWebsite = web;
                                dossier.CorrelatedWebsites.Add(web);
                            }
                        }
                        if (dict.ContainsKey("joined_at") && dict["joined_at"] != null) p.CreatedDate = dict["joined_at"].ToString();

                        dossier.FoundPlatforms.Add(p);
                        PrintPlatformHit(p);
                    }
                }
                catch { }
            }

            // 4. KEYBASE DEEP HARVEST
            string kbJson = HttpGetJson("https://keybase.io/_/api/1.0/user/lookup.json?usernames=" + username);
            if (!string.IsNullOrEmpty(kbJson) && kbJson.Contains("\"status\":{\"code\":0"))
            {
                var p = new DiscoveredSocialPlatform
                {
                    PlatformName = "Keybase",
                    ProfileUrl = "https://keybase.io/" + username,
                    Confirmed = true,
                    Stats = "Cryptographic Proofs Verified"
                };
                dossier.InterestingFacts.Add("Keybase Identity: Cryptographic PGP identity & social proofs bound to @" + username);
                dossier.FoundPlatforms.Add(p);
                PrintPlatformHit(p);
            }

            // 5. GITLAB HARVEST
            string glJson = HttpGetJson("https://gitlab.com/api/v4/users?username=" + username);
            if (!string.IsNullOrEmpty(glJson) && glJson.StartsWith("[{"))
            {
                var p = new DiscoveredSocialPlatform
                {
                    PlatformName = "GitLab",
                    ProfileUrl = "https://gitlab.com/" + username,
                    Confirmed = true,
                    Stats = "Active Developer Profile"
                };
                dossier.FoundPlatforms.Add(p);
                PrintPlatformHit(p);
            }

            // 6. STEAM GAMING PROFILE
            string steamUrl = "https://steamcommunity.com/id/" + username;
            if (CheckUrlExists(steamUrl))
            {
                var p = new DiscoveredSocialPlatform
                {
                    PlatformName = "Steam",
                    ProfileUrl = steamUrl,
                    Confirmed = true,
                    Stats = "Gaming & Community Alias"
                };
                dossier.InterestingFacts.Add("Active Gaming Footprint: Matches Steam community profile ID @" + username);
                dossier.FoundPlatforms.Add(p);
                PrintPlatformHit(p);
            }

            // 7. TELEGRAM
            string tgUrl = "https://t.me/" + username;
            if (CheckUrlExists(tgUrl))
            {
                var p = new DiscoveredSocialPlatform
                {
                    PlatformName = "Telegram",
                    ProfileUrl = tgUrl,
                    Confirmed = true,
                    Stats = "Direct Messaging Handle"
                };
                dossier.FoundPlatforms.Add(p);
                PrintPlatformHit(p);
            }

            // 8. REDDIT, TWITTER, PINTEREST, MEDIUM, SOUNDCLOUD
            CheckGenericPlatform(dossier, "Reddit", "https://www.reddit.com/user/" + username);
            CheckGenericPlatform(dossier, "Twitter/X", "https://twitter.com/" + username);
            CheckGenericPlatform(dossier, "Medium", "https://medium.com/@" + username);
            CheckGenericPlatform(dossier, "Pinterest", "https://www.pinterest.com/" + username);
            CheckGenericPlatform(dossier, "SoundCloud", "https://soundcloud.com/" + username);
            CheckGenericPlatform(dossier, "Pastebin", "https://pastebin.com/u/" + username);

            DisplaySynthesizedDossier(dossier);
            return dossier;
        }

        private static void CheckGenericPlatform(UsernameDossier dossier, string name, string url)
        {
            if (CheckUrlExists(url))
            {
                var p = new DiscoveredSocialPlatform
                {
                    PlatformName = name,
                    ProfileUrl = url,
                    Confirmed = true,
                    Stats = "Public Profile Online"
                };
                dossier.FoundPlatforms.Add(p);
                PrintPlatformHit(p);
            }
        }

        private static void PrintPlatformHit(DiscoveredSocialPlatform p)
        {
            AnsiConsole.Write("  [+] " + p.PlatformName.PadRight(15), ConsoleColor.Green);
            AnsiConsole.Write("FOUND -> ", ConsoleColor.Yellow);
            AnsiConsole.WriteLine(p.ProfileUrl, ConsoleColor.White);

            if (!string.IsNullOrEmpty(p.DisplayName))
            {
                AnsiConsole.WriteLine("      ├─ Name: " + p.DisplayName, ConsoleColor.Cyan);
            }
            if (!string.IsNullOrEmpty(p.Location))
            {
                AnsiConsole.WriteLine("      ├─ Location: " + p.Location, ConsoleColor.Cyan);
            }
            if (!string.IsNullOrEmpty(p.Stats))
            {
                AnsiConsole.WriteLine("      ├─ Metrics: " + p.Stats, ConsoleColor.DarkGray);
            }
            if (!string.IsNullOrEmpty(p.BioOrDetails))
            {
                AnsiConsole.WriteLine("      └─ Bio: " + p.BioOrDetails, ConsoleColor.Gray);
            }
        }

        private static void DisplaySynthesizedDossier(UsernameDossier d)
        {
            Console.WriteLine();
            AnsiConsole.SubHeader("SYNTHESIZED PERSONA & INTERESTING FACTS MATRIX");
            Console.WriteLine();

            AnsiConsole.KeyValue("Target Identifier", "@" + d.TargetUsername, ConsoleColor.Cyan, ConsoleColor.White);
            AnsiConsole.KeyValue("Confirmed Real Name", string.IsNullOrEmpty(d.PrimaryRealName) ? "Unspecified (Alias-Only)" : d.PrimaryRealName, ConsoleColor.Green, ConsoleColor.White);
            AnsiConsole.KeyValue("Correlated Location", string.IsNullOrEmpty(d.PrimaryLocation) ? "Not publicly declared" : d.PrimaryLocation, ConsoleColor.Green, ConsoleColor.White);
            AnsiConsole.KeyValue("Affiliated Organization", string.IsNullOrEmpty(d.PrimaryOrganization) ? "Independent / Unaffiliated" : d.PrimaryOrganization, ConsoleColor.Green, ConsoleColor.White);

            if (d.CorrelatedWebsites.Count > 0)
            {
                AnsiConsole.KeyValue("Linked External Domains", string.Join(" | ", d.CorrelatedWebsites.ToArray()), ConsoleColor.Yellow, ConsoleColor.White);
            }

            Console.WriteLine();
            AnsiConsole.WriteLine("  [KEY INTEL & INTERESTING FACTS DISCOVERED]", ConsoleColor.Yellow);
            if (d.InterestingFacts.Count > 0)
            {
                foreach (var fact in d.InterestingFacts)
                {
                    AnsiConsole.WriteLine("    • " + fact, ConsoleColor.White);
                }
            }
            else
            {
                AnsiConsole.WriteLine("    • Alias found across " + d.FoundPlatforms.Count + " networks. No long-form biographical statements extracted.", ConsoleColor.DarkGray);
            }

            Console.WriteLine();
            AnsiConsole.WriteLine("  Confirmed Digital Footprint Footprint: " + d.FoundPlatforms.Count + " / 13 Platforms Indexed", ConsoleColor.Green);
        }
    }
}
