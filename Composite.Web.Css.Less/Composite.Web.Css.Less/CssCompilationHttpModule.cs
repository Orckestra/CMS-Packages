using System;
using System.IO;
using System.Threading;
using System.Web;
using Composite.C1Console.Security;
using Composite.Core;
using Composite.Core.Collections.Generic;

namespace Orckestra.Web.Css.Less
{
    public class CssCompilationHttpModule : IHttpModule
    {
        private static readonly ReaderWriterLockSlim _compilationLock = new ReaderWriterLockSlim();

        private readonly string _extension;
        private readonly string _fileWatcherMask;
        private readonly Action<string, string, DateTime> _compileAction;

        private class CssCompilationErrorInformation
        {
            public DateTime LastModificationDateUtc;
            public bool IsCssCompilationException;
        }

        private static Hashtable<string, CssCompilationErrorInformation> _filesWithErrors = new Hashtable<string, CssCompilationErrorInformation>();

        public CssCompilationHttpModule(string extension, string fileMask, Action<string, string, DateTime> compileAction)
        {
            _extension = extension;
            _fileWatcherMask = fileMask;
            _compileAction = compileAction;
        }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += (a, b) => context_BeginRequest(application.Context);
        }

        private void context_BeginRequest(HttpContext context)
        {
            var requestPath = context.Request.Path;
            if (!requestPath.EndsWith(_extension))
            {
                return;
            }

            var filePath = context.Server.MapPath(requestPath);
            if (!File.Exists(filePath))
            {
                return;
            }

            DateTime lastTimeUpdatedUtc = StylesHashing.GetLastStyleUpdateTimeUtc(_fileWatcherMask);

            var filePathCss = filePath.Substring(0, filePath.Length - _extension.Length) + ".min.css";

            bool cssFileExists = File.Exists(filePathCss);

            if (!cssFileExists || File.GetLastWriteTimeUtc(filePathCss) < lastTimeUpdatedUtc)
            {
                string fileKey = filePathCss.ToLowerInvariant();

                var errorInfo = _filesWithErrors[fileKey];
                // Not recompiling a file if previous compilations failed
                if (cssFileExists && errorInfo != null
                    && errorInfo.LastModificationDateUtc == lastTimeUpdatedUtc
                    && !(errorInfo.IsCssCompilationException && UserValidationFacade.IsLoggedIn()))
                {
                    context.Response.ContentType = "text/css";
                    context.Response.WriteFile(filePathCss);
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }

                try
                {
                    _compilationLock.EnterWriteLock();

                    try
                    {
                        if (!File.Exists(filePathCss) || File.GetLastWriteTimeUtc(filePathCss) < lastTimeUpdatedUtc)
                        {
                            _compileAction(filePath, filePathCss, lastTimeUpdatedUtc);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (UserValidationFacade.IsLoggedIn() && ex is CssCompileException)
                        {
                            // Showing a friendly error message for logged in users
                            EmitCssMakrupForException(context.Response, requestPath, ex);
                            context.ApplicationInstance.CompleteRequest();
                            return;
                        }

                        if (!cssFileExists)
                        {
                            throw;
                        }

                        _filesWithErrors[fileKey] = new CssCompilationErrorInformation
                        {
                            IsCssCompilationException = ex is CssCompileException,
                            LastModificationDateUtc = lastTimeUpdatedUtc
                        };

                        Log.LogError(GetType().FullName, ex);

                        context.Response.ContentType = "text/css";
                        context.Response.WriteFile(filePathCss);
                        context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                }
                finally
                {
                    _compilationLock.ExitWriteLock();
                }
            }

            if ((DateTime.UtcNow - lastTimeUpdatedUtc).Days >= 1 || !UserValidationFacade.IsLoggedIn())
            {
                context.Response.Cache.SetExpires(DateTime.Now.AddDays(1.0));
            }
            else
            {
                context.Response.Cache.SetExpires(DateTime.Now.AddMinutes(15));
            }


            context.Response.ContentType = "text/css";

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

        private void EmitCssMakrupForException(HttpResponse response, string requestPath, Exception exception)
        {
            var output = string.Format(@"
/* 
CSS COMPILE ERROR:
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
}}", requestPath, EncodeCssContent(exception.Message), EncodeCssComment(exception.Message));

            response.Write(output);

        }

        private string EncodeCssContent(string text)
        {
            return text.Replace("'", "\"").Replace(@"\", @"\\").Replace("\n", @"\A ").Replace("\r", "");
        }

        private string EncodeCssComment(string text)
        {
            return text.Replace("*", "");
        }


        public void Dispose()
        {
        }
    }
}
