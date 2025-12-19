using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;
using Composite.Plugins.Elements.ElementProviders.MediaFileProviderElementProvider;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("MediaFolders")]
    internal class PCMediaFolder : BasePackItem, IPack, IPackToggle
    {
        private const string _storeId = "MediaArchive";

        public Func<IMediaFileData, string, bool> TestFile = (file, path) =>
            path == "/" || file.FolderPath == path || file.FolderPath.StartsWith($"{path}/");

        public Func<IMediaFolderData, string, bool> TestFolder = (file, path) =>
            path == "/" || file.Path == path || file.Path.StartsWith($"{path}/");

        public PCMediaFolder(string name) : base(name)
        {
        }

        public string Path
        {
            get
            {
                if (!Guid.TryParse(Name, out _))
                    return Name;

                var mediaFolder = GetMediaFolder();
                return mediaFolder?.Path;
            }
        }

        public override string ActionLabel => PackageCreatorFacade.GetLocalization($"{CategoryName}.Add.Label");

        public override string ActionToolTip => PackageCreatorFacade.GetLocalization($"{CategoryName}.Add.ToolTip");

        public void Pack(PackageCreator creator)
        {
            var path = Path;

            Func<IMediaFileData, bool> fileFilter = file =>
                TestFile(file, path) && !creator.ExcludedPaths.Any(p => TestFile(file, p));
            Func<IMediaFolderData, bool> folderFilter = folder =>
                TestFolder(folder, path) && !creator.ExcludedPaths.Any(p => TestFolder(folder, p));

            creator.AddData(fileFilter);
            creator.AddData(folderFilter);

            var files = DataFacade.GetData<IMediaFileData>().AsEnumerable().Where(fileFilter).ToList();
            foreach (var mediaFileData in files) creator.AddFileIfExists(@"App_Data\Media\" + mediaFileData.Id);
        }

        public ActionCheckedStatus CheckedStatus
        {
            get
            {
                var status = Tree.Media.GetState(Path);
                return status == TreeState.IncludedTree ? ActionCheckedStatus.Checked : ActionCheckedStatus.Unchecked;
            }
        }

        public bool Disabled
        {
            get
            {
                var status = Tree.Media.GetState(Name);
                switch (status)
                {
                    case TreeState.NotIncluded:
                    case TreeState.IncludedTree:
                        return false;
                }

                return true;
            }
        }

        public EntityToken GetEntityToken()
        {
            if (Name == "/") return GetRootEntityToken();

            var mediaFolder = GetMediaFolder();

            return mediaFolder?.GetDataEntityToken();
        }

        public override string GetLabel()
        {
            return Path ?? "<empty>";
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (entityToken is DataEntityToken dataEntityToken)
            {
                if (dataEntityToken.Data is IMediaFileFolder mediaFolder)
                    yield return new PCMediaFolder(mediaFolder.Path);
            }
            else if (entityToken is MediaRootFolderProviderEntityToken mediaRootFolderProviderEntityToken)
            {
                if (mediaRootFolderProviderEntityToken.Id == _storeId)
                    yield return new PCMediaFolder("/");
            }
        }

        private IMediaFileFolder GetMediaFolder()
        {
            return Guid.TryParse(Name, out var mediaFolderId)
                ? DataFacade.GetData<IMediaFileFolder>().FirstOrDefault(m => m.Id == mediaFolderId)
                : DataFacade.GetData<IMediaFileFolder>().FirstOrDefault(m => m.Path == Name);
        }

        public override void RemoveFromConfiguration(XElement config)
        {
            base.RemoveFromConfiguration(config);
            Tree.Media.ClearCache();
        }

        public static EntityToken GetRootEntityToken()
        {
            return new MediaRootFolderProviderEntityToken(_storeId);
        }
    }
}