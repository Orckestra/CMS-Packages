using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Composite.Core.IO;
using Composite.Core.WebClient;
using Composite.Core.WebClient.Renderings.Page;

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
					return HttpUtility.UrlDecode(HttpContext.Current.Request.PathInfo);
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
			if (!string.IsNullOrEmpty(PathInfo))
			{
				Path = PathInfo;
			}
			return RenderInternal(Path) ?? new XDocument();
		}

		public static XDocument RenderView(string Path)
		{
			return RenderInternal(Path) ?? new XDocument();
		}

		private static XDocument RenderInternal(string path)
		{
			var responseWriter = new StringWriter();

			var mvcResponse = new MvcPlayerHttpResponseWrapper(responseWriter, PageRenderer.CurrentPageId);
			var mvcRequest = new MvcPlayerHttpRequestWrapper(Request, path);
			var mvcContext = new MvcPlayerHttpContextWrapper(Context, mvcResponse, mvcRequest);

			lock (HttpContext.Current)
			{
				new MvcHttpHandlerWrapper().PublicProcessRequest(mvcContext);
			}

			// Redirects
			if (!string.IsNullOrEmpty(mvcResponse.RedirectLocation))
			{
				Response.Redirect(mvcResponse.RedirectLocation, false);
				Response.Flush();
				Response.Close();
				return null;
			}

			// Ajax requests
			if ((mvcRequest.IsAjaxRequest() || !mvcResponse.IsHtmlResponse()) && path == PathInfo)
			{
				byte[] bytes = Response.ContentEncoding.GetBytes(responseWriter.ToString());

				var page = Context.Handler as System.Web.UI.Page;
				if (page != null)
				{
					page.PreRender += (a, b) =>
					{
						Response.ClearContent();
						Response.ClearHeaders();
						Response.ContentType = mvcResponse.ContentType;
						Response.AddHeader("Content-Length", bytes.Length.ToString());
						Response.AddHeader("Content-Encoding", "none");
						Response.BinaryWrite(bytes);
						Response.Flush();
						Response.End();
					};
					return null;
				}

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
	}

	public class MvcPlayerHttpResponseWrapper : HttpResponseWrapper
	{
		private readonly string _pageUrl;
		public MvcPlayerHttpResponseWrapper(HttpResponse httpResponse, Guid pageId)
			: base(httpResponse)
		{
			PageStructureInfo.TryGetPageUrl(pageId, out _pageUrl);
		}

		public MvcPlayerHttpResponseWrapper(TextWriter responseWriter, Guid pageId)
			: this(new HttpResponse(responseWriter), pageId)
		{
		}

		public override string ApplyAppPathModifier(string virtualPath)
		{
			if (_pageUrl != "/" && virtualPath.StartsWith(UrlUtils.PublicRootPath))
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

	public class MvcPlayerHttpRequestWrapper : HttpRequestWrapper
	{
		private readonly string _path;

		public MvcPlayerHttpRequestWrapper(HttpRequest httpRequest, string path)
			: base(httpRequest)
		{
			_path = path;
		}

		public override string Path
		{
			get
			{
				return _path;
			}
		}

		public override string AppRelativeCurrentExecutionFilePath
		{
			get
			{
				return string.Format("~{0}", _path);
			}
		}

		public override string PathInfo
		{
			get
			{
				return string.Empty;
			}
		}
	}

	public class MvcHttpHandlerWrapper : MvcHttpHandler
	{
		public MvcHttpHandlerWrapper()
		{
			RouteCollection = MvcPlayerRouteTable.Routes;
		}

		public void PublicProcessRequest(HttpContextBase httpContext)
		{
			base.ProcessRequest(httpContext);
		}
	}

	public class MvcPlayerHttpContextWrapper : HttpContextWrapper
	{
		private readonly HttpResponseBase _response;
		private readonly HttpRequestBase _request;
		public MvcPlayerHttpContextWrapper(HttpContext httpContext, HttpResponseBase response, HttpRequestBase request)
			: base(httpContext)
		{
			_response = response;
			_request = request;
		}

		public override HttpResponseBase Response
		{
			get
			{
				return _response;
			}
		}

		public override HttpRequestBase Request
		{
			get
			{
				return _request;
			}
		}

		public override void SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior sessionStateBehavior)
		{
#warning does not support SessionStateAttribute
			//base.SetSessionStateBehavior(sessionStateBehavior);
		}
	}

	internal static class MvcPlayerExtension
	{
		public static bool IsHtmlResponse(this HttpResponseBase response)
		{
			return response.ContentType == "text/html";
		}
	}
}