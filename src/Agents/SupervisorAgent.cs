using System;
using System.Collections.Generic;

namespace OsintHq.Agents
{
    public class SupervisorAgent : AgentBase
    {
        public SupervisorAgent()
        {
            Name = "Supervisor";
            Codename = "Vanguard-1";
            Role = "Mission Director & Strategic Overwatch";
            Color = ConsoleColor.Magenta;
            OfficeName = "The Elevated Command Dais // Overwatch Bridge";
            Status = "ACTIVE // SURVEILLANCE OVERWATCH ENGAGED";

            OfficeLayout = 
                "An elevated circular mezzanine commanding a 360-degree panoramic view of the entire\n" +
                "reconnaissance precinct. At the center stands an obsidian-glass war-table with magnetic levitation\n" +
                "holoscreens projecting orbital paths, threat matrices, and the synchronized heartbeats of all four sub-stations.\n" +
                "A hardwired emergency landline sits under a locked acrylic shield for direct command aborts.";

            OfficeAscii = 
@"             ._____________________________________________________________.
            /                   VANGUARD-1 :: COMMAND DAIS                  \
           |                              .-----.                            |
           |                            /  .-.    \                          |
           |                           |  | O |    |  <-- [HOLOPROJECTOR]    |
           |                            \  '-'    /                          |
           |                        .____`-------'____.                      |
           |                       /  TACTICAL WAR-MAP \                     |
           |                      | [GLOBAL RECON GRID] |                    |
           |                      | [PRIORITY: ALPHA-1] |                    |
           |           .__________|_____________________|__________.         |
           |          / [DIRECT LINE]  [AUTH-STAMP]  [TEAM STATUS] \         |
           |         |   SECURE COMM    KILL-KEY       5/5 ACTIVE   |        |
           |         \______________________________________________/        |
           |           | |                                       | |         |
           |           |_|=== ELEVATED HYDRAULIC COMMAND BASE ===|_|         |
           \_________________________________________________________________/";

            Appearance = "Tall silhouette clad in a dark tactical trench coat with woven carbon threads; an amber monocle HUD reflects real-time telemetry while their right gauntlet controls the orbital holo-globe.";
            Behavior = "Analytical, calm under intense operational pressure, speaks in measured tactical directives; cuts through noise to prioritize high-value intelligence.";
            CollaborationStyle = "Coordinates the investigation lifecycle: tasks Flow with pipeline assembly, orders Security to audit exposure, reviews Researcher's telemetry, and validates Documentation's final brief.";

            Equipment.Add("Volumetric 3D Holographic Tactical Globe (Orbital Feeds)");
            Equipment.Add("Encrypted Dedicated Red Comm-Terminal (Operator Line)");
            Equipment.Add("Cryptographic Authorization Keyring & Mission Seal");
            Equipment.Add("Multi-Channel Master Agent HUD (Synapse-V4)");
            Equipment.Add("Pneumatic Elevation Controller for Command Dais");
        }

        public override string RespondToInquiry(string query)
        {
            query = (query ?? "").ToLower();
            if (query.Contains("target") || query.Contains("mission"))
            {
                return "Vanguard-1: 'Every mission begins with a clear scope. Give our researcher a target—be it an IP, domain, username, or organization—and I will authorize Flow to spin up our reconnaissance pipeline.'";
            }
            if (query.Contains("security") || query.Contains("aegis") || query.Contains("safe"))
            {
                return "Vanguard-1: 'Aegis-9 keeps us invisible. I never greenlight active probing until Security gives the all-clear on proxy integrity and canary tripwires.'";
            }
            if (query.Contains("team") || query.Contains("who") || query.Contains("agent"))
            {
                return "Vanguard-1: 'We operate as a single tactical organism: Researcher uncovers raw truth, Flow routes the telemetry at wire-speed, Security insulates our footprint, Documentation records undeniable proof, and I keep our compass true.'";
            }
            return "Vanguard-1: 'Stay sharp, Operator. The digital landscape never forgets and never sleeps. What are your operational orders?'";
        }
    }
}

