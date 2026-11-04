using System;
using System.Collections.Generic;
using System.Diagnostics;
using OsintHq.Agents;
using OsintHq.Engine;

namespace OsintHq.Workspace
{
    public class VirtualWorkspace
    {
        public SupervisorAgent Supervisor { get; private set; }
        public FlowAgent Flow { get; private set; }
        public HeadSecurityAgent Security { get; private set; }
        public ResearcherAgent Researcher { get; private set; }
        public DocumentationAgent Documentation { get; private set; }

        private readonly Random _rand = new Random();

        public VirtualWorkspace()
        {
            Supervisor = new SupervisorAgent();
            Flow = new FlowAgent();
            Security = new HeadSecurityAgent();
            Researcher = new ResearcherAgent();
            Documentation = new DocumentationAgent();
        }

        public void UpdateRealtimeTelemetry()
        {
            // Dynamically simulate real-time shifting operational duties in the offices
            string[] supStates = new string[]
            {
                "ORBITAL SWEEP // SECTOR 04 ACTIVE",
                "MISSION TRIAGE // SENSORS SYNCHRONIZED",
                "THREAT OVERWATCH // GLOBAL MATRIX STABLE",
                "OPERATOR BUS // ENCRYPTED COMMS ARMED"
            };

            string[] flowStates = new string[]
            {
                "ZERO-DROP // 1,480 PKTS/SEC BUFFERED",
                "DAG STREAM // LATENCY: 0.68ms",
                "ASYNC THREADS // 16 WORKERS WARM",
                "MULTIPLEXING // LOW-JITTER ROUTING"
            };

            string[] secStates = new string[]
            {
                "FARADAY ARMORED // 0 TRIPWIRES PULSED",
                "CANARY PROBE // HONEYPOT DETECTOR OK",
                "AIR-GAP INTEGRITY // ZERO LEAKS DETECTED",
                "KILL-SWITCH ENGAGED // RF SHIELD MAX"
            };

            string[] resStates = new string[]
            {
                "GLOBAL HARVEST // DOH & GEOIP LIVE",
                "SPECTRAL SDR // RF SPECTRUM TUNED",
                "WHOIS CRAWL // 64 PUBLIC SOCKETS",
                "BREACH MONITOR // HASH CORRELATOR UP"
            };

            string[] docStates = new string[]
            {
                "TELETYPE WARM // PIN-FEED READY",
                "EVIDENTIARY HASH // SHA-256 ARMED",
                "CASE DOSSIER // COMPILING CHRONICLE",
                "SEAL STAMPER // READY FOR CERTIFICATION"
            };

            Supervisor.Status = supStates[_rand.Next(supStates.Length)];
            Flow.Status = flowStates[_rand.Next(flowStates.Length)];
            Security.Status = secStates[_rand.Next(secStates.Length)];
            Researcher.Status = resStates[_rand.Next(resStates.Length)];
            Documentation.Status = docStates[_rand.Next(docStates.Length)];
        }

        public void RenderFloorplan()
        {
            AnsiConsole.SafeClear();
            UpdateRealtimeTelemetry();

            long memoryUsed = 0;
            try { memoryUsed = Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024); } catch { }

            AnsiConsole.Header("RESPLICON // REAL-TIME MULTI-AGENT VIRTUAL HEADQUARTERS", ConsoleColor.Cyan);
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("  FACILITY   : SITE-09 CYBER RECON PRECINCT  |  TIME: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC  |  MEM: " + memoryUsed + " MB", ConsoleColor.DarkGray);
            AnsiConsole.WriteLine("  DEVICE ID  : " + DeviceProfileManager.Profile.DeviceId + "  |  CLEARANCE: " + DeviceProfileManager.Profile.ClearanceLevel, ConsoleColor.Yellow);
            AnsiConsole.WriteLine("  OPERATOR   : " + DeviceProfileManager.Profile.OperatorCountry + " [" + DeviceProfileManager.Profile.OperatorCountryCode + "]  |  SPLOKENS: " + DeviceProfileManager.Profile.SplokensRemaining.ToString("N0") + " / 25,000 SPLOK", ConsoleColor.Green);
            AnsiConsole.WriteLine();

