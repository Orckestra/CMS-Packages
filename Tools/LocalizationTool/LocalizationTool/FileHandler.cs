using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;
using System.Diagnostics;

namespace LocalizationTool
{
	public static class FileHandler
	{
		#region Variables
		private readonly static string _sourceFileEnding = string.Format(".{0}.xml", Settings.SourceCulture.Name);
		private readonly static string _targetFileEnding = string.Format(".{0}.xml", Settings.TargetCulture.Name);
		private static IEnumerable<string> _sourceFileStems;
		private static Dictionary<string, List<string>> _stringsCurrentDataSource;

		private static List<string> _changedTargetFiles = new List<string>();


		private static Dictionary<string, XDocument> _sourceFilesBySterm;
		private static Dictionary<string, string> _sourceFilesAsStringsBySterm;
		private static int _totalCountOfSourceTranstations;
		private static string _patternStringItemByKey = "<string\\s*key\\s*=\\s*\"({0})\"\\s*value\\s*=\\s*\"(?<1>[^\"]*)\"\\s*/>";
		private static string _patternStringItem = "<string\\s*key\\s*=\\s*\"(?<1>[^\"]*)\"\\s*value\\s*=\\s*\"(?<2>[^\"]*)\"\\s*/>";
		private static Regex _regexString = new Regex(_patternStringItem, RegexOptions.Compiled);
		#endregion

		#region Static Constructor

		static FileHandler()
		{
			_sourceFileStems = GetSourceFileStems();
			_sourceFilesBySterm = new Dictionary<string, XDocument>();
			_sourceFilesAsStringsBySterm = new Dictionary<string, string>();
			_stringsCurrentDataSource = new Dictionary<string, List<string>>();

			_totalCountOfSourceTranstations = 0;
			foreach (var sterm in _sourceFileStems)
			{
				var sourceDoc = GetSourceDocument(sterm);
				_sourceFilesBySterm.Add(sterm, sourceDoc);
				_sourceFilesAsStringsBySterm.Add(sterm, GetSourceDocumentAsString(sterm));
				_totalCountOfSourceTranstations += sourceDoc.Root.Elements("string").Count();
				_stringsCurrentDataSource.Add(sterm, GetStringKeys(sterm).ToList());

			}

		}

		#endregion

		#region Public Static Properties
		public static string SourceFileEnding
		{
			get
			{
				return _sourceFileEnding;
			}
		}

		public static string TargetFileEnding
		{
			get
			{
				return _targetFileEnding;
			}
		}

		public static Dictionary<string, List<string>> StringsCurrentDataSource
		{
			get { return _stringsCurrentDataSource; }
		}

		public static IEnumerable<string> SourceFileStems
		{
			get { return _sourceFileStems; }
		}

		public static Dictionary<string, XDocument> SourceFilesBySterm
		{
			get { return _sourceFilesBySterm; }
		}

		public static Dictionary<string, string> SourceFilesAsStringsByStem
		{
			get { return _sourceFilesAsStringsBySterm; }
		}

		public static int TotalCountOfSourceTranstations
		{
			get { return _totalCountOfSourceTranstations; }
		}

		#endregion

		#region Public Methods

		public static IEnumerable<string> GetStringKeys(string fileStem)
		{
			return SourceFilesBySterm[fileStem].Descendants("string").Attributes("key").Select(f => f.Value);
		}

		public static string GetSourceString(string fileStem, string stringKey)
		{
			return SourceFilesBySterm[fileStem].Descendants("string").Where(f => f.Attribute("key").Value == stringKey).Select(f => f.Attribute("value").Value).First();
		}

		public static string GetTargetString(string fileStem, string stringKey)
		{
			XDocument localizationDocument = GetTargetDocument(fileStem);
			return localizationDocument != null ? localizationDocument.Descendants("string").Where(f => f.Attribute("key").Value == stringKey).Select(f => f.Attribute("value").Value).FirstOrDefault() : "";
		}

