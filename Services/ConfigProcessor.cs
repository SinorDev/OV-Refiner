using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net; // Added for IPAddress check
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OVRefiner.Models;

namespace OVRefiner.Services
{
    public class ConfigProcessor
    {
        private readonly DnsService _dnsService;
        private readonly LoggerService _logger;

        // Internal buffers to hold results before writing
        private List<string> _generatedConfigs = new List<string>();
        private List<string> _originalConfigs = new List<string>();

        public ConfigProcessor(DnsService dnsService, LoggerService logger)
        {
            _dnsService = dnsService;
            _logger = logger;
        }

        public void ClearBuffers()
        {
            _generatedConfigs.Clear();
            _originalConfigs.Clear();
        }

        public async Task<bool> ProcessV2RayLineAsync(string line, bool includeOriginal)
        {
            if (string.IsNullOrWhiteSpace(line)) return true;

            string protocol = GetProtocol(line);
            if (protocol == "unknown") return true;

            string domain = ExtractDomain(line, protocol);
            string originalName = ExtractName(line, protocol);

            if (string.IsNullOrEmpty(domain))
            {
                _logger.LogWarning($"No domain found in config: {originalName}");
                return false;
            }

            // --- CHANGE START: Check if it's already an IP ---
            if (IsIpAddress(domain))
            {
                _logger.LogInfo($"Ignored (Already IP): {domain}");
                return true; // Skip processing, return success
            }
            // --- CHANGE END ---

            _logger.LogInfo($"Resolving: {domain} ({originalName})");

            List<string> ips;
            try
            {
                ips = await _dnsService.ResolveDomainAsync(domain);
            }
            catch (Exception ex)
            {
                _logger.LogError($"DNS Error [{domain}]: {ex.Message}");
                return false;
            }

            if (ips.Count == 0)
            {
                _logger.LogWarning($"No IPs returned for {domain}");
                return false;
            }

            // 1. Prepare Original (if requested)
            if (includeOriginal)
            {
                string taggedName = $"{originalName} | Domain";
                string finalConfig = RenameConfig(line, protocol, taggedName);
                _originalConfigs.Add(finalConfig);
            }

            // 2. Prepare Generated IPs
            if (ips.Count == 1)
            {
                string newConfig = ReplaceDomain(line, protocol, ips[0]);

                // USER REQUEST: Add "| IP" to name if single IP
                string newName = $"{originalName} | IP";
                newConfig = RenameConfig(newConfig, protocol, newName);

                _generatedConfigs.Add(newConfig);
            }
            else
            {
                for (int i = 0; i < ips.Count; i++)
                {
                    string ip = ips[i];
                    string newName = $"{originalName} | IP {i + 1}";
                    string configWithIp = ReplaceDomain(line, protocol, ip);
                    string finalConfig = RenameConfig(configWithIp, protocol, newName);
                    _generatedConfigs.Add(finalConfig);
                }
            }

            _logger.LogSuccess($"Resolved {domain} -> {ips.Count} IPs");
            return true;
        }

        public async Task SaveV2RayResultsAsync(string fullOutputPath)
        {
            // Write Generated First
            if (_generatedConfigs.Count > 0)
            {
                await File.AppendAllLinesAsync(fullOutputPath, _generatedConfigs, Encoding.UTF8);
            }

            // Write Originals at the END
            if (_originalConfigs.Count > 0)
            {
                await File.AppendAllTextAsync(fullOutputPath, $"\n", Encoding.UTF8);
                await File.AppendAllLinesAsync(fullOutputPath, _originalConfigs, Encoding.UTF8);
            }
        }

        public async Task<bool> ProcessOvpnFileAsync(string filePath, string outputFolder, bool includeOriginal)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string content = await File.ReadAllTextAsync(filePath);
                var match = Regex.Match(content, @"^remote\s+(?<domain>[\w\.-]+)\s+(?<port>\d+)",
                    RegexOptions.Multiline);

                if (!match.Success)
                {
                    _logger.LogError($"No remote line in {fileName}");
                    return false;
                }

                string domain = match.Groups["domain"].Value;

