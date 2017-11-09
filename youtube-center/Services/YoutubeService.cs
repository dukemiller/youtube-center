using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using youtube_center.Models;
using youtube_center.Services.Interface;

namespace youtube_center.Services
{
    public class YoutubeService: IYoutubeService
    {
        public async Task<IEnumerable<Video>> RetrieveVideos(Channel channel)
        {
            var document = new HtmlDocument();

            var url = new Uri($@"https://www.youtube.com/feeds/videos.xml?channel_id={channel.Id}");

            using (var client = new WebClient())
            {
                var xml = await client.DownloadStringTaskAsync(url);
                document.LoadHtml(xml);
            }

            return document.DocumentNode.SelectNodes("//entry").Select(ToVideo);
        }

        public Video ToVideo(HtmlNode node)
        {
            var id = node.SelectSingleNode(".//*[name()='yt:videoid']").InnerText;

            var title = node.SelectSingleNode(".//*[name()='media:title']").InnerText;

            var author = new Author
            {
                Name = node.SelectSingleNode("author/name").InnerText,
                ChannelUrl = node.SelectSingleNode("author/uri").InnerText
            };

            var description = node.SelectSingleNode(".//*[name()='media:description']").InnerText;

            var views = int.TryParse(node.SelectSingleNode(".//*[name()='media:statistics']").Attributes["views"].Value, out var viewsResult)
                ? viewsResult
                : 0;

            var uploaded = DateTime.TryParse(node.SelectSingleNode("published").InnerText, out var uploadedResult)
                ? uploadedResult
                : DateTime.MinValue;

            var updated = DateTime.TryParse(node.SelectSingleNode("updated").InnerText, out var updatedResult)
                ? updatedResult
                : DateTime.MinValue;

            var thumbnailNode = node.SelectSingleNode(".//*[name()='media:thumbnail']");

            var thumbnail = new Thumbnail
            {
                Url = thumbnailNode.Attributes["url"].Value,
                Width = thumbnailNode.Attributes["width"].Value,
                Height = thumbnailNode.Attributes["height"].Value
            };

            return new Video
            {
                Id = id,
                Title = title,
                Author = author,
                Description = description,
                Views = views,
                Uploaded = uploaded,
                Updated = updated,
                Thumbnail = thumbnail
            };
        }


    }
}