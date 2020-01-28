using Orckestra.Web.Typescript.Classes;
using Orckestra.Web.Typescript.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace Orckestra.Web.Typescript.Services
{
    public class TypescriptWatcherService : ITypescriptWatcherService
    {
        private Action _action;
        private string _fileMask;
        private IEnumerable<string> _pathsToWatch;

        public ITypescriptWatcherService ConfigureService(Action action, string fileMask, IEnumerable<string> pathsToWatch)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _fileMask = fileMask ?? throw new ArgumentNullException(nameof(fileMask));
            _pathsToWatch = pathsToWatch ?? throw new ArgumentNullException(nameof(pathsToWatch));
            return this;
        }

        public ITypescriptWatcherService InvokeService()
        {
            foreach (string el in _pathsToWatch)
            {
                string path = Helper.GetAbsoluteServerPath(el);
                if (!Directory.Exists(path))
                {
                    throw new FileNotFoundException($"Incorrect folder path {path}");
                }
                FileSystemWatcher fw = new FileSystemWatcher(path, _fileMask)
                {
                    IncludeSubdirectories = true
                };
                fw.Created += FileSystemWatcherEvent;
                fw.Changed += FileSystemWatcherEvent;
                fw.Deleted += FileSystemWatcherEvent;
                fw.Renamed += FileSystemWatcherEvent;
                fw.EnableRaisingEvents = true;
                WatcherPool.Register(fw);
            }
            return this;
        }

        private void FileSystemWatcherEvent(object sender, FileSystemEventArgs e) => _action();
    }
}
