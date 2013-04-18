using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Composite.AppFeed.Server.Providers;
using Composite.Core;

namespace Composite.AppFeed.Server
{
	public class FeedManager
	{
		private static object _instanceLock = new object();
		private static FeedManager _instance = null;
		private static Exception _lastLoggedException = null;

		public static FeedManager Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_instanceLock)
					{
						if (_instance == null)
						{
							_instance = new FeedManager();
						}
					}
				}

				return _instance;
			}
		}

		private Dictionary<string, List<Group>> cachedGroups = new Dictionary<string, List<Group>>();
		private Dictionary<string, List<Content>> cachedContent = new Dictionary<string, List<Content>>();
		private Dictionary<string, IAppFeedContentProvider> providers = new Dictionary<string, IAppFeedContentProvider>();

		private FeedManager()
		{
			try
			{
				InitFromConfig();
				Settings.OnConfigChanged += ConfigChanged;
			}
			catch (Exception ex)
			{
				if (_lastLoggedException == null || _lastLoggedException.Message != ex.Message)
				{
					Log.LogError("Feed Manager", ex.Message);
					_lastLoggedException = ex;
				}
				throw;
			}
		}


		public IEnumerable<Group> Groups
		{
			get
			{
				foreach (List<Group> groupList in cachedGroups.Values)
				{
					foreach (Group group in groupList)
					{
						yield return group;
					}
				}
			}
		}



		public IEnumerable<Content> Content
		{
			get
			{
				foreach (List<Content> contentList in cachedContent.Values)
				{
					foreach (Content content in contentList)
					{
						yield return content;
					}
				}
			}
		}




		// private parts

		private void InitFromConfig()
		{
			foreach (var providerBlueprint in GetProviderBlueprints())
			{
				Action updateCache = delegate
				{
					RefreshCache(providerBlueprint.Name);
				};

				var provider = providerBlueprint.Factory.Build(providerBlueprint.ConfigElement, updateCache);

				providers.Add(providerBlueprint.Name, provider);

				updateCache();
			}
		}

		// fired on config file changes
		private void ConfigChanged()
		{
			Settings.OnConfigChanged -= ConfigChanged;
			_instance = null;
			_lastLoggedException = null; // re-log any errors now
		}


		private IEnumerable<ProviderBlueprint> GetProviderBlueprints()
		{
			if (!File.Exists(Settings.ConfigFilePath))
			{
				Log.LogError("Composite AppFeed", "Config file '{0}' not found", Settings.ConfigFileVirtualPath);
				throw new InvalidOperationException(string.Format("Config file '{0}' not found", Settings.ConfigFileVirtualPath));
			}

			var configDoc = XDocument.Load(Settings.ConfigFilePath);

			XElement providersRoot = configDoc.Root.Element("Providers");

			if (providersRoot == null) throw new InvalidOperationException(string.Format("Config file '{0}' is missing expected '/AppFeedConfiguration/Providers' element.", Settings.ConfigFileVirtualPath));

			foreach (var providerElement in providersRoot.Elements("Provider"))
			{
				string providerName = (string)providerElement.Attribute("name");
				string providerFactoryTypeName = (string)providerElement.Attribute("factoryType");

				if (providerName == null) throw new InvalidOperationException(string.Format("Config file '{0}' is missing expected 'name' attribute on a '/AppFeedConfiguration/Providers/Provider' element.", Settings.ConfigFileVirtualPath));
				if (providerFactoryTypeName == null) throw new InvalidOperationException(string.Format("Config file '{0}' is missing expected 'name' attribute on a '/AppFeedConfiguration/Providers/Provider' element.", Settings.ConfigFileVirtualPath));

				Type providerFactoryType = Type.GetType(providerFactoryTypeName);

				if (providerFactoryType == null) throw new InvalidOperationException(string.Format("Unknown type '{0}' specified in config file '{1}'. Are you missing an assembly in ~/bin?", providerFactoryTypeName, Settings.ConfigFileVirtualPath));

				IAppFeedContentProviderFactory providerFactory = Activator.CreateInstance(providerFactoryType) as IAppFeedContentProviderFactory;

				yield return new ProviderBlueprint { ConfigElement = providerElement, Name = providerName, Factory = providerFactory };
			}
		}



		private void RefreshCache(string providerName)
		{
			cachedContent.Remove(providerName);
			cachedGroups.Remove(providerName);

			var provider = providers[providerName];

			cachedGroups.Add(providerName, provider.GetGroups().ToList());
			cachedContent.Add(providerName, provider.GetContent().ToList());
		}

		private class ProviderBlueprint
		{
			public XElement ConfigElement { get; set; }
			public IAppFeedContentProviderFactory Factory { get; set; }
			public string Name { get; set; }
		}

	}
}