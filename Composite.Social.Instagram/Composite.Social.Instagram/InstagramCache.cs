using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace Composite.Social.Instagram
{
    public class InstagramCache : ICache 
    {
        public void Add(string key, object obj)
        {
            HttpContext.Current.Cache.Add(key, obj,
                                 null, DateTime.Now.AddSeconds(180),
                                 Cache.NoSlidingExpiration,
                                 CacheItemPriority.Default,
                                 null);
        }

        public void Add(string key, object obj, int timeout)
        {
            HttpContext.Current.Cache.Add(key, obj,
                                null, DateTime.Now.AddSeconds(timeout),
                                Cache.NoSlidingExpiration,
                                CacheItemPriority.Default,
                                null);
        }

        public void Remove(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        public object Get(string key)
        {
            return  HttpContext.Current.Cache[key];
        }

        public T Get<T>(string key)
        {
            return (T)HttpContext.Current.Cache[key];
        }

        public bool Exists(string key)
        {
            return HttpContext.Current.Cache[key] != null;
        }
    }
}
