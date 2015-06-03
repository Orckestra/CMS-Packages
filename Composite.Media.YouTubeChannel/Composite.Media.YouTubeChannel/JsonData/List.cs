using System.Collections.Generic;

namespace Composite.Media.YouTubeChannel.JSonData
{
    public class List
    {
        public string nextPageToken { get; set; }
        public string prevPageToken { get; set; }
        public PageInfo pageinfo { get; set; }
        public List<ListItem> items { get; set; }
    }
}