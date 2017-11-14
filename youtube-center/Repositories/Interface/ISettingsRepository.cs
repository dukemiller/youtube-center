using youtube_center.Enums;

namespace youtube_center.Repositories.Interface
{
    public interface ISettingsRepository
    {
        /// <summary>
        ///     The action to commit when double clicking a video.
        /// </summary>
        DoubleClickAction DoubleClickAction { get; set; }

        /// <summary>
        ///     Play a sound on receiving new videos.
        /// </summary>
        bool SoundOnNew { get; set; }

        /// <summary>
        ///     Save to disk.
        /// </summary>
        void Save();
    }
}