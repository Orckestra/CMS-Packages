using Composite.Core;
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;

namespace Composite.Media.YouTube
{
    public class YoutubeChannelFacade
    {
        public static XName XML_ENTRY = XName.Get("entry", "http://www.w3.org/2005/Atom");
        public static readonly XName XML_CONTENT_ENTRY = XName.Get("content", "http://www.w3.org/2005/Atom");
        public static readonly XName XML_TOTALRESULTS = XName.Get("totalResults", "http://a9.com/-/spec/opensearch/1.1/");
        public static readonly XName XML_STARTINDEX = XName.Get("startIndex", "http://a9.com/-/spec/opensearch/1.1/");
        public static readonly XName XML_ITEMSPERPAGE = XName.Get("itemsPerPage", "http://a9.com/-/spec/opensearch/1.1/");
   
                                                      

        private static string PlaylistsFeedUrl = "https://gdata.youtube.com/feeds/api/users/{0}/playlists?v=2";
        private static string ChannelFeedUrl = "http://gdata.youtube.com/feeds/api/videos?&author={0}&max-results={1}&start-index={2}&orderby=published&v=2";
     
        public static int CacheTimeInSecons = 60 * 60;

        private static object cahceLock = new object();

        public static string ParseYouTubeChannelReference(string reference)
        {
            string urlUserPattern = @"http://www.youtube.com/user/(?<1>[^\s/]*)";
            Match m = Regex.Match(reference, urlUserPattern,
                      RegexOptions.IgnoreCase | RegexOptions.Compiled,
                      TimeSpan.FromSeconds(1));
            if (m.Success)
            {
                reference = m.Groups[1].Value;
            }
            return reference;
        }

        public static YoutubeChannel GetYouTubePlaylists(string userid)
        {
            var feed = string.Format(PlaylistsFeedUrl, userid);

            return GetYouTubeFeed(feed, "playlists");

        }

        public static YoutubeChannel GetYouTubeChannel(string userid, int maxResults, int startIndex)
        {
            var feed = string.Format(ChannelFeedUrl, userid, maxResults, startIndex);
            return GetYouTubeFeed(feed, "videos");

        }

       
        private static YoutubeChannel GetYouTubeFeed(string feed, string feedType)
        {
            
            if (HttpContext.Current.Cache[feed] == null)
            {
                lock (cahceLock)
                {
                    try
                    {
                        if (HttpContext.Current.Cache[feed] == null)
                            HttpContext.Current.Cache.Add(feed, new YoutubeChannel(XDocument.Load(feed).Root, feedType),
                                null, DateTime.Now.AddSeconds(CacheTimeInSecons),
                                Cache.NoSlidingExpiration,
                                CacheItemPriority.Default,
                                null);
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("GetYouTubeFeed", feed + ": " +ex.Message);
                    }
                }
            }
            return HttpContext.Current.Cache[feed] as YoutubeChannel;
        }

    }
}
