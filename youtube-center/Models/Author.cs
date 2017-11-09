using System;
using Newtonsoft.Json;

namespace youtube_center.Models
{
    [Serializable]
    public class Author
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string ChannelUrl { get; set; }
    }
}