using System;
using Composite.Data;

namespace Orckestra.Media.StaticFileLinkMediaProvider.Impl
{
    public enum MediaElementType
    {
        File = 1,
        Folder = 2,
        Store = 3
    }


    public sealed class MediaDataId : IDataId
    {
        public MediaDataId()
        {
        }

        public Guid Id { get; set; }
        public MediaElementType MediaType { get; set; }
    }
}