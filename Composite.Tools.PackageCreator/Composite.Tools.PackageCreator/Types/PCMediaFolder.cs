using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.PackageCreator.Types
{
    [PCCategory("MediaFolders")]
    internal class PCMediaFolder : SimplePackageCreatorItem, IPackageable
    {
        public PCMediaFolder(string name)
            : base(name)
        {
        }

        public override string ActionLabel
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.Label", this.CategoryName)); }
        }

        public override string ActionToolTip
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.ToolTip", this.CategoryName)); }
        }

        public override string GetLabel()
        {
            var mediaFolder = GetMediaFolder();
            return mediaFolder != null ? mediaFolder.Path : "<empty>";
        }

        public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
        {
            if (entityToken is DataEntityToken)
            {
                DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
                if (dataEntityToken.InterfaceType == typeof(IMediaFileFolder))
                {
                    var mediaFolder = dataEntityToken.Data as IMediaFileFolder;

                    yield return new PCMediaFolder(mediaFolder.Id.ToString());
                }
            }
        }

        private IMediaFileFolder GetMediaFolder()
        {
            Guid mediaFolderId = new Guid(Id);

            return DataFacade.GetData<IMediaFileFolder>().FirstOrDefault(m => m.Id == mediaFolderId);
        }

		public Func<IMediaFileData, string, bool> TestFile = (file, path) => (file.FolderPath == path || file.FolderPath.StartsWith(path + "/"));
		public Func<IMediaFolderData, string, bool> TestFolder = (file, path) => (file.Path == path || file.Path.StartsWith(path + "/"));

		

        public void Pack(PackageCreator creator)
        {
            var mediaFolder = GetMediaFolder();

            string path = mediaFolder.Path;

			Func<IMediaFileData, bool> fileFilter = file => TestFile(file, path) && !creator.ExludedPaths.Where(p => TestFile(file,p)).Any();
			Func<IMediaFolderData, bool> folderFilter = folder => TestFolder(folder, path) && !creator.ExludedPaths.Where(p => TestFolder(folder, p)).Any();

            creator.AddData(fileFilter);
            creator.AddData(folderFilter);

            var files = DataFacade.GetData<IMediaFileData>().Where(fileFilter).ToList();
            foreach (var mediaFileData in files)
            {
                creator.AddFileIfExists(@"App_Data\Media\" + mediaFileData.Id);
            }
        }
    }
}
