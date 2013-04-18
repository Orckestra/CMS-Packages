using System;
using System.Xml.Linq;
using Composite.AppFeed.Server.Providers;

namespace Composite.AppFeed.Provider.Magazine
{
	public class MagazineAppFeedProviderFactory : IAppFeedContentProviderFactory
	{
		public IAppFeedContentProvider Build(XElement providerConfigElement, Action cacheRefresh)
		{
			return new MagazineAppFeedProvider(cacheRefresh);
		}
	}
}
