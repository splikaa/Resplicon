using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace OsintHq.Engine
{
    public class DeviceProfile
    {
        public string DeviceId { get; set; }
        public string MachineName { get; set; }
        public string OperatingSystem { get; set; }
        public int CoreCount { get; set; }
        public string FirstRegisteredUtc { get; set; }
        public string LastSessionUtc { get; set; }
        public int TotalLaunches { get; set; }
        public int TotalMissions { get; set; }
        public int TotalQueries { get; set; }
        public string ClearanceLevel { get; set; }
        public bool SecretUnlocked { get; set; }
        public string AppliedCodeHash { get; set; }

        // Splokens (Resplicon Token Economics - 25,000 user limit)
        public int SplokensRemaining { get; set; }
        public int SplokensConsumed { get; set; }

        // Operator Geo-Location & Public Egress
        public string OperatorCountry { get; set; }
        public string OperatorCountryCode { get; set; }
        public string OperatorCity { get; set; }
        public string OperatorIsp { get; set; }
        public string OperatorPublicIp { get; set; }

        public DeviceProfile()
        {
            SplokensRemaining = 25000;
            SplokensConsumed = 0;
            OperatorCountry = "Detecting...";
            OperatorCountryCode = "GL";
            OperatorCity = "Unknown";
            OperatorIsp = "Direct Network";
            OperatorPublicIp = "0.0.0.0";
        }
    }

    public static class DeviceProfileManager
    {
        private static readonly string ConfigPath = Path.Combine("config", "resplicon_device.json");
        private static readonly JavaScriptSerializer _json = new JavaScriptSerializer();
        private static DeviceProfile _currentProfile;

        private static readonly string[] SecretCodeHashes = new string[]
        {
            "b07d98ddda7e640dfbb6d38261c4776a720b9e1eb2b87d2e4a38e874fc1d0ca2", // RESPLICON-PRIME
            "20c1ce8672bbedad06f37e657622ffb2ee2459c966967ec57820e932c99330ec", // BLACKICE-99
            "6d0e2dd467f3ef93dad53f34f2b1357d2c4506a834b6e95b6e5699bba675a11a", // SHADOW-ACCESS
            "6ee43b5d5c098a525af86ed9b91522a8c524e070a63c4785c5f7bd6c1424bfd6", // OVERRIDE-ALPHA
            "5a92d827327ddf0dc563716b6971c04f417e5214a8708aa1dd88d799887c662b"  // RESPLICON-2026
        };

        public static DeviceProfile Profile
        {
            get
            {
                if (_currentProfile == null) LoadOrCreateProfile();
                return _currentProfile;
            }
        }

        public static void Initialize()
        {
            LoadOrCreateProfile();
            _currentProfile.TotalLaunches++;
            _currentProfile.LastSessionUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC";

            // If Splokens is 0 on legacy profile, set default 25,000
            if (_currentProfile.SplokensRemaining <= 0 && _currentProfile.SplokensConsumed == 0)
            {
                _currentProfile.SplokensRemaining = 25000;
            }

            // Real-time Country Detection on Startup
            DetectOperatorLocation();
            SaveProfile();
        }

        public static void DetectOperatorLocation()
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.Create("http://ip-api.com/json/?fields=status,country,countryCode,city,regionName,isp,query");
                req.Timeout = 3500;
                req.UserAgent = "Resplicon/2.0-GeoAudit";
                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var reader = new StreamReader(resp.GetResponseStream()))
                {
                    string json = reader.ReadToEnd();
                    var dict = _json.Deserialize<System.Collections.Generic.Dictionary<string, object>>(json);
                    if (dict.ContainsKey("status") && dict["status"].ToString() == "success")
                    {
                        _currentProfile.OperatorCountry = dict.ContainsKey("country") ? dict["country"].ToString() : "Philippines";
                        _currentProfile.OperatorCountryCode = dict.ContainsKey("countryCode") ? dict["countryCode"].ToString() : "PH";
                        _currentProfile.OperatorCity = dict.ContainsKey("city") ? dict["city"].ToString() : "Local";
                        _currentProfile.OperatorIsp = dict.ContainsKey("isp") ? dict["isp"].ToString() : "ISP";
                        _currentProfile.OperatorPublicIp = dict.ContainsKey("query") ? dict["query"].ToString() : "0.0.0.0";
                    }
                }
            }
            catch
            {
                if (string.IsNullOrEmpty(_currentProfile.OperatorCountry) || _currentProfile.OperatorCountry == "Detecting...")
                {
                    _currentProfile.OperatorCountry = "Philippines";
                    _currentProfile.OperatorCountryCode = "PH";
                    _currentProfile.OperatorCity = "Manila";
                }
            }
        }

        public static bool DeductSplokens(int cost, string reason, out int remaining)
        {
            if (Profile.SplokensRemaining < cost)
            {
                remaining = Profile.SplokensRemaining;
                AnsiConsole.WriteLine("\n[!] INSUFFICIENT SPLOKENS: Required " + cost + " SPLOK. Current balance: " + remaining + " SPLOK.", ConsoleColor.Red);
                AnsiConsole.WriteLine("    Enter an unlisted authorization code to recharge your token allocation.", ConsoleColor.Yellow);
                return false;
            }

            Profile.SplokensRemaining -= cost;
            Profile.SplokensConsumed += cost;
            remaining = Profile.SplokensRemaining;
            SaveProfile();

            // Print real-time Sploken transaction ledger
            AnsiConsole.Write("  [SPLOKEN LEDGER] ", ConsoleColor.DarkCyan);
            AnsiConsole.Write("-" + cost + " SPLOK ", ConsoleColor.Yellow);
            AnsiConsole.Write("(" + reason + ") ", ConsoleColor.DarkGray);
            AnsiConsole.Write("-> Balance: ", ConsoleColor.DarkCyan);
            AnsiConsole.WriteLine(remaining.ToString("N0") + " / 25,000 SPLOK", ConsoleColor.Green);
            return true;
        }

        public static void RefillSplokens(int amount = 25000)
        {
            Profile.SplokensRemaining = amount;
            SaveProfile();
            AnsiConsole.WriteLine("\n[+] SPLOKEN BALANCE RECHARGED: 25,000 SPLOK GRANTED.", ConsoleColor.Green);
        }

        private static void LoadOrCreateProfile()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    _currentProfile = _json.Deserialize<DeviceProfile>(json);
                    if (_currentProfile != null && !string.IsNullOrEmpty(_currentProfile.DeviceId))
                    {
                        if (_currentProfile.SplokensRemaining <= 0 && _currentProfile.SplokensConsumed == 0)
                        {
                            _currentProfile.SplokensRemaining = 25000;
                        }
                        return;
                    }
                }
            }
            catch { }

            string rawHw = Environment.MachineName + "|" + Environment.UserName + "|" +
                           Environment.OSVersion.VersionString + "|" + Environment.ProcessorCount + "|" + GetMacSignature();

            string deviceId = "RSP-" + ComputeHash(rawHw).Substring(0, 16).ToUpper();

            _currentProfile = new DeviceProfile
            {
                DeviceId = deviceId,
                MachineName = Environment.MachineName,
                OperatingSystem = Environment.OSVersion.VersionString,
                CoreCount = Environment.ProcessorCount,
                FirstRegisteredUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC",
                LastSessionUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC",
                TotalLaunches = 0,
                TotalMissions = 0,
                TotalQueries = 0,
                ClearanceLevel = "STANDARD // TACTICAL OPERATOR",
                SecretUnlocked = false,
                AppliedCodeHash = "",
                SplokensRemaining = 25000,
                SplokensConsumed = 0
            };

            SaveProfile();
        }

        private static string GetMacSignature()
        {
            try
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        return nic.GetPhysicalAddress().ToString();
                    }
                }
            }
            catch { }
            return "DEFAULT-NIC-SIG";
        }

        public static void RecordMission()
        {
            Profile.TotalMissions++;
            SaveProfile();
        }

        public static void RecordQuery()
        {
            Profile.TotalQueries++;
            SaveProfile();
        }

        public static bool TryApplySecretCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return false;
            string clean = code.Trim().ToUpper();
            string hash = ComputeHash(clean);

            if (clean == "REFILL-25K" || clean == "SPLOK-REFILL" || clean == "RESET-TOKENS")
            {
                RefillSplokens(25000);
                return true;
            }

            foreach (var h in SecretCodeHashes)
            {
                if (h.Equals(hash, StringComparison.OrdinalIgnoreCase))
                {
                    Profile.SecretUnlocked = true;
                    Profile.ClearanceLevel = "BLACK-OPS // RESPLICON PRIME ACCESS (ELEVATED)";
                    Profile.AppliedCodeHash = hash.Substring(0, 12);
                    Profile.SplokensRemaining = 25000;
                    SaveProfile();
                    return true;
                }
            }

            if (clean == "RESPLICON-PRIME" || clean == "BLACKICE-99" || clean == "SHADOW-ACCESS" || clean == "OVERRIDE-ALPHA" || clean == "RESPLICON-2026")
            {
                Profile.SecretUnlocked = true;
                Profile.ClearanceLevel = "BLACK-OPS // RESPLICON PRIME ACCESS (ELEVATED)";
                Profile.AppliedCodeHash = hash.Substring(0, 12);
                Profile.SplokensRemaining = 25000;
                SaveProfile();
                return true;
            }

            if (clean.StartsWith("RSP-") && clean.Length >= 10)
            {
                Profile.SecretUnlocked = true;
                Profile.ClearanceLevel = "BLACK-OPS // RESPLICON CUSTOM ACCESS";
                Profile.AppliedCodeHash = hash.Substring(0, 12);
                Profile.SplokensRemaining = 25000;
                SaveProfile();
                return true;
            }

            return false;
        }

        private static void SaveProfile()
        {
            try
            {
                string dir = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string json = _json.Serialize(_currentProfile);
                File.WriteAllText(ConfigPath, json);
            }
            catch { }
        }

        public static string ComputeHash(string input)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
