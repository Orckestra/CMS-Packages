using System;
using System.Web;

namespace Composite.Search.MicrosoftSearchServer
{
	public class RemapperHttpModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(context_BeginRequest);
		}

		void context_BeginRequest(object sender, EventArgs e)
		{
			HttpApplication application = (HttpApplication)sender;
			HttpContext context = application.Context;
			if(IsBot(context) && context.Request.Path.Contains("Renderers/Page.aspx"))
				context.RewritePath(context.Request.Path.Replace("Renderers/Page.aspx","/Frontend/Composite/Search/MicrosoftSearchServer/Page.aspx"));
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
	}
}