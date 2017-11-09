using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using youtube_center.Repositories.Interface;

namespace youtube_center.Models
{
    [Serializable]
    public class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("videos")]
        public List<Video> Videos { get; set; } = new List<Video>();

        // 

        [JsonIgnore]
        public string ThumbnailPath => Path.Combine(SettingsRepository.ImageDirectory, Id);
    }
}