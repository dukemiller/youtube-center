using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using youtube_center.Models;
using youtube_center.Repositories.Interface;

namespace youtube_center.Repositories
{
    [Serializable]
    public class ChannelRepository: IChannelRepository
    {
        /// <summary>
        ///     The path to the folder containing all settings and configuration files.
        /// </summary>
        [JsonIgnore]
        public static string ApplicationDirectory => Path.Combine(Environment
                .GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "youtube_center");

        [JsonIgnore]
        public static string ChannelsPath => Path.Combine(ApplicationDirectory, "channels.json");

        // 

        [JsonProperty("last_checked")]
        public DateTime LastChecked { get; set; }

        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; } = new List<Channel>();
        
        public void Save()
        {
            using (var stream = new StreamWriter(ChannelsPath))
                stream.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        // 

        public static ChannelRepository Load()
        {
            if (!Directory.Exists(ApplicationDirectory))
                Directory.CreateDirectory(ApplicationDirectory);

            if (File.Exists(ChannelsPath))
                using (var stream = new StreamReader(ChannelsPath))
                    return JsonConvert.DeserializeObject<ChannelRepository>(stream.ReadToEnd());

            return new ChannelRepository();
        }
    }
}