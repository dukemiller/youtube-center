using System;
using Newtonsoft.Json;

namespace youtube_center.Models
{
    [Serializable]
    public class Thumbnail
    {
        [JsonProperty("path")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }
    }
}