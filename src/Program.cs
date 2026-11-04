using System;
using System.IO;
using System.Threading;
using OsintHq.Engine;
using OsintHq.Modules;
using OsintHq.Workspace;

namespace OsintHq
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RESPLICON // GLOBAL MULTI-AGENT CYBERSECURITY OSINT MATRIX";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072 | (System.Net.SecurityProtocolType)768 | System.Net.SecurityProtocolType.Tls;
                Console.WindowWidth = Math.Min(118, Console.LargestWindowWidth);
                Console.WindowHeight = Math.Min(42, Console.LargestWindowHeight);
            }
            catch { }

            // Ensure directories exist
            if (!Directory.Exists("reports")) Directory.CreateDirectory("reports");
            if (!Directory.Exists("config")) Directory.CreateDirectory("config");

            // Initialize persistent device hardware profile & detect operator country
            DeviceProfileManager.Initialize();

            var workspace = new VirtualWorkspace();
            var collab = new CollaborationEngine(
                workspace.Supervisor,
                workspace.Flow,
                workspace.Security,
                workspace.Researcher,
                workspace.Documentation
            );

            // Handle CLI flags
            if (args.Length > 0)
            {
                string first = args[0].ToLower();
                if (first == "--web" || first == "-w" || first == "web" || first == "gui")
                {
                    HtmlWorkspaceGenerator.GenerateAndOpenWorkspace();
                    return;
                }
                if (first == "--floorplan" || first == "-f" || first == "floorplan")
                {
                    workspace.RenderFloorplan();
                    return;
                }
                if (first == "--help" || first == "-h")
                {
                    PrintBanner();
                    AnsiConsole.WriteLine("RESPLICON COMMAND-LINE USAGE:", ConsoleColor.Yellow);
                    Console.WriteLine();
                    AnsiConsole.WriteLine("  Resplicon.exe                 Launch Interactive Cyber Command Console", ConsoleColor.White);
                    AnsiConsole.WriteLine("  Resplicon.exe <target>        Autonomous 5-Agent Mission on Target", ConsoleColor.White);
                    AnsiConsole.WriteLine("  Resplicon.exe --web           Open Realtime Interactive 2.5D Web Deck", ConsoleColor.White);
                    AnsiConsole.WriteLine("  Resplicon.exe --floorplan     Render Realtime ASCII Operations Floorplan", ConsoleColor.White);
                    return;
                }

                collab.ExecuteCollaborativeMission(args[0]);
                return;
            }

            // Optional Fast Startup Telemetry (Only on first load in session)
            PrintStartupBootSequence();

            // Main Interactive Command Loop
            while (true)
            {
                AnsiConsole.SafeClear();
                PrintBanner();
                PrintTelemetryBox();
                PrintMenu();

                Console.WriteLine();
                string input = AnsiConsole.Prompt("  RESPLICON-OPS >>", ConsoleColor.Yellow);
                if (string.IsNullOrEmpty(input)) continue;

                string upper = input.Trim().ToUpper();

                if (upper == "Q" || upper == "EXIT" || upper == "QUIT")
                {
                    Console.WriteLine();
                    AnsiConsole.WriteLine("  [+] Powering down Resplicon Core. Device session preserved.", ConsoleColor.Cyan);
                    Console.WriteLine();
                    break;
                }

                // CHECK UNLISTED SECRET CODES (Hidden from menu)
                if (DeviceProfileManager.TryApplySecretCode(input))
                {
                    Console.WriteLine();
                    AnsiConsole.WriteLine("  ╔═══════════════════════════════════════════════════════════════════════════════════════╗", ConsoleColor.Red);
                    AnsiConsole.WriteLine("  ║          [***] CLASSIFIED AUTHORIZATION CODE DETECTED: ACCESS GRANTED [***]           ║", ConsoleColor.Yellow);
                    AnsiConsole.WriteLine("  ╚═══════════════════════════════════════════════════════════════════════════════════════╝", ConsoleColor.Red);
                    Console.WriteLine();
                    AnsiConsole.KeyValue("OPERATOR CLEARANCE", DeviceProfileManager.Profile.ClearanceLevel, ConsoleColor.DarkCyan, ConsoleColor.Green);
                    AnsiConsole.KeyValue("DEVICE FINGERPRINT", DeviceProfileManager.Profile.DeviceId, ConsoleColor.DarkCyan, ConsoleColor.Cyan);
                    AnsiConsole.KeyValue("SPLOKEN BALANCE", DeviceProfileManager.Profile.SplokensRemaining.ToString("N0") + " / 25,000 SPLOK (RECHARGED)", ConsoleColor.DarkCyan, ConsoleColor.Green);
                    AnsiConsole.KeyValue("SECURITY PRIVILEGE", "Deep Phishing Audits, Hacking IOCs & Unredacted Telemetry Unlocked", ConsoleColor.DarkCyan, ConsoleColor.White);
                    AnsiConsole.KeyValue("PERSISTENCE STATE", "Bound to config/resplicon_device.json", ConsoleColor.DarkCyan, ConsoleColor.DarkGray);
                    Console.WriteLine();
                    AnsiConsole.PressAnyKey("Press any key to return to elevated operations deck...");
                    continue;
                }

                switch (upper)
                {
                    case "0":
                    case "HUNT":
                        Console.WriteLine();
                        string target = AnsiConsole.Prompt("  ENTER RECON TARGET (IP / Domain / Phone / Alias):", ConsoleColor.Magenta);
                        if (!string.IsNullOrEmpty(target))
                        {
                            collab.ExecuteCollaborativeMission(target);
                        }
                        break;

                    case "W":
                    case "WORKSPACE":
                    case "FLOORPLAN":
                    case "OFFICE":
                        workspace.InteractiveWalkthrough();
                        break;

                    case "G":
                    case "GUI":
                    case "WEB":
                        HtmlWorkspaceGenerator.GenerateAndOpenWorkspace();
                        AnsiConsole.PressAnyKey();
                        break;

                    case "H":
                    case "PHISH":
                    case "PHISHING":
                    case "HACK":
                        Console.WriteLine();
                        AnsiConsole.WriteLine("  [1] Phishing Campaign & URL Investigation (Typo-squatting, Fake Forms, Webhooks)", ConsoleColor.Cyan);
                        AnsiConsole.WriteLine("  [2] Hacking & Compromised Host IOC Audit (Backdoors, Ports, Webshells)", ConsoleColor.Yellow);
                        Console.WriteLine();
                        string hMode = AnsiConsole.Prompt("  SELECT MODE (1 or 2) >>", ConsoleColor.White);
                        Console.WriteLine();
                        if (hMode == "2")
                        {
                            string host = AnsiConsole.Prompt("  Enter Target Host or IP to Audit:", ConsoleColor.Cyan);
                            OsintModules.RunHackingInvestigation(host);
                        }
                        else
                        {
                            string url = AnsiConsole.Prompt("  Enter Suspect URL or Domain to Investigate:", ConsoleColor.Red);
                            OsintModules.RunPhishingInvestigation(url);
                        }
                        AnsiConsole.PressAnyKey();
                        break;

                    case "A":
                    case "SLOP":
                    case "AI":
                        Console.WriteLine();
                        AnsiConsole.WriteLine("  AI SLOP DETECTOR: Enter Web Link (URL) or text to audit for AI Hallucinations:", ConsoleColor.DarkCyan);
                        string slopInput = AnsiConsole.Prompt("  LINK / INPUT >>", ConsoleColor.White);
                        Console.WriteLine();
                        OsintModules.RunAiSlopDetection(slopInput);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "P":
                    case "PRIVACY":
                    case "OPSEC":
                        Console.WriteLine();
                        string privTarget = AnsiConsole.Prompt("  Enter Target/Profile for Privacy & OPSEC Audit:", ConsoleColor.Green);
                        Console.WriteLine();
                        OsintModules.RunPrivacyAudit(privTarget);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "K":
                    case "KNOWLEDGE":
                    case "SKILLS":
                        Console.WriteLine();
                        OsintModules.RunCyberKnowledgeBase("");
                        AnsiConsole.PressAnyKey();
                        break;

                    case "1":
                    case "IP":
                        Console.WriteLine();
                        string ip = AnsiConsole.Prompt("  Enter Target IP Address (default: 8.8.8.8):", ConsoleColor.Cyan);
                        Console.WriteLine();
                        OsintModules.RunIpLookup(ip);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "2":
                    case "DOMAIN":
                        Console.WriteLine();
                        string dom = AnsiConsole.Prompt("  Enter Target Domain (default: google.com):", ConsoleColor.Yellow);
                        Console.WriteLine();
                        OsintModules.RunDomainLookup(dom);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "3":
                    case "PHONE":
                    case "GLOBAL PHONE":
                        Console.WriteLine();
                        AnsiConsole.WriteLine("  GLOBAL PHONE ENGINE: Enter any international number (+1, +44, +63, +49, +81, +91...):", ConsoleColor.DarkCyan);
                        string ph = AnsiConsole.Prompt("  PHONE NUMBER >>", ConsoleColor.Magenta);
                        Console.WriteLine();
                        OsintModules.RunPhoneLookup(ph);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "4":
                    case "DNS":
                    case "DNS MAP":
                        Console.WriteLine();
                        string dnsDom = AnsiConsole.Prompt("  Enter Domain for DNS Tree Mapping (default: google.com):", ConsoleColor.Cyan);
                        Console.WriteLine();
                        OsintModules.RunDnsMap(dnsDom);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "5":
                    case "METADATA":
                        Console.WriteLine();
                        string path = AnsiConsole.Prompt("  Enter Path to Local File (or press Enter for self):", ConsoleColor.Green);
                        Console.WriteLine();
                        OsintModules.RunMetadataLookup(path);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "6":
                    case "REVERSE":
                    case "REVERSE IP":
                        Console.WriteLine();
                        string rev = AnsiConsole.Prompt("  Enter IP or Hostname for Reverse Mapping:", ConsoleColor.Cyan);
                        Console.WriteLine();
                        OsintModules.RunReverseLookup(rev);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "7":
                    case "HONEYPOT":
                        Console.WriteLine();
                        string hp = AnsiConsole.Prompt("  Enter Target Host/IP for Honeypot Scan:", ConsoleColor.Red);
                        Console.WriteLine();
                        OsintModules.RunHoneypotCheck(hp);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "8":
                    case "MAC":
                    case "MAC LOOKUP":
                        Console.WriteLine();
                        string mac = AnsiConsole.Prompt("  Enter MAC Address (e.g. 00:1A:2B:3C:4D:5E):", ConsoleColor.Cyan);
                        Console.WriteLine();
                        OsintModules.RunMacLookup(mac);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "9":
                    case "IPHEATMAP":
                    case "HEATMAP":
                        Console.WriteLine();
                        string hm = AnsiConsole.Prompt("  Enter Target IP for Heatmap & Coordinates:", ConsoleColor.Yellow);
                        Console.WriteLine();
                        OsintModules.RunIpHeatmap(hm);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "10":
                    case "TORRENT":
                        Console.WriteLine();
                        string tor = AnsiConsole.Prompt("  Enter Target IP for P2P/Torrent Exposure Check:", ConsoleColor.Cyan);
                        Console.WriteLine();
                        OsintModules.RunTorrentCheck(tor);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "11":
                    case "USERNAME":
                    case "USER":
                        Console.WriteLine();
                        string u = AnsiConsole.Prompt("  Enter Username to Discover (e.g. splikaa):", ConsoleColor.Cyan);
                        Console.WriteLine();
                        OsintModules.RunUsernameLookup(u);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "12":
                    case "IP2PROXY":
                    case "PROXY":
                        Console.WriteLine();
                        string px = AnsiConsole.Prompt("  Enter IP to Audit for VPN / Datacenter / Proxy:", ConsoleColor.Yellow);
                        Console.WriteLine();
                        OsintModules.RunIp2Proxy(px);
                        AnsiConsole.PressAnyKey();
                        break;

                    case "13":
                    case "MAIL BREACH":
                    case "BREACH":
                        Console.WriteLine();
                        string mail = AnsiConsole.Prompt("  Enter Target Domain or Email for Breach Audit:", ConsoleColor.Magenta);
                        Console.WriteLine();
                        OsintModules.RunMailBreach(mail);
                        AnsiConsole.PressAnyKey();
                        break;

                    default:
                        Console.WriteLine();
                        AnsiConsole.WriteLine("  [!] Command not recognized. Select an option from the operations deck.", ConsoleColor.Red);
                        Thread.Sleep(700);
                        break;
                }
            }
        }

        private static void PrintStartupBootSequence()
        {
            var p = DeviceProfileManager.Profile;
            AnsiConsole.WriteLine("  [INIT] Synchronizing Resplicon Multi-Agent Kernel...", ConsoleColor.DarkCyan);
            Thread.Sleep(80);
            AnsiConsole.WriteLine("  [AUTH] Device Bound: " + p.DeviceId + " | Clearance: " + p.ClearanceLevel, ConsoleColor.DarkCyan);
            Thread.Sleep(80);
            AnsiConsole.WriteLine("  [GEO]  Operator Country Detected: " + p.OperatorCountry + " [" + p.OperatorCountryCode + "] (" + p.OperatorCity + ")", ConsoleColor.Green);
            Thread.Sleep(80);
            AnsiConsole.WriteLine("  [ECON] Sploken Ledger Loaded: " + p.SplokensRemaining.ToString("N0") + " / 25,000 SPLOK Active", ConsoleColor.Yellow);
            Thread.Sleep(120);
        }

        private static void PrintBanner()
        {
            Console.WriteLine();
            string banner = 
@"   ██████╗ ███████╗███████╗██████╗ ██╗     ██╗ ██████╗ ██████╗ ███╗   ██╗
   ██╔══██╗██╔════╝██╔════╝██╔══██╗██║     ██║██╔════╝██╔═══██╗████╗  ██║
   ██████╔╝█████╗  ███████╗██████╔╝██║     ██║██║     ██║   ██║██╔██╗ ██║
   ██╔══██╗██╔══╝  ╚════██║██╔═══╝ ██║     ██║██║     ██║   ██║██║╚██╗██║
   ██║  ██║███████╗███████║██║     ███████╗██║╚██████╗╚██████╔╝██║ ╚████║
   ╚═╝  ╚═╝╚══════╝╚══════╝╚═╝     ╚══════╝╚═╝ ╚═════╝ ╚═════╝ ╚═╝  ╚═══╝";

            AnsiConsole.WriteLine(banner, ConsoleColor.Cyan);
            Console.WriteLine();
            AnsiConsole.WriteLine("          [ GLOBAL MULTI-AGENT CYBERSECURITY OSINT & RECON MATRIX ]", ConsoleColor.White);
            AnsiConsole.WriteLine("                  Autonomous Threat Verification & Defensive Intel", ConsoleColor.DarkCyan);
            Console.WriteLine();
        }

        private static void PrintTelemetryBox()
        {
            var p = DeviceProfileManager.Profile;
            int pct = (int)Math.Round((double)p.SplokensRemaining / 25000.0 * 100.0);
            ConsoleColor spClr = p.SplokensRemaining > 10000 ? ConsoleColor.Green : (p.SplokensRemaining > 3000 ? ConsoleColor.Yellow : ConsoleColor.Red);

            int boxWidth = 104;
            AnsiConsole.WriteLine("  ╔" + new string('═', boxWidth) + "╗", ConsoleColor.DarkGray);

            // Row 1: Device, Sessions, Missions, Queries, Country
            string cStr = p.OperatorCountryCode + " " + p.OperatorCountry;
            if (cStr.Length > 18) cStr = cStr.Substring(0, 18);

            AnsiConsole.PrintBoxedRow(boxWidth,
                new FormattedSpan("DEVICE: ", ConsoleColor.DarkCyan),
                new FormattedSpan(p.DeviceId, ConsoleColor.White),
                new FormattedSpan(" │ ", ConsoleColor.DarkGray),
                new FormattedSpan("SESSIONS: ", ConsoleColor.DarkCyan),
                new FormattedSpan(p.TotalLaunches.ToString(), ConsoleColor.Cyan),
                new FormattedSpan(" │ ", ConsoleColor.DarkGray),
                new FormattedSpan("MISSIONS: ", ConsoleColor.DarkCyan),
                new FormattedSpan(p.TotalMissions.ToString(), ConsoleColor.Cyan),
                new FormattedSpan(" │ ", ConsoleColor.DarkGray),
                new FormattedSpan("QUERIES: ", ConsoleColor.DarkCyan),
                new FormattedSpan(p.TotalQueries.ToString(), ConsoleColor.Cyan),
                new FormattedSpan(" │ ", ConsoleColor.DarkGray),
                new FormattedSpan("COUNTRY: ", ConsoleColor.DarkCyan),
                new FormattedSpan(cStr, ConsoleColor.Green)
            );

            // Row 2: Splokens, Clearance, Realtime
            string splokStr = p.SplokensRemaining.ToString("N0") + " / 25,000 (" + pct + "%)";
            ConsoleColor clr = p.SecretUnlocked ? ConsoleColor.Red : ConsoleColor.Yellow;
            string clrStr = p.ClearanceLevel;
            if (p.SecretUnlocked)
            {
                clrStr = "BLACK-OPS // RESPLICON PRIME ACCESS";
            }
            else if (clrStr.Length > 35)
            {
                clrStr = clrStr.Substring(0, 35);
            }

            AnsiConsole.PrintBoxedRow(boxWidth,
                new FormattedSpan("SPLOKENS: ", ConsoleColor.DarkYellow),
                new FormattedSpan(splokStr, spClr),
                new FormattedSpan(" │ ", ConsoleColor.DarkGray),
                new FormattedSpan("CLEARANCE: ", ConsoleColor.DarkCyan),
                new FormattedSpan(clrStr, clr),
                new FormattedSpan(" │ ", ConsoleColor.DarkGray),
                new FormattedSpan("TIME: ", ConsoleColor.DarkCyan),
                new FormattedSpan(DateTime.UtcNow.ToString("HH:mm:ss") + " UTC", ConsoleColor.White)
            );

            AnsiConsole.WriteLine("  ╚" + new string('═', boxWidth) + "╝", ConsoleColor.DarkGray);
            Console.WriteLine();
        }

        private static void PrintMenu()
        {
            AnsiConsole.WriteLine("  >>> MULTI-AGENT SYNCHRONIZED OPERATIONS <<<", ConsoleColor.Yellow);
            AnsiConsole.WriteLine("  [0]  AUTONOMOUS TEAM HUNT       Synchronized 5-agent investigation & sealed dossier", ConsoleColor.White);
            AnsiConsole.WriteLine("  [W]  REAL-TIME WORKSPACE        Inspect live agent stands, physical desks & floorplan", ConsoleColor.White);
            AnsiConsole.WriteLine("  [G]  LAUNCH WEB DECK            Interactive 2.5D visual headquarters in browser", ConsoleColor.White);
            Console.WriteLine();

            AnsiConsole.WriteLine("  >>> CYBERSECURITY & THREAT INVESTIGATIONS <<<", ConsoleColor.Red);
            AnsiConsole.WriteLine("  [H]  HACKING & PHISHING AUDIT   Phishing campaigns, typo-squatting, fake forms & IOCs", ConsoleColor.Red);
            AnsiConsole.WriteLine("  [A]  AI SLOP DETECTOR (LINKS)   Ingest webpage URL or text to audit AI hallucinations", ConsoleColor.Green);
            AnsiConsole.WriteLine("  [P]  PRIVACY & OPSEC AUDIT      Audit query exposure, PII footprint & egress defense", ConsoleColor.Green);
            AnsiConsole.WriteLine("  [K]  CYBER KNOWLEDGE BASE       Defensive tradecraft, MITRE ATT&CK & forensic skills", ConsoleColor.Cyan);
            Console.WriteLine();

            AnsiConsole.WriteLine("  >>> TACTICAL RECONNAISSANCE MODULES <<<", ConsoleColor.DarkCyan);
            PrintMenuItem("1.", "IP INTELLIGENCE", "Enumerate information & GeoIP from IP Address");
            PrintMenuItem("2.", "DOMAIN RECON", "Gather DNS, WHOIS & registrar records for domain");
            PrintMenuItem("3.", "GLOBAL PHONE", "Track any global phone number (+1 to +998) & carrier");
            PrintMenuItem("4.", "DNS MAP TREE", "Hierarchical Cloudflare DoH mapping (A/AAAA/MX/TXT/NS)");
            PrintMenuItem("5.", "FILE FORENSICS", "Extract metadata, timestamps, MD5 & SHA-256 hashes");
            PrintMenuItem("6.", "REVERSE IP MAP", "Reverse PTR lookup & infrastructure hostname clustering");
            PrintMenuItem("7.", "HONEYPOT PROBE", "Detect honeypot deception nodes, sinkholes & canaries");
            PrintMenuItem("8.", "MAC OUI VENDOR", "Hardware manufacturer resolution via IEEE OUI database");
            PrintMenuItem("9.", "IP HEATMAP", "Cartographic coordinates & tactical ASCII radar locator");
            PrintMenuItem("10.", "TORRENT / P2P", "P2P swarm DHT exposure & torrent activity audit");
            PrintMenuItem("11.", "USERNAME SCOUT", "Multi-threaded footprint check across 12+ networks");
            PrintMenuItem("12.", "IP2PROXY AUDIT", "VPN, Datacenter, Tor exit node & proxy classifier");
            PrintMenuItem("13.", "MAIL BREACH", "Email exposure, MX posture, SPF/DMARC hygiene check");
            Console.WriteLine();
            AnsiConsole.WriteLine("  [Q]  SHUTDOWN RESPLICON TERMINAL", ConsoleColor.DarkGray);
        }

        private static void PrintMenuItem(string num, string title, string description)
        {
            AnsiConsole.Write("  " + num.PadRight(4), ConsoleColor.White);
            AnsiConsole.Write(title.PadRight(20), ConsoleColor.Cyan);
            AnsiConsole.WriteLine(description, ConsoleColor.Gray);
        }
    }
}
