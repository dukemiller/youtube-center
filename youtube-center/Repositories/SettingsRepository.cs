using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using youtube_center.Enums;
using youtube_center.Models;
using youtube_center.Repositories.Interface;

namespace youtube_center.Repositories
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
        
        // 

        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; } = new List<Channel>();

        [JsonProperty("last_checked")]
        public DateTime LastChecked { get; set; }

        [JsonProperty("doubleclick_action")]
        public DoubleClickAction DoubleClickAction { get; set; }

        [JsonProperty("sound_on_new")]
        public bool SoundOnNew { get; set; } = true;

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