            string plan = 
@"  ╔══════════════════════════════════════════════════════════════════════════════════════════╗
  ║                 [ RESPLICON REAL-TIME MULTI-AGENT INTELLIGENCE CITADEL ]                 ║
  ╠══════════════════════════════════════════════════════════════════════════════════════════╣
  ║                                                                                          ║
  ║      [OFFICE 01: COMMAND DAIS]                       [OFFICE 02: SECURITY REDOUBT]       ║
  ║     ┌──────────────────────────────┐                ┌──────────────────────────────┐     ║
  ║     │ 👑 SUPERVISOR (Vanguard-1)   │                │ 🛡️ HEAD SECURITY (Aegis-9)    │     ║
  ║     │ Stand : 3D Volumetric Globe  │                │ Stand : Blast Shield Vault   │     ║
  ║     │ Duty  : Strategic Overwatch  │                │ Duty  : OPSEC & Honeypot Veto│     ║
  ║     │ Pulse : " + PadRight(Supervisor.Status, 21) + @"│                │ Pulse : " + PadRight(Security.Status, 21) + @"│     ║
  ║     └──────────────┬───────────────┘                └──────────────┬───────────────┘     ║
  ║                    │                                               │                     ║
  ║                    ▼                                               ▼                     ║
  ║                    ═════════════════════════════════════════════════                     ║
  ║                                            │                                             ║
  ║                                [OFFICE 03: FLOW MATRIX]                                  ║
  ║                               ┌──────────────────────────────┐                           ║
  ║                               │ ⚡ FLOW AGENT (Nexus-7)       │                           ║
  ║                               │ Stand : Fiber Routing Ring   │                           ║
  ║                               │ Duty  : Concurrency Pipelines│                           ║
  ║                               │ Pulse : " + PadRight(Flow.Status, 21) + @"│                           ║
  ║                               └──────────────┬───────────────┘                           ║
  ║                                              │                                           ║
  ║                    ═════════════════════════════════════════════════                     ║
  ║                    │                                               │                     ║
  ║                    ▼                                               ▼                     ║
  ║      [OFFICE 04: DEEP RECON]                         [OFFICE 05: ARCHIVES BUREAU]        ║
  ║     ┌──────────────────────────────┐                ┌──────────────────────────────┐     ║
  ║     │ 📡 RESEARCHER (Specter-4)    │                │ 📜 DOCUMENTATION (Scribe-0)  │     ║
  ║     │ Stand : 5-Monitor Panoramic  │                │ Stand : Teletype Scribe Desk │     ║
  ║     │ Duty  : Multi-Source Crawling│                │ Duty  : Dossier Hash Binding │     ║
  ║     │ Pulse : " + PadRight(Researcher.Status, 21) + @"│                │ Pulse : " + PadRight(Documentation.Status, 21) + @"│     ║
  ║     └──────────────────────────────┘                └──────────────────────────────┘     ║
  ╚══════════════════════════════════════════════════════════════════════════════════════════╝";

            AnsiConsole.WriteLine(plan, ConsoleColor.Cyan);
            Console.WriteLine();
        }

        private string PadRight(string text, int length)
        {
            if (string.IsNullOrEmpty(text)) text = "ACTIVE";
            if (text.Length > length) return text.Substring(0, length);
            return text.PadRight(length);
        }

