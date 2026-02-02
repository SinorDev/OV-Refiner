using System.Collections.Generic;
using Newtonsoft.Json;

namespace OVRefiner.Models
{
    // Model to deserialize JSON response from DoH providers (Google/Cloudflare)
    public class DohResponse
    {
        [JsonProperty("Status")]
        public int Status { get; set; }

        [JsonProperty("Answer")]
        public List<DnsAnswer> Answer { get; set; }
    }

    public class DnsAnswer
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; } // 1 = A Record (IPv4)

        [JsonProperty("data")]
        public string Data { get; set; } // The IP address
    }
}