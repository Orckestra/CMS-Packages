using System;
using System.Web;
using Composite.Core;
using Composite.Core.Routing;
using Composite.Core.WebClient;

namespace Composite.Tools.LegacyUrlHandler
{
	public class LegacyUrlHttpModule : IHttpModule
	{
		private const string PathInfoToken = ".aspx/";
		private readonly int _pathInfoTokenLength = PathInfoToken.Length;

		public void Dispose()
		{

		}

		public void Init(HttpApplication context)
		{
			context.BeginRequest += context_BeginRequest;
			context.Error += ErrorHandler;
		}

		private void ErrorHandler(object sender, EventArgs eventArgs)
		{
			var application = (HttpApplication)sender;

			try
			{
				// Gather information
				var currentException = application.Server.GetLastError();
				var httpException = currentException as HttpException;
				if (httpException != null && httpException.GetHttpCode() == 404)
				{
					BrokenLinks.Functions.SaveBrokenLink();
				}
			}
			catch (Exception ex)
			{
				Log.LogInformation("Composite.Tools.LegacyUrlHandler.ErrorHandler", ex.Message);
			}
		}

		void context_BeginRequest(object sender, EventArgs e)
		{
			const string cacheKey = "LegacyUrlHandler";
			var application = (HttpApplication)sender;
			var context = application.Context;
            var request = context.Request;
            
			var requestPath = request.Path;
			var requestPathInfo = "";

			int pathInfoTokenIndex = requestPath.IndexOf(PathInfoToken);
			if (pathInfoTokenIndex > -1)
			{
				requestPathInfo = requestPath.Substring(pathInfoTokenIndex + _pathInfoTokenLength - 1);
				requestPath = requestPath.Substring(0, pathInfoTokenIndex + _pathInfoTokenLength - 1);
			}

			//LoggingService.LogInformation("Legacy URL in:", string.Format("{0} - {1}", requestPath, requestPathInfo));
			LegacyUrlHandlerFacade.UrlMappings legacyUrlMappings;

			if (!Cache.Get(cacheKey, out legacyUrlMappings))
			{
				legacyUrlMappings = LegacyUrlHandlerFacade.GetMappingsFromXml();
				Cache.Add(cacheKey, legacyUrlMappings, LegacyUrlHandlerFacade.XmlFileName);
			}

		    string mappingPath = legacyUrlMappings.GetMappedUrl(request.Url.Host, requestPath);
            if (mappingPath == null)
			{
				return;
			}

			
			if (PageUrlHelper.IsInternalUrl(mappingPath) || PageUrlHelper.IsPublicUrl(mappingPath))
			{
				var pageUrlOptions = PageUrls.ParseUrl(mappingPath);
				if (pageUrlOptions != null)
				{
                    var publicUrl = PageUrls.BuildUrl(pageUrlOptions, UrlKind.Public, new UrlSpace());
					if (publicUrl != requestPath)
					{
						if (!string.IsNullOrEmpty(requestPathInfo))
							publicUrl = string.Format("{0}{1}", publicUrl, requestPathInfo);

						var queryString = context.Request.QueryString;
						if (queryString.Count > 0)
							publicUrl = string.Format("{0}{1}?{2}", publicUrl, requestPathInfo, queryString);

						context.Response.RedirectPermanent(publicUrl, true);
					}
				}
			}
			else
			{
				context.Response.RedirectPermanent(mappingPath, true);
			}
		}

		
	}
}