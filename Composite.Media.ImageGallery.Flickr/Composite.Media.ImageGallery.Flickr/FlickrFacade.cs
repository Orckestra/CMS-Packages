using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;
using Composite.Media.ImageGallery.Flickr.Data;
using Composite.Core;

namespace Composite.Media.ImageGallery.Flickr
{
    static public class FlickrFacade
    {

        const string FlickrApiUrlFormat = "https://api.flickr.com/services/rest/?method={0}&api_key={1}&format=rest{2}";

        public static PhotoSet GetPhotoSet(string apiKey, string setId)
        {
            var url = GetMethodUrl("flickr.photosets.getInfo", apiKey, null, setId, "description,owner_name");
            try
            {
                var data = GetData(url);
                if (data == null || data.Root == null)
                {
                    return null;
                }
                if (data.Root.Descendants("err").Any())
                {
                    Log.LogWarning("Composite.Media.ImageGallery.Flickr",
                        data.Root.Descendants("err").First().Attribute("msg").Value);
                }
                else
                    return
                         data.Root.Elements("photoset").Select(el => new PhotoSet(el))
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.LogError("Composite.Media.ImageGallery.Flickr", ex.Message);
            }
            return null;
        }

        public static List<Comment> GetPhotoSetComments(string apiKey, string setId)
        {
            var result = new List<Comment>();
            var url = GetMethodUrl("flickr.photosets.comments.getList", apiKey, null, setId, null);
            try
            {
                var data = GetData(url);
                if (ValidateData(data))
                {
                    data.Root.Descendants("comment").Select(el => new Comment(el))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Composite.Media.ImageGallery.Flickr", ex.Message);
            }
            return result;
        }

        public static List<Photo> GetPhotos(string apiKey, string userId, string setId = null)
        {
            var methodName = string.IsNullOrEmpty(setId)
               ? "flickr.people.getPublicPhotos"
               : "flickr.photosets.getPhotos";

            return GetPhotos(methodName, apiKey, userId, setId);
        }

        public static List<Photo> GetFavoritesPhotos(string apiKey, string userId)
        {
            return GetPhotos("flickr.favorites.getPublicList", apiKey, userId, null);
        }

        private static List<Photo> GetPhotos(string methodName, string apiKey, string userId, string setId)
        {
            var result = new List<Photo>();

            var url = GetMethodUrl(methodName, apiKey, userId, setId, "description,owner_name");
            try
            {
                var data = GetData(url);
                if (ValidateData(data))
                {
                    return
                        data.Root.Descendants("photo").Select(el => new Photo()
                        {
                            Title = el.Attribute("title") != null ? el.Attribute("title").Value : string.Empty,
                            Description = el.Attribute("description") != null ? el.Attribute("description").Value : string.Empty,
                            Url = string.Format("http://farm{0}.static.flickr.com/{1}/{2}_{3}.jpg", el.Attribute("farm").Value, el.Attribute("server").Value, el.Attribute("id").Value, el.Attribute("secret").Value),
                            ThumbnailUrl = string.Format("http://farm{0}.static.flickr.com/{1}/{2}_{3}_s.jpg", el.Attribute("farm").Value, el.Attribute("server").Value, el.Attribute("id").Value, el.Attribute("secret").Value),
                            OwnerName = el.Attribute("ownername") != null ? el.Attribute("ownername").Value : string.Empty,
                            OwnerUrl = el.Attribute("owner") != null ? string.Format("http://www.flickr.com/photos/{0}/{1}", el.Attribute("owner").Value, el.Attribute("id").Value) : string.Empty
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Composite.Media.ImageGallery.Flickr", ex.Message);
            }

            return result;
        }

        public static List<PhotoSet> GetPhotoSetsList(string apiKey, string userId)
        {
            var url = GetMethodUrl("flickr.photosets.getList", apiKey, userId, null, null);
            var result = new List<PhotoSet>();
            try
            {
                var data = GetData(url);
                if (ValidateData(data))
                {
                    return data.Root.Descendants("photoset").Select(el => new PhotoSet(el)).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Composite.Media.ImageGallery.Flickr", ex.Message);
            }
            return result;
        }

        #region private methods
        private static XDocument GetData(string url)
        {
            if (HttpContext.Current.Cache[url] != null) return HttpContext.Current.Cache[url] as XDocument;
            var xd = XDocument.Load(url);

            HttpContext.Current.Cache.Add(url, xd,
                null, DateTime.Now.AddSeconds(60),
                Cache.NoSlidingExpiration,
                CacheItemPriority.Default,
                null);
            return HttpContext.Current.Cache[url] as XDocument;
        }

        private static bool ValidateData(XDocument data)
        {
            if (data == null || data.Root == null)
            {
                return false;
            }
            if (data.Root.Descendants("err").Any())
            {
                Log.LogWarning("Composite.Media.ImageGallery.Flickr",
                    data.Root.Descendants("err").First().Attribute("msg").Value);
                return false;
            }
            return true;
        }

        private static string GetMethodUrl(string methodName, string apiKey, string userId, string photosetId, string extras)
        {

            var extraParam = new StringBuilder();

            if (!string.IsNullOrEmpty(photosetId))
                extraParam.Append("&photoset_id=").Append(photosetId);

            if (!string.IsNullOrEmpty(extras))
                extraParam.Append("&extras=").Append(extras);

            if (!string.IsNullOrEmpty(userId))
                extraParam.Append("&user_id=").Append(userId);



            return string.Format(FlickrApiUrlFormat, methodName, apiKey, extraParam);
        }
        #endregion

    }
}
