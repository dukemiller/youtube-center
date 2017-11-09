using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using youtube_center.Models;

namespace youtube_center.Repositories.Interface
{
    [Serializable]
    public class SettingsRepository: ISettingsRepository
    {
        /// <summary>
        ///     The path to the folder containing all settings and configuration files.
        /// </summary>
        [JsonIgnore]
        public static string ApplicationDirectory => Path.Combine(Environment
                .GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "youtube_center");

        [JsonIgnore]
        public static string SettingsPath => Path.Combine(ApplicationDirectory, "settings.json");

        [JsonIgnore]
        public static string ImageDirectory => Path.Combine(ApplicationDirectory, "thumbnails");

        // 

        public List<Channel> Channels { get; set; } = new List<Channel>();

        public DateTime LastChecked { get; set; }

        public void Save()
        {
            using (var stream = new StreamWriter(SettingsPath))
                stream.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        // 

        public static SettingsRepository Load()
        {
            if (!Directory.Exists(ApplicationDirectory))
                Directory.CreateDirectory(ApplicationDirectory);

            if (File.Exists(SettingsPath))
                using (var stream = new StreamReader(SettingsPath))
                    return JsonConvert.DeserializeObject<SettingsRepository>(stream.ReadToEnd());

            return new SettingsRepository();
        }
        
    }
}