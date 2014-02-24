using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Hosting;
using Composite.C1Console.Events;
using Composite.Core.Configuration;

namespace Composite.Web.Css.Less
{
    /// <summary>
    /// Style hashing designed to preserve last style file change time.
    /// </summary>
    public static class StylesHashing
    {
        private static readonly ConcurrentDictionary<string, FileHashCalculator> 
            _hashCalculators = new ConcurrentDictionary<string, FileHashCalculator>();

        public static DateTime GetLastStyleUpdateTimeUtc(string mask)
        {
            var hashCalculator = GetCalculator(mask);

            return hashCalculator.LastStyleChangeTimeUtc;
        }

        private static FileHashCalculator GetCalculator(string mask)
        {
            return _hashCalculators.GetOrAdd(mask, m => new FileHashCalculator(m));
        }


        private class FileHashCalculator
        {
            private string _mask;
            private readonly string _hashFilePath;

            Guid? _hash;
            DateTime? _lastFileChangeTime;

            private readonly object _calculationLock = new object();

            private readonly FileSystemWatcher _fileSystemWatcher;

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


                _fileSystemWatcher = new FileSystemWatcher(GetRootFolder(), _mask);
                _fileSystemWatcher.IncludeSubdirectories = true;
                _fileSystemWatcher.Created += (a, b) => InvalidateCache();
                _fileSystemWatcher.Changed += (a, b) => InvalidateCache();
                _fileSystemWatcher.Deleted += (a, b) => InvalidateCache();
                _fileSystemWatcher.Renamed += (a, b) => InvalidateCache();
                _fileSystemWatcher.EnableRaisingEvents = true;
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
