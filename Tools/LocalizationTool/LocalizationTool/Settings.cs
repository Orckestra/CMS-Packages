using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace LocalizationTool
{
	public static class Settings
	{
		public static string ApplicationDirectory
		{
			get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
		}

		public static string LocalizationDirectory
		{
			get { return Path.Combine(Settings.ApplicationDirectory, Settings.CompositeSiteRelativePath + "\\Composite\\localization"); }
		}

		public static string TargetLocalizationDirectory
		{
			get
			{
				return Path.Combine(Settings.ApplicationDirectory,
									Settings.CompositeSiteRelativePath + "\\App_Data\\Composite\\LanguagePacks",
									ConfigurationManager.AppSettings["targetCultureName"]);
			}
		}

		public static CultureInfo SourceCulture
		{
			get
			{
				var sourceCultureName = ConfigurationManager.AppSettings["sourceCultureName"];
				return CultureInfo.CreateSpecificCulture(sourceCultureName);
			}
		}

		public static CultureInfo TargetCulture
		{
			get
			{
				var targetCultureName = ConfigurationManager.AppSettings["targetCultureName"];
				return CultureInfo.CreateSpecificCulture(targetCultureName);
			}
		}

		public static string CompositeConfigRelativePath
		{
			get
			{
				return Path.Combine(Settings.ApplicationDirectory,
									Settings.CompositeSiteRelativePath + "\\App_Data\\Composite\\Composite.config");
			}
		}

		public static string WebConfigRelativePath
		{
			get { return Path.Combine(Settings.ApplicationDirectory, Settings.CompositeSiteRelativePath + "\\web.config"); }
		}

		public static string CompositeSiteRelativePath
		{
			get
			{
				var sitePath = ConfigurationManager.AppSettings["websitePath"];
				return Path.Combine(Settings.ApplicationDirectory, sitePath);
			}
		}

		public static string ReportsDirectory
		{
			get { return Path.Combine(ApplicationDirectory, "reports"); }
		}

		public static string UnknownStringsFilePath
		{
			get { return Path.Combine(ApplicationDirectory, "UnknownStrings.xml"); }
		}

		public static string FlagsFilePath
		{
			get { return Path.Combine(ApplicationDirectory, "Flags.xml"); }
		}
	}
}