                // --- CHANGE START: Check if it's already an IP ---
                if (IsIpAddress(domain))
                {
                    _logger.LogInfo($"Ignored OVPN (Already IP): {fileName}");
                    return true;
                }
                // --- CHANGE END ---

                _logger.LogInfo($"Resolving OVPN: {domain}");

                var ips = await _dnsService.ResolveDomainAsync(domain);

                if (ips.Count == 0) return false;

                // Handle IPs
                if (ips.Count == 1)
                {
                    string ip = ips[0];
                    string newContent = content.Replace(domain, ip);

                    // USER REQUEST: Rename logic for single IP
                    string newFileName = $"{fileName} - IP.ovpn";

                    await File.WriteAllTextAsync(Path.Combine(outputFolder, newFileName), newContent, Encoding.UTF8);
                }
                else
                {
                    for (int i = 0; i < ips.Count; i++)
                    {
                        string ip = ips[i];
                        string newContent = content.Replace(domain, ip);
                        string newFileName = $"{fileName} - IP{i + 1}.ovpn";
                        await File.WriteAllTextAsync(Path.Combine(outputFolder, newFileName), newContent,
                            Encoding.UTF8);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing OVPN: {ex.Message}");
                return false;
            }
        }

        // --- Helpers ---

        // New Helper Method to check IP validity
        private bool IsIpAddress(string host)
        {
            return IPAddress.TryParse(host, out _);
        }

        private string GetProtocol(string line)
        {
            if (line.StartsWith("vmess://")) return "vmess";
            if (line.StartsWith("vless://")) return "vless";
            if (line.StartsWith("trojan://")) return "trojan";
            if (line.StartsWith("ss://")) return "ss";
            return "unknown";
        }

        private string ExtractDomain(string line, string protocol)
        {
            try
            {
                if (protocol == "vmess")
                {
                    string base64 = line.Substring(8);
                    string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                    var vmess = JsonConvert.DeserializeObject<VmessJson>(json);
                    return vmess.add;
                }
                else if (protocol == "vless" || protocol == "trojan")
                {
                    var match = Regex.Match(line, @"@(?<domain>[\w\.-]+):");
                    return match.Groups["domain"].Value;
                }
                else if (protocol == "ss")
                {
                    var match = Regex.Match(line, @"@(?<domain>[\w\.-]+):");
                    if (match.Success) return match.Groups["domain"].Value;
                    return "";
                }
            }
            catch
            {
            }

            return "";
        }

        private string ExtractName(string line, string protocol)
        {
            try
            {
                if (protocol == "vmess")
                {
                    string base64 = line.Substring(8);
                    string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                    var vmess = JsonConvert.DeserializeObject<VmessJson>(json);
                    return vmess.ps;
                }
                else
                {
                    int hashIndex = line.LastIndexOf('#');
                    if (hashIndex > -1) return System.Net.WebUtility.UrlDecode(line.Substring(hashIndex + 1));
                }
            }
            catch
            {
            }

            return "Unknown";
        }

        private string ReplaceDomain(string line, string protocol, string newIp)
        {
            if (protocol == "vmess")
            {
                string base64 = line.Substring(8);
                string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                var vmess = JsonConvert.DeserializeObject<VmessJson>(json);
                vmess.add = newIp;
                string newJson = JsonConvert.SerializeObject(vmess);
                return "vmess://" + Convert.ToBase64String(Encoding.UTF8.GetBytes(newJson));
            }
            else
            {
                return Regex.Replace(line, @"@([\w\.-]+):", $"@{newIp}:");
            }
        }

        private string RenameConfig(string line, string protocol, string newName)
        {
            if (protocol == "vmess")
            {
                string base64 = line.Substring(8);
                string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                var vmess = JsonConvert.DeserializeObject<VmessJson>(json);
                vmess.ps = newName;
                string newJson = JsonConvert.SerializeObject(vmess);
                return "vmess://" + Convert.ToBase64String(Encoding.UTF8.GetBytes(newJson));
            }
            else
            {
                int hashIndex = line.LastIndexOf('#');
                // Use EscapeDataString to handle spaces as %20
                if (hashIndex > -1) return line.Substring(0, hashIndex) + "#" + Uri.EscapeDataString(newName);
                else return line + "#" + Uri.EscapeDataString(newName);
            }
        }
    }
}