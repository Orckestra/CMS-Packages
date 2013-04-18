using System;
using System.Collections.Generic;
using System.Linq;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.AppFeed.Server
{
	internal enum ImageType
	{
		Large = 0,
		GroupView = 1
	}

	internal static class ImageUtilities
	{
		private static readonly Random _random = new Random();
		private static readonly List<string> _genericBackgrounds = new List<string>();

		static ImageUtilities()
		{
			_genericBackgrounds.AddRange(GetGenericBackgroundPaths());
			Settings.OnConfigChanged += ConfigChanged;
		}

		// fired on config file changes
		private static void ConfigChanged()
		{
			_genericBackgrounds.Clear();
			_genericBackgrounds.AddRange(GetGenericBackgroundPaths());
		}


		public static string GetImagePathFromId(string imageId, int width, int height, string action)
		{
			return String.Format("/Renderers/ShowMedia.ashx?id={0}&w={1}&h={2}&action={3}", imageId, width, height, action);
		}



		public static string RandomGenericImage(ImageType imageType)
		{
			if (_genericBackgrounds.Count == 0)
			{
				return "";
			}

			switch (imageType)
			{
				case ImageType.Large:
					throw new NotImplementedException();

				case ImageType.GroupView:

					return _genericBackgrounds[NextNumber(_genericBackgrounds.Count, 0)];
				default:
					break;
			}

			return "";
		}



		private static int NextNumber(int numberSpan, int firstNumber)
		{
			return _random.Next(numberSpan) + firstNumber;
		}


		private static IEnumerable<string> GetGenericBackgroundPaths()
		{
			using (var connection = new DataConnection())
			{
				string folderPath = connection.Get<IMediaFileFolder>().Where(f => f.Id == Settings.GenericGroupViewImageFolderId).Select(f => f.Path).FirstOrDefault();
				var paths = connection.Get<IImageFile>().Where(f => f.FolderPath == folderPath).Select(f => f.CompositePath);

				foreach (string imageId in paths)
				{
					yield return GetImagePathFromId(imageId, Settings.GroupViewImageWidth, Settings.GroupViewImageHeight, "fill");
				}
			}
		}
	}
}