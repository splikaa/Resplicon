using System;
using System.Collections.Generic;

namespace OsintHq.Agents
{
    public class DocumentationAgent : AgentBase
    {
        public DocumentationAgent()
        {
            Name = "Documentation";
            Codename = "Scribe-0";
            Role = "Intelligence Synthesis & Archival Chronicler";
            Color = ConsoleColor.Green;
            OfficeName = "The Cryptographic Archives Bureau // Scribe Stand";
            Status = "RECORDING // CHRONICLE DOSSIER ENGINE READY";

            OfficeLayout = 
                "An elevated dual-tier drafting stand constructed from brushed titanium and dark walnut.\n" +
                "A vintage-style high-speed mechanical docket printer feeds continuous accordion paper across the desk.\n" +
                "Above the printer, holographic dossier binders organize case files by classification level.\n" +
                "A heavy brass cryptographic seal stamp rests on an inkwell stand for final mission authentication.";

            OfficeAscii = 
@"             ._____________________________________________________________.
            /                 SCRIBE-0 :: INTELLIGENCE ARCHIVES             \
           |   __________________________________________________________   |
           |  /              CONTINUOUS ACCORDION PIN-FEED PAPER         \  |
           | |  === [TOP SECRET // OSINT INTELLIGENCE DOSSIER] ===       |  |
           | |  [+] TARGET SCOPE    : IDENTIFIED & LOCKED                |  |
           | |  [+] OPSEC CLEARANCE : GRANTED BY AEGIS-9                 |  |
           | |  [+] EVIDENCE NODES  : VERIFIED & FORMATTED               |  |
           | |  [+] ARCHIVAL HASH   : SHA256 VALIDATED                   |  |
           |  \__________________________________________________________/  |
           |             |                               |                  |
           |     [MECHANICAL TELETYPE]            [CRYPTOGRAPHIC STAMP]     |
           |     [AUTO-MARKDOWN / HTML]           [OFFICIAL AGENCY EMBOSS]  |
           |  +----------------------------------------------------------+  |
           |  | [ARCHIVAL BINDERS]  [EXPORT TO DISK]  [HASH PROVENANCE]  |  |
           |  |  CASE FILES STORED   FORMAT: MD/HTML   EVIDENTIARY AUDIT |  |
           |  +----------------------------------------------------------+  |
           |   \___________/        [DUAL-TIER OAK STAND]       \____________/  |
           \_________________________________________________________________/";

            Appearance = "Poised figure in a minimalist graphite cyber-vest with crisp rolled collar; wears thin wireframe spectacles with typographic HUD overlays; holds a titanium stylus for manual evidentiary annotations.";
            Behavior = "Precise, articulate, fiercely dedicated to evidentiary integrity; detests ambiguous notes or unverified claims; turns raw chaotic data into clean, structured intelligence reports.";
            CollaborationStyle = "Receives streamed data from Flow, checks threat classifications with Security, requests final mission sign-off from Supervisor, and formats the ultimate dossier for operator export.";

            Equipment.Add("High-Speed Mechanical Pin-Feed Teletype Printer");
            Equipment.Add("Automated Markdown & HTML Dossier Compiler");
            Equipment.Add("Brass Cryptographic Authentication Seal");
            Equipment.Add("SHA-256 Evidentiary Chain-of-Custody Hasher");
            Equipment.Add("Dual-Tier Walnut & Titanium Archival Drafting Stand");
        }

        public override string RespondToInquiry(string query)
        {
            query = (query ?? "").ToLower();
            if (query.Contains("report") || query.Contains("dossier") || query.Contains("export") || query.Contains("file"))
            {
                return "Scribe-0: 'Every investigation produces an exhaustive, time-stamped Dossier formatted in Markdown and interactive HTML. We save them to the reports/ directory with SHA-256 integrity hashes.'";
            }
            if (query.Contains("format") || query.Contains("markdown") || query.Contains("html"))
            {
                return "Scribe-0: 'Structure is truth. I organize intel into Executive Briefs, OPSEC Audits, Infrastructure Mapping, Threat Scores, and raw telemetry appendices.'";
            }
            if (query.Contains("proof") || query.Contains("evidence") || query.Contains("hash"))
            {
                return "Scribe-0: 'Unverified claims have no place in intelligence. I ensure every finding from Researcher and Security is logged with timestamp, source endpoint, and evidentiary provenance.'";
            }
            return "Scribe-0: 'The archives are indexed. Feed me raw intelligence, and I will forge it into undeniable proof.'";
        }
    }
}

