using System.Collections.Generic;
using Composite.Media.YouTubeChannel.JsonData;

namespace Composite.Media.YouTubeChannel.JSonData
{
    public class ListItem
    {
        public string id { get; set; }
        public Snippet snippet { get; set; }
        public Dictionary<string, string> contentDetails { get; set; }
    }
}
