using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml.Linq;

namespace LocalizationTool
{
    public class ResxFileHandler
    {
        public static void UpdateAllResx(string dirPath, string targetLocale)
        {
            foreach (var file in GetReferenceResxInDirectory(dirPath))
            {
                UpdateResx(file, targetLocale);
            }
        }

        public static void CleanUpAll(string dirPath, string targetLocale)
        {
            foreach (var file in GetReferenceResxInDirectory(dirPath))
            {
                CleanUp(file, targetLocale);
            }
        }

        public static void InitAllResxResources(string dirPath, string targetLocale)
        {
            foreach (var file in GetReferenceResxInDirectory(dirPath))
            {
                InitResxResources(file, targetLocale);
            }
        }

        private static IEnumerable<string> GetReferenceResxInDirectory(string dirPath)
        {
            return Directory.GetFiles(
                dirPath, "*.resx")
                .Where(g => !CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .Select(f => "." + f.IetfLanguageTag + ".resx").Any(g.Contains))
                .Select(s => Path.GetDirectoryName(s) + "\\" + Path.GetFileNameWithoutExtension(s));
        }

        private static void UpdateResx(string res, string targetLocale)
        {
            UpdateResxWithXml(res + "." + targetLocale + ".xml", res + "." + targetLocale + ".resx");
            UpdateResxWithXml(res + ".en-US.xml", res + ".resx");
        }

        private static void CleanUp(string res, string targetLocale)
        {
            File.Delete(res + "." + targetLocale + ".xml");
            File.Delete(res + ".en-US.xml");
        }

        private static void InitResxResources(string res, string targetLocale)
        {
            ConvertResxToXml(res + ".resx", res + ".en-US.xml");
            if (!File.Exists(res + "." + targetLocale + ".resx"))
            {
                InitResx(res + ".resx", res + "." + targetLocale + ".resx");
            }
            else
            {
                ConvertResxToXml(res + "."+ targetLocale+".resx", res + "."+ targetLocale+".xml");
            }
        }

        private static void ConvertResxToXml(string resxPath, string xmlPath)
        {
            var resxReader = new ResXResourceReader(resxPath);
            var xdoc = new XDocument(new XElement("strings"));
            foreach (DictionaryEntry d in resxReader)
            {
                if (d.Value is string)
                {
                    xdoc.Root.Add(new XElement("string", new XAttribute("key", d.Key.ToString()), new XAttribute("value", d.Value)));
                }
            }
            xdoc.Save(xmlPath);
            //Close the resxReader
            resxReader.Close();
        }

        private static void ConvertXmlToResx(string xmlPath, string resxPath)
        {
            var xml = XDocument.Load(xmlPath);//"Composite.Management.en-us.xml");
            ResXResourceWriter writer = new ResXResourceWriter(resxPath);//"FooBar.resx");

            foreach (var node in xml.Root.Descendants())
            {
                var a = node.Attribute("key").Value;
                var b = node.Attribute("value").Value;
                writer.AddResource(a, b);
            }

            writer.Generate();
            writer.Close();
        }

        private static void InitResx(string resxPathSrc, string resxPathDest)
        {
            var writer = new ResXResourceWriter(resxPathDest);//"FooBar.resx");
            var reader = new ResXResourceReader(resxPathSrc);

            foreach (DictionaryEntry node in reader)
            {
                if (!(node.Value is string))
                {
                    writer.AddResource(node.Key.ToString(), node.Value);
                }

            }

            writer.Generate();
            writer.Close();
        }

        private static void UpdateResxWithXml(string xmlPath, string resxPath)
        {
            if (!File.Exists(xmlPath))
                return;
            var xml = XDocument.Load(xmlPath);

            foreach (var node in xml.Root.Descendants())
            {
                var a = node.Attribute("key").Value;
                var b = node.Attribute("value").Value;
                AddOrUpdateResource(resxPath, a, b);
            }
        }

        private static void AddOrUpdateResource(string resourceFilepath, string key, string value)
        {
            var resx = new List<DictionaryEntry>();
            using (var reader = new ResXResourceReader(resourceFilepath))
            {
                resx = reader.Cast<DictionaryEntry>().ToList();
                var existingResource = resx.FirstOrDefault(r => r.Key.ToString() == key);
                if (existingResource.Key == null && existingResource.Value == null) 
                {
                    resx.Add(new DictionaryEntry() { Key = key, Value = value });
                }
                else 
                {
                    var modifiedResx = new DictionaryEntry()
                    { Key = existingResource.Key, Value = value };
                    resx.Remove(existingResource);  
                    resx.Add(modifiedResx);  
                }
            }
            using (var writer = new ResXResourceWriter(resourceFilepath))
            {
                resx.ForEach(r =>
                {
                    writer.AddResource(r.Key.ToString(), r.Value);
                });
                writer.Generate();
            }
        }
    }
}
