using System;
using System.Collections.Generic;

namespace OsintHq.Agents
{
    public class ResearcherAgent : AgentBase
    {
        public ResearcherAgent()
        {
            Name = "Researcher";
            Codename = "Specter-4";
            Role = "Deep Reconnaissance & Multi-Source Intelligence Hunter";
            Color = ConsoleColor.Cyan;
            OfficeName = "The Spectral Recon Array // Sensor Station-4";
            Status = "SCANNING // GLOBAL TELEMETRY PROBES ONLINE";

            OfficeLayout = 
                "A darkened alcove bathed in deep cyan light. A cockpit of five ultrawide high-resolution displays\n" +
                "arcs around a long workstation. High-gain SDR antenna cables snake into rackmount spectrum demodulators.\n" +
                "The desk is packed with crawler telemetry logs, WHOIS registrar lookups, DNS map branches, social\n" +
                "username matrices, and a half-drained thermal mug of black coffee.";

            OfficeAscii = 
@"             ._____________________________________________________________.
            /                 SPECTER-4 :: DEEP RECON ARRAY                 \
           |  +--------------------+     +-------------------------------+  |
           |  | DNS & SUBDOMAIN    |     | GEO-IP & SATELLITE MAP        |  |
           |  | A: 142.250.190.46  |     | LAT: 39.03  LON: -77.50       |  |
           |  | MX: aspmx.l.google |     | ISP: GOOGLE-PUBLIC-DNS        |  |
           |  +--------------------+     +-------------------------------+  |
           |             |                               |                  |
           |      [WHOIS REGISTRY]                [BREACH & USER HARVEST]   |
           |      [CREATION: 1997]                [72 PLATFORMS SCANNED]    |
           |  +----------------------------------------------------------+  |
           |  | [SDR ANTENNA]  [EXIF EXTRACTOR]  [MAC OUI VENDOR LOOKUP] |  |
           |  | [###] KEYBOARD TRAY   [=] COFFEE MUG   [*] SCRATCHPAD    |  |
           |  +----------------------------------------------------------+  |
           |   \___________/       [HEAVY OAK SENSOR DESK]      \____________/  |
           \_________________________________________________________________/";

            Appearance = "Slender figure in a dark charcoal tactical hoodie with sleeves pushed up; a multi-band headset rests around their neck, and cyan monitor glare glints off their wireframe spectacles.";
            Behavior = "Inquisitive, methodical, relentless; spots patterns across disparate data fragments like DNS TXT records, email header trails, and social media breadcrumbs.";
            CollaborationStyle = "Executes the reconnaissance heavy-lifting for the team; passes raw DNS, WHOIS, IP, breach, and username hits to Flow for buffering and to Documentation for archival synthesis.";

            Equipment.Add("Five-Display Curved Panoramic Reconnaissance Console");
            Equipment.Add("Software-Defined Radio (SDR) Signal Ingestion Rack");
            Equipment.Add("Multi-Threaded Social Footprint Crawler Array");
            Equipment.Add("Public DNS-Over-HTTPS (DoH) Resolver Cluster");
            Equipment.Add("IEEE OUI MAC Hardware Fingerprint Cache");
        }

        public override string RespondToInquiry(string query)
        {
            query = (query ?? "").ToLower();
            if (query.Contains("whois") || query.Contains("domain") || query.Contains("dns"))
            {
                return "Specter-4: 'Domains are goldmines. Give me a domain and I'll pull its A, MX, TXT, and NS records via DoH, map its subdomains, and identify who owns the registry.'";
            }
            if (query.Contains("ip") || query.Contains("geo") || query.Contains("location"))
            {
                return "Specter-4: 'Every IP tells a story—ASN number, upstream ISP, physical coordinates, city, and routing path. I can pinpoint where the packet terminates in seconds.'";
            }
            if (query.Contains("username") || query.Contains("social") || query.Contains("person"))
            {
                return "Specter-4: 'People reuse aliases across platforms. I probe GitHub, Twitter, Reddit, Instagram, and dozens more simultaneously to map an alias's digital footprint.'";
            }
            if (query.Contains("breach") || query.Contains("leak") || query.Contains("email"))
            {
                return "Specter-4: 'Leaked credential databases reveal exposed passwords and past compromises. Cross-referencing an email against known breach corpora cracks open investigations.'";
            }
            return "Specter-4: 'Probes deployed. Give me a target—I never stop digging until all breadcrumbs are mapped.'";
        }
    }
}

