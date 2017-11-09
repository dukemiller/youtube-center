using Newtonsoft.Json;

namespace youtube_center.Models.Youtube
{
    public class Localized
    {

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}