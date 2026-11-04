using System;
using System.Collections.Generic;

namespace OsintHq.Agents
{
    public class HeadSecurityAgent : AgentBase
    {
        public HeadSecurityAgent()
        {
            Name = "Head Security";
            Codename = "Aegis-9";
            Role = "OPSEC Warden & Threat Isolation Architect";
            Color = ConsoleColor.Red;
            OfficeName = "The Armored Threat Isolation Vault // Redoubt-3";
            Status = "SHIELDED // TRIPWIRES & HONEYPOT ALARMS ARMED";

            OfficeLayout = 
                "A fortified blast-proof isolation booth enclosed within lead-lined ballistic glass and double-layered\n" +
                "Faraday mesh. The steel workstation is equipped with two dual-throw physical circuit breakers for instant\n" +
                "network severance, a spectrum analyzer watching for back-connects, and a bank of crimson warning beacons\n" +
                "ready to flash if an investigator triggers a honeypot, tracking canary, or DNS leak.";

            OfficeAscii = 
@"             ._____________________________________________________________.
            /                 AEGIS-9 :: THREAT ISOLATION VAULT             \
           |  [!] SHIELDING: FARADAY ACTIVE   [!] BLAST SHUTTERS: PRIMED    |
           |  +--------------------+     +-------------------------------+  |
           |  | HONEYPOT TRIPWIRE  |     | ANONYMITY & PROXY SENTINEL    |  |
           |  | STATUS: [MONITOR]  |     | LEAK DETECTION: 0 LEAKS       |  |
           |  +--------------------+     +-------------------------------+  |
           |             |                               |                  |
           |      [KILL-SWITCH 1]                 [KILL-SWITCH 2]           |
           |       [PHYSICAL CUT]                  [RF DUMP JAM]            |
           |  +----------------------------------------------------------+  |
           |  | [FARADAY MESH] [BIOMETRIC VAULT] [COUNTER-SURVEILLANCE]  |  |
           |  |  AIR-GAP OK     THREAT SCORE: 0   SIGNATURE SCAN ACTIVE  |  |
           |  +----------------------------------------------------------+  |
           |   \___________/     [HEAVY ARMORED PLINTH]     \____________/  |
           \_________________________________________________________________/";

            Appearance = "Broad-shouldered operative clad in carbon-composite ballistic tactical armor; wears a mirrored helmet with a red HUD visor that reflects incoming packet headers and port signatures.";
            Behavior = "Vigilant, cynical, protective; views every open port as a potential trap and every public query as an OPSEC vulnerability; prioritizes investigator survival over curiosity.";
            CollaborationStyle = "Interrogates every target before Researcher touches it; checks for honeypot signatures, proxy leakage, and rate-limit tripwires; delivers risk ratings to Documentation.";

            Equipment.Add("Physical Dual-Pole Air-Gap Disconnect Lever");
            Equipment.Add("Faraday Shielded RF Spectrum Leak Detector");
            Equipment.Add("Honeypot & Deception Signature Engine");
            Equipment.Add("Real-Time IP2Proxy & Tor Node Validation Array");
            Equipment.Add("Biometric Emergency Override Panel");
        }

        public override string RespondToInquiry(string query)
        {
            query = (query ?? "").ToLower();
            if (query.Contains("honeypot") || query.Contains("trap") || query.Contains("threat"))
            {
                return "Aegis-9: 'Never trust a server that responds too eagerly. Sophisticated targets deploy fake SSH services and poisoned MX records to log incoming probes. I scan for deception signatures before we touch them.'";
            }
            if (query.Contains("proxy") || query.Contains("vpn") || query.Contains("ip2proxy") || query.Contains("anonymity"))
            {
                return "Aegis-9: 'If your true IP is exposed, you are already burned. I mandate VPN/proxy validation on every egress packet. If an exit node is tainted, I pull the plug.'";
            }
            if (query.Contains("kill") || query.Contains("abort") || query.Contains("safe"))
            {
                return "Aegis-9: 'My physical kill-switch cuts fiber lines in under twelve milliseconds. If a target begins tracing back or counter-scanning, we sever the wire instantly.'";
            }
            return "Aegis-9: 'Eyes open, Operator. The hunters are often hunted in OSINT. Maintain your OPSEC perimeter.'";
        }
    }
}

