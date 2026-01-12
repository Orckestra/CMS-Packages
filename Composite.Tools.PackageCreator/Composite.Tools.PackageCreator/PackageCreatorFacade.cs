using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using Composite.C1Console.Elements;
using Composite.C1Console.Forms.CoreUiControls;
using Composite.C1Console.Security;
using Composite.C1Console.Users;
using Composite.Core.IO;
using Composite.Core.Logging;
using Composite.Core.ResourceSystem;
using Composite.Tools.PackageCreator.Actions;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;
using Composite.Tools.PackageCreator.Types;

namespace Composite.Tools.PackageCreator
{
    public static class PackageCreatorFacade
    {
        private const string PackageCreatorPath = "App_Data/PackageCreator";
        public const string AllowOverwrite = "allowOverwrite";
        public const string IncludePageData = "includePageData";

        //private static readonly string _providerName = "Composite.Tools.PackageCreator";
        private const string ActiveFilePath = "ActivePackage";
        private static string _activePackageName;
        private static readonly XNamespace NsPc = PackageCreator.NsPc;
        private static readonly XNamespace NsMi = PackageCreator.NsMi;
        private static readonly XNamespace NsHelp = PackageCreator.Help;

        private static readonly object LockEditPackage = new object();

        [ThreadStatic] private static ContextItems _currentContextItems;

        private static bool _isPackageCreatorPathCreated;
        private static readonly object IsPackageCreatorPathCreatedLock = new object();

        public static string ProviderName => "Composite.Tools.PackageCreator";
        public static string RazorFunctionProviderName => "RazorFunctionProvider";
        public static string UserControlFunctionProviderName => "UserControlFunctionProvider";

        public static string ActivePackageName
        {
            get
            {
                if (_activePackageName == null && File.Exists(GetAbsolutePath(ActiveFilePath))) 
                    _activePackageName = File.ReadAllText(GetAbsolutePath(ActiveFilePath)).Trim();

                if (_activePackageName == null || !File.Exists(GetPackageConfigPath(_activePackageName)))
                    ActivePackageName = GetPackageNames().FirstOrDefault();

                return _activePackageName;
            }
            set
            {
                _activePackageName = value;

                if (_activePackageName != null) 
                    File.WriteAllText(GetAbsolutePath(ActiveFilePath), _activePackageName);
            }
        }

        private static ContextItems CurrentContextItems
        {
            get
            {
                if (_currentContextItems == null || !_currentContextItems.IsCurrentContext)
                    _currentContextItems = new ContextItems();

                return _currentContextItems;
            }
        }

        public static ActionType ActionType => ActionType.Other;

        /// <summary>
        ///     Save package information to the config file
        /// </summary>
        /// <param name="package">Package information</param>
        public static void SavePackageInformation(PackageInformation package)
        {
            SavePackageInformation(package, package.Name);
        }

        /// <summary>
        ///     Save package information to the config file
        /// </summary>
        /// <param name="package">Package information</param>
        /// <param name="packageName">Previous package name</param>
        public static void SavePackageInformation(PackageInformation package, string packageName)
        {
            lock (LockEditPackage)
            {
                if (!package.Name.Equals(packageName))
                    try
                    {
                        File.Move(GetPackageConfigPath(packageName), GetPackageConfigPath(package.Name));
                    }
                    catch
                    {
                        throw new InvalidOperationException("New package name is not valid");
                    }

                var packageXml = GetPackageXml(package.Name);
                var packageInformation = packageXml.Root.ForceElement(NsMi + "PackageInformation");
                var packageRequirements = packageXml.Root.ForceElement(NsMi + "PackageRequirements");
                packageInformation.SetAttributeValue("name", package.Name);
                packageInformation.SetAttributeValue("groupName", package.GroupName);
                packageInformation.SetAttributeValue("version", package.Version);
                packageInformation.SetAttributeValue("author", package.Author);
                packageInformation.SetAttributeValue("website", package.Website);
                packageInformation.SetAttributeValue("readMoreUrl", package.ReadMoreUrl);
                packageInformation.SetAttributeValue("id", package.Id);
                packageInformation.SetAttributeValue("canBeUninstalled", package.CanBeUninstalled);
                packageInformation.SetAttributeValue("systemLocking", package.SystemLockingType);
                packageInformation.SetAttributeValue("flushOnCompletion", package.FlushOnCompletion);
                packageInformation.SetAttributeValue(AllowOverwrite, package.OverwriteDataOnInstall);
                packageInformation.SetAttributeValue(IncludePageData, package.IncludePageData);
                if (package.ReloadConsoleOnCompletion)
                    packageInformation.SetAttributeValue("reloadConsoleOnCompletion", "true");
                packageInformation.ForceElement("Description").Value = package.Description;
                packageInformation.ForceElement("TechnicalDetails").Value = package.TechnicalDetails;

                packageRequirements.SetAttributeValue("minimumCompositeVersion", package.MinCompositeVersionSupported.ToString());
                packageRequirements.SetAttributeValue("maximumCompositeVersion", package.MaxCompositeVersionSupported.ToString());

                packageXml.SaveTabbed(GetPackageConfigPath(package.Name));
            }
        }