        public void InteractiveWalkthrough()
        {
            while (true)
            {
                RenderFloorplan();
                AnsiConsole.WriteLine("SELECT AN AGENT OFFICE TO ENTER OR INSPECT THEIR REAL-TIME WORKSPACE:", ConsoleColor.Yellow);
                AnsiConsole.WriteLine("  [1] Office 01: Supervisor's Command Dais (Vanguard-1)", ConsoleColor.Magenta);
                AnsiConsole.WriteLine("  [2] Office 02: Head Security's Blast Vault (Aegis-9)", ConsoleColor.Red);
                AnsiConsole.WriteLine("  [3] Office 03: Flow's Routing Matrix & Conduit (Nexus-7)", ConsoleColor.Yellow);
                AnsiConsole.WriteLine("  [4] Office 04: Researcher's Deep Recon Array (Specter-4)", ConsoleColor.Cyan);
                AnsiConsole.WriteLine("  [5] Office 05: Documentation's Archives Stand (Scribe-0)", ConsoleColor.Green);
                AnsiConsole.WriteLine("  [C] Listen in on Encrypted Team Intercom", ConsoleColor.DarkYellow);
                AnsiConsole.WriteLine("  [0] Return to Main Recon Deck", ConsoleColor.DarkGray);
                Console.WriteLine();

                string choice = AnsiConsole.Prompt("OFFICE-NAV >>", ConsoleColor.White).ToUpper();
                if (choice == "0" || choice == "Q" || choice == "EXIT") break;

                switch (choice)
                {
                    case "1":
                        InspectAgent(Supervisor);
                        break;
                    case "2":
                        InspectAgent(Security);
                        break;
                    case "3":
                        InspectAgent(Flow);
                        break;
                    case "4":
                        InspectAgent(Researcher);
                        break;
                    case "5":
                        InspectAgent(Documentation);
                        break;
                    case "C":
                        SimulateTeamIntercom();
                        break;
                    default:
                        AnsiConsole.WriteLine("Invalid office choice.", ConsoleColor.Red);
                        System.Threading.Thread.Sleep(700);
                        break;
                }
            }
        }

        private void InspectAgent(AgentBase agent)
        {
            agent.DisplayOffice();

            AnsiConsole.WriteLine("AGENT OFFICE ACTIONS:", ConsoleColor.Yellow);
            AnsiConsole.WriteLine("  [T] Talk / Interrogate " + agent.Codename + " at their desk", ConsoleColor.White);
            AnsiConsole.WriteLine("  [0] Step away from office", ConsoleColor.DarkGray);
            Console.WriteLine();

            string act = AnsiConsole.Prompt("ACTION >>", ConsoleColor.Cyan).ToUpper();
            if (act == "T")
            {
                Console.WriteLine();
                AnsiConsole.WriteLine("You step up to " + agent.Codename + "'s workspace. What do you say?", ConsoleColor.Gray);
                string input = AnsiConsole.Prompt("YOU >>", ConsoleColor.Green);
                if (!string.IsNullOrEmpty(input))
                {
                    Console.WriteLine();
                    agent.Think("Parsing operator query at local console...");
                    System.Threading.Thread.Sleep(400);
                    string reply = agent.RespondToInquiry(input);
                    AnsiConsole.Typewriter("  " + reply, agent.Color, 12);
                    Console.WriteLine();
                    AnsiConsole.PressAnyKey("Press any key to step away from the desk...");
                }
            }
        }

        public void SimulateTeamIntercom()
        {
            AnsiConsole.SafeClear();
            AnsiConsole.Header("RESPLICON ENCRYPTED INTERCOM // SITE-09 CHATTER", ConsoleColor.DarkYellow);
            AnsiConsole.WriteLine("Monitoring encrypted audio channels across all 5 workstations...", ConsoleColor.DarkGray);
            Console.WriteLine();

            Supervisor.Speak("Real-time telemetry sync across all stations. Sound off.");
            System.Threading.Thread.Sleep(500);
            Security.Speak("Redoubt-3 sealed. Faraday mesh energized. No external probe leakage detected.");
            System.Threading.Thread.Sleep(500);
            Flow.Speak("Routing matrix operating at sub-millisecond throughput. Concurrency queues clear.");
            System.Threading.Thread.Sleep(500);
            Researcher.Speak("Spectral array locked on global feeds. Global ITU-T numbers and DNS resolvers primed.");
            System.Threading.Thread.Sleep(500);
            Documentation.Speak("Continuous paper feed ready in the teletype. Evidentiary SHA-256 hashes verified.");
            System.Threading.Thread.Sleep(500);
            Supervisor.Speak("Maintain active vigilance. Vanguard-1 out.");

            Console.WriteLine();
            AnsiConsole.PressAnyKey("Press any key to return to floorplan...");
        }
    }
}
