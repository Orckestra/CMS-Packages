using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Composite.C1Console.Security;
using Composite.Core.Collections.Generic;

namespace Composite.Web.Css.Less
{
    public class LessHttpModule : IHttpModule
    {
        private static readonly ReaderWriterLockSlim _compilationLock = new ReaderWriterLockSlim();
        private static readonly Hashtable<string, CachedDirectoryInfo>  _directoryInfo = new Hashtable<string, CachedDirectoryInfo>();

        public void Init(HttpApplication application)
        {
            application.BeginRequest += (a, b) => context_BeginRequest(application.Context);
        }

        private void context_BeginRequest(HttpContext context)
        {
            var requestPath = context.Request.Path;
            if (!requestPath.Contains(".less"))
            {
                return;
            }

            var filePathLess = context.Server.MapPath(requestPath);
            if (!File.Exists(filePathLess))
            {
                return;
            }

            string directory = Path.GetDirectoryName(filePathLess);

            DateTime folderLastUpdatedUtc = GetCachedFolderLastUpdateDateUtc(directory, "*.less");

            var filePathCss = filePathLess.Substring(0, filePathLess.Length - ".less".Length) + ".min.css";
            if (!File.Exists(filePathCss) || File.GetLastWriteTimeUtc(filePathCss) < folderLastUpdatedUtc)
            {
                try
                {
                    _compilationLock.EnterWriteLock();

                    try
                    {
                        if (!File.Exists(filePathCss) || File.GetLastWriteTimeUtc(filePathCss) < folderLastUpdatedUtc)
                        {
                            CompressFiles.CompressLess(filePathLess, filePathCss, folderLastUpdatedUtc);
                        }
                    }
                    catch (CssCompileException ex)
                    {
                        if (!UserValidationFacade.IsLoggedIn())
                        {
                            throw;
                        }

                        context.Response.StatusCode = 500;
                        context.Response.Write(ex.Message);
                        context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                }
                finally
                {
                    _compilationLock.ExitWriteLock();
                }
            }

            if (!UserValidationFacade.IsLoggedIn())
            {
                context.Response.Cache.SetExpires(DateTime.Now.AddDays(1.0));
            }

            try
            {
                _compilationLock.EnterReadLock();

                context.Response.WriteFile(filePathCss);
            }
            finally
            {
                _compilationLock.ExitReadLock();
            }
            
            context.ApplicationInstance.CompleteRequest();
        }

        private DateTime GetCachedFolderLastUpdateDateUtc(string directory, string fileMask)
        {
            var cacheRecord = _directoryInfo[directory];

            if (cacheRecord == null)
            {
                lock (_directoryInfo)
                {
                    cacheRecord = _directoryInfo[directory];
                    if (cacheRecord == null)
                    {
                        cacheRecord = new CachedDirectoryInfo();

                        cacheRecord.FileSystemWatcher = new FileSystemWatcher(directory, fileMask);
                        cacheRecord.FileSystemWatcher.IncludeSubdirectories = true;
                        cacheRecord.FileSystemWatcher.Created += (a, b) => cacheRecord.LastTimeUpdatedUtc = null;
                        cacheRecord.FileSystemWatcher.Changed += (a, b) => cacheRecord.LastTimeUpdatedUtc = null;
                        cacheRecord.FileSystemWatcher.Deleted += (a, b) => cacheRecord.LastTimeUpdatedUtc = null;
                        cacheRecord.FileSystemWatcher.Renamed += (a, b) => cacheRecord.LastTimeUpdatedUtc = null;

                        cacheRecord.FileSystemWatcher.EnableRaisingEvents = true;

                        _directoryInfo[directory] = cacheRecord;
                    }
                }
            }

            var cachedResult = cacheRecord.LastTimeUpdatedUtc;

            if (cachedResult.HasValue) return cachedResult.Value;

            var files = Directory.GetFiles(directory, fileMask, SearchOption.AllDirectories);

            if (files.Length == 0) return DateTime.Now;

            DateTime result = File.GetLastWriteTimeUtc(files[0]);
            foreach (var file in files.Skip(1))
            {
                DateTime lastUpdatedUtc = File.GetLastWriteTimeUtc(file);
                if (lastUpdatedUtc > result)
                {
                    result = lastUpdatedUtc;
                }
            }

            cacheRecord.LastTimeUpdatedUtc = result;

            return result;
        }

        public void Dispose()
        {
        }

        private class CachedDirectoryInfo
        {
            public FileSystemWatcher FileSystemWatcher { get; set; }
            public DateTime? LastTimeUpdatedUtc { get; set; }
        }
    }
}