using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Composite.Core.WebClient;

namespace Composite.Media.Silverlight
{
	public class Functions
	{
		private static readonly string[] RootFoldersToIgnore = new[] { "bin", "app_data", "app_code", "composite" };

		public static IEnumerable<string> ListOfSilverlightFiles()
		{
			string rootPath = UrlUtils.PublicRootPath;
			if (!rootPath.EndsWith("/"))
			{
				rootPath += "/";
			}

			var xadFiles = new List<string>();

			string rootFilePath = HostingEnvironment.MapPath(rootPath);

			FindFilesRec(rootFilePath, xadFiles, true);

			return xadFiles.Select(path => path.Substring(rootFilePath.Length - 1).Replace("\\", "/")).ToList();
		}

		private static void FindFilesRec(string path, List<string> xadFiles, bool isRootFolder)
		{
			xadFiles.AddRange(Directory.GetFiles(path, "*.xap"));

			foreach (string folder in Directory.GetDirectories(path))
			{
				if (isRootFolder)
				{
					string folderName = Path.GetFileName(folder).ToLower();
					if (RootFoldersToIgnore.Contains(folderName))
					{
						continue;
					}
				}

				FindFilesRec(folder, xadFiles, false);
			}
		}
	}
}
