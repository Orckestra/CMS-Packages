using System;
using System.IO;
using Composite.Data.Streams;
using Composite.Data.Types;

namespace Orckestra.Media.StaticFileLinkMediaProvider.Impl
{
    class StaticFileFileStreamManager : IFileStreamManager
    {
        public Stream GetReadStream(IFile file)
        {
            if (file is StaticFile staticFile)
            {
                return File.OpenRead(staticFile.FullPath);
            }

            return null;
        }

        public Stream GetNewWriteStream(IFile file)
        {
            throw new NotSupportedException();
        }

        public void SubscribeOnFileChanged(IFile file, OnFileChangedDelegate handler)
        {
        }
    }
}
