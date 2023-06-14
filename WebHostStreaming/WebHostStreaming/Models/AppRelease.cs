using Newtonsoft.Json;

namespace WebHostStreaming.Models
{
    public class AppRelease
    {
        public string Name { get; set; }

        public ReleaseAsset[] Assets { get; set; }
    }

    public class ReleaseAsset
    {
        public string Name { get; set; }

        [JsonProperty("browser_download_url")]
        public string Url { get; set; }
    }
}
