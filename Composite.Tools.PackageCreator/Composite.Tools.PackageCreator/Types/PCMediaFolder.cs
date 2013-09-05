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
            Guid mediaFolderId = new Guid(Name);

            return DataFacade.GetData<IMediaFileFolder>().FirstOrDefault(m => m.Id == mediaFolderId);
        }

        public void Pack(PackageCreator creator)
        {
            var mediaFolder = GetMediaFolder();

            string folderPath = mediaFolder.Path;
            string folderPrefix = mediaFolder.Path + "/";

            Func<IMediaFileData, bool> fileFilter = file => (file.FolderPath == folderPath) || file.FolderPath.StartsWith(folderPrefix);
            Func<IMediaFolderData, bool> folderFilter = file => (file.Path == folderPath) || file.Path.StartsWith(folderPrefix);

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
