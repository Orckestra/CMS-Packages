using C1PackageDevProvisioner.C1Packages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace C1PackageDevProvisioner.TestSite
{
    public class FileChangeSyncManager : IDisposable
    {
        List<FileSystemWatcher> _projectFileSystemWatchers = new List<FileSystemWatcher>();
        List<C1Package> _packagesToWatch = null;

        public void StartSync(bool syncFromProjectToC1)
        {
            _packagesToWatch = PackageManager.GetPackages().ToList();

            if (syncFromProjectToC1)
            {
                if (_projectFileSystemWatchers.Any()) throw new InvalidOperationException("I'm already watching over things - silly dev!");

                foreach (var packageProjectPath in Configration.PackageProjectsPaths)
                {
                    FileSystemWatcher watcher = new FileSystemWatcher(Configration.PackageProjectsPaths.First());
                    watcher.InternalBufferSize = 2621440;
                    watcher.IncludeSubdirectories = true;
                    watcher.Created += ProjectFileWatcher_Created;
                    watcher.Changed += ProjectFileWatcher_Changed;
                    watcher.Renamed += ProjectFileWatcher_Changed;
                    watcher.Error += ProjectFileWatcher_Error;
                    watcher.EnableRaisingEvents = true;

                    _projectFileSystemWatchers.Add(watcher);
                }
            }
        }

        private void ProjectFileWatcher_Error(object sender, ErrorEventArgs e)
        {
            EventInfo.Queue.Enqueue("FileSystemWatcher error: " + e.GetException().Message);
        }

        private void ProjectFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var changedFilePath = e.FullPath;

            if (changedFilePath.EndsWith("install.xml", StringComparison.InvariantCultureIgnoreCase))
            {
                RefreshPackageFileMappingsFromPath(changedFilePath);
            }

            SyncFile(changedFilePath);
        }

        private void ProjectFileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            var newFilePath = e.FullPath;

            if (newFilePath.EndsWith("~") || newFilePath.EndsWith("tmp", StringComparison.InvariantCultureIgnoreCase)) return;

            RefreshPackageFileMappingsFromPath(newFilePath);

            SyncFile(newFilePath);
        }

        private IEnumerable<string> GetFileMappings(C1Package package, string filePath)
        {
            var matches = package.FileMappings.Where(f => f.Key == filePath).Select(f => f.Value);

            if (Path.GetExtension(filePath).ToLowerInvariant() == ".dll" && !matches.Any())
            {
                matches = package.FileMappings.Where(f => Path.GetFileName(f.Key) == Path.GetFileName(filePath)).Select(f => f.Value);
            }
            return matches;
        }

        private object _fileWriteLock = new object();

        private void SyncFile(string projectFilePath)
        {
            foreach (var affectedPackage in GetAffectedInstalledPackages(projectFilePath))
            {
                foreach (var mapping in GetFileMappings(affectedPackage, projectFilePath))
                {
                    lock (_fileWriteLock)
                    {
                        if (!FileUtil.FilesAreEqual(projectFilePath, mapping))
                        {
                            try
                            {
                                if (File.Exists(projectFilePath))
                                {
                                    File.Copy(projectFilePath, mapping, true);
                                    EventInfo.Queue.Enqueue(string.Format("{0}: {1} copied to C1", affectedPackage.Name, Path.GetFileName(projectFilePath)));
                                }
                            }
                            catch (IOException ex)
                            {
                                EventInfo.Queue.Enqueue(string.Format("{0}: {1} NOT copied - {2}", affectedPackage.Name, Path.GetFileName(projectFilePath), ex.Message));
                            }
                        }
                    }
                }
            }
        }

        private void RefreshPackageFileMappingsFromPath(string projectFilePath)
        {
            var affectedPackages = GetAffectedInstalledPackages(projectFilePath);
            foreach (var affectedPackage in affectedPackages)
            {
                affectedPackage.RefreshFileMappings();
            }
        }

        private IEnumerable<C1Package> GetAffectedInstalledPackages(string changedProjectFilePath)
        {
            return _packagesToWatch.Where(p => changedProjectFilePath.StartsWith(p.ProjectFilesBasePath) && p.IsInstalled);
        }

        public void Dispose()
        {
            if (_projectFileSystemWatchers.Any())
            {
                foreach (var watcher in _projectFileSystemWatchers)
                {
                    watcher.EnableRaisingEvents = false;
                }
            }
        }

    }
}
