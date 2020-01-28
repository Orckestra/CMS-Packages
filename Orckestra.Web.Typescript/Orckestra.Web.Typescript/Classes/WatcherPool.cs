using System.Collections.Generic;
using System.IO;

namespace Orckestra.Web.Typescript.Classes
{
    internal static class WatcherPool
    {
        private static readonly List<FileSystemWatcher> _fileSystemWatchers = new List<FileSystemWatcher>();
        internal static void Register(FileSystemWatcher fileSystemWatcher) => _fileSystemWatchers.Add(fileSystemWatcher);
    }
}
