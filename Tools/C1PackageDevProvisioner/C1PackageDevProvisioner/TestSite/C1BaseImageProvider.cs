using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace C1PackageDevProvisioner.TestSite
{
    public static class C1BaseImageProvider
    {
        private static void EnsureDirectories()
        {

        }
        public static void InitializeBaseImage()
        {
            Uri zipUri = new Uri(Configration.C1BaseImageZipUrl);
            string zipFileName = Path.GetFileName(zipUri.LocalPath);
            string zipFilePath =  FileUtil.MapBaseImagePath(Path.Combine("Zipped", zipFileName));

            if (!File.Exists(zipFilePath))
            {
                using (var client = new WebClient())
                {
                    FileUtil.EnsureDirectoryExist(zipFilePath);
                    client.DownloadFile(Configration.C1BaseImageZipUrl, zipFilePath);
                }
                string unzippedPath = GetBaseImagePath();

                ZipFile.ExtractToDirectory(zipFilePath, unzippedPath);
            }


        }

        public static string GetBaseImagePath()
        {
            Uri zipUri = new Uri(Configration.C1BaseImageZipUrl);
            string zipFileName = Path.GetFileName(zipUri.LocalPath);

            return FileUtil.MapBaseImagePath(Path.GetFileNameWithoutExtension(zipFileName));
        }
    }
}
