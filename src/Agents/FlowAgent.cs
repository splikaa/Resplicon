using System;
using System.Collections.Generic;

namespace OsintHq.Agents
{
    public class FlowAgent : AgentBase
    {
        public FlowAgent()
        {
            Name = "Flow";
            Codename = "Nexus-7";
            Role = "Pipeline Orchestrator & Telemetry Stream Engine";
            Color = ConsoleColor.Yellow;
            OfficeName = "The Core Routing Matrix & Conduit Hub";
            Status = "ONLINE // ZERO-DROP PIPELINE SYNCHRONIZED";

            OfficeLayout = 
                "A sunken hexagonal station anchored into the floor grating. Thick bundles of luminescent optical\n" +
                "fiber weave through transparent liquid-cooled conduit tubes below the desk. Suspended curved glass displays\n" +
                "render live directed acyclic graphs (DAG), asynchronous query queues, and real-time data ingestion streams.\n" +
                "A modular patch-bay lets Nexus-7 hot-swap routing topologies in milliseconds.";

            OfficeAscii = 
@"             ._____________________________________________________________.
            /                   NEXUS-7 :: ROUTING MATRIX                   \
           |   /\_/\_/\_/\_/\_/\_/\_/\_/\_/\_/\_/\_/\_/\_/\_/\_/\_/\_/\_/\  |
           |  [======== OPTICAL FIBER CONDUIT // 100 Gbps FLOW ========]    |
           |  +--------------------+     +-------------------------------+  |
           |  | QUEUE: 00 PENDING  | --> | PIPELINE: ACTIVE [DAG-ONLINE] |  |
           |  | BUF  : 1024 KB     |     | LATENCY : 1.4 ms              |  |
           |  +--------------------+     +-------------------------------+  |
           |             |                               |                  |
           |      [RESEARCH HOOK]                 [SECURITY FILTER]         |
           |             |                               |                  |
           |  +----------------------------------------------------------+  |
           |  | [PATCH MATRIX]  [ASYNC THREADPOOL]  [CHUNKER ENGINE]     |  |
           |  | [O][O][O][O][O] [O][O][O][O][O]     [O][O][O][O][O]      |  |
           |  +----------------------------------------------------------+  |
           |   \___________/  [HEXAGONAL COOLING GRATE]  \____________/     |
           \_________________________________________________________________/";

            Appearance = "Agile figure wearing a sleek dark-carbon jacket with glowing amber optical fiber lines running down the sleeves; sports a twin-lens neural data-visor flickering with raw hex payloads.";
            Behavior = "High-energy, speaks rapidly in networking metaphors; abhors bottlenecks and unhandled exceptions; treats intelligence gathering like an optimized asynchronous stream.";
            CollaborationStyle = "Receives target directives from Supervisor, routes requests through Security's sanitizers, distributes scraping threads to Researcher, aggregates data packets, and pipes the output directly to Documentation.";

            Equipment.Add("Hexagonal Liquid-Cooled Optical Patch Array");
            Equipment.Add("Real-Time DAG Flow Visualizer & Latency Scope");
            Equipment.Add("High-Throughput Asynchronous Task Dispatcher");
            Equipment.Add("Dynamic Buffer Routing Switch (Brocade Cyber-V)");
            Equipment.Add("Direct Low-Latency Bus to Documentation & Security");
        }

        public override string RespondToInquiry(string query)
        {
            query = (query ?? "").ToLower();
            if (query.Contains("pipeline") || query.Contains("flow") || query.Contains("speed"))
            {
                return "Nexus-7: 'Pipelines are hot and clean! Target inputs hit Security first, branch into Researcher's 8 concurrent threads, and stream directly into Scribe's printer with zero packet loss.'";
            }
            if (query.Contains("queue") || query.Contains("task") || query.Contains("status"))
            {
                return "Nexus-7: 'Task queues are clear. All worker threads are warm. Feed me a target and watch the data cascade through our routing matrix.'";
            }
            if (query.Contains("research") || query.Contains("specter"))
            {
                return "Nexus-7: 'Specter-4 is our workhorse. When I spin up parallel sockets, Researcher drains remote endpoints faster than firewalls can notice.'";
            }
            return "Nexus-7: 'Packets routed, pipelines primed. Keep the data moving—latency is the enemy!'";
        }
    }
}

