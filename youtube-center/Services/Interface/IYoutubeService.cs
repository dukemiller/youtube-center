using System.Collections.Generic;
using System.Threading.Tasks;
using youtube_center.Models;

namespace youtube_center.Services.Interface
{
    public interface IYoutubeService
    {
        Task<IEnumerable<Video>> RetrieveVideos(Channel channel);
    }
}