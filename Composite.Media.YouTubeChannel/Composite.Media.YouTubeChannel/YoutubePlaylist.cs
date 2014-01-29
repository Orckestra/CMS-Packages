using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;

namespace Composite.Media.YouTube
{
    public class YoutubePlaylist : GoogleXmlFeedEntry
    {

        private static readonly XName SUMMARY_ENTRY = XName.Get("summary", "http://www.w3.org/2005/Atom");
        private static readonly XName LINK_ENTRY = XName.Get("link", "http://www.w3.org/2005/Atom");
        private static readonly XName COUNT_HIT_ENTRY = XName.Get("countHint", "http://gdata.youtube.com/schemas/2007");
        private static readonly XName PLAYLIST_ID_ENTRY = XName.Get("playlistId", "http://gdata.youtube.com/schemas/2007");

        private static readonly XName MEDIA_GROUP_ENTRY = XName.Get("group", "http://search.yahoo.com/mrss/");
        private static readonly XName MEDIA_THUMBNAIL_ENTRY = XName.Get("thumbnail", "http://search.yahoo.com/mrss/");

        private string _playlistFeedSrc { get; set; }
        public YoutubePlaylist(XElement entry)
            : base(entry)
        {

            Summary = (string)entry.Element(SUMMARY_ENTRY);
            _playlistFeedSrc = (string)entry.Element(YoutubeChannelFacade.XML_CONTENT_ENTRY).Attribute("src");
            VideoCount = (int)entry.Element(COUNT_HIT_ENTRY);
            Id = (string)entry.Element(PLAYLIST_ID_ENTRY);

            if (VideoCount > 0)
                Thumbnail = (string)entry.Descendants(MEDIA_THUMBNAIL_ENTRY).Last().Attribute("url");

        }

        public int VideoCount { get; set; }

        public IEnumerable<YoutubeVideo> Videos
        {
            get
            {
                if (HttpContext.Current.Cache[_playlistFeedSrc] == null)
                    HttpContext.Current.Cache.Add(_playlistFeedSrc, parsePlaylistFeed(_playlistFeedSrc),
                        null, DateTime.Now.AddSeconds(YoutubeChannelFacade.CacheTimeInSecons),
                        Cache.NoSlidingExpiration,
                        CacheItemPriority.Default,
                        null);

                return HttpContext.Current.Cache[_playlistFeedSrc] as IEnumerable<YoutubeVideo>;
            }
        }


        public string Id { get; set; }
     
        public string Thumbnail { get; set; }

        public string Summary { get; set; }

        private static IEnumerable<YoutubeVideo> parsePlaylistFeed(string url)
        {
            var list = new List<YoutubeVideo>();

            var feed = XDocument.Load(url).Root;
            list.AddRange(feed.Elements(YoutubeChannelFacade.XML_ENTRY).Select(entry => new YoutubeVideo(entry)));
            var next = feed.Elements(LINK_ENTRY).FirstOrDefault(l => (string)l.Attribute("rel") == "next");
            if (next != null)
                list.AddRange(parsePlaylistFeed((string)next.Attribute("href")));

            return list;

        }
    }
}
