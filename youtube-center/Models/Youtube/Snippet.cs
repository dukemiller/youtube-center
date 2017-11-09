using System;
using Newtonsoft.Json;

namespace youtube_center.Models.Youtube
{
    public class Snippet
    {

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("customUrl")]
        public string CustomUrl { get; set; }

        [JsonProperty("publishedAt")]
        public DateTime PublishedAt { get; set; }

        [JsonProperty("thumbnails")]
        public Thumbnails Thumbnails { get; set; }

        [JsonProperty("localized")]
        public Localized Localized { get; set; }
    }
}