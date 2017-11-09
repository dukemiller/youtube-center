using Newtonsoft.Json;

namespace youtube_center.Models.Youtube
{
    public class Thumbnails
    {

        [JsonProperty("default")]
        public Default Default { get; set; }

        [JsonProperty("medium")]
        public Medium Medium { get; set; }

        [JsonProperty("high")]
        public High High { get; set; }
    }
}