		public static bool SaveTargetString(string fileStem, string key, string newValue)
		{
			bool isNeedToSave = false;
			var targetFilePath = GetTargetDocumentPath(fileStem);

			XDocument targeDoc = File.Exists(targetFilePath) ? XDocument.Load(targetFilePath) : new XDocument(new XElement("strings"));
			var targetValues = targeDoc.Root.Elements("string").ToDictionary(k => k.Attribute("key").Value, v => v.Attribute("value").Value);

			if (targetValues.ContainsKey(key))
			{
				if (targetValues[key] != newValue)
				{
					var element = targeDoc.Root.Elements("string").Where(el => el.Attribute("key").Value == key).Single();
					element.Attribute("value").Value = newValue;
					isNeedToSave = true;
				}
			}
			else
			{
				isNeedToSave = true;
				targeDoc.Root.Add(new XElement("string", new XAttribute("key", key), new XAttribute("value", newValue)));
			}

			if (isNeedToSave)
			{
				targeDoc.Save(targetFilePath);
				ResaveWebConfig();
				if (!_changedTargetFiles.Contains(fileStem))
					_changedTargetFiles.Add(fileStem);
			}
			return isNeedToSave;
		}

		public static bool SaveTargetStringOld(string fileStem, string key, string newValue)
		{
			bool isNeedToSave = false;
			var targetFilePath = GetTargetDocumentPath(fileStem);

			var targetValues = new Dictionary<string, string>();
			if (File.Exists(targetFilePath))
				targetValues = GetTargetDocument(fileStem).Root.Elements("string").ToDictionary(k => k.Attribute("key").Value, v => v.Attribute("value").Value);

			var sourceFileContentCopy = SourceFilesAsStringsByStem[fileStem];
			var targetFileContent = string.Empty;
			var newTargetString = string.Format("<string key=\"{0}\" value=\"{1}\" />", key, HttpUtility.HtmlEncode(newValue));

			if (targetValues.ContainsKey(key))
			{
				if (targetValues[key] != newValue)
				{
					isNeedToSave = true;
					targetFileContent = File.ReadAllText(targetFilePath);
					var regexPattern = string.Format(_patternStringItemByKey, key);
					targetFileContent = Regex.Replace(targetFileContent, regexPattern, newTargetString);
				}
			}
			else
			{
				isNeedToSave = true;
				// adding a new value to the values dictionary
				targetValues.Add(key, newValue);
				//take a copy of the source file and re-save it as a target file
				var sourceMathes = _regexString.Matches(sourceFileContentCopy);
				var targetValue = string.Empty;

				foreach (Match m in sourceMathes)
				{
					var mKey = m.Groups[1].Value;
					if (targetValues.TryGetValue(mKey, out targetValue))
					{
						newTargetString = string.Format("<string key=\"{0}\" value=\"{1}\" />", mKey, HttpUtility.HtmlEncode(targetValue));
						sourceFileContentCopy = sourceFileContentCopy.Replace(m.Value, newTargetString);
					}
					else
					{
						sourceFileContentCopy = sourceFileContentCopy.Replace(m.Value, Environment.NewLine);
					}
				}
				targetFileContent = sourceFileContentCopy;
			}

			if (isNeedToSave)
			{
				File.WriteAllText(targetFilePath, targetFileContent);
				ResaveWebConfig();
			}
			return isNeedToSave;
		}

		public static string FindNextMissingKey(string fileStem)
		{
			XDocument targetDocument = GetTargetDocument(fileStem);
			List<string> sourceKeys = StringsCurrentDataSource[fileStem];
			if (targetDocument == null) return sourceKeys.FirstOrDefault();
			List<string> targetKeys = targetDocument.Root.Elements().Attributes("key").Select(f => f.Value).ToList();

			return sourceKeys.Except(targetKeys).FirstOrDefault();
		}

		public static int CountOfMissingStrings(string fileStem)
		{
			XDocument sourceDocument = SourceFilesBySterm[fileStem];
			int count = sourceDocument.Root.Elements("string").Count();
			var filePath = GetTargetDocumentPath(fileStem);
			if (File.Exists(filePath))
			{
				XDocument targetDocument = XDocument.Load(filePath);
				List<string> sourceKeys = sourceDocument.Root.Elements().Attributes("key").Select(f => f.Value).ToList();
				List<string> targetKeys = targetDocument.Root.Elements().Attributes("key").Select(f => f.Value).ToList();
				count = sourceKeys.Except(targetKeys).Count();
			}
			return count;
		}

