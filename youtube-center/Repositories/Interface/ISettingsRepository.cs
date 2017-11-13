﻿using System;
using System.Collections.Generic;
using youtube_center.Enums;
using youtube_center.Models;

namespace youtube_center.Repositories.Interface
{
    public interface ISettingsRepository
    {
        /// <summary>
        ///     All channeled added by user and parsed back.
        /// </summary>
        List<Channel> Channels { get; set; }

        /// <summary>
        ///     Last time the video sources were checked.
        /// </summary>
        DateTime LastChecked { get; set; }

        DoubleClickAction DoubleClickAction { get; set; }

        bool SoundOnNew { get; set; }

        /// <summary>
        ///     Save to disk.
        /// </summary>
        void Save();
    }
}