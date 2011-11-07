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
using Composite.Tools.PackageCreator.ElementProvider;
using Composite.Tools.PackageCreator.Types;


namespace Composite.Tools.PackageCreator
{
	public static class PackageCreatorFacade
	{
		private static readonly string _packageCreatorPath = "App_Data/PackageCreator";
		private static readonly string _providerName = "Composite.Tools.PackageCreator";
		private static readonly string _activeFilePath = "ActivePackage";
		private static string _activePackageName;
		private static XNamespace pc = PackageCreator.pc;
		private static XNamespace mi = PackageCreator.mi;
		private static XNamespace help = PackageCreator.help;

		private static object _lockEditPackage = new object();

		public static string ProviderName
		{
			get
			{
				return _providerName;
			}
		}


		public static string ActivePackageName
		{
			get
			{
				if (_activePackageName == null)
				{
					if (File.Exists(GetAbsolutePath(_activeFilePath)))
						_activePackageName = File.ReadAllText(GetAbsolutePath(_activeFilePath)).Trim();
				}
				if (_activePackageName == null || !File.Exists(GetPackageConfigPath(_activePackageName)))
				{
					ActivePackageName = GetPackageNames().FirstOrDefault();
				}
				return _activePackageName;
			}
			set
			{
				_activePackageName = value;
				if (_activePackageName != null)
				{
					File.WriteAllText(GetAbsolutePath(_activeFilePath), _activePackageName);
				}
			}
		}

		[ThreadStatic]
		private static ContextItems _currentContextItems;

		private static ContextItems CurrentContextItems
		{
			get
			{
				if (_currentContextItems == null)
				{
					_currentContextItems = new ContextItems();
				}
				else if (!_currentContextItems.IsCurrentContext)
				{
					_currentContextItems = new ContextItems();
				}
				return _currentContextItems;
			}
		}

		public static bool IsHaveAccess
		{
			get
			{
				return CurrentContextItems.IsHaveAccess;
			}
		}

		internal class ContextItems
		{
			private int _contextHashCode;
			public bool IsCurrentContext
			{
				get
				{
					if (HttpContext.Current == null)
					{
						return false;
					}
					if (HttpContext.Current.GetHashCode() == _contextHashCode)
					{
						return true;
					}
					return false;
				}
			}
			public ContextItems()
			{
				if (HttpContext.Current != null)
				{
					_contextHashCode = HttpContext.Current.GetHashCode();
				}
			}

			private bool? _isHaveAccess;
			public bool IsHaveAccess
			{
				get
				{
					if (_isHaveAccess == null)
					{
						_isHaveAccess = false;
						if (UserValidationFacade.IsLoggedIn())
						{
							EntityToken packageCreatorPerspective = UserPerspectiveFacade.GetEntityTokens(UserValidationFacade.GetUsername()).Where(e => e.Id == "Composite.Tools.PackageCreator").FirstOrDefault();

							//If user does no have access to the Package Creator Perspective
							if (packageCreatorPerspective != null)
								_isHaveAccess = true;
						}
					}
					return _isHaveAccess.Value;
				}
			}
		}

		/// <summary>
		/// Save package information to the config file
		/// </summary>
		/// <param name="package">Package information</param>
		public static void SavePackageInformation(PackageInformation package)
		{
			SavePackageInformation(package, package.Name);
		}

		/// <summary>
		/// Save package information to the config file
		/// </summary>
		/// <param name="package">Package information</param>
		/// <param name="packageName">Previous package name</param>
		public static void SavePackageInformation(PackageInformation package, string packageName)
		{

			lock (_lockEditPackage)
			{
				if (!package.Name.Equals(packageName))
				{
					try
					{
						File.Move(GetPackageConfigPath(packageName), GetPackageConfigPath(package.Name));
					}
					catch
					{
						throw new InvalidOperationException("New package name is not valid");
					}
				}
				XDocument packageXml = GetPackageXml(package.Name);
				var packageInformation = packageXml.Root.ForceElement(mi + "PackageInformation");
				var packageRequirements = packageXml.Root.ForceElement(mi + "PackageRequirements");
				packageInformation.SetAttributeValue("name", package.Name);
				packageInformation.SetAttributeValue("groupName", package.GroupName);
				packageInformation.SetAttributeValue("version", package.Version);
				packageInformation.SetAttributeValue("author", package.Author);
				packageInformation.SetAttributeValue("website", package.Website);
				packageInformation.SetAttributeValue("id", package.Id);
				packageInformation.SetAttributeValue("canBeUninstalled", package.CanBeUninstalled);
				packageInformation.SetAttributeValue("systemLocking", package.SystemLockingType);
				packageInformation.SetAttributeValue("flushOnCompletion", package.FlushOnCompletion);
				if (package.ReloadConsoleOnCompletion)
				{
					packageInformation.SetAttributeValue("reloadConsoleOnCompletion", "true");
				}
				packageInformation.ForceElement("Description").Value = package.Description;

				packageRequirements.SetAttributeValue("minimumCompositeVersion", package.MinCompositeVersionSupported.ToString());
				packageRequirements.SetAttributeValue("maximumCompositeVersion", package.MaxCompositeVersionSupported.ToString());

				packageXml.SaveTabbed(GetPackageConfigPath(package.Name));
			}
		}

