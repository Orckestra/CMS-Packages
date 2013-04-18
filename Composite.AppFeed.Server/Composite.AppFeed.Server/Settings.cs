using System;
using System.IO;
using System.Xml.Linq;
using Composite.Core;
using Composite.Core.IO;

namespace Composite.AppFeed.Server
{
	internal static class Settings
	{
		private static FileSystemWatcher configFileWatcher;
		internal delegate void ConfigUpdateHandler();
		internal static event ConfigUpdateHandler OnConfigChanged;


		static Settings()
		{
			configFileWatcher = new FileSystemWatcher(Path.GetDirectoryName(Settings.ConfigFilePath));
			configFileWatcher.Filter = Path.GetFileName(Settings.ConfigFilePath);
			configFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
			configFileWatcher.Changed += ConfigChanged;
			configFileWatcher.EnableRaisingEvents = true;
		}


		// fired on config file changes
		private static void ConfigChanged(object sender, FileSystemEventArgs e)
		{
			if (OnConfigChanged != null)
			{
				OnConfigChanged();
			}
		}



		internal const string ConfigFileVirtualPath = "~/App_Data/Composite/Configuration/Composite.AppFeed.xml";

		internal static string ConfigFilePath
		{
			get
			{
				return PathUtil.Resolve(ConfigFileVirtualPath);
			}
		}

		internal const int GroupViewImageWidth = 250;
		internal const int GroupViewImageHeight = 250;
		internal const int PrimaryImageWidth = 480;
		internal const int PrimaryImageHeight = 600;

		internal static Guid GenericGroupViewImageFolderId
		{
			get
			{
				try
				{
					XDocument configDoc = XDocument.Load(ConfigFilePath);
					return (Guid)configDoc.Root.Element("Settings").Attribute("GenericGroupViewImageFolderId");
				}
				catch (Exception ex)
				{
					Log.LogError("App Feed", "Failed reading guid value from '/AppFeedConfiguration/Settings/@GenericGroupViewImageFolderId' in config file '{0}'", ConfigFileVirtualPath);
					Log.LogError("App Feed", ex);
					throw new InvalidOperationException(string.Format("Failed reading guid value from '/AppFeedConfiguration/Settings/@GenericGroupViewImageFolderId' in config file '{0}'", ConfigFileVirtualPath));
				}
			}
		}
	}
}
