using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace C1PackageDevProvisioner
{
    public static class Configration
    {
        private const string CfgName_C1SitePath = "C1SitePath";
        private const string CfgName_PackageProjectsPaths = "PackageProjectsPaths";
        private const string CfgName_C1BaseImageZipUrl = "C1BaseImageZipUrl";
        private const string CfgName_BaseImagesPath = "BaseImagesPath";
        private const string CfgName_IISExpressPath = "IISExpressPath";
        private const string CfgName_MockPackageServerPort = "MockPackageServerPort";
        private const string CfgName_MockPackageServerPath = "MockPackageServerPath";
        private const string CfgName_SetupDescriptionPath = "SetupDescriptionPath";
        private const string CfgName_MsBuildPath = "MsBuildPath";
        private const string CfgName_NugetPath = "NugetPath";


        public static string C1SitePath
        {
            get
            {
                if (Path.IsPathRooted(C1SitePathRaw))
                {
                    return C1SitePathRaw;
                }

                var fullPath = FileUtil.MapIfRelative(C1SitePathRaw);
                return fullPath;
            }
        }

        public static IEnumerable<string> PackageProjectsPaths
        {
            get
            {
                foreach (var path in PackageProjectsPathsRaw.Split(',').Where(f => string.IsNullOrEmpty(f) == false).Select(f => f.Trim()))
                {
                    if (Path.IsPathRooted(path))
                    {
                        yield return path;
                    }
                    else
                    {
                        var fullPath = FileUtil.MapIfRelative(path);
                        yield return fullPath;
                    }
                }
            }
        }


        public static string C1AppDataCompositePath
        {
            get
            {
                var fullPath = Path.Combine(C1SitePath, "App_Data/Composite");
                return fullPath;
            }
        }


        public static string C1BaseImageZipUrl
        {
            get
            {
                return ConfigurationManager.AppSettings[CfgName_C1BaseImageZipUrl];
            }
        }


        public static string BaseImagesPath
        {
            get
            {
                return ConfigurationManager.AppSettings[CfgName_BaseImagesPath];
            }
        }


        public static string IISExpressPath
        {
            get
            {
                return ConfigurationManager.AppSettings[CfgName_IISExpressPath];
            }
        }


        public static string MockPackageServerPort
        {
            get
            {
                return ConfigurationManager.AppSettings[CfgName_MockPackageServerPort];
            }
        }


        public static string MockPackageServerPath
        {
            get
            {
                return ConfigurationManager.AppSettings[CfgName_MockPackageServerPath];
            }
        }


        public static string SetupDescriptionPath
        {
            get
            {
                return ConfigurationManager.AppSettings[CfgName_SetupDescriptionPath];
            }
        }


        public static string MsBuildPath
        {
            get
            {
                return FileUtil.MapIfRelative(MsBuildPathRaw);
            }
        }


        public static string NugetPath
        {
            get
            {
                return FileUtil.MapIfRelative(NugetPathRaw);
            }
        }



        private static string C1SitePathRaw
        {
            get
            {
                var val = ConfigurationManager.AppSettings[CfgName_C1SitePath];
                return val ?? "";
            }
        }


        private static string PackageProjectsPathsRaw
        {
            get
            {
                var val = ConfigurationManager.AppSettings[CfgName_PackageProjectsPaths];
                return val ?? "";
            }
        }


        private static string MsBuildPathRaw
        {
            get
            {
                return ConfigurationManager.AppSettings[CfgName_MsBuildPath];
            }
        }

        private static string NugetPathRaw
        {
            get
            {
                return ConfigurationManager.AppSettings[CfgName_NugetPath];
            }
        }

        public static Dictionary<string, string> GetValidationErrors()
        {
            var errors = new Dictionary<string, string>();

            try
            {

                if (string.IsNullOrEmpty(C1SitePathRaw))
                {
                    errors.Add(CfgName_C1SitePath, "Missing in config. Make it point to root directory in your C1 test site.");
                }

                if (string.IsNullOrEmpty(PackageProjectsPathsRaw))
                {
                    errors.Add(CfgName_PackageProjectsPaths, "Missing in config. Make it point to common root for your package projects.");
                }
                else
                {
                    foreach (var path in PackageProjectsPaths)
                    {
                        if (!Directory.Exists(path))
                        {
                            errors.Add(CfgName_PackageProjectsPaths, string.Format("Path '{0}' not found.", path));
                        }
                    }
                }

                if (PackageProjectsPaths.Any(f => f.StartsWith(C1SitePath)))
                {
                    errors.Add(CfgName_PackageProjectsPaths, string.Format("Package projects path cannot be sub directory of test website - that will just confuse me."));
                }

                if (string.IsNullOrEmpty(C1BaseImageZipUrl))
                {
                    errors.Add(CfgName_C1BaseImageZipUrl, "Missing URL");

                }
                else
                {
                    try
                    {
                        Uri parseTest = new Uri(C1BaseImageZipUrl);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(CfgName_C1BaseImageZipUrl, ex.Message);
                    }
                }

                if (!File.Exists(IISExpressPath))
                {
                    errors.Add(CfgName_IISExpressPath, "Path not found");
                }

                int tmpInt;
                if (!int.TryParse(MockPackageServerPort, out tmpInt))
                {
                    errors.Add(CfgName_MockPackageServerPort, "Not an int");
                }


                if (!Directory.Exists(FileUtil.MapIfRelative(MockPackageServerPath)))
                {
                    errors.Add(CfgName_MockPackageServerPath, "Cannot map to an existing folder");
                }

                if (!File.Exists(SetupDescriptionPath))
                {
                    errors.Add(CfgName_SetupDescriptionPath, "Path not found");

                }

                if (string.IsNullOrWhiteSpace(MsBuildPathRaw))
                {
                    errors.Add(CfgName_MsBuildPath, "Missing in config. This should point to MSBUILD.EXE");
                }
                else
                {
                    if (!File.Exists(MsBuildPath))
                    {
                        errors.Add(CfgName_MsBuildPath, "Path not found");
                    }
                }

                if (string.IsNullOrWhiteSpace(NugetPathRaw))
                {
                    errors.Add(CfgName_NugetPath, "Missing in config. This should point to NUGET.EXE");
                }
                else
                {
                    if (!File.Exists(NugetPath))
                    {
                        errors.Add(CfgName_NugetPath, "Path not found");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add("Exception", ex.Message);
            }

            return errors;
        }
    }
}
