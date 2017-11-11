using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;
using youtube_center.Classes;
using youtube_center.Models;
using youtube_center.Models.Youtube;
using youtube_center.Services.Interface;

namespace youtube_center.Services
{
    public class YoutubeService: IYoutubeService
    {
        private readonly HttpClient _client;

        private static readonly Regex UserRegex = new Regex(@"https:\/\/www\.youtube\.com\/user\/(\w+)");

        private static readonly Regex IdRegex = new Regex(@"https:\/\/www\.youtube\.com\/channel\/(\w+)");

        private static readonly WebClient Downloader = new WebClient();

        //

        public YoutubeService()
        {
            _client = new HttpClient();
        }

        //

        public async Task<IEnumerable<Video>> RetrieveVideos(Channel channel)
        {
            try
            {
                var document = new HtmlDocument();
                var url = new Uri($@"https://www.youtube.com/feeds/videos.xml?channel_id={channel.Id}");
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var xml = await client.DownloadStringTaskAsync(url);
                    document.LoadHtml(xml);
                }
                return document.DocumentNode?.SelectNodes("//entry")?.Select(ToVideo) ?? new List<Video>();
            }

            catch
            {
                return new List<Video>();
            }
        }

        public async Task<(bool successful, string username, string id)> FindDetails(string channelUrl)
        {
            string q;

            // TODO: yeah wow make this better

            if (UserRegex.IsMatch(channelUrl))
                q = "forUsername=" + UserRegex.Match(channelUrl).Groups[1].Value;
            else if (IdRegex.IsMatch(channelUrl))
                q = "id=" + IdRegex.Match(channelUrl).Groups[1].Value;
            else
                return (false, "", "");

            try
            {
                var url = "https://www.googleapis.com/youtube/v3/channels" +
                          $"?key={ApiKeys.Youtube}" +
                          $"&{q}" +
                          "&part=id,snippet";

                var request = await _client.GetAsync(url);
                var json = await request.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<YoutubeResponse>(json);

                if (response.Items.Count == 0)
                    return (false, "", "");

                var id = response.Items.First().Id;
                var username = response.Items.First().Snippet.Title;

                return (true, username, id);
            }

            // TODO: yeah wow make this better x2
            catch
            {
                return (false, "", "");
            }

        }
        
        public async Task ThumbnailCheck(Channel channel, IEnumerable<Video> videos)
        {
            if (!Directory.Exists(channel.ThumbnailPath))
                Directory.CreateDirectory(channel.ThumbnailPath);

            foreach (var video in videos)
            {
                var image = video.Thumbnail.Url;
                var path = Path.Combine(channel.ThumbnailPath, $"{video.Id}.png");

                try
                {
                    if (!File.Exists(video.Thumbnail.Url))
                        await Downloader.DownloadFileTaskAsync(image, path);
                    video.Thumbnail.Url = path;
                }

                catch
                {
                    // TODO: Set a default image later    
                }
            }
        }

        // 

        private static Video ToVideo(HtmlNode node)
        {
            var id = node.SelectSingleNode(".//*[name()='yt:videoid']").InnerText;

            var title = HttpUtility.HtmlDecode(node.SelectSingleNode(".//*[name()='media:title']").InnerText);

            var author = new Author
            {
                Name = node.SelectSingleNode("author/name").InnerText,
                ChannelUrl = node.SelectSingleNode("author/uri").InnerText
            };

            var description = HttpUtility.HtmlDecode(node.SelectSingleNode(".//*[name()='media:description']").InnerText);

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