using Composite.Data;
using Composite.Data.Types;

namespace Orckestra.Media.StaticFileLinkMediaProvider.Impl
{
    internal sealed class MediaFileStore : IMediaFileStore
    {
        public MediaFileStore(string id, string title, string description, DataSourceId dataSourceId)
        {
            Id = id;
            Title = title;
            Description = description;
            DataSourceId = dataSourceId;
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsReadOnly { get { return true; } }

        public bool ProvidesMetadata
        {
            get { return true; }
        }

        public DataSourceId DataSourceId
        {
            get;
            private set;
        }
    }
}
