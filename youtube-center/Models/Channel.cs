using System;
using System.IO;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using youtube_center.Repositories;

namespace youtube_center.Models
{
    [Serializable]
    public class Channel: ObservableObject
    {
        private string _id = string.Empty;

        private string _name = string.Empty;

        // 

        [JsonProperty("id")]
        public string Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }

        [JsonProperty("name")]
        public string Name
        {
            get => _name;
            set => Set(() => Name, ref _name, value);
        }

        // 

        [JsonIgnore]
        public string Url => $"https://www.youtube.com/channel/{Id}";

        [JsonIgnore]
        public string ThumbnailPath => Path.Combine(VideoRepository.ImageDirectory, Id);
    }
}