        internal static void DeletePackageInformation(string packageName)
        {
            lock (LockEditPackage)
            {
                var packageConfigPath = GetPackageConfigPath(packageName);

                if (File.Exists(packageConfigPath)) 
                    File.Delete(packageConfigPath);
            }
        }

        public static PackageInformation GetPackageInformation(string packageName)
        {
            var package = new PackageInformation();
            var packageXml = GetPackageXml(packageName);
            var packageInformation = packageXml.Root.ForceElement(NsMi + "PackageInformation");
            var packageRequirements = packageXml.Root.ForceElement(NsMi + "PackageRequirements");

            package.Id = packageInformation.AttributeValue("id") != null
                ? new Guid(packageInformation.AttributeValue("id"))
                : Guid.NewGuid();

            package.Author = packageInformation.AttributeValue("author") ?? string.Empty;

            package.GroupName = packageInformation.AttributeValue("groupName") ?? UserSettings.LastSpecifiedNamespace;
            package.Name = packageInformation.AttributeValue("name") ?? package.GroupName + ".NewPackage";
            package.ReadMoreUrl = packageInformation.AttributeValue("readMoreUrl") ?? string.Empty;

            package.Version = packageInformation.AttributeValue("version") ?? "1.0.0";
            var request = HttpContext.Current.Request;
            package.Website = packageInformation.AttributeValue("website") ?? new Uri(request.Url, request.ApplicationPath).ToString();

            package.Description = string.IsNullOrEmpty(packageInformation.ForceElement("Description").Value)
                ? string.Empty
                : packageInformation.ForceElement("Description").Value; //string.Format("Created by {0}", package.Author) : packageInformation.ForceElement("Description").Value;
            package.TechnicalDetails = string.IsNullOrEmpty(packageInformation.ForceElement("TechnicalDetails").Value)
                ? string.Empty
                : packageInformation.ForceElement("TechnicalDetails").Value;

            package.FlushOnCompletion = bool.Parse(packageInformation.AttributeValue("flushOnCompletion") ?? true.ToString());
            package.CanBeUninstalled = bool.Parse(packageInformation.AttributeValue("canBeUninstalled") ?? true.ToString());
            package.SystemLockingType = packageInformation.AttributeValue("systemLocking") ?? "hard";

            package.ReloadConsoleOnCompletion = bool.Parse(packageInformation.AttributeValue("reloadConsoleOnCompletion") ?? false.ToString());
            package.OverwriteDataOnInstall = bool.Parse(packageInformation.AttributeValue(AllowOverwrite) ?? false.ToString());
            package.IncludePageData = bool.Parse(packageInformation.AttributeValue(IncludePageData) ?? false.ToString());

            package.MinCompositeVersionSupported = packageRequirements.AttributeValue("minimumCompositeVersion") != null
                ? new Version(packageRequirements.AttributeValue("minimumCompositeVersion"))
                : RuntimeInformation.ProductVersion;
            package.MaxCompositeVersionSupported = new Version(packageRequirements.AttributeValue("maximumCompositeVersion") ?? "9.9999.9999.9999");

            return package;
        }

        /// <summary>
        ///     Return absolute path of file from packagecreator directory
        /// </summary>
        /// <param name="path">relative path in packagecreator directory</param>
        /// <returns>Absolute file path</returns>
        private static string GetAbsolutePath(string path)
        {
            var directory = PathCombine(PathUtil.Resolve(PathUtil.BaseDirectory), PackageCreatorPath);

            if (!_isPackageCreatorPathCreated)
                lock (IsPackageCreatorPathCreatedLock)
                {
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    _isPackageCreatorPathCreated = true;
                }

            return PathCombine(directory, path);
        }

        /// <summary>
        ///     Return absolute path to package config file
        /// </summary>
        /// <param name="packageName">package name</param>
        /// <returns>Absolute file path</returns>
        public static string GetPackageConfigPath(string packageName)
        {
            return GetAbsolutePath($"{packageName}.xml");
        }