		internal static void DeletePackageInformation(string packageName)
		{
			lock (_lockEditPackage)
			{
				var packageConfigPath = GetPackageConfigPath(packageName);
				if (File.Exists(packageConfigPath))
				{
					File.Delete(packageConfigPath);
				}
			}
		}

		public static PackageInformation GetPackageInformation(string packageName)
		{
			var package = new PackageInformation();
			var packageXml = GetPackageXml(packageName);
			var packageInformation = packageXml.Root.ForceElement(mi + "PackageInformation");
			var packageRequirements = packageXml.Root.ForceElement(mi + "PackageRequirements");

			package.Id = packageInformation.AttributeValue("id") != null ? new Guid(packageInformation.AttributeValue("id")) : Guid.NewGuid();

			package.Author = packageInformation.AttributeValue("author") ?? "";
			package.GroupName = packageInformation.AttributeValue("groupName") ?? UserSettings.LastSpecifiedNamespace;
			package.Name = packageInformation.AttributeValue("name") ?? package.GroupName + ".NewPackage";
			package.Version = packageInformation.AttributeValue("version") ?? "1.0.0";
			var request = HttpContext.Current.Request;
			package.Website = packageInformation.AttributeValue("website") ?? (new Uri(request.Url, request.ApplicationPath)).ToString();
			package.Description = string.IsNullOrEmpty(packageInformation.ForceElement("Description").Value) ? string.Empty : packageInformation.ForceElement("Description").Value;//string.Format("Created by {0}", package.Author) : packageInformation.ForceElement("Description").Value;

			package.FlushOnCompletion = bool.Parse(packageInformation.AttributeValue("flushOnCompletion") ?? false.ToString());
			package.CanBeUninstalled = bool.Parse(packageInformation.AttributeValue("canBeUninstalled") ?? true.ToString());
			package.SystemLockingType = packageInformation.AttributeValue("systemLocking")??"hard";

			package.ReloadConsoleOnCompletion = bool.Parse(packageInformation.AttributeValue("reloadConsoleOnCompletion") ?? false.ToString()); ;

			package.MinCompositeVersionSupported = (packageRequirements.AttributeValue("minimumCompositeVersion") != null) ? new Version(packageRequirements.AttributeValue("minimumCompositeVersion")) : RuntimeInformation.ProductVersion;
			package.MaxCompositeVersionSupported = new Version(packageRequirements.AttributeValue("maximumCompositeVersion") ?? "9.9999.9999.9999");

			return package;
		}

		private static bool _isPackageCreatorPathCreated = false;
		private static object _isPackageCreatorPathCreatedLock = new object();
		/// <summary>
		/// Return absolute path of file from packagecreator directory
		/// </summary>
		/// <param name="path">relative path in packagecreator directory</param>
		/// <returns>Absolute file path</returns>
		private static string GetAbsolutePath(string path)
		{
			var directory = PathCombine(PathUtil.Resolve(PathUtil.BaseDirectory), _packageCreatorPath);

			if (!_isPackageCreatorPathCreated)
			{
				lock (_isPackageCreatorPathCreatedLock)
				{
					if (!Directory.Exists(directory))
						Directory.CreateDirectory(directory);
					_isPackageCreatorPathCreated = true;
				}
			}

			return PathCombine(directory, path);
		}

		/// <summary>
		/// Return absolute path to package config file
		/// </summary>
		/// <param name="packageName">package name</param>
		/// <returns>Absolute file path</returns>
		public static string GetPackageConfigPath(string packageName)
		{
			return GetAbsolutePath(string.Format("{0}.xml", packageName));
		}

		/// <summary>
		/// Multiply combine path strings
		/// </summary>
		/// <param name="path">Path strings</param>
		/// <returns>A string containing the combined paths</returns>
		private static string PathCombine(params string[] path)
		{
			if (path.Length == 0)
				return string.Empty;
			return path.Aggregate((path1, path2) => Path.Combine(path1, path2));
		}

		public static ActionType ActionType
		{
			get
			{
				return ActionType.Other;
			}
		}

		/// <summary>
		/// Create package
		/// </summary>
		/// <param name="PackageName">package name</param>
		/// <returns>Web uri to the created package</returns>
		public static string CreatePackage(string packageName)
		{
			var packageCreator = new PackageCreator(GetAbsolutePath("."), packageName);
			var packagePath = packageCreator.CreatePackagePack();
			var request = HttpContext.Current.Request;
			var fullpath = GetAbsolutePath(packagePath);
			return fullpath;

		}

		internal static void AddItem(IPackageCreatorItem item)
		{
			AddItem(item, ActivePackageName);

		}

		internal static void AddItem(IPackageCreatorItem item, string packageName)
		{
			lock (_lockEditPackage)
			{
				var packageXml = GetPackageXml(packageName);
				item.AddToConfiguration(packageXml.Root);
				packageXml.SaveTabbed(GetPackageConfigPath(packageName));
			}

		}

