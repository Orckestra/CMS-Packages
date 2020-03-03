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
        private readonly List<FileSystemWatcher> _fileSystemWatchers = new List<FileSystemWatcher>();

        public bool ConfigureService(string taskName, Action action, string fileMask, IEnumerable<string> pathsToWatch)
        {
            string warnMessage = ComposeExceptionInfo(nameof(ConfigureService), _taskName);

            _taskName = taskName;

            if (action is null)
            {
                RegisterException($"{warnMessage} Parameter \"{nameof(action)}\" cannot be null.", typeof(ArgumentNullException));
                return false;
            }
            _action = action;

            if (string.IsNullOrEmpty(fileMask))
            {
                RegisterException($"{warnMessage} Parameter \"{nameof(fileMask)}\" cannot be null or empty.", typeof(ArgumentNullException));
                return false;
            }
            _fileMask = fileMask;

            if (pathsToWatch is null || !pathsToWatch.Any())
            {
                RegisterException($"{warnMessage} Parameter \"{nameof(pathsToWatch)}\" is null or has no values.", typeof(ArgumentNullException));
                return false;
            }
            _pathsToWatch = pathsToWatch;

            return true;
        }

        public bool InvokeService()
        {
            string warnMessage = ComposeExceptionInfo(nameof(InvokeService), _taskName);

            List<string> absolutePaths = new List<string>();
            foreach (string el in _pathsToWatch)
            {
                string path = HostingEnvironment.MapPath(el); 
                if (string.IsNullOrEmpty(path))
                {
                    RegisterException($"{warnMessage} \"{nameof(path)}\" value cannot be null or empty.", typeof(ArgumentNullException));
                    return false;
                }
                else if (!Directory.Exists(path))
                {
                    RegisterException($"{warnMessage} Folder path \"{path}\" does not exist.", typeof(DirectoryNotFoundException));
                    return false;
                }
                absolutePaths.Add(path);
            }

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
            return true;
        }

        private void FileSystemWatcherEvent(object sender, FileSystemEventArgs e) => _action();
    }
}