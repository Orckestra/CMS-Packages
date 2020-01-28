using Orckestra.Web.Typescript.Classes.Models;
using Orckestra.Web.Typescript.Enums;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Serialization;

namespace Orckestra.Web.Typescript.Classes
{
    internal static class Helper
    {
        private static HttpServerUtility _ctxCache;
        internal static string GetAbsoluteServerPath(string currentPath)
        {
            if (string.IsNullOrEmpty(currentPath))
            {
                return null;
            }
            _ctxCache = HttpContext.Current?.Server ?? _ctxCache;
            return 
                Path.IsPathRooted(currentPath) 
                && !Path.GetPathRoot(currentPath).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                ? currentPath
                : _ctxCache.MapPath(currentPath);
        }

        internal static Settings GetSettings()
        {
            Settings settings;
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            string settingsFilePath = string.Concat(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath, ".cmp.xml");

            using (FileStream fileStream = new FileStream(settingsFilePath, FileMode.Open))
            {
                settings = (Settings)serializer.Deserialize(fileStream);
            }

            settings.TypescriptTasks = settings.TypescriptTasks.Where(x => x.Mode != Mode.Off).ToList();

            foreach (TypescriptTask el in settings.TypescriptTasks)
            {
                if (el.Mode == Mode.Dynamic && (el.PathsToWatch is null || !el.PathsToWatch.Any()))
                {
                    throw new ArgumentException("To use dynamic mode you have to specify watching paths in the config file");
                }
            }
            return settings;
        }
    }
}
