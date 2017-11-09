using System.Collections.Generic;

namespace youtube_center.Models
{
    public class Channel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<Video> Videos { get; set; } = new List<Video>();
    }
}