using System;
using static System.Configuration.ConfigurationManager;

namespace Orckestra.Web.BundlingAndMinification
{
    public static class CommonValues
    {
        public const string AppNameForLogs = "Bundling and minification";
        private const string AppSettingsPath = "Orckestra.Web.BundlingAndMinification";
        public const string BundlesCacheFolder = "~/App_Data/Composite/Cache/Bundles/";

        public static readonly bool BundleMinifyScripts = AppSettings[$"{AppSettingsPath}.BundleAndMinifyScripts"]
                                                            .Equals("true", StringComparison.OrdinalIgnoreCase);

        public static readonly bool BundleMinifyStyles = AppSettings[$"{AppSettingsPath}.BundleAndMinifyStyles"]
                                                            .Equals("true", StringComparison.OrdinalIgnoreCase);

        public const string BundlePathPartScripts = "Scripts_";
        public const string BundlePathPartStyles = "Styles_";
        public const string CacheFileTypeScripts = ".scripts";
        public const string CacheFileTypeStyles = ".styles";
    }
}