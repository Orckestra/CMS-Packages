using Orckestra.Web.Typescript.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using static Orckestra.Web.Typescript.Classes.Helper;

namespace Orckestra.Web.Typescript.Classes.Services
{
    public class TypescriptWatcherService : TypescriptService, ITypescriptWatcherService
    {
        private string _taskName;
        private Action _action;
        private string _fileMask;
        private IEnumerable<string> _pathsToWatch;
        private List<FileSystemWatcher> _fileSystemWatchers = new List<FileSystemWatcher>();

        public void ConfigureService(string taskName, Action action, string fileMask, IEnumerable<string> pathsToWatch)
        {
            _configured = false;
            _invoked = false;

            string warnMessage = ComposeExceptionInfo(nameof(ConfigureService), _taskName);

            _taskName = taskName;

            if (action is null)
            {
                RegisterException($"{warnMessage} Param {nameof(action)} cannot be null.", typeof(ArgumentNullException));
                return;
            }
            _action = action;

            if (string.IsNullOrEmpty(fileMask))
            {
                RegisterException($"{warnMessage} Param {nameof(fileMask)} cannot be null or empty.", typeof(ArgumentNullException));
                return;
            }
            _fileMask = fileMask;

            if (pathsToWatch is null || !pathsToWatch.Any())
            {
                RegisterException($"{warnMessage} Param {nameof(pathsToWatch)} is null or has no values.", typeof(ArgumentNullException));
                return;
            }
            _pathsToWatch = pathsToWatch;

            _configured = true;
        }

        public void InvokeService()
        {
            _invoked = false;

            string warnMessage = ComposeExceptionInfo(nameof(InvokeService), _taskName);

            if (!_configured)
            {
                RegisterException($"{warnMessage} Service is not configured.", typeof(InvalidOperationException));
                return;
            }

            List<string> absolutePaths = new List<string>();
            foreach (string el in _pathsToWatch)
            {
                string path = HostingEnvironment.MapPath(el); 
                if (string.IsNullOrEmpty(path))
                {
                    RegisterException($"{warnMessage} {nameof(path)} value cannot be null or empty.", typeof(ArgumentNullException));
                    return;
                }
                else if (!Directory.Exists(path))
                {
                    RegisterException($"{warnMessage} Folder path {path} does not exist.", typeof(DirectoryNotFoundException));
                    return;
                }
                absolutePaths.Add(path);
            }

            //create filewatchers only if everything was okay as it disposable
            foreach (string el in absolutePaths)
            {
                FileSystemWatcher fw = new FileSystemWatcher(el, _fileMask)
                {
                    IncludeSubdirectories = true
                };
                fw.Created += FileSystemWatcherEvent;
                fw.Changed += FileSystemWatcherEvent;
                fw.Deleted += FileSystemWatcherEvent;
                fw.Renamed += FileSystemWatcherEvent;
                fw.EnableRaisingEvents = true;
                _fileSystemWatchers.Add(fw);
            }

            _invoked = true;
        }

        public void ResetInvokeState()
        {
            foreach(FileSystemWatcher el in _fileSystemWatchers)
            {
                el.Dispose();
            }
            _fileSystemWatchers = new List<FileSystemWatcher>();
            _invoked = false;
        }
        private void FileSystemWatcherEvent(object sender, FileSystemEventArgs e) => _action();
    }
}