		internal static void RemoveItem(IPackageCreatorItem item, string packageName)
		{
			lock (_lockEditPackage)
			{
				var packageXml = GetPackageXml(packageName);
				item.RemoveFromConfiguration(packageXml.Root);
				packageXml.SaveTabbed(GetPackageConfigPath(packageName));
			}
		}

		internal static IEnumerable<PCCategoryAttribute> GetCategories(string packageName)
		{
			XDocument packageXml;
			lock (_lockEditPackage)
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

		internal static IEnumerable<IPackageCreatorItem> GetItems(string categoryName, string packageName)
		{
			XDocument packageXml;
			lock (_lockEditPackage)
			{
				packageXml = GetPackageXml(packageName);
			}
			return GetItems(PackageCreatorActionFacade.CategoryTypes.Get(categoryName), packageXml.Root);
		}

		internal static IEnumerable<IPackageCreatorItem> GetItems(Type type, XElement config)
		{
			var method = type.GetMethod("GetItems",
				BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static,
				null,
				CallingConventions.Any,
				new Type[] { typeof(Type), typeof(XElement) },
				null
			);

			if (method == null)
			{
				LoggingService.LogError("Type {0} doesnot have static methos GetItems", type.FullName);
				yield break;
			}
			var results = (IEnumerable<IPackageCreatorItem>)method.Invoke(Type.Missing, new object[] { type, config });
			foreach (var result in results)
			{
				yield return result;
			}
		}


		internal static IEnumerable<string> GetPackageNames()
		{
			return from f in Directory.GetFiles(GetAbsolutePath("."), "*.xml")
				   select Path.GetFileNameWithoutExtension(f);
		}

		/// <summary>
		/// Get package configuration
		/// </summary>
		/// <param name="packageName"></param>
		/// <returns>Package Configuration</returns>
		private static XDocument GetPackageXml(string packageName)
		{
			XDocument package = null;
			if (File.Exists(GetPackageConfigPath(packageName)))
			{
				try
				{
					package = XDocument.Load(GetPackageConfigPath(packageName));
				}
				catch
				{
					LoggingService.LogError("PackageCreator", string.Format("Package config {0} is not xml", packageName));
				}
			}
			if (package == null)
			{
				package = new XDocument();
			}

			if (package.Root == null || package.Root.Name != pc + "PackageCreator")
			{
				var Root = new XElement(pc + "PackageCreator");
				Root.Add(new XAttribute(XNamespace.Xmlns + "mi", mi));
				Root.Add(new XAttribute(XNamespace.Xmlns + "pc", pc));
				Root.Add(new XAttribute(XNamespace.Xmlns + "help", help));
				package.RemoveNodes();
				package.Add(Root);
			}

			return package;
		}

		internal static string GetLocalization(string part)
		{
			return StringResourceSystemFacade.GetString(ProviderName, string.Format("{0}.{1}", ProviderName, part));
		}

		internal static XDocument GetConfigurationDocument()
		{
			var configurationPath = PathUtil.Resolve("~/App_Data/Composite/Composite.config");
			var configuration = XDocument.Load(configurationPath);
			return configuration;
		}

		internal static void AddConfig(UploadedFile uploadedFile)
		{
			var extension = Path.GetExtension(uploadedFile.FileName);
			if (extension.ToLower() != ".xml")
			{
				throw new Exception(GetLocalization("Error.FileIsNotXml"));
			}
			var packageName = Path.GetFileNameWithoutExtension(uploadedFile.FileName);
			lock (_lockEditPackage)
			{
				var configPath = PackageCreatorFacade.GetPackageConfigPath(packageName);
				if (File.Exists(configPath))
					throw new Exception(GetLocalization("Error.PackageAlreadyExist"));

				using (Stream readStream = uploadedFile.FileStream)
				{
					using (FileStream writeStream = File.Open(configPath, FileMode.CreateNew))
					{
						readStream.CopyTo(writeStream);
					}

				}
			}
		}
	}

	internal static class PackageCreatorFacadeExtension
	{
		/// <summary>
		/// Gets the first (in document order) or new(if doesnot exists) child element with the specified System.Xml.Linq.XName
		/// </summary>
		/// <param name="element"></param>
		/// <param name="name">The System.Xml.Linq.XName to match</param>
		/// <returns> A System.Xml.Linq.XElement that matches the specified System.Xml.Linq.XName or null if element is null</returns>
		public static XElement ForceElement(this XElement element, XName name)
		{
			if (element == null)
				return null;

			var result = element.Element(name);

			if (result == null)
			{
				result = new XElement(name);
				element.AddFirst(result);
			}

			return result;
		}

		public static string AttributeValue(this XElement element, XName name)
		{
			var attribute = element.Attribute(name);
			if (attribute != null)
			{
				return attribute.Value;
			}
			return null;
		}

		private static readonly string[] indexNames = new string[] { "name", "fullName", "path", "fileName" };

		public static string IndexAttributeValue(this XElement element)
		{
			foreach (var name in indexNames)
			{
				var value = element.AttributeValue(name);
				if (value != null)
					return value;
			}
			return null;
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
