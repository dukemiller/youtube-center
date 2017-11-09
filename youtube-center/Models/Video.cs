using System;

namespace youtube_center.Models
{
    public class Video
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Views { get; set; }
        public DateTime Uploaded { get; set; }
        public DateTime Updated { get; set; }
        public Thumbnail Thumbnail { get; set; }
        public Author Author { get; set; }
    }
}