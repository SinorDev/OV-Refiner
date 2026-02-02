using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OVRefiner.Models;

namespace OVRefiner.Services
{
    public class DnsService
    {
        private readonly HttpClient _httpClient;
        private string _dohUrl;
        private bool _useBase64Query;

        public DnsService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }

        public void Configure(string url, bool useBase64)
        {
            _dohUrl = url;
            _useBase64Query = useBase64;
        }

        public async Task<List<string>> ResolveDomainAsync(string domain)
        {
            if (_useBase64Query)
            {
                return await ResolveViaBinaryDnsAsync(domain);
            }
            else
            {
                return await ResolveViaJsonAsync(domain);
            }
        }

        // --- Method 1: Standard JSON API (Google/Cloudflare/Custom JSON) ---
        private async Task<List<string>> ResolveViaJsonAsync(string domain)
        {
            var ips = new HashSet<string>();
            try
            {
                string queryUrl = $"{_dohUrl}?name={domain}&type=A";
                var request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/dns-json"));

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string jsonContent = await response.Content.ReadAsStringAsync();
                var dnsData = JsonConvert.DeserializeObject<DohResponse>(jsonContent);

                if (dnsData != null && dnsData.Answer != null)
                {
                    foreach (var record in dnsData.Answer)
                    {
                        if (record.Type == 1 && !string.IsNullOrWhiteSpace(record.Data))
                        {
                            ips.Add(record.Data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"JSON DNS Error for {domain}: {ex.Message}");
            }
            return ips.ToList();
        }

        // --- Method 2: RFC 8484 Binary DNS (Base64 Url Encoded) ---
        private async Task<List<string>> ResolveViaBinaryDnsAsync(string domain)
        {
            var ips = new List<string>();
            try
            {
                // 1. Construct DNS Query Packet (Header + Question)
                byte[] dnsPacket = BuildDnsQueryPacket(domain);

                // 2. Base64Url Encode
                string base64Dns = Convert.ToBase64String(dnsPacket)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');

                // 3. Send Request
                // Note: Standard RFC 8484 uses /dns-query path usually, but we stick to user provided URL path + param
                // If user URL is "https://example.com", we append "/dns-query?dns=..." or just "?dns=..." depending on input?
                // Assuming user provides base URL like "https://doh.server/dns-query"
                string queryUrl = $"{_dohUrl}?dns={base64Dns}";
                
                var request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/dns-message"));

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();

                // 4. Parse Binary Response
                ips = ParseDnsResponse(responseBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Binary DNS Error for {domain}: {ex.Message}");
            }
            return ips;
        }

        // --- DNS Packet Helpers (Minimal Implementation) ---
        private byte[] BuildDnsQueryPacket(string domain)
        {
            List<byte> packet = new List<byte>();
            
            // Header (12 bytes)
            packet.AddRange(new byte[] { 0xAA, 0xBB }); // Transaction ID (Random)
            packet.AddRange(new byte[] { 0x01, 0x00 }); // Flags (Standard Query, Recursion Desired)
            packet.AddRange(new byte[] { 0x00, 0x01 }); // Questions: 1
            packet.AddRange(new byte[] { 0x00, 0x00 }); // Answer RRs: 0
            packet.AddRange(new byte[] { 0x00, 0x00 }); // Authority RRs: 0
            packet.AddRange(new byte[] { 0x00, 0x00 }); // Additional RRs: 0

            // Question Section
            // QNAME: Labels (e.g., 3www6google3com0)
            foreach (var part in domain.Split('.'))
            {
                packet.Add((byte)part.Length);
                packet.AddRange(Encoding.ASCII.GetBytes(part));
            }
            packet.Add(0x00); // Root null byte

            packet.AddRange(new byte[] { 0x00, 0x01 }); // QTYPE: A (IPv4)
            packet.AddRange(new byte[] { 0x00, 0x01 }); // QCLASS: IN

            return packet.ToArray();
        }

        private List<string> ParseDnsResponse(byte[] data)
        {
            // Disclaimer: This is a very simplified parser for A records only. 
            // Real-world DNS parsing requires handling pointers (compression), etc.
            List<string> result = new List<string>();
            
            if (data.Length < 12) return result;

            int qCount = (data[4] << 8) | data[5];
            int anCount = (data[6] << 8) | data[7];

            int pos = 12; // Skip Header

            // Skip Questions
            for (int i = 0; i < qCount; i++)
            {
                while (pos < data.Length && data[pos] != 0) // Skip labels
                {
                    if ((data[pos] & 0xC0) == 0xC0) { pos += 2; goto QuestionEnded; } // Compression pointer
                    pos += data[pos] + 1;
                }
                pos++; // Skip null byte
                QuestionEnded:
                pos += 4; // Skip QTYPE and QCLASS
            }

            // Read Answers
            for (int i = 0; i < anCount; i++)
            {
                if (pos >= data.Length) break;

                // Name (skip)
                if ((data[pos] & 0xC0) == 0xC0) pos += 2; // Pointer
                else
                {
                    while (pos < data.Length && data[pos] != 0) pos += data[pos] + 1;
                    pos++;
                }

                int type = (data[pos] << 8) | data[pos + 1]; pos += 2;
                int _class = (data[pos] << 8) | data[pos + 1]; pos += 2;
                long ttl = (data[pos] << 24) | (data[pos + 1] << 16) | (data[pos + 2] << 8) | data[pos + 3]; pos += 4;
                int dataLen = (data[pos] << 8) | data[pos + 1]; pos += 2;

                if (type == 1 && dataLen == 4) // Type A (IPv4)
                {
                    string ip = $"{data[pos]}.{data[pos + 1]}.{data[pos + 2]}.{data[pos + 3]}";
                    result.Add(ip);
                }
                pos += dataLen;
            }

            return result.Distinct().ToList();
        }
    }
}