        /// <summary>
        ///     Multiply combine path strings
        /// </summary>
        /// <param name="path">Path strings</param>
        /// <returns>A string containing the combined paths</returns>
        private static string PathCombine(params string[] path)
        {
            return path.Length == 0 ? string.Empty : path.Aggregate(Path.Combine);
        }

        /// <summary>
        ///     Create package
        /// </summary>
        /// <param name="packageName">package name</param>
        /// <returns>Web uri to the created package</returns>
        public static string CreatePackage(string packageName)
        {
            var packageCreator = new PackageCreator(GetAbsolutePath("."), packageName);
            var packagePath = packageCreator.CreatePackagePack();
            var fullPath = GetAbsolutePath(packagePath);
            return fullPath;
        }

        internal static void AddItem(IPackItem item)
        {
            AddItem(item, ActivePackageName);
            ActivePackageName = item.Id;
        }

        internal static void AddItem(IPackItem item, string packageName)
        {
            lock (LockEditPackage)
            {
                var packageXml = GetPackageXml(packageName);
                item.AddToConfiguration(packageXml.Root);
                packageXml.SaveTabbed(GetPackageConfigPath(packageName));
            }
        }

        internal static void RemoveItem(IPackItem item, string packageName)
        {
            lock (LockEditPackage)
            {
                var packageXml = GetPackageXml(packageName);
                item.RemoveFromConfiguration(packageXml.Root);
                packageXml.SaveTabbed(GetPackageConfigPath(packageName));
            }
        }

        internal static void ToggleIncludePageData(string packageName)
        {
            lock (LockEditPackage)
            {
                var packageXml = GetPackageXml(packageName);
                var element = packageXml.Root.Elements(PackageCreator.NsMi + "PackageInformation").FirstOrDefault();

                if (element == null)
                    return;

                element.SetAttributeValue(IncludePageData, !element.IncludePageDataAttributeValue());
                packageXml.SaveTabbed(GetPackageConfigPath(packageName));
            }
        }

        internal static bool DoesIncludePageData(string packageName)
        {
            var packageXml = GetPackageXml(packageName);
            var element = packageXml.Root.Elements(PackageCreator.NsMi + "PackageInformation").FirstOrDefault();

            return element != null && element.IncludePageDataAttributeValue();
        }

        internal static IEnumerable<PackCategoryAttribute> GetCategories(string packageName)
        {
            XDocument packageXml;
            lock (LockEditPackage)
            {
                packageXml = GetPackageXml(packageName);
            }

            foreach (var category in PackageCreatorActionFacade.CategoryTypes)
            {
                var items = GetItems(category.Value, packageXml.Root);
                if (items.GetEnumerator().MoveNext())
                    yield return category.Key;
            }
        }

        internal static IEnumerable<T> GetItems<T>() where T : IPackItem
        {
            return GetItems(typeof(T), ActivePackageName).Cast<T>();
        }

        internal static IEnumerable<IPackItem> GetItems(string categoryName, string packageName)
        {
            return GetItems(PackageCreatorActionFacade.CategoryTypes.Get(categoryName), packageName);
        }

        internal static IEnumerable<IPackItem> GetItems(Type categoryType, string packageName)
        {
            XDocument packageXml;
            lock (LockEditPackage)
            {
                packageXml = GetPackageXml(packageName);
            }

            return GetItems(categoryType, packageXml.Root);
        }

        internal static IEnumerable<IPackItem> GetItems(Type type, XElement config)
        {
            var manager = ItemManagerCache.GetItemManager(type);
            if (manager != null)
            {
                var results = manager.GetItems(type, config);
                foreach (var result in results) 
                    yield return result;
            }
            else
            {
                var method = type.GetMethod("GetItems",
                    BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static,
                    null,
                    CallingConventions.Any,
                    new[] { typeof(Type), typeof(XElement) },
                    null
                );

                if (method == null)
                {
                    LoggingService.LogError("Type {0} doesn't have static methods GetItems", type.FullName);
                    yield break;
                }

                var results = (IEnumerable<IPackItem>)method.Invoke(Type.Missing, new object[] { type, config });
                foreach (var result in results) 
                    yield return result;
            }
        }


        internal static IEnumerable<string> GetPackageNames()
        {
            return Directory.GetFiles(GetAbsolutePath("."), "*.xml").Select(Path.GetFileNameWithoutExtension);
        }

