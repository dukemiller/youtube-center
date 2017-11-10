using System.Collections.Generic;
using System.Threading.Tasks;
using youtube_center.Models;

namespace youtube_center.Services.Interface
{
    public interface IYoutubeService
    {
        /// <summary>
        ///     Retrieve the last ~20? videos for the given channel.
        /// </summary>
        Task<IEnumerable<Video>> RetrieveVideos(Channel channel);

        /// <summary>
        ///     Find details from the given channel url.
        /// </summary>
        Task<(bool successful, string username, string id)> FindDetails(string channelUrl);

        /// <summary>
        ///     Download the thumbnails locally for every video on the channel, if they dont exist.
        /// </summary>
        Task ThumbnailCheck(Channel channel, IEnumerable<Video> videos);
    }
}