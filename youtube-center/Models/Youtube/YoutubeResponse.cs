using System.Collections.Generic;
using Newtonsoft.Json;

namespace youtube_center.Models.Youtube
{
    public class YoutubeResponse
    {

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("pageInfo")]
        public PageInfo PageInfo { get; set; }

        [JsonProperty("items")]
        public IList<Item> Items { get; set; }
    }
}