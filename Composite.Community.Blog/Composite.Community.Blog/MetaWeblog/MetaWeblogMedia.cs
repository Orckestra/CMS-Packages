using System;
using System.IO;
using System.Linq;
using Composite.Core.Extensions;
using Composite.Core.IO;
using Composite.Core.WebClient;
using Composite.Data;
using Composite.Data.Types;
using Composite.Plugins.Elements.ElementProviders.MediaFileProviderElementProvider;

namespace Composite.Community.Blog.MetaWeblog
{
	internal class MetaWeblogMedia : MetaWeblogData
	{
		public MetaWeblogMedia(string username, string password)
			: base(username, password)
		{

		}

		public string NewMediaObject(FileData mediaObject)
		{
			WorkflowMediaFile mediaFile = new WorkflowMediaFile();
			mediaFile.FileName = mediaObject.name;
			
			mediaFile.Title = mediaObject.name; ;
			mediaFile.Culture = DataLocalizationFacade.DefaultLocalizationCulture.Name;
			mediaFile.Length = mediaObject.bits.Count();
			mediaFile.MimeType = MimeTypeInfo.GetCanonical(mediaObject.type);

			if (mediaFile.MimeType == MimeTypeInfo.Default)
			{
				mediaFile.MimeType = MimeTypeInfo.GetCanonicalFromExtension(Path.GetExtension(mediaFile.FileName));
			}

			using (Stream readStream = new MemoryStream(mediaObject.bits))
			{
				using (Stream writeStream = mediaFile.GetNewWriteStream())
				{
					readStream.CopyTo(writeStream);
				}
			}
			var folderPath = string.Format("/Blog/{0}/{1:yyyy-MM-dd}", Author.Name, DateTime.Now);
			mediaFile.FolderPath = ForceGetMediaFolderPath(folderPath);
			IMediaFile addedFile = DataFacade.AddNew<IMediaFile>(mediaFile);
			return BlogFacade.GetFullPath(MediaUrlHelper.GetUrl(addedFile));
		}

		private string ForceGetMediaFolderPath(string path)
		{
			if (path == "/")
				return path;
			if (!path.IsCorrectFolderName('/'))
				throw new ArgumentException("Invalid folder name");
			var folder = (from item in DataFacade.GetData<IMediaFileFolder>()
						  where item.Path == path
						  select item).FirstOrDefault();
			if (folder == null)
			{
				ForceGetMediaFolderPath(GetParentFolderPath(path));
				folder = DataFacade.BuildNew<IMediaFileFolder>();
				folder.Path = path;
				folder = DataFacade.AddNew<IMediaFileFolder>(folder);
			}
			return folder.Path;
		}

		private string GetParentFolderPath(string path)
		{
			if (path == "/")
			{
				return path;
			}

			string parentPath = path.Substring(0, path.LastIndexOf("/"));
			if (parentPath == "")
			{
				return "/";
			}

			return parentPath;
		}

	}


}