using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Hosting;
using Composite.C1Console.Events;
using Composite.Core;
using Composite.Core.Configuration;

namespace Composite.Web.Css.Less
{
    /// <summary>
    /// Style hashing designed to preserve last style file change time.
    /// </summary>
    public static class StylesHashing
    {
        private static readonly string LogTitle = "LessCss";

        private static readonly ConcurrentDictionary<string, Lazy<FileHashCalculator>>
            _hashCalculators = new ConcurrentDictionary<string, Lazy<FileHashCalculator>>();

        public static DateTime GetLastStyleUpdateTimeUtc(string mask)
        {
            var hashCalculator = GetCalculator(mask);

            return hashCalculator.LastStyleChangeTimeUtc;
        }

        private static FileHashCalculator GetCalculator(string mask)
        {
            return _hashCalculators.GetOrAdd(mask, m => new Lazy<FileHashCalculator>(() => new FileHashCalculator(m))).Value;
        }


        private class FileHashCalculator
        {
            private readonly string _mask;
            private readonly string _hashFilePath;

            Guid? _hash;
            DateTime? _lastFileChangeTime;

            private readonly object _calculationLock = new object();
            private readonly string _rootFolder;

            private readonly List<FileSystemWatcher> _fileSystemWatchers = new List<FileSystemWatcher>();

            public DateTime LastStyleChangeTimeUtc
            {
                get
                {
                    var result = _lastFileChangeTime;

                    if (result != null) return result.Value;

                    lock (_calculationLock)
                    {
                        if (_lastFileChangeTime != null) return _lastFileChangeTime.Value;

                        _hash = CalculateStyleFilesHash();
                        SaveHashToCache(_hash.Value);

                        return (_lastFileChangeTime = DateTime.UtcNow).Value;
                    }
                }
            }

            public FileHashCalculator(string mask)
            {
                _mask = mask;

                _hashFilePath = HostingEnvironment.MapPath(GlobalSettingsFacade.CacheDirectory) 
                        + "\\" + typeof(CssCompilationHttpModule).Name + mask.Replace("*", "") + ".hash";

                _hash = CalculateStyleFilesHash();

                Guid? hashFromCache = GetHashFromCache();

                if (hashFromCache == null)
                {
                    SaveHashToCache(_hash.Value);
                    _lastFileChangeTime = DateTime.UtcNow;
                }
                else
                {
                    if (hashFromCache.Value == _hash.Value)
                    {
                        _lastFileChangeTime = File.GetLastWriteTimeUtc(_hashFilePath);
                    }
                    else
                    {
                        SaveHashToCache(_hash.Value);

                        _lastFileChangeTime = DateTime.UtcNow;
                    }
                }


                _rootFolder = GetRootFolder();

                AddFileWatcher(_rootFolder);

                AddWatchesForSymbolicallyLinkedSubfolders(_rootFolder);

            }

            private void AddWatchesForSymbolicallyLinkedSubfolders(string folder)
            {
                if (folder.Contains(":\\") && folder.Length >= 248)
                {
                    return;
                }

                if (!ReparsePointUtils.DirectoryIsReparsePoint(folder))
                {
                    foreach (var subfolder in Directory.GetDirectories(folder))
                    {
                        AddWatchesForSymbolicallyLinkedSubfolders(subfolder);
                    }
                    return;
                }
                
                string target;

                try
                {
                    target = ReparsePointUtils.GetDirectoryReparsePointTarget(folder);
                }
                catch (Exception ex)
                {
                    Log.LogError(LogTitle, "Failed to get symbolic link or junction target");
                    Log.LogError(LogTitle, ex);
                    return;
                }

                if (target.StartsWith(_rootFolder + @"\", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                Log.LogInformation(LogTitle, "Adding a file system watcher for linked directory {0}", target);

                try
                {
                    AddFileWatcher(target);
                }
                catch (Exception ex)
                {
                    Log.LogError(LogTitle, ex);
                }
            }

            private void AddFileWatcher(string directory)
            {
                var fileSystemWatcher = new FileSystemWatcher(directory, _mask) { IncludeSubdirectories = true };
                fileSystemWatcher.Created += (a, b) => InvalidateCache();
                fileSystemWatcher.Changed += (a, b) => InvalidateCache();
                fileSystemWatcher.Deleted += (a, b) => InvalidateCache();
                fileSystemWatcher.Renamed += (a, b) => InvalidateCache();
                fileSystemWatcher.EnableRaisingEvents = true;

                _fileSystemWatchers.Add(fileSystemWatcher);
            }

            private void InvalidateCache()
            {
                lock (_calculationLock)
                {
                    _hash = null;
                    _lastFileChangeTime = null;
                }

                GlobalEventSystemFacade.FireDesignChangeEvent();
            }

            private Guid? GetHashFromCache()
            {
                if (!File.Exists(_hashFilePath))
                {
                    return null;
                }

                return Guid.Parse(File.ReadAllText(_hashFilePath));
            }

            private void SaveHashToCache(Guid value)
            {
                File.WriteAllText(_hashFilePath, value.ToString());
            }

            private Guid CalculateStyleFilesHash()
            {
                var files = Directory.GetFiles(GetRootFolder(), _mask, SearchOption.AllDirectories);

                var sb = new StringBuilder();

                foreach (var file in files.OrderBy(File.GetLastWriteTimeUtc))
                {
                    sb.Append(Path.GetFileName(file)).Append(File.GetLastWriteTimeUtc(file));
                }
                
                using (MD5 md5Hash = MD5.Create())
                {
                    return new Guid(md5Hash.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString())));
                }
            }

            private string GetRootFolder()
            {
                return HostingEnvironment.MapPath("~/");
            }
        }

    }
}
