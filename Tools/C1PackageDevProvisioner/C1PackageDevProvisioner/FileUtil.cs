using System;
using System.IO;

namespace C1PackageDevProvisioner
{
    public static class FileUtil
    {
        public static void EnsureDirectoryExist(string path)
        {
            path = MapIfRelative(path);

            string dirName = Path.GetDirectoryName(path);

            var parentDirectoryInfo = Directory.GetParent(dirName);

            if (!parentDirectoryInfo.Exists)
            {
                EnsureDirectoryExist(parentDirectoryInfo.FullName);
            }

            if(!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
        }


        public static string MapBaseImagePath(string relativePath)
        {
            var basePath = MapIfRelative(Configration.BaseImagesPath);
            return MapRelative(basePath, relativePath);
        }

        public static string MapIfRelative(string relativePath)
        {
            if (Path.IsPathRooted(relativePath)) return relativePath;
            return MapRelative(Directory.GetCurrentDirectory(), relativePath);
        }

        public static string MapRelative(string basePath, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Path empty", nameof(relativePath));
            if (!Path.IsPathRooted(basePath)) throw new ArgumentException("Path not rooted: " + basePath, nameof(basePath));
            if (Path.IsPathRooted(relativePath)) throw new ArgumentException("Path is rooted: " + relativePath, nameof(relativePath));


            relativePath = relativePath.Replace("/", "\\");
            relativePath = relativePath.Replace("~\\", "");

            
            return Path.GetFullPath(Path.Combine(basePath, relativePath));
        }


        const int BYTES_TO_READ = sizeof(Int64);

        public static bool FilesAreEqual(string first, string second)
        {
            if (!File.Exists(first) || !File.Exists(second))
            {
                return false;
            }

            var firstFileInfo = new FileInfo(first);
            var secondFileInfo = new FileInfo(second);

            return FilesAreEqual(firstFileInfo, secondFileInfo);
        }

        public static bool FilesAreEqual(FileInfo first, FileInfo second)
        {
            if (first.Length != second.Length)
                return false;

            int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ);
                    fs2.Read(two, 0, BYTES_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }
    }
}
