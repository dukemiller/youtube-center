using Newtonsoft.Json;

namespace youtube_center.Models.Youtube
{
    public class Medium
    {

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}