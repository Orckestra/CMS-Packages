using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Composite.C1Console.Security;
using Composite.Core.Collections.Generic;

namespace Composite.Web.Js.TypeScript
{
    public class TypeScriptHttpModule : IHttpModule
    {
        private static readonly ReaderWriterLockSlim _compilationLock = new ReaderWriterLockSlim();
        private static readonly Hashtable<string, CachedDirectoryInfo> _directoryInfo = new Hashtable<string, CachedDirectoryInfo>();

        private readonly string _fileWatcherMask;

        public TypeScriptHttpModule()
        {
            _fileWatcherMask = "*.ts";
        }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += (a, b) => context_BeginRequest(application.Context);
        }

        // TODO: refactor and move common code from projects Composite.Web.Js.TypeScript Composite.Web.Css.Sass Composite.Web.Css.Less to separate project
        private void context_BeginRequest(HttpContext context)
        {
            // TODO: don't read the config on each request
            var typeScriptConfig = TypeScriptConfig.Get();
            var requestPath = context.Request.Path;
            var bundleLocation = typeScriptConfig.BundleLocation;
            var bundleMapLocation = typeScriptConfig.BundleLocation + ".map";

            if (!requestPath.EndsWith(bundleLocation) || requestPath.EndsWith(bundleMapLocation))
            {
                return;
            }

            var filePath = context.Server.MapPath(requestPath);
            string directory = Path.GetDirectoryName(filePath);
            DateTime folderLastUpdatedUtc = GetCachedFolderLastUpdateDateUtc(directory, _fileWatcherMask);

            if (!File.Exists(filePath) || File.GetLastWriteTimeUtc(filePath) < folderLastUpdatedUtc)
            {
                try
                {
                    _compilationLock.EnterWriteLock();

                    try
                    {
                        if (!File.Exists(filePath) || File.GetLastWriteTimeUtc(filePath) < folderLastUpdatedUtc)
                        {
                            var baseDir = HttpContext.Current.Server.MapPath("~");
                            TypeScriptCompiler.Compile(TypeScriptConfig.Get(), baseDir);
                        }
                    }
                    catch (TypeScriptCompileException ex)
                    {
                        if (!UserValidationFacade.IsLoggedIn())
                        {
                            throw;
                        }

                        // Showing a friendly error message for logged in users
                        context.Response.Write(string.Format(@"
/* 
TYPESCRIPT COMPILE ERROR:
{2}
*/
body:before {{
   position: fixed;
   top: 10px;
   display: block;
   width: 60%;
   margin-left: 19%;
   padding: 20px;
   border: 2px solid red;
   font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif;
   font-size: 12px;
   font-weight: bold;
   color: InfoText;
   white-space:pre;
   background-color: InfoBackground;
   content: 'Error in {0}:\A\A {1}';
   z-index: 10000;
}}", requestPath, EncodeContent(ex.Message), EncodeComment(ex.Message + ex.Details)));
                        context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                }
                finally
                {
                    _compilationLock.ExitWriteLock();
                }
            }

            if ((DateTime.UtcNow - folderLastUpdatedUtc).Days >= 1 || !UserValidationFacade.IsLoggedIn())
            {
                context.Response.Cache.SetExpires(DateTime.Now.AddDays(1.0));
            }

            context.Response.ContentType = "text/javascript";

            try
            {
                _compilationLock.EnterReadLock();

                context.Response.WriteFile(filePath);
            }
            finally
            {
                _compilationLock.ExitReadLock();
            }

            context.ApplicationInstance.CompleteRequest();
        }

        private string EncodeContent(string text)
        {
            return text.Replace("'", "\"").Replace(@"\", @"\\").Replace("\n", @"\A ");
        }

        private string EncodeComment(string text)
        {
            return text.Replace("*", "");
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
    };

}
