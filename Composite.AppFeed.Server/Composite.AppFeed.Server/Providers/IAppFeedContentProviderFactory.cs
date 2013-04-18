using System;
using System.Xml.Linq;

namespace Composite.AppFeed.Server.Providers
{
	public interface IAppFeedContentProviderFactory
	{
		IAppFeedContentProvider Build(XElement providerConfigElement, Action cacheRefresh);
	}
}
