using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using Composite.Core.WebClient;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Composite.Core.Xml;

namespace Composite.Search.MicrosoftSearchServer
{
	public class RemapperHttpModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			context.PostMapRequestHandler += new EventHandler(context_PostMapRequestHandler);
		}

		void context_PostMapRequestHandler(object sender, EventArgs e)
		{
			var httpContext = HttpContext.Current;

			if (IsBot(httpContext)
				&& httpContext.Handler != null
				&& httpContext.Handler is Page
				&& !httpContext.Request.Url.PathAndQuery.StartsWith(UrlUtils.AdminRootPath))
			{
				httpContext.Response.Filter = new ReplacementStream(httpContext.Response.Filter);
			}
		}

		public bool IsBot(HttpContext context)
		{
			//Mozilla/4.0 (compatible; MSIE 4.01; Windows NT; MS Search 5.0 Robot)
			if (context.Request.UserAgent != null)
			{
				string userAgent = context.Request.UserAgent.ToLower();
				string[] botKeywords = new string[] { "robot", "ms search" };
				if (userAgent.Contains(botKeywords[0]) && userAgent.Contains(botKeywords[1]))
					return true;
			}
			if (context.Request.QueryString["MicrosoftSearchServer"] != null)
				return true;

			return false;
		}

		public void Dispose()
		{

		}

		private class ReplacementStream : Utf8StringTransformationStream
		{
			public ReplacementStream(Stream innerStream) : base(innerStream) { }

			public override string Process(string str)
			{

				var document = XDocument.Parse(Regex.Replace(str, @"xmlns:(\w*)=""http://www.w3.org/1999/xhtml""", ""));
				//fix Media Urls
				foreach (var a in document.Descendants(Namespaces.Xhtml + "a"))
				{
					if (a.Attribute("href") != null)
					{
						var href = a.Attribute("href").Value;
						var re = new Regex(@"ShowMedia.ashx\?(.*)");
						var match = re.Match(href);
						if (match.Success)
						{
							try
							{
								var querystring = HttpUtility.ParseQueryString(match.Groups[1].Value);
								a.SetAttributeValue("href", href.Replace("ShowMedia.ashx?", string.Format("ShowMedia.ashx/{0}?", Regex.Replace(MediaUrlHelper.GetFileFromQueryString(querystring).FileName, @"[^\w\d.]", ""))));
							}
							catch
							{ }
						}
					}
				};
				//remove "noindex" elements
				foreach (var el in document.Descendants().Where(d => d.Attribute("class") != null && d.Attribute("class").Value.ToLower().Contains("noindex")).Reverse())
				{
					el.Remove();
				}
				return document.ToString();
			}
		}
	}
}