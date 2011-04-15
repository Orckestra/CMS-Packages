using System;
using System.IO;

namespace Utility.IO
{
	/// <summary>
	/// Filesystem
	/// </summary>
	public class FileSystem
	{
		// Copy directory structure recursively
		public static void copyDirectory(string Src, string Dst)
		{
			String[] Files;

			if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
				Dst += Path.DirectorySeparatorChar;
			if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);
			Files = Directory.GetFileSystemEntries(Src);
			foreach (string Element in Files)
			{
				// Sub directories
				if (Directory.Exists(Element))
				{
					copyDirectory(Element, Dst + Path.GetFileName(Element));
				}
				// Files in directory
				else
				{
					File.Copy(Element, Dst + Path.GetFileName(Element), true);
					SetFileReadAccess(Dst + Path.GetFileName(Element), false);
				}
			}
		}

		// Sets the read-only value of a file.
		public static void SetFileReadAccess(string FileName, bool SetReadOnly)
		{
			// Create a new FileInfo object.
			FileInfo fInfo = new FileInfo(FileName);

			// Set the IsReadOnly property.
			fInfo.IsReadOnly = SetReadOnly;

		}

	}

}
