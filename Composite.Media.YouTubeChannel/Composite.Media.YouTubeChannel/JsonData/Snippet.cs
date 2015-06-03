using System;
using System.Collections.Generic;

namespace Composite.Media.YouTubeChannel.JsonData
{
    public class Snippet
    {
        public string title { get; set; }
        public string description { get; set; }
        public Dictionary<string, Thumbnail> thumbnails { get; set; }
        public Resource resourceId { get; set; }
        public DateTime publishedAt { get; set; }
    }
}
