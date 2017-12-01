using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using youtube_center.Models;
using youtube_center.Repositories.Interface;

namespace youtube_center.Repositories
{
    [Serializable]
    public class VideoRepository: IVideoRepository
    {
        [JsonIgnore]
        public static string VideosPath => Path.Combine(SettingsRepository.ApplicationDirectory, "videos.json");

        [JsonIgnore]
        public static string ImageDirectory => Path.Combine(SettingsRepository.ApplicationDirectory, "thumbnails");
        
        [JsonProperty("videos")]
        public Dictionary<string, IEnumerable<Video>> Videos { get; set; } = new Dictionary<string, IEnumerable<Video>>();

        public IEnumerable<Video> VideosFor(Channel channel) => Videos.ContainsKey(channel.Id) ? Videos[channel.Id] : new List<Video>();

        public async Task Save()
        {
            using (var stream = new StreamWriter(VideosPath))
                await stream.WriteAsync(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        // 

        public static VideoRepository Load()
        {
            if (!Directory.Exists(SettingsRepository.ApplicationDirectory))
                Directory.CreateDirectory(SettingsRepository.ApplicationDirectory);

            if (File.Exists(VideosPath))
                using (var stream = new StreamReader(VideosPath))
                    return JsonConvert.DeserializeObject<VideoRepository>(stream.ReadToEnd());

            return new VideoRepository();
        }
    }
}