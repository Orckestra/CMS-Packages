using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Composite.Core.IO;
using Composite.Data;
using Composite.Data.Plugins.DataProvider;
using Composite.Data.Streams;
using Composite.Data.Types;

namespace Orckestra.Media.StaticFileLinkMediaProvider.Impl
{
    [FileStreamManager(typeof(StaticFileFileStreamManager))]
    public class StaticFile : IMediaFile
    {
        private static readonly MD5 HashingAlgorithm = MD5.Create();

        public StaticFile( string fullPath, DataProviderContext context, string storeId, string providerRoot)
        {
            string websitePath = PathUtil.GetWebsitePath(fullPath);
            string providerRelativePath = websitePath.Substring(providerRoot.Length - 1);

            FullPath = fullPath;
            Id = CalculateId(providerRelativePath);
            DownloadUrl = websitePath;
            FileName = Path.GetFileName(providerRelativePath);
            FolderPath = Path.GetDirectoryName(providerRelativePath).Replace("\\","/");
            StoreId = storeId;
            DataSourceId = context.CreateDataSourceId( new MediaDataId { Id = this.Id, MediaType = MediaElementType.File }, typeof(IMediaFile) );

            var fileInfo = new FileInfo(FullPath);
            CreationTime = fileInfo.CreationTimeUtc;
            LastWriteTime = fileInfo.LastWriteTimeUtc;
            Length = (int) fileInfo.Length;
        }

        internal string FullPath { get; }

        public Guid Id { get; internal set; }

        public string DownloadUrl { get; internal set; }

        public string KeyPath
        {
            get { return this.GetKeyPath(); }
        }

        public string CompositePath
        {
            get { return this.GetCompositePath(); }
            set { throw new NotImplementedException(); }
        }

        public string StoreId
        {
            get;
            set;
        }

        public string Title
        {
            get
            {
                return this.FileName;
            }
            set
            {
                this.FileName = value;
            }
        }

        public string Description
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public string Culture
        {
            get
            {
                return CultureInfo.InvariantCulture.Name;
            }
            set
            {
            }
        }

        public string MimeType
        {
            get { return MimeTypeInfo.GetCanonicalFromExtension(Path.GetExtension(this.FileName)); }
        }

        public int? Length { get; set; }

        public DateTime? CreationTime { get; set; }

        public DateTime? LastWriteTime { get; set; }

        public bool IsReadOnly
        {
            get { return true; }
            set { }
        }

        public string FolderPath
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public DataSourceId DataSourceId
        {
            get;
            private set;
        }

        public string Tags
        {
            get
            {
                return "";
            }
            set
            {
            }
        }


        internal static Guid CalculateId(string websitePath)
        {
            return GetHashValue(websitePath);
        }

        private static Guid GetHashValue(string value)
        {
            var bytes = HashingAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(value));
            return new Guid(bytes);
        }
    }
}