using System.Collections;
using System.Collections.Generic;
using youtube_center.Models;

namespace youtube_center.Repositories.Interface
{
    public interface IVideoRepository
    {
        // channelid: Video[]
        Dictionary<string, IEnumerable<Video>> Videos { get; set; }

        IEnumerable<Video> VideosFor(Channel channel);

        /// <summary>
        ///     Save to disk.
        /// </summary>
        void Save();
    }
}