		public static int TotalCountOfMissingStrings()
		{
			int total = 0;
			foreach (var fileSterm in SourceFileStems)
			{
				total += CountOfMissingStrings(fileSterm);
			}
			return total;
		}

		public static bool CleanUnknownStrings()
		{
			var unknownStringsDoc = new XDocument(new XElement("strings"));
			var needToSaveUnknownStringsDoc = false;

			if (!Directory.Exists(Settings.TargetLocalizationDirectory))
				return false;

			foreach (var sterm in GetTargetFileStems())
			{
				var targetFilePath = GetTargetDocumentPath(sterm);
				var targetFile = XDocument.Load(targetFilePath);

				//if source file is absent, then delete this target file
				if (!SourceFileStems.Contains(sterm))
				{
					unknownStringsDoc.Root.Add(new XComment(sterm));
					unknownStringsDoc.Root.Add(targetFile.Root.Elements());
					File.Delete(targetFilePath);
					unknownStringsDoc.Save(Settings.UnknownStringsFilePath);
					return true;
				}

				var sourceKeys = SourceFilesBySterm[sterm].Root.Elements().Attributes("key").Select(f => f.Value);
				var targetKeys = targetFile.Root.Elements().Attributes("key").Select(f => f.Value);
				var unknownKeys = targetKeys.Except(sourceKeys).ToList();

				if (unknownKeys.Any())
				{
					needToSaveUnknownStringsDoc = true;
					unknownStringsDoc.Root.Add(new XComment(sterm));
					unknownKeys.ForEach(key =>
						{
							var elementToRemove = targetFile.Root.Elements("string").FirstOrDefault(e => e.Attribute("key").Value == key);
							unknownStringsDoc.Root.Add(elementToRemove);
							elementToRemove.Remove();
						});
					targetFile.Save(targetFilePath);
					if (!_changedTargetFiles.Contains(sterm))
						_changedTargetFiles.Add(sterm);
				}
			}

			if (needToSaveUnknownStringsDoc)
				unknownStringsDoc.Save(Settings.UnknownStringsFilePath);

			return needToSaveUnknownStringsDoc;
		}

		public static void AutoTranslateEmptyStrings()
		{
			foreach (var fileSterm in SourceFileStems)
			{
				var sourceDoc = GetSourceDocument(fileSterm);
				var sourceEmptyStrings = sourceDoc.Root.Elements("string").Where(el => string.IsNullOrEmpty(el.Attribute("value").Value)).ToList();
				if (sourceEmptyStrings.Count == 0)
					continue;

				var targetFilePath = GetTargetDocumentPath(fileSterm);
				var targetDoc = !File.Exists(targetFilePath) ? new XDocument(new XElement("strings")) : XDocument.Load(targetFilePath);
				var targetValues = targetDoc.Root.Elements("string").ToDictionary(k => k.Attribute("key").Value, v => v.Attribute("value").Value);
				sourceEmptyStrings.ForEach(el =>
				{
					if (!targetValues.ContainsKey(el.Attribute("key").Value))
						targetDoc.Root.Add(el);
				});
				targetDoc.Save(targetFilePath);
				if (!_changedTargetFiles.Contains(fileSterm))
					_changedTargetFiles.Add(fileSterm);
			}
		}

		public static void SaveTargetFilesStructureAsSource()
		{
			foreach (var file in _changedTargetFiles)
			{
				SaveTargetFileStructureAsSource(file);
			}
		}

