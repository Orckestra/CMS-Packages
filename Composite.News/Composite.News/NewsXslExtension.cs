using System;
using Composite.Plugins.Functions.XslExtensionsProviders.ConfigBasedXslExtensionsProvider;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Composite.News
{
	[ConfigurationElementType(typeof(ConfigBasedXslExtensionInfo))] 
	public class NewsXslExtension
	{
		public string GetPathInfo(string titleUrl, string date)
		{
			var dateTime = DateTime.Parse(date);
			return NewsFacade.GetPathInfo(titleUrl, dateTime);
		}

		public bool IsNewsList()
		{
			var pathInfoParts = NewsFacade.GetPathInfoParts();
			if (pathInfoParts != null && pathInfoParts.Length > 4)
			{
				return false;
			}
			return true;
		}
	}
}
