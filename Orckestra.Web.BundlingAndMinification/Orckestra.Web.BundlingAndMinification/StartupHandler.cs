﻿using Composite.Core.Application;
using Composite.Core.WebClient.Renderings.Page;
using Microsoft.Extensions.DependencyInjection;

namespace Orckestra.Web.BundlingAndMinification
{
	[ApplicationStartup]
	public static class StatupHandler
	{
		public static void OnBeforeInitialize() { }

		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(typeof(IPageContentFilter), typeof(PageContentFilter));
		}
	}
}