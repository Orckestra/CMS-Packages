using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace C1PackageDevProvisioner.C1Packages
{
    public static class PackageManager
    {
        public static IEnumerable<C1Package> GetPackages()
        {
            foreach (var projectPath in Configration.PackageProjectsPaths)
            {
                var installXmlPaths = Directory.GetFiles(projectPath, "install.xml", SearchOption.AllDirectories);

                foreach (string installXmlPath in installXmlPaths.Where(f => !f.Contains("\\bin\\")))
                {
                    if (!installXmlPath.StartsWith(Configration.C1SitePath))
                    {
                        yield return new C1Package(installXmlPath, projectPath);
                    }
                }
            }
        }
    }
}
