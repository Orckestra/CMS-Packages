using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.Core;
using Composite.Core.IO;
using Composite.Core.PackageSystem;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Composite.Data;

namespace Composite.Localization.C1Console.FragmentInstaller
{
	public class C1ConsoleFragmentInstaller : BasePackageFragmentInstaller
	{
		public override IEnumerable<PackageFragmentValidationResult> Validate()
		{
			yield break;
		}

		public override IEnumerable<XElement> Install()
		{
			if (typeof(IData).Assembly.GetName().Version.Major >= 4)
			{
				Log.LogInformation("Composite.Localization.C1Console.FragmentInstaller", "Start fixing Composite.config");
				FixCompositeConfig();
				Log.LogInformation("Composite.Localization.C1Console.FragmentInstaller", "Composite.config - fixed..");
			}

			yield break;
		}

		void FixCompositeConfig()
		{
			bool changed = false;

			string configFilePath = PathUtil.Resolve("~/App_Data/Composite/Composite.config");
			XDocument config = XDocument.Load(configFilePath);

			var plugins = config.XPathSelectElement("/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins");

			//OutPut(plugins.ToString());

			var includedFolders = new List<string>();
			foreach (var plugin in plugins.Elements())
			{
				if (plugin.Attribute("type").Value == "Composite.Plugins.ResourceSystem.XmlLocalizationProvider.XmlLocalizationProvider, Composite")
				{
					includedFolders.Add(plugin.Attribute("directory").Value);
				}
			}

			var newFolders = new List<NewFolder>();

			foreach (var plugin in plugins.Elements().ToList())
			{
				if (plugin.Attribute("type").Value != "Composite.Plugins.ResourceSystem.XmlStringResourceProvider.XmlStringResourceProvider, Composite")
				{
					continue;
				}

				var fileNodes = plugin.XPathSelectElements("./Cultures/add");

				foreach (var n in fileNodes)
				{
					string xmlFilePath = n.Attribute("xmlFile").Value;

					if (includedFolders.Any(f => xmlFilePath.StartsWith(f + "/", true, CultureInfo.InvariantCulture)))
					{
						continue;
					}

					string folder = xmlFilePath.Substring(0, xmlFilePath.LastIndexOf("/") - 1);

					if (!newFolders.Any(f => f.Path == folder))
					{
						newFolders.Add(new NewFolder { Path = folder, PluginName = plugin.Attribute("name").Value });
					}

					// OutPut(n.ToString());
				}

				plugin.Remove();
				changed = true;
			}

			foreach (var newFolder in newFolders)
			{
				plugins.Add(new XElement("add",
					new XAttribute("defaultCultureName", "en-US"),
					new XAttribute("directory", newFolder.Path),
					new XAttribute("type", "Composite.Plugins.ResourceSystem.XmlLocalizationProvider.XmlLocalizationProvider, Composite"),
					new XAttribute("name", newFolder.PluginName)));
				changed = true;
			}

			if (!changed) return;

			config.Save(configFilePath);
			//OutPut(plugins.ToString());
		}

	}

	public class C1ConsoleFragmentUninstaller : BasePackageFragmentUninstaller
	{
		public override void Uninstall()
		{
			return;
		}

		public override IEnumerable<PackageFragmentValidationResult> Validate()
		{
			yield break;
		}
	}


	internal class NewFolder
	{
		public string Path;
		public string PluginName;
	}
}
