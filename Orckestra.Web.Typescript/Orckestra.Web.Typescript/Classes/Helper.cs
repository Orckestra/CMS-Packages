using Composite.Core;
using Orckestra.Web.Typescript.Classes.Models;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Orckestra.Web.Typescript.Classes
{
    internal static class Helper
    {
        private static readonly string _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly bool PackageEnabled = ConfigurationManager.AppSettings["Orckestra.Web.Typescript.Enable"] == "true";
        internal static Settings GetSettings()
        {
            Settings settings;
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            string settingsFilePath = string.Concat(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath, ".cmp.xml");

            using (FileStream fileStream = new FileStream(settingsFilePath, FileMode.Open))
            {
                settings = (Settings)serializer.Deserialize(fileStream);
            }
            return settings;
        }

        internal static void RegisterException(string message, Type type)
        {
            Exception instance;
            try
            {
                instance = (Exception)Activator.CreateInstance(type, message);
            }
            catch (Exception ex)
            {
                Log.LogWarning(_assemblyName, ex);
                return;
            }
            RegisterException(instance);
        }

        internal static void RegisterException(Exception exception)
        {
            if (exception is null)
            {
                Log.LogWarning(_assemblyName, "No specified exception to be registered");
                return;
            }
            Log.LogWarning(_assemblyName, exception);
            if (TypescriptHttpModule.IsDebugMode)
            {
                throw exception;
            }
        }
    }
}