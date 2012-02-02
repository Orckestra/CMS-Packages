using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace LocalizationTool
{
	public static class FileHandler
	{
		public static string SourceFileEnding
		{
			get
			{
				return string.Format(".{0}.xml", Settings.SourceCulture.Name);
			}
		}

		public static string TargetFileEnding
		{
			get
			{
				return string.Format(".{0}.xml", Settings.TargetCulture.Name);
			}
		}

		public static IEnumerable<string> GetSourceFileStems()
		{
			string sourceFileNamePattern = string.Format("*{0}", FileHandler.SourceFileEnding);
			int sourceFileEndingLength = FileHandler.SourceFileEnding.Length;

			foreach (string filePath in Directory.GetFiles(Settings.LocalizationDirectory, sourceFileNamePattern))
			{
				string fullFileName = Path.GetFileName(filePath);
				yield return fullFileName.Substring(0, fullFileName.Length - sourceFileEndingLength);
			}
		}

		public static IEnumerable<string> GetStringKeys(string fileStem)
		{
			XDocument localizationDocument = GetSourceDocument(fileStem);

			return localizationDocument.Descendants("string").Attributes("key").Select(f => f.Value);
		}

		public static string GetSourceString(string fileStem, string stringKey)
		{
			XDocument localizationDocument = GetSourceDocument(fileStem);
			return localizationDocument.Descendants("string").Where(f => f.Attribute("key").Value == stringKey).Select(f => f.Attribute("value").Value).Single();
		}

		public static string GetTargetString(string fileStem, string stringKey)
		{
			XDocument localizationDocument = GetTargetDocument(fileStem);
			return localizationDocument.Descendants("string").Where(f => f.Attribute("key").Value == stringKey).Select(f => f.Attribute("value").Value).FirstOrDefault();
		}

		public static void SetTargetString(string fileStem, string stringKey, string stringValue)
		{
			XDocument localizationDocument = GetTargetDocument(fileStem);
			bool resavewebconfig = true;
			XElement stringElement = localizationDocument.Root.Elements("string").Where(f => f.Attribute("key").Value == stringKey).FirstOrDefault();

			if (string.IsNullOrEmpty(stringValue) == false)
			{
				if (stringElement == null)
				{
					stringElement =
						new XElement("string",
							new XAttribute("key", stringKey),
							new XAttribute("value", stringValue));
					localizationDocument.Root.Add(stringElement);
				}
				else
				{
					resavewebconfig = (stringElement.Attribute("value").Value != stringValue);
					stringElement.Attribute("value").Value = stringValue;
				}
			}
			else
			{
				if (stringElement != null) stringElement.Remove();
			}


			SetTargetDocument(fileStem, localizationDocument);
			if (resavewebconfig) 
			{ 
				ResaveWebConfig();
			}

		}

		private static void SetTargetDocument(string fileStem, XDocument localizationDocument)
		{
			string fileName = string.Format("{0}{1}", fileStem, FileHandler.TargetFileEnding);
			string filePath = Path.Combine(Settings.TargetLocalizationDirectory, fileName);

			localizationDocument.Save(filePath);
		}

		private static XDocument GetSourceDocument(string fileStem)
		{
			return XDocument.Load(GetSourceDocumentPath(fileStem));
		}

		public static string GetSourceDocumentPath(string fileStem)
		{
			string fileName = string.Format("{0}{1}", fileStem, FileHandler.SourceFileEnding);
			string filePath = Path.Combine(Settings.LocalizationDirectory, fileName);

			return filePath;
		}

		private static XDocument GetTargetDocument(string fileStem)
		{
			string fileName = string.Format("{0}{1}", fileStem, FileHandler.TargetFileEnding);
			string filePath = Path.Combine(Settings.TargetLocalizationDirectory, fileName);

			if (File.Exists(filePath) == true)
			{
				return XDocument.Load(filePath);
			}
			else
			{
				XDocument copy = GetSourceDocument(fileStem);
				copy.Descendants("string").Remove();
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
				copy.Save(filePath);

				return copy;
			}

		}

		public static string FindNextMissingKey(string fileStem)
		{
			XDocument sourceDocument = GetSourceDocument(fileStem);
			XDocument targetDocument = GetTargetDocument(fileStem);

			List<string> sourceKeys = sourceDocument.Root.Elements().Attributes("key").Select(f => f.Value).ToList();
			List<string> targetKeys = targetDocument.Root.Elements().Attributes("key").Select(f => f.Value).ToList();

			string missingKey = sourceKeys.Where(f => targetKeys.Contains(f) == false).FirstOrDefault();

			return missingKey;
		}

		public static void RegisterInCompositeConfig(string fileStem)
		{
			string targetCulture = Settings.TargetCulture.Name;
			string defaultCulture = "en-US";

			XDocument config = XDocument.Load(Settings.CompositeConfigRelativePath);
			var fileNameCultureResource = (from xml2 in config.Root.Descendants("ResourceProviderPlugins").Elements("add")
										   where xml2.Attribute("type").Value == "Composite.Plugins.ResourceSystem.XmlStringResourceProvider.XmlStringResourceProvider, Composite"
										   select xml2).Elements("Cultures").Elements("add").Where(a => a.Attribute("xmlFile").Value.Contains(fileStem)).ToList();

			var targetCultureNode = fileNameCultureResource.Find(a => a.Attribute("cultureName").Value == targetCulture);

			if (targetCultureNode != null)
				return;
			else
			{
				//add to Composite.config
				var defaultCultureNode = fileNameCultureResource.Find(a => a.Attribute("cultureName").Value == defaultCulture);
				var culturesNode = defaultCultureNode.Parent;
				targetCultureNode = new XElement(defaultCultureNode);
				targetCultureNode.Attribute("cultureName").Value = targetCulture;
				targetCultureNode.Attribute("xmlFile").Value = string.Format("~/App_Data/Composite/LanguagePacks/{0}/{1}", targetCulture, Path.GetFileName(targetCultureNode.Attribute("xmlFile").Value.Replace(defaultCulture.ToLower(), targetCulture)));
				culturesNode.Add(targetCultureNode);

				// check if applicationCultureNames contains target culture
				var applicationCultureNames = (from xml2 in config.Root.Descendants("GlobalSettingsProviderPlugins").Elements("add")
											   where xml2.Attribute("type").Value == "Composite.Plugins.GlobalSettings.GlobalSettingsProviders.ConfigBasedGlobalSettingsProvider, Composite"
											   select xml2).SingleOrDefault().Attribute("applicationCultureNames");
				if (applicationCultureNames.Value.IndexOf(targetCulture) < 0)
				{
					applicationCultureNames.Value += "," + targetCulture;
				}

				config.Save(Settings.CompositeConfigRelativePath);
			}
		}

		public static void ResaveWebConfig()
		{
			//re-save web.config
			XDocument webconfig = XDocument.Load(Settings.WebConfigRelativePath);
			webconfig.Save(Settings.WebConfigRelativePath);
		}

	}
}