		public static void FilterDataSource(string search, bool isFlaged)
		{
			search = search.ToLower();
			_stringsCurrentDataSource = new Dictionary<string, List<string>>();
			if (isFlaged && !File.Exists(Settings.FlagsFilePath)) return;

			if (string.IsNullOrEmpty(search) && !isFlaged)
			{
				foreach (var file in SourceFileStems)
				{
					_stringsCurrentDataSource[file] = GetStringKeys(file).ToList();
				}
				return;
			}


			if (isFlaged)
			{
				//filter by Flags
				var flagDocument = GetFlagsDocument();
				foreach (var el in flagDocument.Root.Elements("Flag").OrderBy(el => el.Attribute("File").Value).ToList())
				{
					var file = el.Attribute("File").Value;
					if (!SourceFileStems.Contains(file)) continue;
					var key = el.Attribute("Key").Value;
					if (!GetStringKeys(file).Contains(key)) continue;
					List<string> keys = new List<string>();
					if (!_stringsCurrentDataSource.TryGetValue(file, out keys))
					{
						_stringsCurrentDataSource[file] = new List<string>();
					}
					_stringsCurrentDataSource[file].Add(key);
				}
				if (!string.IsNullOrEmpty(search))
				{
					//filter by search
					var files = _stringsCurrentDataSource.Keys.ToList();
					foreach (var file in files)
					{
						var keys = new List<string>(_stringsCurrentDataSource[file]);
						foreach (var key in keys)
						{
							var sourceValue = GetSourceString(file, key);
							var targetValue = GetTargetString(file, key);
							if (!sourceValue.ToLower().Contains(search) && !key.ToLower().Contains(search))
							{
								if (string.IsNullOrEmpty(targetValue) || !targetValue.ToLower().Contains(search))
									_stringsCurrentDataSource[file].Remove(key);
							}
						}
						if (_stringsCurrentDataSource[file].Count() == 0)
						{
							_stringsCurrentDataSource.Remove(file);
						}
					}
				}
				return;
			}
			if (!isFlaged)
			{
				//filter only by Search term
				foreach (var file in SourceFileStems)
				{
					var sourceFile = SourceFilesBySterm[file];
					var targetFile = GetTargetDocument(file);
					var sourceElements = sourceFile.Root.Elements("string").Where(el => el.Attribute("key").Value.ToLower().Contains(search) || el.Attribute("value").Value.ToLower().Contains(search)).Select(el => el.Attribute("key").Value).ToList();
					if (targetFile != null)
					{
						var targetValues = targetFile.Root.Elements("string").Where(el => el.Attribute("value").Value.ToLower().Contains(search)).Select(el => el.Attribute("key").Value).ToList();
						sourceElements = targetValues.Except(sourceElements).Union(sourceElements).ToList();
					}
					if (sourceElements.Count() == 0) continue;

					_stringsCurrentDataSource[file] = new List<string>();
					sourceElements.ForEach(el =>
					{
						_stringsCurrentDataSource[file].Add(el);
					});
				}
				return;

			}
		}

		#region Flags methods
		public static XDocument GetFlagsDocument()
		{
			if (!File.Exists(Settings.FlagsFilePath))
			{
				var newFile = new XDocument(new XElement("Files"));
				newFile.Save(Settings.FlagsFilePath);
				return newFile;
			}

			return XDocument.Load(Settings.FlagsFilePath);
		}

		public static void AddFlag(string file, string key, string comment)
		{
			XDocument flagsDoc = GetFlagsDocument();
			XElement flag = new XElement("Flag", new XAttribute("File", file), new XAttribute("Key", key), new XAttribute("Comment", comment));
			flagsDoc.Root.Add(flag);
			flagsDoc.Save(Settings.FlagsFilePath);
		}

		public static XElement GetFlag(string file, string key)
		{
			if (!File.Exists(Settings.FlagsFilePath)) return null;

			XDocument flagsDoc = GetFlagsDocument();
			return flagsDoc.Root.Elements("Flag").Where(el => el.Attribute("File").Value == file && el.Attribute("Key").Value == key).FirstOrDefault();
		}

		public static void UpdateFlag(string file, string key, string comment)
		{
			XDocument flagsDoc = GetFlagsDocument();
			XElement flag = flagsDoc.Root.Elements("Flag").Where(el => el.Attribute("File").Value == file && el.Attribute("Key").Value == key).FirstOrDefault();
			flag.Attribute("Comment").Value = comment;
			flagsDoc.Save(Settings.FlagsFilePath);
		}

		public static void RemoveFlag(string file, string key, string comment)
		{
			XDocument flagsDoc = GetFlagsDocument();
			XElement flag = flagsDoc.Root.Elements("Flag").Where(el => el.Attribute("File").Value == file && el.Attribute("Key").Value == key).FirstOrDefault();
			if (flag != null)
			{
				flag.Remove();
				flagsDoc.Save(Settings.FlagsFilePath);
			}
		}

