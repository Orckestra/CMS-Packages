using System;
using System.Security.Cryptography;
using System.Text;
using Composite.Data;
using Composite.Data.Types;
using Composite.Data.Plugins.DataProvider;
using Composite.Core.IO;

namespace Orckestra.Media.StaticFileLinkMediaProvider.Impl
{
    public class StaticFolder: IMediaFileFolder
    {
        private static readonly MD5 HashingAlgorithm = MD5.Create();

        public StaticFolder(string fullPath, DataProviderContext context, string storeId, string providerRoot)
        {
            string websitePath = PathUtil.GetWebsitePath(fullPath);
            string providerRelativePath = websitePath.Substring(providerRoot.Length - 1);

            Id = CalculateId(providerRelativePath);
            StoreId = storeId;
            DataSourceId = context.CreateDataSourceId(new MediaDataId { Id = this.Id, MediaType = MediaElementType.Folder }, typeof(IMediaFileFolder));

            Description = "";
            Title = System.IO.Path.GetFileName(providerRelativePath);
            StoreId = storeId;
            Path = providerRelativePath;

            KeyPath = storeId + ":" + Id;
            CompositePath = storeId + ":" + providerRelativePath;
        }


        private static Guid CalculateId(string websitePath)
        {
            return GetHashValue(websitePath);
        }

        private static Guid GetHashValue(string value)
        {
            var bytes = HashingAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(value));
            return new Guid(bytes);
        }

        public StaticFolder(string path, string storeId, Composite.Plugins.Data.DataProviders.MediaFileProvider.MediaFileProvider.MediaDataId dataId, string providerName)
        {
            string[] pathParts = path.Split('/');

        }

        public Guid Id
        {
            get;
            private set;
        }

        public string KeyPath
        {
            get;
            private set;
        }

        public string CompositePath
        {
            get;
            set;
        }

        public string StoreId
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public bool IsReadOnly
        {
            get { return true;  }
            set { }
        }

        public DataSourceId DataSourceId
        {
            get;
            private set;
        }
    }
}
