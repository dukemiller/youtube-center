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
        private string _url = string.Empty;

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

        [JsonProperty("url")]
        public string Url
        {
            get => _url;
            set => Set(() => Url, ref _url, value);
        }

        // 

        [JsonIgnore]
        public string ThumbnailPath => Path.Combine(VideoRepository.ImageDirectory, Id);
    }
}