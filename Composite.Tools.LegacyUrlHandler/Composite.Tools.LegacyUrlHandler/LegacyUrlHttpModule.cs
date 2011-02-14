using System;
using System.Collections.Generic;
using System.Web;
using Composite.Core.Logging;
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
		}

		void context_BeginRequest(object sender, EventArgs e)
		{
			const string cacheKey = "LegacyUrlHandler";
			var application = (HttpApplication)sender;
			var context = application.Context;
			var requestPath = context.Request.Path;
			var requestPathInfo = "";

			int pathInfoTokenIndex = requestPath.IndexOf(PathInfoToken);
			if (pathInfoTokenIndex > -1)
			{
				requestPathInfo = requestPath.Substring(pathInfoTokenIndex + _pathInfoTokenLength - 1);
				requestPath = requestPath.Substring(0, pathInfoTokenIndex + _pathInfoTokenLength - 1);
			}

			//LoggingService.LogInformation("Legacy URL in:", string.Format("{0} - {1}", requestPath, requestPathInfo));
			Dictionary<string, string> legacyUrlMappings;

			if (!Cache.Get(cacheKey, out legacyUrlMappings))
			{
				legacyUrlMappings = LegacyUrlHandlerFacade.GetMappingsFromXml();
				Cache.Add(cacheKey, legacyUrlMappings, LegacyUrlHandlerFacade.XmlFileName);
			}

			if (legacyUrlMappings.ContainsKey(requestPath))
			{
				var mappingPath = legacyUrlMappings[requestPath];
				if (PageUrlHelper.IsInternalUrl(mappingPath) || PageUrlHelper.IsPublicUrl(mappingPath))
				{
					var pageUrlOptions = PageUrlHelper.ParseUrl(mappingPath);
					if (pageUrlOptions != null)
					{
						var publicUrl = PageUrlHelper.BuildUrl(UrlType.Public, pageUrlOptions).ToString();
						if (publicUrl != requestPath)
						{
							if (string.IsNullOrEmpty(requestPathInfo) == false)
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
}