        /// <summary>
        ///     Get package configuration
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns>Package Configuration</returns>
        private static XDocument GetPackageXml(string packageName)
        {
            XDocument package = null;
            if (File.Exists(GetPackageConfigPath(packageName)))
                try
                {
                    package = XDocument.Load(GetPackageConfigPath(packageName));
                }
                catch
                {
                    LoggingService.LogError("PackageCreator", $"Package config {packageName} is not xml");
                }

            if (package == null) package = new XDocument();

            if (package.Root == null || package.Root.Name != NsPc + "PackageCreator")
            {
                var root = new XElement(NsPc + "PackageCreator");
                root.Add(new XAttribute(XNamespace.Xmlns + "mi", NsMi));
                root.Add(new XAttribute(XNamespace.Xmlns + "pc", NsPc));
                root.Add(new XAttribute(XNamespace.Xmlns + "help", NsHelp));
                package.RemoveNodes();
                package.Add(root);
            }

            return package;
        }

        internal static string GetLocalization(string part)
        {
            return StringResourceSystemFacade.GetString(ProviderName, $"{ProviderName}.{part}");
        }

        internal static XDocument GetConfigurationDocument(string source)
        {
            var configurationPath = GetConfigurationPath(source);
            if (configurationPath == null) 
                return null;

            configurationPath = PathUtil.Resolve(configurationPath);
            return XDocument.Load(configurationPath);

        }

        internal static string GetConfigurationPath(string source)
        {
            switch (source)
            {
                case PCCompositeConfig.Source:
                    return PCCompositeConfig.Path;
                case PCWebConfig.Source:
                    return PCWebConfig.Path;
            }

            return null;
        }

        internal static void AddConfig(UploadedFile uploadedFile)
        {
            var extension = Path.GetExtension(uploadedFile.FileName);

            if (extension.ToLower() != ".xml") 
                throw new Exception(GetLocalization("Error.FileIsNotXml"));

            var packageName = Path.GetFileNameWithoutExtension(uploadedFile.FileName);

            lock (LockEditPackage)
            {
                var configPath = GetPackageConfigPath(packageName);

                if (File.Exists(configPath))
                    throw new Exception(GetLocalization("Error.PackageAlreadyExist"));

                using (var readStream = uploadedFile.FileStream)
                {
                    using (var writeStream = File.Open(configPath, FileMode.CreateNew))
                    {
                        readStream.CopyTo(writeStream);
                    }
                }
            }
        }

        internal class ContextItems
        {
            private readonly int _contextHashCode;

            public ContextItems()
            {
                if (HttpContext.Current != null) _contextHashCode = HttpContext.Current.GetHashCode();
            }

            public bool IsCurrentContext
            {
                get
                {
                    if (HttpContext.Current == null) return false;
                    return HttpContext.Current.GetHashCode() == _contextHashCode;
                }
            }
        }
    }

    internal static class PackageCreatorFacadeExtension
    {
        private static readonly string[] indexNames = { "name", "fullName", "path", "fileName" };

        /// <summary>
        ///     Gets the first (in document order) or new (if doesn't exist) child element with the specified System.Xml.Linq.XName
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name">The System.Xml.Linq.XName to match</param>
        /// <returns> A System.Xml.Linq.XElement that matches the specified System.Xml.Linq.XName or null if element is null</returns>
        public static XElement ForceElement(this XElement element, XName name)
        {
            if (element == null)
                return null;

            var result = element.Element(name);

            if (result != null) 
                return result;

            result = new XElement(name);
            element.AddFirst(result);

            return result;
        }

        public static string AttributeValue(this XElement element, XName name)
        {
            var attribute = element.Attribute(name);
            return attribute?.Value;
        }

        public static string IndexAttributeValue(this XElement element)
        {
            return indexNames.Select(name => element.AttributeValue(name)).FirstOrDefault(value => value != null);
        }

        public static bool AllowOverwriteAttributeValue(this XElement element)
        {
            var value = element.AttributeValue(PackageCreatorFacade.AllowOverwrite);
            return value != null && bool.Parse(value);
        }

        public static bool IncludePageDataAttributeValue(this XElement element)
        {
            var value = element.AttributeValue(PackageCreatorFacade.IncludePageData);
            return value != null && bool.Parse(value);
        }

        public static string IndexAttributeName(this XElement element)
        {
            return indexNames.FirstOrDefault();
        }

        public static EntityToken GetEntityToken(this PackageInformation packageInformation)
        {
            return new PackageCreatorPackageElementProviderEntityToken(packageInformation.Name);
        }
    }
}