		#endregion

		#region .config methods
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
				if (defaultCultureNode != null)
				{
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
		}

		public static void ResaveWebConfig()
		{
			//re-save web.config
			XDocument webconfig = XDocument.Load(Settings.WebConfigRelativePath);
			webconfig.Save(Settings.WebConfigRelativePath);
		}
		#endregion
		#endregion

		#region Private methods
		private static IEnumerable<string> GetSourceFileStems()
		{
			string sourceFileNamePattern = string.Format("*{0}", FileHandler.SourceFileEnding);
			int sourceFileEndingLength = FileHandler.SourceFileEnding.Length;

			foreach (string filePath in Directory.GetFiles(Settings.LocalizationDirectory, sourceFileNamePattern))
			{
				string fullFileName = Path.GetFileName(filePath);
				yield return fullFileName.Substring(0, fullFileName.Length - sourceFileEndingLength);
			}
		}

		private static IEnumerable<string> GetTargetFileStems()
		{
			string targetFileNamePattern = string.Format("*{0}", FileHandler.TargetFileEnding);
			int targetFileEndingLength = FileHandler.TargetFileEnding.Length;

			foreach (string filePath in Directory.GetFiles(Settings.TargetLocalizationDirectory, targetFileNamePattern))
			{
				string fullFileName = Path.GetFileName(filePath);
				yield return fullFileName.Substring(0, fullFileName.Length - targetFileEndingLength);
			}
		}

		private static XDocument GetSourceDocument(string fileStem)
		{
			return XDocument.Load(GetSourceDocumentPath(fileStem));
		}

		private static string GetSourceDocumentAsString(string fileStem)
		{
			return File.ReadAllText(GetSourceDocumentPath(fileStem));
		}

		public static XDocument GetTargetDocument(string fileStem)
		{
			if (File.Exists(GetTargetDocumentPath(fileStem)))
				return XDocument.Load(GetTargetDocumentPath(fileStem));
			return null;
		}

		private static void SaveTargetFileStructureAsSource(string fileStem)
		{
			string filePath = GetTargetDocumentPath(fileStem);
			var copyOfsourceDocAsString = GetSourceDocumentAsString(fileStem);

			if (!File.Exists(filePath))
			{
				copyOfsourceDocAsString = _regexString.Replace(copyOfsourceDocAsString, Environment.NewLine);
				File.WriteAllText(filePath, copyOfsourceDocAsString);
				return;
			}

			var targetValues = GetTargetDocument(fileStem).Root.Elements("string").ToDictionary(k => k.Attribute("key").Value, v => v.Attribute("value").Value);
			var sourceMathes = Regex.Matches(copyOfsourceDocAsString, _patternStringItem);

			var regexPattern = string.Empty;
			string targetString;
			var targetValue = string.Empty;
			foreach (Match m in sourceMathes)
			{
				var key = m.Groups[1].Value;
				if (targetValues.TryGetValue(key, out targetValue))
				{
					targetString = string.Format("<string key=\"{0}\" value=\"{1}\" />", key, HttpUtility.HtmlEncode(targetValue));
					copyOfsourceDocAsString = copyOfsourceDocAsString.Replace(m.Value, targetString);
				}
				else
					copyOfsourceDocAsString = copyOfsourceDocAsString.Replace(m.Value, Environment.NewLine);
			}
			File.WriteAllText(filePath, copyOfsourceDocAsString);
		}

		private static string GetSourceDocumentPath(string fileStem)
		{
			string fileName = string.Format("{0}{1}", fileStem, FileHandler.SourceFileEnding);
			string filePath = Path.Combine(Settings.LocalizationDirectory, fileName);

			return filePath;
		}

		private static string GetTargetDocumentPath(string fileStem)
		{
			string fileName = string.Format("{0}{1}", fileStem, FileHandler.TargetFileEnding);
			string filePath = Path.Combine(Settings.TargetLocalizationDirectory, fileName);

			return filePath;
		}
		#endregion

	}
}
