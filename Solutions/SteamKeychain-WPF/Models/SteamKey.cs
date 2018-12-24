using Newtonsoft.Json;

namespace SteamKeychain.Models
{
    [JsonObject]
    public class SteamKey
    {
        public string title = default;
        public string storeUrl = default;
        public float basePrice = default;
        public string content = default;
        public bool hidden = default;
    }
}