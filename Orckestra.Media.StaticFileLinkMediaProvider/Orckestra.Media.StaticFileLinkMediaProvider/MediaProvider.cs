using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder;
using Composite.Core.IO;
using Composite.Core.Routing;
using Composite.Data;
using Composite.Data.Plugins.DataProvider;
using Composite.Data.Types;
using Orckestra.Media.StaticFileLinkMediaProvider.Impl;

namespace Orckestra.Media.StaticFileLinkMediaProvider
{
    [ConfigurationElementType(typeof(MediaProviderData))]
    public class MediaProvider : IDataProvider, IMediaUrlProvider
    {
        internal MediaProvider(string storeId, string storeTitle, string storeDescription, string basePath)
        {
            StoreId = storeId;
            StoreDescription = storeDescription;
            StoreTitle = storeTitle;
            BasePath = basePath;

            _fileSystemWatcher = new FileSystemWatcher(PathUtil.Resolve(BasePath), "*.*")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
            };

            _fileSystemWatcher.Created += FileChange;
            _fileSystemWatcher.Changed += FileChange;
            _fileSystemWatcher.Deleted += FileChange;
            _fileSystemWatcher.Renamed += FileChange;
            _fileSystemWatcher.Error += FileChangeOverflow;

            MediaUrls.RegisterMediaUrlProvider(storeId, this);
        }

        public DataProviderContext Context { set; private get; }

        private FileSystemWatcher _fileSystemWatcher;
        private string StoreId { get; set; }
        private string StoreDescription { get; set; }
        private string StoreTitle { get; set; }
        private string BasePath { get; set; }
        private List<MediaFileStore> StoreList { get; set; }
        private List<StaticFile> FileList { get; set; }
        private List<StaticFolder> FolderList { get; set; }

        public string GetPublicMediaUrl(string storeId, Guid mediaId)
        {
            if (this.StoreId == storeId)
            {
                StaticFile file = GetFileList().Where(f => f.Id == mediaId).FirstOrDefault();
                if (file != null)
                {
                    return file.DownloadUrl;
                }
            }

            return null;
        }


        public IEnumerable<Type> GetSupportedInterfaces()
        {
            return new[] { typeof(IMediaFile), typeof(IMediaFileFolder), typeof(IMediaFileStore) };
        }

        IQueryable<T> IDataProvider.GetData<T>()
        {
            if (typeof(T) == typeof(IMediaFile))
            {
                return GetFileQueryable() as IQueryable<T>;
            }

            if (typeof(T) == typeof(IMediaFileFolder))
            {
                return GetFolderQueryable() as IQueryable<T>;
            }

            if (typeof(T) == typeof(IMediaFileStore))
            {
                return GetStoreQueryable() as IQueryable<T>;
            }

            throw new InvalidOperationException("Unexpected type parameter: " + typeof(T).FullName);
        }

        T IDataProvider.GetData<T>(IDataId dataId)
        {
            var mediaDataId = dataId as MediaDataId;
            switch (mediaDataId.MediaType)
            {
                case MediaElementType.File:
                    return GetFileList().Where(f => f.Id == mediaDataId.Id).FirstOrDefault() as T;
                case MediaElementType.Folder:
                    return GetFolderList().Where(f => f.Id == mediaDataId.Id).FirstOrDefault() as T;
                case MediaElementType.Store:
                    return GetStoreList().FirstOrDefault() as T;
                default:
                    throw new NotImplementedException();
            }
        }


        private IQueryable<IMediaFile> GetFileQueryable()
        {
            return GetFileList().AsQueryable();
        }

        private IQueryable<IMediaFileFolder> GetFolderQueryable()
        {
            return GetFolderList().AsQueryable();
        }

        private List<StaticFile> GetFileList()
        {
            List<StaticFile> fileList = this.FileList;

            if (fileList==null)
            {
                string rootFolder = PathUtil.Resolve(BasePath);
                string[] filesInStructure = Directory.GetFiles(rootFolder, "*.*", SearchOption.AllDirectories);
                fileList = filesInStructure.Select(f => new StaticFile(f, this.Context, this.StoreId, this.BasePath)).ToList();
                this.FileList = fileList;
            }

            return fileList;
        }

        private List<StaticFolder> GetFolderList()
        {
            List<StaticFolder> folderList = this.FolderList;

            if (folderList == null)
            {
                string rootFolder = PathUtil.Resolve(BasePath);
                string[] foldersInStructure = Directory.GetDirectories(rootFolder, "*", SearchOption.AllDirectories);
                folderList = foldersInStructure.Select(f => new StaticFolder(f, this.Context, this.StoreId, this.BasePath)).ToList();
                this.FolderList = folderList;
            }

            return folderList;
        }


        private List<MediaFileStore> GetStoreList()
        {
            if (StoreList == null)
            {
                var store = new MediaFileStore(StoreId, StoreTitle, StoreDescription,
                    Context.CreateDataSourceId(new MediaDataId { MediaType = MediaElementType.Store }, typeof(IMediaFileStore)));
                StoreList = new List<MediaFileStore> { store };
            }

            return StoreList;
        }

        private IQueryable<IMediaFileStore> GetStoreQueryable()
        {
            return GetStoreList().AsQueryable();
        }

        private void FileChange(object sender, FileSystemEventArgs e)
        {
            this.FileList = null;
            this.FolderList = null;
        }

        private void FileChangeOverflow(object sender, ErrorEventArgs e)
        {
            this.FileList = null;
            this.FolderList = null;
        }
    }



    [Assembler(typeof(MediaProviderAssembler))]
    public class MediaProviderData : DataProviderData
    {

        private const string _storeIdProperty = "storeId";
        [ConfigurationProperty(_storeIdProperty, IsRequired = true)]
        public string StoreId
        {
            get { return (string)base[_storeIdProperty]; }
            set { base[_storeIdProperty] = value; }
        }



        private const string _storeDescriptionProperty = "storeDescription";
        [ConfigurationProperty(_storeDescriptionProperty, IsRequired = true)]
        public string StoreDescription
        {
            get { return (string)base[_storeDescriptionProperty]; }
            set { base[_storeDescriptionProperty] = value; }
        }



        private const string _storeTitleProperty = "storeTitle";
        [ConfigurationProperty(_storeTitleProperty, IsRequired = true)]
        public string StoreTitle
        {
            get { return (string)base[_storeTitleProperty]; }
            set { base[_storeTitleProperty] = value; }
        }


        private const string _basePathProperty = "basePath";
        [ConfigurationProperty(_basePathProperty, IsRequired = true)]
        public string BasePath
        {
            get { return (string)base[_basePathProperty]; }
            set { base[_basePathProperty] = value; }
        }
    }


    internal class MediaProviderAssembler : IAssembler<IDataProvider, DataProviderData>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public IDataProvider Assemble(IBuilderContext context, DataProviderData objectConfiguration, IConfigurationSource configurationSource, ConfigurationReflectionCache reflectionCache)
        {
            var configuration = objectConfiguration as MediaProviderData;

            if (configuration == null) throw new ArgumentException("Expected configuration to be of type MediaDataProviderData", "objectConfiguration");

            return new MediaProvider(configuration.StoreId, configuration.StoreTitle, configuration.StoreDescription, configuration.BasePath);
        }

    }


}
