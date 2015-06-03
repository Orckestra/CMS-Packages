using System.IO;
using System.Net;
using Composite.Core;
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Script.Serialization;
using Composite.Media.YouTubeChannel.JSonData;

namespace Composite.Media.YouTubeChannel
{
    public class YoutubeChannelFacade
    {

        private static string PlaylistItemsUrl = "https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&playlistId={0}&maxResults={1}&pageToken={2}&key={3}";
        private static string PlaylistsUrl = "https://www.googleapis.com/youtube/v3/playlists?part=snippet,contentDetails&key={0}&maxResults={1}&channelId={2}&pageToken={3}";
        private static string ChannelUrl = "https://www.googleapis.com/youtube/v3/channels?part=contentDetails&forUsername={0}&key={1}";
     
     
        public static int CacheTimeInSeconds = 60 * 60;

        private static object cahceLock = new object();

        public static string ParseYouTubeChannelReference(string reference)
        {
            string urlUserPattern = @"http://www.youtube.com/user/(?<1>[^\s/]*)";
            var m = Regex.Match(reference, urlUserPattern,
                      RegexOptions.IgnoreCase | RegexOptions.Compiled,
                      TimeSpan.FromSeconds(1));
            if (m.Success)
            {
                reference = m.Groups[1].Value;
            }
            return reference;
        }

        public static string GetJson(string url)
        {
            var jsonResponse = string.Empty;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;
            request.Accept = "application/json";
            using (var s = request.GetResponse().GetResponseStream())
            {
                using (var sr = new StreamReader(s))
                {
                    jsonResponse = sr.ReadToEnd();
                    
                }
            }
            return jsonResponse;
        }

        public static List GetYouTubeChannel(string user,int maxResults,string pageToken, string apiKey)
        {
            var cacheKey = string.Format("YOUTUBECHANNEL_{0}_{1}_{2}_{3}", apiKey, user, maxResults, pageToken);
            var result = GetFromCache(cacheKey);
            if (result == null)
            {
                try
                {
                    var response = GetJson(string.Format(ChannelUrl, user, apiKey));
                    dynamic channel = new JavaScriptSerializer().DeserializeObject(response);
                    var playlistId = channel["items"][0]["contentDetails"]["relatedPlaylists"]["uploads"];
                    result = GetYouTubePlayListItems(playlistId, maxResults, pageToken, apiKey);
                    SaveToCache(cacheKey, result);
                }
                catch {}
              
            }

            return result;
        }

        public static List GetYouTubePlayLists(string apiKey, string user, int maxResults, string pageToken)
        {
            var cacheKey = string.Format("YOUTUBEPLAYLISTS_{0}_{1}_{2}", apiKey, user, pageToken);
            var result = GetFromCache(cacheKey);
            if (result == null)
            {
               
                try
                {
                    var response = GetJson(string.Format(ChannelUrl, user, apiKey));
                    dynamic channel = new JavaScriptSerializer().DeserializeObject(response);
                    var channelId = channel["items"][0]["id"];
                    response = GetJson(string.Format(PlaylistsUrl, apiKey, maxResults, channelId, pageToken));
                    result = new JavaScriptSerializer().Deserialize<List>(response);
                    SaveToCache(cacheKey, result);
                }
                catch {}
            }

            return result;
        }

        public static List GetYouTubePlayListItems(string playlistId, int maxResults, string pageToken, string apiKey)
        {
            var cacheKey = string.Format("YOUTUBEPLAYLISTITEMS_{0}_{1}_{2}_{3}", apiKey, playlistId, pageToken, maxResults);
            var result = GetFromCache(cacheKey);
            if (result == null)
            {
                var response = GetJson(string.Format(PlaylistItemsUrl, playlistId, maxResults, pageToken, apiKey));
                result =  new JavaScriptSerializer().Deserialize<List>(response);
                    SaveToCache(cacheKey, result);
            }
            return result;
        }

        private static List GetFromCache(string cacheKey)
        {
            return HttpContext.Current.Cache[cacheKey] as List;
        }

        private static void SaveToCache(string cacheKey, List cahceValue)
        {
            if (HttpContext.Current.Cache[cacheKey] == null)
            {
                lock (cahceLock)
                {
                    try
                    {
                        if (HttpContext.Current.Cache[cacheKey] == null)
                            HttpContext.Current.Cache.Add(cacheKey, cahceValue,
                                null, DateTime.Now.AddSeconds(CacheTimeInSeconds),
                                Cache.NoSlidingExpiration,
                                CacheItemPriority.Default,
                                null);
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("GetYouTubeChannel", ex.Message);
                    }
                }
            }
            
        }

    }
}
