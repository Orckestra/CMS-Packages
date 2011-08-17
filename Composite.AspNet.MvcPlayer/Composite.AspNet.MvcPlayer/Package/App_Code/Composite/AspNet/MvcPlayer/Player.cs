using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Composite.Core;
using Composite.Core.IO;
using Composite.Core.WebClient;
using Composite.Core.WebClient.Renderings.Page;
using System.Reflection;

namespace Composite.AspNet.MvcPlayer
{
	/// <summary>
	/// Composite.AspNet.MvcPlayer.Functions
	/// </summary>
	public class Functions
	{
		internal static HttpResponse Response { get { return HttpContext.Current.Response; } }
		internal static HttpRequest Request { get { return HttpContext.Current.Request; } }
		internal static HttpContext Context { get { return HttpContext.Current; } }

		private static string PathInfo
		{
			get
			{
				Type pageRoute = typeof(Composite.Data.IData).Assembly.GetType("Composite.Core.Routing.Pages.C1PageRoute", false);

				if (pageRoute == null)
				{
					// Support for version 2.1.2
					return HttpContext.Current.Request.PathInfo;
				}

				// Support for version 2.1.3+
				string result = (pageRoute
					.GetMethod("GetPathInfo", BindingFlags.Public | BindingFlags.Static)
					.Invoke(null, new object[0]) as string) ?? string.Empty;

				if (result != string.Empty)
				{
					pageRoute
						.GetMethod("RegisterPathInfoUsage", BindingFlags.Public | BindingFlags.Static)
						.Invoke(null, new object[0]);
				}

				return result;

			}
		}

		public static XDocument Render(string Path)
		{
			return RenderInternal(Path) ?? new XDocument();
		}

		private static XDocument RenderInternal(string path)
		{
			if (!string.IsNullOrEmpty(PathInfo))
			{
				path = PathInfo;
			}

			Guid pageId = PageRenderer.CurrentPageId;

			var responseWriter = new StringWriter();

			var httpResponceBase = new MvcPlayerHttpResponseWrapper(new HttpResponse(responseWriter), pageId);
			var httpContext = new MvcPlayerHttpContextWrapper(Context, httpResponceBase);


			ProcessRequest(httpContext, path);

			// Redirects
			if (!string.IsNullOrEmpty(httpResponceBase.RedirectLocation))
			{
				Response.Redirect(httpResponceBase.RedirectLocation, false);
				Response.Flush();
				Response.Close();
				return null;
			}

			// Ajax requests
			if (httpContext.Request.IsAjaxRequest())
			{
				byte[] bytes = Response.ContentEncoding.GetBytes(responseWriter.ToString());
				Response.Buffer = true;
				Response.ClearContent();
				Response.ClearHeaders();
				Response.ContentType = httpResponceBase.ContentType;
				Response.AddHeader("Content-Length", bytes.Length.ToString());
				Response.AddHeader("Content-Encoding", "none");
				Response.BinaryWrite(bytes);
				Response.Flush();
				Response.Close();
				return null;
			}

			var sbHtml = new StringBuilder();
			sbHtml
				.Append(@"<html xmlns=""http://www.w3.org/1999/xhtml""> 
                <head/>
                <body>")
				.Append(responseWriter.ToString())
				.Append(@"
                </body>
                </html>");

			try
			{
				return XDocument.Parse(sbHtml.ToString());
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("MvcPlayer: failed to parse result markup as xml", ex);
			}
		}

		public static void ProcessRequest(HttpContextWrapper httpContext, string path)
		{
			httpContext.RewritePath(string.Format("~/{0}", path));
			new MvcHttpHandlerWrapper().ProcessRequest(httpContext);
		}
	}

	public class MvcPlayerHttpResponseWrapper : HttpResponseWrapper
	{
		private string _pageUrl;
		public MvcPlayerHttpResponseWrapper(HttpResponse httpResponse, Guid pageId)
			: base(httpResponse)
		{
			PageStructureInfo.TryGetPageUrl(pageId, out _pageUrl);
		}

		public override string ApplyAppPathModifier(string virtualPath)
		{
			if (virtualPath.StartsWith(UrlUtils.PublicRootPath))
			{
				var path = virtualPath.Substring(UrlUtils.PublicRootPath.Length);
				if (!File.Exists(PathUtil.BaseDirectory + path))
				{
					virtualPath = _pageUrl + path;
				}
			}
			return virtualPath;
		}


		public override void Redirect(string url, bool endResponse)
		{
			RedirectLocation = url;
		}
	}

	public class MvcHttpHandlerWrapper : MvcHttpHandler
	{
		public void ProcessRequest(HttpContextBase httpContext)
		{
			base.ProcessRequest(httpContext);
		}
	}

	public class MvcPlayerHttpContextWrapper : HttpContextWrapper
	{
		private HttpResponseBase _response;
		public MvcPlayerHttpContextWrapper(HttpContext httpContext, HttpResponseBase response)
			: base(httpContext)
		{
			_response = response;
		}

		public override HttpResponseBase Response
		{
			get
			{
				return _response;
			}
		}

		public override void SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior sessionStateBehavior)
		{
#warning does not support SessionStateAttribute
			//base.SetSessionStateBehavior(sessionStateBehavior);
		}
	}
}