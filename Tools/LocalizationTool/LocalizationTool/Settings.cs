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
			get
			{
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}

		public static string LocalizationDirectory
		{
			get
			{
				return Path.Combine(Settings.ApplicationDirectory, Settings.CompositeSiteRelativePath + "\\Composite\\localization");
			}
		}

		public static string TargetLocalizationDirectory
		{
			get
			{
				return Path.Combine(Settings.ApplicationDirectory, Settings.CompositeSiteRelativePath + "\\App_Data\\Composite\\LanguagePacks", ConfigurationSettings.AppSettings["targetCultureName"]);
			}
		}

		public static CultureInfo SourceCulture
		{
			get
			{
				string sourceCultureName = ConfigurationSettings.AppSettings["sourceCultureName"];
				return CultureInfo.CreateSpecificCulture(sourceCultureName);
			}
		}

		public static CultureInfo TargetCulture
		{
			get
			{
				string targetCultureName = ConfigurationSettings.AppSettings["targetCultureName"];
				return CultureInfo.CreateSpecificCulture(targetCultureName);
			}
		}

		public static string CompositeConfigRelativePath
		{
			get
			{
					return Path.Combine(Settings.ApplicationDirectory, Settings.CompositeSiteRelativePath + "\\App_Data\\Composite\\Composite.config");
			}
		}

		public static string WebConfigRelativePath
		{
			get
			{
				return Path.Combine(Settings.ApplicationDirectory, Settings.CompositeSiteRelativePath + "\\web.config");
			}
		}

		public static string CompositeSiteRelativePath
		{
			get
			{
				string sitePath = ConfigurationSettings.AppSettings["websitePath"];
				return Path.Combine(Settings.ApplicationDirectory, sitePath);
			}
		}

	}
}
