using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.Community.Blog.Localization;
using System.Web;

namespace Composite.Community.Blog
{
	public class BlogXsltExtensionsFunction
	{
		public string GetUrlFromTitle(string title)
		{
			return BlogFacade.GetUrlFromTitle(title);
		}

		public string GetBlogUrl(DateTime date, string title)
		{
			return BlogFacade.GetBlogUrl(date, title);
		}

		public string CustomDateFormat(DateTime date, string dateFormat)
		{
			return BlogFacade.CustomDateFormat(date, dateFormat);
		}

		public static XPathNavigator GetBlogTags(string tags)
		{
	        return  (new XElement("Tags", BlogFacade.GetBlogTags(tags).Select(t=> new XElement("Tag", t.Tag)))).CreateNavigator();
		}

		public static bool IsBlogList()
		{
			var pathInfoParts = BlogFacade.GetPathInfoParts();

			return pathInfoParts != null && (pathInfoParts.Count() == 5 ? true : false);
		}

		public string GetCurrentCultureName()
		{
			return BlogFacade.GetCurrentCultureName();
		}

		public string GetCurrentPath()
		{
			return BlogFacade.GetCurrentPath();
		}

		public string Encode(string text)
		{
			return BlogFacade.Encode(text);
		}

		public string GetLocalized(string resourceName, string key)
		{
			return Resource.GetLocalized(resourceName, key);
		}

		public void SetNoCache()
		{
			HttpContext.Current.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
		}

	}
}
