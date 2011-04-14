using System;
using System.Web;
using System.Web.Caching;

namespace Composite.Community.OpenID
{
	public static class Cache
	{
		public static void Add<T>(string key, T o)
		{
			HttpContext.Current.Cache.Insert(
				key,
				o,
				null,
				DateTime.Now.AddMinutes(1440),
				System.Web.Caching.Cache.NoSlidingExpiration);
		}

		public static void Add<T>(string key, T o, string d)
		{
			HttpContext.Current.Cache.Insert(
				key,
				o,
				new CacheDependency(d),
				DateTime.Now.AddMinutes(1440),
				System.Web.Caching.Cache.NoSlidingExpiration);
		}

		public static void Clear(string key)
		{
			HttpRuntime.Cache.Remove(key);
		}

		public static bool Exists(string key)
		{
			return HttpRuntime.Cache[key] != null;
		}

		public static bool Get<T>(string key, out T value)
		{
			try
			{
				if (Exists(key))
				{
					value = (T)HttpRuntime.Cache[key];
					return true;
				}
				else
				{
					value = default(T);
					return false;
				}
			}
			catch
			{
				value = default(T);
				return false;
			}
		}
	}
}
