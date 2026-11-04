using System;
using System.IO;
using System.Diagnostics;
using OsintHq.Engine;

namespace OsintHq.Workspace
{
    public static class HtmlWorkspaceGenerator
    {
        public static string GenerateAndOpenWorkspace()
        {
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workspace.html");
            string html = GetHtmlContent();

            File.WriteAllText(htmlPath, html);
            AnsiConsole.WriteLine("Visual Real-Time Workspace generated at:", ConsoleColor.Green);
            AnsiConsole.WriteLine("  " + Path.GetFullPath(htmlPath), ConsoleColor.White);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = htmlPath,
                    UseShellExecute = true
                });
                AnsiConsole.WriteLine("[ OK ] Opened Resplicon Cyber Citadel in your default browser.", ConsoleColor.Cyan);
            }
            catch
            {
                AnsiConsole.WriteLine("Could not automatically launch browser. Please open the file manually: " + htmlPath, ConsoleColor.Yellow);
            }

            return htmlPath;
        }

        private static string GetHtmlContent()
        {
            var p = DeviceProfileManager.Profile;
            string devId = p != null ? p.DeviceId : "RSP-CORE-001";
            string clearance = p != null ? p.ClearanceLevel : "STANDARD // TACTICAL OPERATOR";
            string country = p != null ? p.OperatorCountry + " [" + p.OperatorCountryCode + "]" : "Philippines [PH]";
            int splokens = p != null ? p.SplokensRemaining : 25000;

            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>RESPLICON // GLOBAL MULTI-AGENT CYBERSECURITY OSINT MATRIX</title>
<style>
  :root {
    --bg-dark: #050811;
    --panel-bg: rgba(11, 17, 32, 0.92);
    --border-color: #1e293b;
    --neon-blue: #00f0ff;
    --neon-green: #00ff66;
    --neon-red: #ff3366;
    --neon-yellow: #ffb800;
    --neon-purple: #c084fc;
    --text-primary: #e2e8f0;
    --text-dim: #94a3b8;
  }
  * { box-sizing: border-box; margin: 0; padding: 0; }
  body {
    background-color: var(--bg-dark);
    color: var(--text-primary);
    font-family: 'Segoe UI', system-ui, -apple-system, sans-serif;
    min-height: 100vh;
    overflow-x: hidden;
    background-image: 
      radial-gradient(circle at 50% 10%, rgba(0, 240, 255, 0.08) 0%, transparent 60%),
      linear-gradient(rgba(30, 41, 59, 0.25) 1px, transparent 1px),
      linear-gradient(90deg, rgba(30, 41, 59, 0.25) 1px, transparent 1px);
    background-size: 100% 100%, 36px 36px, 36px 36px;
  }
  header {
    background: rgba(8, 12, 22, 0.95);
    border-bottom: 1px solid var(--border-color);
    padding: 16px 32px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    position: sticky;
    top: 0;
    z-index: 100;
    backdrop-filter: blur(10px);
  }
  .brand { display: flex; align-items: center; gap: 14px; }
  .logo-icon {
    width: 44px; height: 44px;
    background: linear-gradient(135deg, #00f0ff, #ff3366);
    border-radius: 8px;
    display: flex; align-items: center; justify-content: center;
    font-weight: 900; font-size: 24px; color: #fff;
    box-shadow: 0 0 20px rgba(0, 240, 255, 0.4);
  }
  .brand-text h1 { font-size: 22px; font-weight: 900; letter-spacing: 2px; color: #fff; }
  .brand-text p { font-size: 11px; color: var(--neon-blue); letter-spacing: 1.5px; font-family: monospace; }
  .status-cluster { display: flex; gap: 14px; font-family: monospace; font-size: 12px; }
  .status-badge {
    padding: 6px 14px; border-radius: 6px; background: rgba(30, 41, 59, 0.6);
    border: 1px solid var(--border-color); display: flex; align-items: center; gap: 8px;
  }
  .dot { width: 8px; height: 8px; border-radius: 50%; background: var(--neon-green); box-shadow: 0 0 10px var(--neon-green); }
  .dot.pulse { animation: blink 1.2s infinite; }
  @keyframes blink { 0%, 100% { opacity: 1; } 50% { opacity: 0.3; } }

  .main-layout {
    display: grid;
    grid-template-columns: 1fr 440px;
    gap: 24px;
    padding: 28px;
    max-width: 1750px;
    margin: 0 auto;
  }

  .deck-container {
    background: var(--panel-bg);
    border: 1px solid var(--border-color);
    border-radius: 14px;
    padding: 24px;
    box-shadow: 0 12px 35px rgba(0,0,0,0.6);
  }
  .deck-header {
    display: flex; justify-content: space-between; align-items: center;
    margin-bottom: 24px; border-bottom: 1px solid rgba(255,255,255,0.06); padding-bottom: 16px;
  }
  .deck-header h2 { font-size: 18px; letter-spacing: 1.5px; color: var(--neon-blue); text-transform: uppercase; }

  /* FLOORPLAN GRID */
  .floorplan-grid {
    display: grid;
    grid-template-areas:
      ""supervisor supervisor security""
      ""researcher flow security""
      ""researcher flow docs"";
    grid-template-columns: 1.2fr 1fr 1.2fr;
    gap: 20px;
  }

  .station-card {
    background: rgba(14, 21, 38, 0.85);
    border: 1px solid var(--border-color);
    border-radius: 12px;
    padding: 20px;
    position: relative;
    cursor: pointer;
    transition: all 0.25s ease;
    overflow: hidden;
  }
  .station-card:hover {
    transform: translateY(-4px);
    box-shadow: 0 10px 30px rgba(0, 240, 255, 0.18);
    border-color: var(--neon-blue);
  }
  .station-card::before {
    content: '';
    position: absolute;
    top: 0; left: 0; width: 4px; height: 100%;
    background: var(--agent-accent, var(--neon-blue));
  }
  .supervisor-station { grid-area: supervisor; --agent-accent: var(--neon-purple); }
  .flow-station { grid-area: flow; --agent-accent: var(--neon-yellow); }
  .security-station { grid-area: security; --agent-accent: var(--neon-red); }
  .researcher-station { grid-area: researcher; --agent-accent: var(--neon-blue); }
  .docs-station { grid-area: docs; --agent-accent: var(--neon-green); }

  .station-top { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 12px; }
  .station-name { font-size: 17px; font-weight: 800; color: #fff; }
  .station-tag {
    font-size: 11px; font-family: monospace; padding: 3px 8px;
    border-radius: 4px; background: rgba(255,255,255,0.08); color: var(--agent-accent);
  }
  
  .stand-visual {
    background: rgba(6, 10, 18, 0.8);
    border: 1px dashed rgba(255, 255, 255, 0.15);
    border-radius: 8px;
    padding: 14px;
    margin: 14px 0;
    min-height: 95px;
  }
  .stand-label {
    font-size: 12px; font-family: monospace; color: var(--agent-accent);
    text-transform: uppercase; margin-bottom: 6px; display: flex; align-items: center; gap: 8px;
  }
  .stand-props { font-size: 12px; color: var(--text-dim); line-height: 1.5; }

  .station-status {
    display: flex; align-items: center; gap: 8px; font-size: 11px;
    font-family: monospace; color: var(--text-dim); margin-top: 14px;
  }
  .status-pulse { width: 8px; height: 8px; border-radius: 50%; background: var(--agent-accent); box-shadow: 0 0 8px var(--agent-accent); }

  /* SIDEBAR */
  .sidebar { display: flex; flex-direction: column; gap: 20px; }
  .panel {
    background: var(--panel-bg);
    border: 1px solid var(--border-color);
    border-radius: 12px;
    padding: 20px;
    box-shadow: 0 6px 20px rgba(0,0,0,0.4);
  }
  .panel h3 {
    font-size: 13px; text-transform: uppercase; letter-spacing: 1px;
    color: var(--neon-blue); margin-bottom: 12px; display: flex; align-items: center; gap: 8px;
  }
  .mission-input-group { display: flex; flex-direction: column; gap: 10px; }
  input[type=""text""], textarea {
    background: rgba(8, 12, 22, 0.9);
    border: 1px solid var(--border-color);
    border-radius: 6px;
    padding: 11px 14px;
    color: #fff;
    font-family: monospace;
    font-size: 13px;
    outline: none;
    transition: border 0.2s;
  }
  input[type=""text""]:focus, textarea:focus { border-color: var(--neon-blue); box-shadow: 0 0 12px rgba(0, 240, 255, 0.25); }
  .btn-primary {
    background: linear-gradient(135deg, #00f0ff, #0070f3);
    color: #000;
    font-weight: 800;
    font-size: 12px;
    letter-spacing: 1px;
    padding: 11px;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    transition: all 0.2s;
    text-transform: uppercase;
  }
  .btn-primary:hover { opacity: 0.9; transform: translateY(-1px); box-shadow: 0 4px 18px rgba(0, 240, 255, 0.4); }

  .terminal-box {
    background: #04060c;
    border: 1px solid #161e30;
    border-radius: 8px;
    padding: 14px;
    font-family: 'Consolas', 'Courier New', monospace;
    font-size: 12px;
    line-height: 1.5;
    color: #cbd5e1;
    height: 300px;
    overflow-y: auto;
  }
  .log-line { margin-bottom: 5px; word-break: break-all; }
  .log-time { color: #64748b; margin-right: 6px; }
  .log-agent { font-weight: bold; }

  /* MODAL */
  .modal-overlay {
    position: fixed; top: 0; left: 0; width: 100%; height: 100%;
    background: rgba(0,0,0,0.85); backdrop-filter: blur(8px);
    display: none; align-items: center; justify-content: center; z-index: 1000;
  }
  .modal-overlay.open { display: flex; }
  .modal-content {
    background: #0a0f1d; border: 1px solid var(--neon-blue);
    border-radius: 14px; width: 680px; max-width: 95%; padding: 28px;
    box-shadow: 0 0 50px rgba(0, 240, 255, 0.3); position: relative;
  }
  .close-modal {
    position: absolute; top: 18px; right: 22px; font-size: 24px;
    cursor: pointer; color: var(--text-dim); transition: color 0.2s;
  }
  .close-modal:hover { color: #fff; }
  .modal-badge {
    display: inline-block; font-size: 11px; font-family: monospace;
    padding: 3px 8px; border-radius: 4px; background: rgba(0, 240, 255, 0.15);
    color: var(--neon-blue); margin-bottom: 12px;
  }
  .modal-content h3 { font-size: 22px; margin-bottom: 6px; color: #fff; }
  .modal-section { margin-top: 16px; }
  .modal-section h4 { font-size: 12px; text-transform: uppercase; color: var(--neon-yellow); margin-bottom: 6px; }
  .modal-section p { font-size: 13px; color: var(--text-dim); line-height: 1.5; }
  .equip-list { list-style: none; margin-top: 8px; }
  .equip-list li { font-size: 12px; font-family: monospace; color: var(--neon-green); margin-bottom: 4px; }
</style>
</head>
<body>

<header>
  <div class=""brand"">
    <div class=""logo-icon"">⚡</div>
    <div class=""brand-text"">
      <h1>RESPLICON</h1>
      <p>GLOBAL MULTI-AGENT CYBERSECURITY OSINT MATRIX // SITE-09</p>
    </div>
  </div>
  <div class=""status-cluster"">
    <div class=""status-badge""><div class=""dot pulse""></div> OPERATOR: " + country + @"</div>
    <div class=""status-badge"" id=""splokBadge"" style=""color: var(--neon-yellow);"">SPLOKENS: " + splokens.ToString("N0") + @" / 25,000</div>
    <div class=""status-badge"">DEVICE: " + devId + @"</div>
    <div class=""status-badge"" id=""clearanceBadge"" style=""color: var(--neon-green);"">" + clearance + @"</div>
  </div>
</header>

<div class=""main-layout"">
  <div class=""deck-container"">
    <div class=""deck-header"">
      <h2>Real-Time Operations Citadel & Tangible Agent Offices</h2>
      <span style=""font-size: 12px; color: var(--text-dim);"">Click any office station to inspect physical stand & speak</span>
    </div>

    <div class=""floorplan-grid"">
      <div class=""station-card supervisor-station"" onclick=""openAgent('supervisor')"">
        <div class=""station-top"">
          <div class=""station-name"">Vanguard-1</div>
          <div class=""station-tag"">SUPERVISOR</div>
        </div>
        <div class=""stand-visual"">
          <div class=""stand-label"">🌐 Elevated Command Dais</div>
          <div class=""stand-props"">Volumetric 3D War-Table, Red Direct-Line Phone, Master Authorization Keyring</div>
        </div>
        <div class=""station-status"">
          <div class=""status-pulse""></div>
          <span>Active Overwatch & Mission Triage</span>
        </div>
      </div>

      <div class=""station-card security-station"" onclick=""openAgent('security')"">
        <div class=""station-top"">
          <div class=""station-name"">Aegis-9</div>
          <div class=""station-tag"">HEAD SECURITY</div>
        </div>
        <div class=""stand-visual"">
          <div class=""stand-label"">🛡️ Threat Isolation Vault</div>
          <div class=""stand-props"">Faraday Leaded Glass, Dual Physical Kill-Switches, Honeypot Tripwire Strobe</div>
        </div>
        <div class=""station-status"">
          <div class=""status-pulse""></div>
          <span>Faraday Cage Active // 0 Leaks</span>
        </div>
      </div>

      <div class=""station-card researcher-station"" onclick=""openAgent('researcher')"">
        <div class=""station-top"">
          <div class=""station-name"">Specter-4</div>
          <div class=""station-tag"">RESEARCHER</div>
        </div>
        <div class=""stand-visual"">
          <div class=""stand-label"">📡 Deep Recon Array</div>
          <div class=""stand-props"">5-Monitor Cockpit, Deep User Discovery Engine, Domain Tech Stack Fingerprinter</div>
        </div>
        <div class=""station-status"">
          <div class=""status-pulse""></div>
          <span>Probing Global Registries & Sockets</span>
        </div>
      </div>

      <div class=""station-card flow-station"" onclick=""openAgent('flow')"">
        <div class=""station-top"">
          <div class=""station-name"">Nexus-7</div>
          <div class=""station-tag"">FLOW</div>
        </div>
        <div class=""stand-visual"">
          <div class=""stand-label"">⚡ Core Routing Matrix</div>
          <div class=""stand-props"">Luminescent Optical Fiber Bus, Asynchronous Task Queue Scope, Hexagonal Patch Bay</div>
        </div>
        <div class=""station-status"">
          <div class=""status-pulse""></div>
          <span>Line-Speed Telemetry Pipeline</span>
        </div>
      </div>

      <div class=""station-card docs-station"" onclick=""openAgent('docs')"">
        <div class=""station-top"">
          <div class=""station-name"">Scribe-0</div>
          <div class=""station-tag"">DOCUMENTATION</div>
        </div>
        <div class=""stand-visual"">
          <div class=""stand-label"">📜 Cryptographic Archives Bureau</div>
          <div class=""stand-props"">High-Speed Pin-Feed Mechanical Teletype, Brass Cryptographic Seal, Case Docket Racks</div>
        </div>
        <div class=""station-status"">
          <div class=""status-pulse""></div>
          <span>Automated Markdown & HTML Dossier Engine</span>
        </div>
      </div>
    </div>
  </div>

  <div class=""sidebar"">
    <div class=""panel"">
      <h3>🎯 Launch Squad Reconnaissance</h3>
      <div class=""mission-input-group"">
        <input type=""text"" id=""targetInput"" placeholder=""Enter target (IP, Domain, Global Phone, Alias)"" value=""shopify.com"">
        <button class=""btn-primary"" onclick=""runMission()"">Deploy 5-Agent Squad (-650 SPLOK)</button>
      </div>
    </div>

    <div class=""panel"">
      <h3 style=""color: var(--neon-yellow);"">🔍 Domain Tech Stack Fingerprinter</h3>
      <div class=""mission-input-group"">
        <input type=""text"" id=""techDomainInput"" placeholder=""Enter domain to expose tech stack..."" value=""shopify.com"">
        <button class=""btn-primary"" style=""background: linear-gradient(135deg, #ffb800, #ff7700);"" onclick=""runTechStackAudit()"">Expose Tech Stack (-110 SPLOK)</button>
      </div>
    </div>

    <div class=""panel"">
      <h3 style=""color: var(--neon-blue);"">👤 Deep Username Facts Harvester</h3>
      <div class=""mission-input-group"">
        <input type=""text"" id=""userInput"" placeholder=""Enter username (e.g. torvalds)..."" value=""torvalds"">
        <button class=""btn-primary"" style=""background: linear-gradient(135deg, #00f0ff, #7928ca); color: #fff;"" onclick=""runUserHarvest()"">Harvest Interesting Facts (-220 SPLOK)</button>
      </div>
    </div>

    <div class=""panel"">
      <h3 style=""color: var(--neon-red);"">🎣 Phishing & Hacking Forensic Audit</h3>
      <div class=""mission-input-group"">
        <input type=""text"" id=""phishInput"" placeholder=""Enter suspect URL or host..."" value=""https://secure-login-paypal.verify-account.xyz"">
        <button class=""btn-primary"" style=""background: linear-gradient(135deg, #ff3366, #ff7b00);"" onclick=""runPhishingAudit()"">Run Phishing Audit (-250 SPLOK)</button>
      </div>
    </div>

    <div class=""panel"">
      <h3>📟 Real-Time Intercom & Telemetry</h3>
      <div class=""terminal-box"" id=""logTerminal"">
        <div class=""log-line""><span class=""log-time"">[00:00:01]</span> <span class=""log-agent"" style=""color:#c084fc"">VANGUARD-1:</span> Resplicon Citadel online. Stands manned.</div>
        <div class=""log-line""><span class=""log-time"">[00:00:02]</span> <span class=""log-agent"" style=""color:#ff3366"">AEGIS-9:</span> Operator Country: " + country + @". Faraday cage active.</div>
        <div class=""log-line""><span class=""log-time"">[00:00:03]</span> <span class=""log-agent"" style=""color:#ffb800"">NEXUS-7:</span> Sploken Ledger: " + splokens.ToString("N0") + @" SPLOK. Concurrency DAG ready.</div>
        <div class=""log-line""><span class=""log-time"">[00:00:04]</span> <span class=""log-agent"" style=""color:#00f0ff"">SPECTER-4:</span> Tech Stack Detector & Deep Username Harvester online.</div>
      </div>
    </div>
  </div>
</div>

<div class=""modal-overlay"" id=""agentModal"">
  <div class=""modal-content"">
    <span class=""close-modal"" onclick=""closeAgent()"">&times;</span>
    <span class=""modal-badge"" id=""mBadge"">SUPERVISOR</span>
    <h3 id=""mName"">Vanguard-1</h3>
    <p id=""mRole"" style=""color: var(--neon-blue); font-size: 13px; font-family: monospace;"">Mission Director</p>

    <div class=""modal-section"">
      <h4>Physical Workstation Architecture</h4>
      <p id=""mOffice"">Office details...</p>
    </div>

    <div class=""modal-section"">
      <h4>Operator Presence & Behavior</h4>
      <p id=""mBio"">Bio details...</p>
    </div>

    <div class=""modal-section"">
      <h4>Deployed Stand Equipment & Instruments</h4>
      <ul class=""equip-list"" id=""mEquip""></ul>
    </div>
  </div>
</div>

<script>
let splokens = " + splokens + @";

function updateSplokens(deduct) {
  splokens = Math.max(0, splokens - deduct);
  document.getElementById('splokBadge').innerText = 'SPLOKENS: ' + splokens.toLocaleString() + ' / 25,000';
}

const agentData = {
  supervisor: {
    badge: 'STRATEGIC OVERWATCH',
    name: 'Vanguard-1 (Supervisor)',
    role: 'Strategic Mission Overwatch & Team Commander',
    office: 'An elevated circular mezzanine overlooking the intelligence precinct. An obsidian-glass console with a suspended volumetric 3D holographic globe displaying real-time orbital passes.',
    bio: 'Tall silhouette clad in a dark tactical trench coat with woven carbon threads; an amber monocle HUD reflects real-time telemetry.',
    equip: ['Volumetric 3D Holographic Tactical Globe', 'Encrypted Dedicated Red Comm-Terminal', 'Cryptographic Authorization Keyring & Mission Seal']
  },
  security: {
    badge: 'OPSEC WARDEN',
    name: 'Aegis-9 (Head Security)',
    role: 'Threat Isolation Architect & Counter-Surveillance',
    office: 'A fortified blast-proof isolation booth enclosed within lead-lined ballistic glass and double-layered Faraday mesh. Features physical circuit breakers for instant network severance.',
    bio: 'Broad-shouldered operative clad in carbon-composite ballistic armor with a mirrored red-tinted face shield. Deeply paranoid, protective, and cynical.',
    equip: ['Physical Dual-Pole Air-Gap Disconnect Lever', 'Faraday Shielded RF Spectrum Leak Detector', 'Honeypot & Phishing Deception Signature Engine']
  },
  flow: {
    badge: 'PIPELINE ORCHESTRATOR',
    name: 'Nexus-7 (Flow)',
    role: 'Telemetry Stream Engine & DAG Concurrency Router',
    office: 'A sunken hexagonal station embedded directly in the raised-access floor. High-density fiber-optic cables pulse with teal and gold light, terminating at optical patch bays.',
    bio: 'Agile figure in a dark-carbon jacket with glowing optical fiber lines running down the sleeves; sports a twin-lens neural data-visor flickering with raw hex payloads.',
    equip: ['Hexagonal Liquid-Cooled Optical Patch Array', 'Real-Time DAG Flow Visualizer & Latency Scope', 'High-Throughput Asynchronous Task Dispatcher']
  },
  researcher: {
    badge: 'INTELLIGENCE HUNTER',
    name: 'Specter-4 (Researcher)',
    role: 'Deep Reconnaissance & Multi-Source Target Crawling',
    office: 'A darkened alcove bathed in deep cyan light. A cockpit of five ultrawide high-resolution displays arcs around a long workstation with global telecom and tech stack signatures.',
    bio: 'Slender figure in a dark charcoal tactical hoodie with sleeves pushed up; a multi-band headset rests around their neck, and cyan monitor glare glints off wireframe spectacles.',
    equip: ['Five-Display Curved Panoramic Reconnaissance Console', 'Wappalyzer-Class Tech Stack Fingerprinter', 'Deep Identity Facts Synthesizer Engine']
  },
  docs: {
    badge: 'ARCHIVAL CHRONICLER',
    name: 'Scribe-0 (Documentation)',
    role: 'Intelligence Synthesis & Cryptographic Archival',
    office: 'An elevated dual-tier drafting stand constructed from brushed titanium and dark walnut. A vintage-style high-speed mechanical docket printer feeds continuous accordion paper across the desk.',
    bio: 'Poised figure in a minimalist graphite cyber-vest; wears thin wireframe spectacles with typographic HUD overlays; holds a titanium stylus for manual evidentiary annotations.',
    equip: ['High-Speed Mechanical Pin-Feed Teletype Printer', 'Automated Markdown & HTML Dossier Compiler', 'Brass Cryptographic Authentication Seal']
  }
};

function openAgent(id) {
  const a = agentData[id];
  if (!a) return;
  document.getElementById('mBadge').innerText = a.badge;
  document.getElementById('mName').innerText = a.name;
  document.getElementById('mRole').innerText = a.role;
  document.getElementById('mOffice').innerText = a.office;
  document.getElementById('mBio').innerText = a.bio;
  
  const el = document.getElementById('mEquip');
  el.innerHTML = '';
  a.equip.forEach(eq => {
    const li = document.createElement('li');
    li.innerText = '• ' + eq;
    el.appendChild(li);
  });
  document.getElementById('agentModal').classList.add('open');
}

function closeAgent() {
  document.getElementById('agentModal').classList.remove('open');
}

function log(agent, msg, color) {
  const box = document.getElementById('logTerminal');
  const d = new Date();
  const time = '[' + d.toTimeString().split(' ')[0] + ']';
  const div = document.createElement('div');
  div.className = 'log-line';
  div.innerHTML = `<span class=""log-time"">${time}</span> <span class=""log-agent"" style=""color:${color}"">${agent}:</span> ${msg}`;
  box.appendChild(div);
  box.scrollTop = box.scrollHeight;
}

function runMission() {
  const target = document.getElementById('targetInput').value.trim() || 'shopify.com';
  updateSplokens(650);
  log('SPLOKEN LEDGER', `-650 SPLOK (Autonomous Squad Mission) -> Balance: ${splokens.toLocaleString()} SPLOK`, '#ffb800');
  log('VANGUARD-1', `Locking target: [${target}]. Deploying squad.`, '#c084fc');
  
  setTimeout(() => {
    log('AEGIS-9', `Auditing OPSEC for [${target}]. Zero tripwires found. Egress masked.`, '#ff3366');
    log('NEXUS-7', `Spawning parallel DAG execution pipeline...`, '#ffb800');
  }, 800);

  setTimeout(() => {
    log('SPECTER-4', `Sweeping DNS records, GeoIP, Tech Stack & Persona for [${target}]...`, '#00f0ff');
  }, 1600);

  setTimeout(() => {
    log('SCRIBE-0', `Dossier compiled & cryptographically stamped: reports/DOSSIER_${target}.md`, '#00ff66');
    log('VANGUARD-1', `Mission accomplished. Evidence secured. Good work, team.`, '#c084fc');
  }, 2600);
}

function runTechStackAudit() {
  const dom = document.getElementById('techDomainInput').value.trim() || 'shopify.com';
  updateSplokens(110);
  log('SPLOKEN LEDGER', `-110 SPLOK (Tech Stack Exposure) -> Balance: ${splokens.toLocaleString()} SPLOK`, '#ffb800');
  log('SPECTER-4', `Inspecting HTTP headers & DOM signatures for: ${dom}`, '#00f0ff');
  
  setTimeout(() => {
    log('SPECTER-4', `[CMS/Platform]: Shopify | [Backend]: Ruby on Rails`, '#00f0ff');
    log('SPECTER-4', `[CDN/Proxy]: Cloudflare & Cloudflare WAF | [Frontend]: Tailwind CSS`, '#00f0ff');
    log('SCRIBE-0', `Tech Stack architecture profile verified & mapped.`, '#00ff66');
  }, 900);
}

function runUserHarvest() {
  const u = document.getElementById('userInput').value.trim() || 'torvalds';
  updateSplokens(220);
  log('SPLOKEN LEDGER', `-220 SPLOK (Identity Facts Harvest) -> Balance: ${splokens.toLocaleString()} SPLOK`, '#ffb800');
  log('SPECTER-4', `Querying multi-platform APIs for persona: @${u}`, '#00f0ff');
  
  setTimeout(() => {
    log('SPECTER-4', `[GitHub]: Linus Torvalds | Portland, OR | 330,390 followers | 12 repos`, '#00f0ff');
    log('SPECTER-4', `[Keybase]: Verified Cryptographic PGP identity & social proofs bound`, '#00f0ff');
    log('SPECTER-4', `[Steam/Telegram/Reddit/Twitter]: Public profiles confirmed`, '#00f0ff');
    log('SCRIBE-0', `Persona synthesized: 9/13 networks indexed with verified identity.`, '#00ff66');
  }, 1200);
}

function runPhishingAudit() {
  const url = document.getElementById('phishInput').value.trim();
  if (!url) return;
  updateSplokens(250);
  log('SPLOKEN LEDGER', `-250 SPLOK (Phishing Campaign Audit) -> Balance: ${splokens.toLocaleString()} SPLOK`, '#ffb800');
  log('AEGIS-9', `Deconstructing target URL: ${url}`, '#ff3366');
  
  setTimeout(() => {
    log('AEGIS-9', `[!] Deceptive Brand Impersonation detected: Spoofs PAYPAL service`, '#ff3366');
    log('AEGIS-9', `[!] High-Abuse TLD detected (.xyz). Phishing Probability: 78% [CRITICAL]`, '#ff3366');
    log('SCRIBE-0', `Logged to case docket under MITRE ATT&CK T1566 (Phishing Link).`, '#00ff66');
  }, 800);
}
</script>
</body>
</html>";
        }
    }
}
