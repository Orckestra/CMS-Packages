using System.IO;

namespace Utility.IO
{
    /// <summary>
    ///     Filesystem
    /// </summary>
    public class FileSystem
    {
        // Copy directory structure recursively
        public static void CopyDirectory(string Src, string Dst)
        {
            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
                Dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);

            var files = Directory.GetFileSystemEntries(Src);
            foreach (var element in files)
                // Sub directories
                if (Directory.Exists(element))
                {
                    CopyDirectory(element, Dst + Path.GetFileName(element));
                }
                // Files in directory
                else
                {
                    File.Copy(element, Dst + Path.GetFileName(element), true);
                    SetFileReadAccess(Dst + Path.GetFileName(element), false);
                }
        }

        // Sets the read-only value of a file.
        public static void SetFileReadAccess(string fileName, bool setReadOnly)
        {
            // Create a new FileInfo object.
            _ = new FileInfo(fileName)
            {
                // Set the IsReadOnly property.
                IsReadOnly = setReadOnly
            };
        }
    }
}