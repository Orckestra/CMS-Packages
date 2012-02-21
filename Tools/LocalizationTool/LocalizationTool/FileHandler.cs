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

        private static Dictionary<string, List<string>> _curDataSourceKeysByFile;


        private static Dictionary<string, XDocument> _sourceFilesBySterm;
        private static Dictionary<string, string> _sourceFilesAsStringsBySterm;
        private static int _totalCountOfSourceTranstations;
        private static string _patternStringItemByKey = "<string\\s*key\\s*=\\s*\"({0})\"\\s*value\\s*=\\s*\"(?<1>[^\"]*)\"\\s*/>";
        private static string _patternStringItem = "<string\\s*key\\s*=\\s*\"(?<1>[^\"]*)\"\\s*value\\s*=\\s*\"(?<2>[^\"]*)\"\\s*/>";
        private static Regex _regexString = new Regex(_patternStringItem, RegexOptions.Compiled);
        #endregion

        static FileHandler()
        {
            _sourceFileStems = GetSourceFileStems();
            _sourceFilesBySterm = new Dictionary<string, XDocument>();
            _sourceFilesAsStringsBySterm = new Dictionary<string, string>();
            _curDataSourceKeysByFile = new Dictionary<string, List<string>>();

            _totalCountOfSourceTranstations = 0;
            foreach (var sterm in _sourceFileStems)
            {
                var sourceDoc = GetSourceDocument(sterm);
                _sourceFilesBySterm.Add(sterm, sourceDoc);
                _sourceFilesAsStringsBySterm.Add(sterm, GetSourceDocumentAsString(sterm));
                _totalCountOfSourceTranstations += sourceDoc.Root.Elements("string").Count();
                _curDataSourceKeysByFile.Add(sterm, GetStringKeys(sterm).ToList());

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

        public static Dictionary<string, List<string>> CurrentDataSourceKeysByFile
        {
            get { return _curDataSourceKeysByFile; }
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
            var timer = Stopwatch.StartNew();
            StringBuilder timerBuilder = new StringBuilder();
            bool isNeedToSave = false;

            var targetFilePath = GetTargetDocumentPath(fileStem);

            var targetValues = new Dictionary<string, string>();
            if (File.Exists(targetFilePath))
                targetValues = GetTargetDocument(fileStem).Root.Elements("string").ToDictionary(k => k.Attribute("key").Value, v => v.Attribute("value").Value);

            timerBuilder.AppendLine("#1:" + timer.ElapsedMilliseconds.ToString());

            var sourceFileContentCopy = SourceFilesAsStringsByStem[fileStem];
            var targetFileContent = string.Empty;
            timerBuilder.AppendLine("#2:" + timer.ElapsedMilliseconds.ToString());

            var newTargetString = string.Format("<string key=\"{0}\" value=\"{1}\" />", key, HttpUtility.HtmlEncode(newValue));

            if (targetValues.ContainsKey(key))
            {
                if (targetValues[key] != newValue)
                {
                    isNeedToSave = true;
                    targetFileContent = File.ReadAllText(targetFilePath);
                    var regexPattern = string.Format(_patternStringItemByKey, key);
                    targetFileContent = Regex.Replace(targetFileContent, regexPattern, newTargetString);
                    timerBuilder.AppendLine("#3:" + timer.ElapsedMilliseconds.ToString());
                }
            }
            else
            {
                isNeedToSave = true;
                // adding a new value to the values dictionary
                targetValues.Add(key, newValue);
                timerBuilder.AppendLine("#4:" + timer.ElapsedMilliseconds.ToString());

                //take a copy of the source file and re-save it as a target file
                var sourceMathes = _regexString.Matches(sourceFileContentCopy);
                timerBuilder.AppendLine("#5:" + timer.ElapsedMilliseconds.ToString());
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
                timerBuilder.AppendLine("#5:" + timer.ElapsedMilliseconds.ToString());
            }

            if (isNeedToSave)
            {
                File.WriteAllText(targetFilePath, targetFileContent);
                ResaveWebConfig();
            }

            timer.Stop();
            timerBuilder.AppendLine("#end:" + timer.ElapsedMilliseconds.ToString());

            File.WriteAllText(Settings.TargetLocalizationDirectory + "/timet.txt", timerBuilder.ToString());
            return isNeedToSave;
        }

        public static string FindNextMissingKey(string fileStem)
        {
            XDocument targetDocument = GetTargetDocument(fileStem);

            List<string> sourceKeys = CurrentDataSourceKeysByFile[fileStem];
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

        public static bool LocateUnknownFilesKeys()
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
                    needToSaveUnknownStringsDoc = true;
                }
                else
                {
                    var sourceKeys = SourceFilesBySterm[sterm].Root.Elements().Attributes("key").Select(f => f.Value).ToList();
                    var targetKeys = targetFile.Root.Elements().Attributes("key").Select(f => f.Value).ToList();

                    var unknownKeys = targetKeys.Except(sourceKeys).ToList();

                    if (unknownKeys.Any())
                    {
                        needToSaveUnknownStringsDoc = true;
                        unknownStringsDoc.Root.Add(new XComment(sterm));
                        string targetAsString = File.ReadAllText(targetFilePath);
                        unknownKeys.ForEach(key =>
                            {
                                var elementToRemove = targetFile.Root.Elements("string").FirstOrDefault(e => e.Attribute("key").Value == key);
                                unknownStringsDoc.Root.Add(elementToRemove);
                                var regexPattern = string.Format(_patternStringItemByKey, key);
                                targetAsString = Regex.Replace(targetAsString, regexPattern, String.Empty);
                            });
                        File.WriteAllText(targetFilePath, targetAsString);
                    }
                    //Temp: delete all  "*** NOT TRANSLATED ***" strings
                    string tempTargetText = File.ReadAllText(targetFilePath);
                    string regexPttern = "<string\\s*key\\s*=\\s*\"(?<1>[^\"]*)\"\\s*value\\s*=\\s*\"([^\"]*NOT TRANSLATED[^\"]*)\"\\s*/>";
                    if (Regex.IsMatch(tempTargetText, regexPttern))
                    {
                        tempTargetText = Regex.Replace(tempTargetText, regexPttern, string.Empty);
                        File.WriteAllText(targetFilePath, tempTargetText, Encoding.UTF8);
                    }
                    // end Temp

                }

            }
            if (needToSaveUnknownStringsDoc)
            {
                unknownStringsDoc.Save(Settings.UnknownStringsFilePath);
            }
            return needToSaveUnknownStringsDoc;
        }

        public static void AutoTranslateEmptyStrings()
        {
            foreach (var fileSterm in SourceFileStems)
            {
                var sourceDoc = GetSourceDocument(fileSterm);
                var sourceEmptyStrings = sourceDoc.Root.Elements("string").Where(el => string.IsNullOrEmpty(el.Attribute("value").Value)).Select(el => el.Attribute("key").Value).ToList();
                if (sourceEmptyStrings.Count == 0)
                    continue;

                var copyOfsourceDocAsString = SourceFilesAsStringsByStem[fileSterm];
                var targetFilePath = GetTargetDocumentPath(fileSterm);

                var sourceMathes = Regex.Matches(copyOfsourceDocAsString, _patternStringItem);
                var regexPattern = string.Empty;
                if (!File.Exists(targetFilePath))
                {
                    foreach (Match m in sourceMathes)
                    {
                        var key = m.Groups[1].Value;
                        regexPattern = string.Format(_patternStringItemByKey, key);

                        if (!sourceEmptyStrings.Contains(key))
                            copyOfsourceDocAsString = Regex.Replace(copyOfsourceDocAsString, regexPattern, Environment.NewLine);
                    }
                    File.WriteAllText(targetFilePath, copyOfsourceDocAsString);
                }
                else
                {
                    var targetDoc = XDocument.Load(targetFilePath);
                    var targetEmptyStrings = targetDoc.Root.Elements("string").Where(el => sourceEmptyStrings.Contains(el.Attribute("key").Value)).Select(el => el.Attribute("key").Value).ToList();
                    var missingEmptyStrings = sourceEmptyStrings.Except(targetEmptyStrings).ToList();
                    if (missingEmptyStrings.Count == 0) continue;
                    missingEmptyStrings.ForEach(key =>
                    {
                        targetDoc.Root.Add(new XElement("string", new XAttribute("key", key), new XAttribute("value", "")));
                    });
                    targetDoc.Save(targetFilePath);
                }
            }

        }

        public static void SaveTargetFilesStructureAsSource()
        {
            foreach (var file in GetTargetFileStems())
            {
                SaveTargetFileStructureAsSource(file);
            }
        }

        public static void FilterDataSource(string search, bool isFlaged)
        {
            search = search.ToLower();
            _curDataSourceKeysByFile = new Dictionary<string, List<string>>();
            if (isFlaged && !File.Exists(Settings.FlagsFilePath)) return;

            if (string.IsNullOrEmpty(search) && !isFlaged)
            {
                foreach (var file in SourceFileStems)
                {
                    _curDataSourceKeysByFile[file] = GetStringKeys(file).ToList();
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
                    if (!_curDataSourceKeysByFile.TryGetValue(file, out keys))
                    {
                        _curDataSourceKeysByFile[file] = new List<string>();
                    }
                    _curDataSourceKeysByFile[file].Add(key);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    //filter by search
                    var files = _curDataSourceKeysByFile.Keys.ToList();
                    foreach (var file in files)
                    {
                        var keys = new List<string>(_curDataSourceKeysByFile[file]);
                        foreach (var key in keys)
                        {
                            var sourceValue = GetSourceString(file, key);
                            var targetValue = GetTargetString(file, key);
                            if (!sourceValue.ToLower().Contains(search) && !key.ToLower().Contains(search))
                            {
                                if (string.IsNullOrEmpty(targetValue) || !targetValue.ToLower().Contains(search))
                                    _curDataSourceKeysByFile[file].Remove(key);
                            }
                        }
                        if (_curDataSourceKeysByFile[file].Count() == 0)
                        {
                            _curDataSourceKeysByFile.Remove(file);
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

                    _curDataSourceKeysByFile[file] = new List<string>();
                    sourceElements.ForEach(el =>
                    {
                        _curDataSourceKeysByFile[file].Add(el);
                    });
                }
                return;

            }
        }

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
            flag.Remove();
            flagsDoc.Save(Settings.FlagsFilePath);
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

        private static string SaveTargetFileStructureAsSource(string fileStem)
        {
            string filePath = GetTargetDocumentPath(fileStem);
            var copyOfsourceDocAsString = GetSourceDocumentAsString(fileStem);

            if (!File.Exists(filePath))
            {
                copyOfsourceDocAsString = Regex.Replace(copyOfsourceDocAsString, _patternStringItem, Environment.NewLine);
            }
            else
            {
                var targetFileText = File.ReadAllText(filePath);
                var sourceMathes = Regex.Matches(copyOfsourceDocAsString, _patternStringItem);

                var regexPattern = string.Empty;
                var targetStringElement = "<string key=\"{0}\" value=\"{1}\" />";
                foreach (Match m in sourceMathes)
                {
                    var key = m.Groups[1].Value;
                    regexPattern = string.Format(_patternStringItemByKey, key);
                    if (Regex.IsMatch(targetFileText, regexPattern))
                    {
                        var targetMatch = Regex.Match(targetFileText, regexPattern);
                        copyOfsourceDocAsString = Regex.Replace(copyOfsourceDocAsString, regexPattern, string.Format(targetStringElement, key, targetMatch.Groups[1].Value));
                    }
                    else
                        copyOfsourceDocAsString = Regex.Replace(copyOfsourceDocAsString, regexPattern, Environment.NewLine);
                }
            }

            File.WriteAllText(filePath, copyOfsourceDocAsString);
            return copyOfsourceDocAsString;
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

    }
}
