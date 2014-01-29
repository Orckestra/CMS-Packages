using System;
using System.Linq;
using System.Xml.Linq;

namespace Composite.Media.YouTube
{
    public class YoutubeVideo : GoogleXmlFeedEntry
    {
        private static readonly XName ID_ENTRY = XName.Get("id", "http://www.w3.org/2005/Atom");
        private static readonly XName DURATION_ENTRY = XName.Get("duration", "http://gdata.youtube.com/schemas/2007");
        private static readonly XName MEDIAGROUP_ENTRY = XName.Get("group", "http://search.yahoo.com/mrss/");
        private static readonly XName STATISTICS_ENTRY = XName.Get("statistics", "http://gdata.youtube.com/schemas/2007");

        public YoutubeVideo(XElement entry)
            : base(entry)
        {

            var url = (string)entry.Element(YoutubeChannelFacade.XML_CONTENT_ENTRY).Attribute("src") ?? (string)entry.Element(ID_ENTRY).Value;
            Id = new Uri(url).AbsolutePath.Split('/').Last();
            XElement mediagroup = entry.Element(MEDIAGROUP_ENTRY);
            if (mediagroup != null)
            {
                Duration = int.Parse(mediagroup.Element(DURATION_ENTRY).Attribute("seconds").Value);

            }
            XElement statistics = entry.Element(STATISTICS_ENTRY);
            if (statistics != null)
            {
                ViewCount = int.Parse(statistics.Attribute("viewCount").Value);
            }

        }

        public string Id { get; set; }
        public int Duration { get; set; }
        public int ViewCount { get; set; }
        public string Desciption { get; set; }

    }
}
