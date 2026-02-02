using Newtonsoft.Json;

namespace OVRefiner.Models
{
    // Standard Vmess JSON structure
    public class VmessJson
    {
        public string v { get; set; }
        public string ps { get; set; } // Remark/Name
        public string add { get; set; } // Address/Domain
        public string port { get; set; }
        public string id { get; set; }
        public string aid { get; set; }
        public string scy { get; set; }
        public string net { get; set; }
        public string type { get; set; }
        public string host { get; set; }
        public string path { get; set; }
        public string tls { get; set; }
        public string sni { get; set; }
        public string alpn { get; set; }
        public string fp { get; set; }
    }
}