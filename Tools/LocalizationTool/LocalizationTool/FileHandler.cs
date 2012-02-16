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
        #region Variables
        private static string _sourceFileEnding;
        private static string _targetFileEnding;
        private static IEnumerable<string> _sourceFileStems;
        private static Dictionary<string, XDocument> _sourceFilesBySterm;
        private static int _totalCountOfSourceTranstations;


        #endregion
        static FileHandler()
        {
            _sourceFileEnding = string.Format(".{0}.xml", Settings.SourceCulture.Name);
            _targetFileEnding = string.Format(".{0}.xml", Settings.TargetCulture.Name);
            _sourceFileStems = GetSourceFileStems();
            _sourceFilesBySterm = new Dictionary<string, XDocument>();
            _totalCountOfSourceTranstations = 0;
            foreach (var sterm in _sourceFileStems)
            {
                var sourceDoc = GetSourceDocument(sterm);
                _sourceFilesBySterm.Add(sterm, sourceDoc);
                _totalCountOfSourceTranstations += sourceDoc.Root.Elements("string").Count();
            }
        }

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

        public static IEnumerable<string> SourceFileStems
        {
            get { return _sourceFileStems; }
        }

        public static Dictionary<string, XDocument> SourceFilesBySterm
        {
            get { return _sourceFilesBySterm; }
        }

        public static int TotalCountOfSourceTranstations
        {
            get { return _totalCountOfSourceTranstations; }
        }

        #endregion

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

        public static IEnumerable<string> GetTargetFileStems()
        {
            string targetFileNamePattern = string.Format("*{0}", FileHandler.TargetFileEnding);
            int targetFileEndingLength = FileHandler.TargetFileEnding.Length;

            foreach (string filePath in Directory.GetFiles(Settings.TargetLocalizationDirectory, targetFileNamePattern))
            {
                string fullFileName = Path.GetFileName(filePath);
                yield return fullFileName.Substring(0, fullFileName.Length - targetFileEndingLength);
            }
        }

        public static IEnumerable<string> GetStringKeys(string fileStem)
        {
            XDocument localizationDocument = SourceFilesBySterm[fileStem];

            return localizationDocument.Descendants("string").Attributes("key").Select(f => f.Value);
        }

        public static string GetSourceString(string fileStem, string stringKey)
        {
            XDocument localizationDocument = SourceFilesBySterm[fileStem];
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

        public static string GetTargetDocumentPath(string fileStem)
        {
            string fileName = string.Format("{0}{1}", fileStem, FileHandler.TargetFileEnding);
            string filePath = Path.Combine(Settings.TargetLocalizationDirectory, fileName);

            return filePath;
        }

        private static XDocument GetTargetDocument(string fileStem)
        {
            string filePath = GetTargetDocumentPath(fileStem);
            XDocument copyOfSource = new XDocument(SourceFilesBySterm[fileStem]);
            //Should ensure that XML is saved with comments and in same order as source file (should be possible to compare the two XML files by hand, if needed)
            if (File.Exists(filePath) == true)
            {
                var targetDoc = XDocument.Load(filePath);
                var tKeys = targetDoc.Root.Elements().Attributes("key").Select(e => e.Value).ToList();
                var sKeys = copyOfSource.Root.Elements().Attributes("key").Select(e => e.Value).ToList();

				if (sKeys.Where(f => tKeys.Contains(f) == false).FirstOrDefault() == null && (targetDoc.Root.Elements().Count() == copyOfSource.Root.Elements().Count()))
                {
                    return targetDoc;
                }
                else
                {
                    var targetValues = targetDoc.Root.Elements("string").ToDictionary(k => k.Attribute("key").Value, v => v.Attribute("value").Value);
                    foreach (var str in copyOfSource.Descendants("string"))
                    {
                        var key = str.Attribute("key").Value;
                        var value = string.Empty;
                        if (targetValues.TryGetValue(key, out value))
                            str.Attribute("value").Value = value;
                        else
                            str.Attribute("value").Value = Settings.NotTranslatedStringValue;
                    }
                }
            }
            else
            {
                foreach (var str in copyOfSource.Descendants("string"))
                {
                    str.Attribute("value").Value = Settings.NotTranslatedStringValue;
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            copyOfSource.Save(filePath);
            return copyOfSource;
        }

        public static string FindNextMissingKey(string fileStem)
        {
            XDocument targetDocument = GetTargetDocument(fileStem);
            var node = targetDocument.Root.Elements().Where(t => t.Attribute("value").Value == Settings.NotTranslatedStringValue).FirstOrDefault();
            if (node != null)
                return node.Attribute("key").Value;
            return null;
        }

        public static int CountOfMissingStrings(string fileStem)
        {
            XDocument sourceDocument = SourceFilesBySterm[fileStem];
            if (File.Exists(GetTargetDocumentPath(fileStem)) == true)
            {
                XDocument targetDocument = GetTargetDocument(fileStem);
                var missingNodes = targetDocument.Root.Elements().Where(t => t.Attribute("value").Value == Settings.NotTranslatedStringValue);
                if (missingNodes != null)
                {
                    return missingNodes.Count();
                }
            }
            else
            {
                return sourceDocument.Root.Elements("string").Count();
            }
            return 0;
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

        public static bool LocateUnknownFilesKeys()
        {
            var unknownStringsDoc = new XDocument(new XElement("strings"));
            var needToSaveUnknownStringsDoc = false;
            if (Directory.Exists(Settings.TargetLocalizationDirectory))
            {
                foreach (var sterm in GetTargetFileStems())
                {
                    var targetFile = XDocument.Load(GetTargetDocumentPath(sterm));

                    if (!SourceFileStems.Contains(sterm))
                    {
                        unknownStringsDoc.Root.Add(new XComment(sterm));
                        unknownStringsDoc.Root.Add(targetFile.Root.Elements());
                        File.Delete(GetTargetDocumentPath(sterm));
                        //TODO - remove from Composite.config
                        needToSaveUnknownStringsDoc = true;
                    }
                    else
                    {
                        List<string> sourceKeys = SourceFilesBySterm[sterm].Root.Elements().Attributes("key").Select(f => f.Value).ToList();
                        List<string> targetKeys = targetFile.Root.Elements().Attributes("key").Select(f => f.Value).ToList();
                        var unknownKeys = targetKeys.Where(f => sourceKeys.Contains(f) == false).ToList();
                        if (unknownKeys.Count() > 0)
                        {
                            needToSaveUnknownStringsDoc = true;
                            unknownStringsDoc.Root.Add(new XComment(sterm));
                            foreach (var key in unknownKeys)
                            {
                                var elementToRemove = targetFile.Root.Elements("string").Where(e => e.Attribute("key").Value == key).FirstOrDefault();
                                unknownStringsDoc.Root.Add(elementToRemove);
                            }
                        }
                    }

                }
                if (needToSaveUnknownStringsDoc)
                {
                    unknownStringsDoc.Save(Settings.UnknownStringsFilePath);
                }
            }
            return needToSaveUnknownStringsDoc;

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
