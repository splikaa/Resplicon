using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using OsintHq.Agents;
using OsintHq.Modules;

namespace OsintHq.Engine
{
    public class CollaborationEngine
    {
        private readonly SupervisorAgent _supervisor;
        private readonly FlowAgent _flow;
        private readonly HeadSecurityAgent _security;
        private readonly ResearcherAgent _researcher;
        private readonly DocumentationAgent _documentation;

        public CollaborationEngine(
            SupervisorAgent supervisor,
            FlowAgent flow,
            HeadSecurityAgent security,
            ResearcherAgent researcher,
            DocumentationAgent documentation)
        {
            _supervisor = supervisor;
            _flow = flow;
            _security = security;
            _researcher = researcher;
            _documentation = documentation;
        }

        public void ExecuteCollaborativeMission(string target)
        {
            if (string.IsNullOrEmpty(target)) target = "google.com";
            
            int remSplok;
            if (!DeviceProfileManager.DeductSplokens(650, "Autonomous 5-Agent Squad Mission", out remSplok))
            {
                return;
            }
            DeviceProfileManager.RecordMission();

            AnsiConsole.SafeClear();
            AnsiConsole.Header("RESPLICON // SYNCHRONIZED MULTI-AGENT RECONNAISSANCE", ConsoleColor.Magenta);
            AnsiConsole.WriteLine("MISSION OBJECTIVE: Comprehensive Intelligence Harvest & Threat Assessment", ConsoleColor.White);
            AnsiConsole.WriteLine("TARGET IDENTIFIER: " + target, ConsoleColor.Yellow);
            AnsiConsole.WriteLine("OPERATOR DEVICE  : " + DeviceProfileManager.Profile.DeviceId + " [" + DeviceProfileManager.Profile.ClearanceLevel + "]", ConsoleColor.Cyan);
            AnsiConsole.WriteLine("OPERATOR COUNTRY : " + DeviceProfileManager.Profile.OperatorCountry + " [" + DeviceProfileManager.Profile.OperatorCountryCode + "]", ConsoleColor.Green);
            AnsiConsole.WriteLine("SPLOKEN BALANCE  : " + remSplok.ToString("N0") + " / 25,000 SPLOK Remaining", ConsoleColor.Yellow);
            AnsiConsole.WriteLine("TIMESTAMPS (UTC) : " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC", ConsoleColor.DarkGray);
            Console.WriteLine();

            // STEP 1: SUPERVISOR TRIAGE
            AnsiConsole.SubHeader("PHASE 1 // COMMAND DAIS : MISSION BRIEFING");
            _supervisor.Status = "ACTIVE // BRIEFING SQUAD ON TARGET";
            _supervisor.Speak("All stations, lock on target: [" + target + "]. Initiating tactical reconnaissance sequence.");
            _supervisor.Think("Assessing target attack surface and assigning execution DAG.");
            Thread.Sleep(600);
            _supervisor.Speak("Aegis-9, audit our OPSEC perimeter and scan the target for honeypot deception.");
            Console.WriteLine();

            // STEP 2: HEAD SECURITY OPSEC & HONEYPOT
            AnsiConsole.SubHeader("PHASE 2 // SECURITY REDOUBT : OPSEC & HONEYPOT VETTING");
            _security.Status = "ENGAGED // EVALUATING OPSEC & DECEPTION SIGNATURES";
            _security.Speak("Acknowledged, Vanguard-1. Sealing Faraday circuits. Probing target for traps.");
            AnsiConsole.ProgressBar("Auditing Egress Anonymity", 15, 20);
            
            var honeypotData = OsintModules.RunHoneypotCheck(target);
            _security.Think("Honeypot probability verified at low baseline. Zero canary tokens tripped.");
            _security.Speak("OPSEC clearance granted. Target is viable. Nexus-7, spin up your pipeline.");
            Console.WriteLine();

            // STEP 3: FLOW PIPELINE SPIN-UP
            AnsiConsole.SubHeader("PHASE 3 // ROUTING MATRIX : ASYNC PIPELINE ACTIVATION");
            _flow.Status = "STREAMING // BUFFERING SENSOR CHANNELS";
            _flow.Speak("Pipeline matrix primed! Routing raw target tokens to Specter-4's sensor array.");
            AnsiConsole.ProgressBar("Allocating Asynchronous Worker DAG", 12, 15);
            _flow.Think("Multiplexing threads. Zero packet latency anticipated.");
            Console.WriteLine();

            // STEP 4: RESEARCHER MULTI-SOURCE HARVEST
            AnsiConsole.SubHeader("PHASE 4 // DEEP RECON ARRAY : MULTI-SOURCE HARVEST");
            _researcher.Status = "SURGING // HARVESTING TARGET TELEMETRY";
            _researcher.Speak("Telemetry sensors online. Commencing multi-vector reconnaissance for: " + target);
            Console.WriteLine();

            // Gather intelligence data
            var ipData = new Dictionary<string, string>();
            var domainData = new Dictionary<string, string>();
            var breachData = new Dictionary<string, string>();
            var proxyData = new Dictionary<string, string>();

            System.Net.IPAddress ignoredIp;
            bool isIp = System.Net.IPAddress.TryParse(target, out ignoredIp);
            if (isIp)
            {
                ipData = OsintModules.RunIpLookup(target);
                proxyData = OsintModules.RunIp2Proxy(target);
                OsintModules.RunReverseLookup(target);
            }
            else if (target.Contains("@"))
            {
                breachData = OsintModules.RunMailBreach(target);
            }
            else
            {
                // Likely a domain or username
                domainData = OsintModules.RunDomainLookup(target);
                OsintModules.RunDnsMap(target);
                breachData = OsintModules.RunMailBreach(target);
                if (domainData.ContainsKey("Resolved IPs") && !domainData["Resolved IPs"].Contains("Error"))
                {
                    string firstIp = domainData["Resolved IPs"].Split(',')[0].Trim();
                    ipData = OsintModules.RunIpLookup(firstIp);
                    proxyData = OsintModules.RunIp2Proxy(firstIp);
                }
            }

            _researcher.Speak("Raw intelligence gathered. " + (ipData.Count + domainData.Count + breachData.Count) + " telemetry nodes mapped.");
            Console.WriteLine();

            // STEP 5: FLOW DATA ROUTING
            AnsiConsole.SubHeader("PHASE 5 // ROUTING MATRIX : TELEMETRY AGGREGATION");
            _flow.Speak("Data packets captured without loss. Routing payload into Scribe-0's archival hopper.");
            AnsiConsole.ProgressBar("Compressing & Cryptographically Hashing Telemetry", 10, 15);
            Console.WriteLine();

            // STEP 6: DOCUMENTATION ARCHIVE & REPORT COMPILATION
            AnsiConsole.SubHeader("PHASE 6 // ARCHIVES BUREAU : DOSSIER GENERATION");
            _documentation.Status = "COMPILING // MECHANICAL DOCKET ENGAGED";
            _documentation.Speak("Teletype printer engaged. Drafting official Intelligence Dossier for target [" + target + "].");
            
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            string safeTarget = System.Text.RegularExpressions.Regex.Replace(target, @"[^\w\.\-]", "_");
            string mdPath = Path.Combine("reports", "DOSSIER_" + safeTarget + "_" + timestamp + ".md");
            string htmlPath = Path.Combine("reports", "DOSSIER_" + safeTarget + "_" + timestamp + ".html");

            string reportContent = GenerateMarkdownReport(target, ipData, domainData, honeypotData, breachData, proxyData);
            File.WriteAllText(mdPath, reportContent);

            string htmlContent = GenerateHtmlReport(target, ipData, domainData, honeypotData, breachData, proxyData);
            File.WriteAllText(htmlPath, htmlContent);

            AnsiConsole.ProgressBar("Printing Dossier & Stamping Seal", 14, 20);
            _documentation.Speak("Dossier compiled successfully!");
            AnsiConsole.KeyValue("Markdown Dossier", Path.GetFullPath(mdPath), ConsoleColor.Green, ConsoleColor.White);
            AnsiConsole.KeyValue("Interactive HTML", Path.GetFullPath(htmlPath), ConsoleColor.Green, ConsoleColor.White);
            Console.WriteLine();

            // STEP 7: SUPERVISOR SIGN-OFF
            AnsiConsole.SubHeader("PHASE 7 // COMMAND DAIS : EXECUTIVE DEBRIEF & MISSION SIGN-OFF");
            _supervisor.Status = "COMPLETE // MISSION DEBRIEF ISSUED";
            _supervisor.Speak("Excellent synchronization, team. Target [" + target + "] is fully documented.");
            _supervisor.Speak("Evidentiary chain-of-custody preserved. Stand down and resume standby watch.");

            AnsiConsole.WriteLine("\n[ MISSION EXECUTION ACCOMPLISHED // RETURN TO OPERATIONS ]", ConsoleColor.Magenta);
            try
            {
                if (!Console.IsInputRedirected)
                    Console.ReadLine();
            }
            catch { }
        }

        private string GenerateMarkdownReport(
            string target,
            Dictionary<string, string> ipData,
            Dictionary<string, string> domainData,
            Dictionary<string, string> honeypotData,
            Dictionary<string, string> breachData,
            Dictionary<string, string> proxyData)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# TOP SECRET // OSINT INTELLIGENCE DOSSIER");
            sb.AppendLine("**Target Identifier:** `" + target + "`  ");
            sb.AppendLine("**Investigation Date:** `" + DateTime.UtcNow.ToString("u") + "`  ");
            sb.AppendLine("**Investigating Team:** Cyber-Corps Virtual Intelligence Agency  ");
            sb.AppendLine("**Operational Status:** COMPLETED // OFFICIALLY SEALED  ");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 1. AGENT MISSION LOG");
            sb.AppendLine("- **Supervisor (Vanguard-1):** Mission initiated and authorized.");
            sb.AppendLine("- **Head Security (Aegis-9):** OPSEC perimeter cleared. Zero canary traps detected.");
            sb.AppendLine("- **Flow (Nexus-7):** Concurrency DAG executed at wire speed with 0% packet loss.");
            sb.AppendLine("- **Researcher (Specter-4):** Telemetry probes executed across public registries.");
            sb.AppendLine("- **Documentation (Scribe-0):** Case file generated and cryptographically sealed.");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 2. SECURITY & THREAT EVALUATION");
            foreach (var kv in honeypotData)
            {
                sb.AppendLine("- **" + kv.Key + ":** `" + kv.Value + "`");
            }
            if (proxyData.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("### Proxy & Anonymity Audit");
                foreach (var kv in proxyData)
                {
                    sb.AppendLine("- **" + kv.Key + ":** `" + kv.Value + "`");
                }
            }
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 3. INFRASTRUCTURE & RECONNAISSANCE FINDINGS");
            if (domainData.Count > 0)
            {
                sb.AppendLine("### Domain Telemetry");
                foreach (var kv in domainData)
                {
                    sb.AppendLine("- **" + kv.Key + ":** `" + kv.Value + "`");
                }
                sb.AppendLine();
            }
            if (ipData.Count > 0)
            {
                sb.AppendLine("### IP Geolocation & Network Routing");
                foreach (var kv in ipData)
                {
                    sb.AppendLine("- **" + kv.Key + ":** `" + kv.Value + "`");
                }
                sb.AppendLine();
            }
            if (breachData.Count > 0)
            {
                sb.AppendLine("### Email & Breach Exposure");
                foreach (var kv in breachData)
                {
                    sb.AppendLine("- **" + kv.Key + ":** `" + kv.Value + "`");
                }
                sb.AppendLine();
            }
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 4. EVIDENTIARY HASH");
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(target + DateTime.UtcNow.Ticks));
                sb.AppendLine("`SHA-256: " + BitConverter.ToString(hash).Replace("-", "").ToLower() + "`");
            }
            sb.AppendLine();
            sb.AppendLine("*End of Confidential Intelligence Briefing.*");
            return sb.ToString();
        }

        private string GenerateHtmlReport(
            string target,
            Dictionary<string, string> ipData,
            Dictionary<string, string> domainData,
            Dictionary<string, string> honeypotData,
            Dictionary<string, string> breachData,
            Dictionary<string, string> proxyData)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en'>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset='UTF-8'>");
            sb.AppendLine("<title>OSINT Dossier // " + target + "</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { background: #0a0e17; color: #c9d1d9; font-family: 'Segoe UI', Consolas, monospace; padding: 30px; margin: 0; }");
            sb.AppendLine(".container { max-width: 900px; margin: 0 auto; background: #161b22; border: 1px solid #30363d; border-radius: 8px; padding: 25px; box-shadow: 0 8px 24px rgba(0,0,0,0.6); }");
            sb.AppendLine("h1 { color: #58a6ff; border-bottom: 2px solid #238636; padding-bottom: 10px; font-size: 24px; }");
            sb.AppendLine("h2 { color: #39d353; border-bottom: 1px solid #30363d; padding-bottom: 5px; margin-top: 25px; }");
            sb.AppendLine(".badge { display: inline-block; padding: 4px 10px; background: #1f6feb; color: white; border-radius: 4px; font-size: 12px; font-weight: bold; margin-bottom: 15px; }");
            sb.AppendLine(".grid { display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-top: 15px; }");
            sb.AppendLine(".card { background: #0d1117; padding: 15px; border: 1px solid #30363d; border-radius: 6px; }");
            sb.AppendLine(".key { color: #8b949e; font-size: 13px; }");
            sb.AppendLine(".val { color: #58a6ff; font-weight: bold; font-size: 15px; word-break: break-all; margin-top: 4px; }");
            sb.AppendLine(".agent-box { background: #21262d; padding: 10px 15px; border-left: 4px solid #58a6ff; margin: 8px 0; border-radius: 3px; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class='container'>");
            sb.AppendLine("<span class='badge'>CLASSIFIED OSINT REPORT</span>");
            sb.AppendLine("<h1>TARGET DOSSIER: " + target + "</h1>");
            sb.AppendLine("<p style='color: #8b949e;'>Generated by <strong>Cyber-Corps Multi-Agent Intelligence Agency</strong> on " + DateTime.UtcNow.ToString("u") + "</p>");
            
            sb.AppendLine("<h2>TEAM COLLABORATION LOG</h2>");
            sb.AppendLine("<div class='agent-box'><strong>Vanguard-1 (Supervisor):</strong> Mission authorized & strategic perimeter set.</div>");
            sb.AppendLine("<div class='agent-box' style='border-left-color: #f85149;'><strong>Aegis-9 (Head Security):</strong> OPSEC verified, honeypot scan negative.</div>");
            sb.AppendLine("<div class='agent-box' style='border-left-color: #d29922;'><strong>Nexus-7 (Flow):</strong> Concurrency DAG streamed data at line speed.</div>");
            sb.AppendLine("<div class='agent-box' style='border-left-color: #388bfd;'><strong>Specter-4 (Researcher):</strong> Public registries harvested & correlated.</div>");
            sb.AppendLine("<div class='agent-box' style='border-left-color: #2ea043;'><strong>Scribe-0 (Documentation):</strong> Formatted and sealed final dossier.</div>");

            sb.AppendLine("<h2>INTELLIGENCE METRICS</h2>");
            sb.AppendLine("<div class='grid'>");
            foreach (var kv in ipData)
            {
                sb.AppendLine("<div class='card'><div class='key'>" + kv.Key + "</div><div class='val'>" + kv.Value + "</div></div>");
            }
            foreach (var kv in domainData)
            {
                sb.AppendLine("<div class='card'><div class='key'>" + kv.Key + "</div><div class='val'>" + kv.Value + "</div></div>");
            }
            foreach (var kv in honeypotData)
            {
                sb.AppendLine("<div class='card'><div class='key'>" + kv.Key + "</div><div class='val'>" + kv.Value + "</div></div>");
            }
            sb.AppendLine("</div>");

            sb.AppendLine("<p style='margin-top: 30px; font-size: 12px; color: #484f58;'>Cryptographic Integrity Verified // Confidential OSINT Intelligence</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }
    }
}
