using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.Community.Blog.Localization;
using System.Web;
using Composite.Data;

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

		public string GetBlogPath(DateTime date, string title)
		{
			return BlogFacade.GetBlogPath(date, title);
		}

		public string GetFullBlogUrl(DateTime date, string title)
		{
			string pageUrl = BlogFacade.GetPageUrlById(SitemapNavigator.CurrentPageId);
			pageUrl = BlogFacade.GetFullPath(pageUrl);
			return BlogFacade.GetBlogUrl(date, title, pageUrl);
		}

		public string CustomDateFormat(DateTime date, string dateFormat)
		{
			return BlogFacade.CustomDateFormat(date, dateFormat);
		}

		public string CustomDateFormat(DateTime date, string dateFormat, string cultureName)
		{
			return BlogFacade.CustomDateFormatCulture(date, dateFormat, cultureName);
		}

		public static XPathNavigator GetBlogTags(string tags)
		{
			var blogTags = new XElement("Tags", tags.Split(',').Where(t => !string.IsNullOrEmpty(t)).Select(t => new XElement("Tag", t)));

			return blogTags.CreateNavigator();
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

		public string GetCurrentPageUrl()
		{
			return BlogFacade.GetCurrentPageUrl();
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
