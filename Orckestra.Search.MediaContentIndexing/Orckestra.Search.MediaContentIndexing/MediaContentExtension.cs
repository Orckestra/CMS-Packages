using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Composite.Core.Configuration;
using Composite.Core.IO;
using Composite.Core.Routing;
using Composite.Core.WebClient;
using Composite.Data;
using Composite.Data.Types;
using Composite.Search.Crawling;

namespace Orckestra.Search.MediaContentIndexing
{
    class MediaContentSearchExtension : ISearchDocumentBuilderExtension
    {
        private string IFilterExecutableRelativePath = "~/App_Data/Search/MediaContentIndexing/filtdump.exe";

        public void Populate(SearchDocumentBuilder searchDocumentBuilder, IData data)
        {
            if (!(data is IMediaFile mediaFile)) return;

            var extension = Path.GetExtension(mediaFile.FileName);
            if(string.IsNullOrEmpty(extension)) return;


            var mimeType = MimeTypeInfo.GetCanonical(mediaFile.MimeType);
            if (!mimeType.StartsWith("application/") && !mimeType.StartsWith("text/"))
            {
                return;
            }

            // Saving the file to a temp directory (to be optimized)
            //var tempFile1 = TempDirectoryFacade.Get
            var tempSourceFile = GetTempFileName(extension);
            var tempTargetFile = GetTempFileName(null);

            using (var mediaStream = mediaFile.GetReadStream())
            using (var file = File.Create(tempSourceFile))
            {
                mediaStream.CopyTo(file);
            }

            var exePath = PathUtil.Resolve(IFilterExecutableRelativePath);
            var workingDirectory = Path.GetDirectoryName(exePath);

            var process = new Process
            {
                StartInfo =
                    {
                        WorkingDirectory = workingDirectory,
                        FileName = "\"" + exePath + "\"",
                        Arguments = $"\"{tempSourceFile}\" \"{tempTargetFile}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
            };

            process.Start();
            process.WaitForExit();

            int exitCode = process.ExitCode;
            if (exitCode != 0)
            {
                // TODO: log exit code and output
                return;
            }

            C1File.Delete(tempSourceFile);

            if (!File.Exists(tempTargetFile)) return;
            
            var text = File.ReadAllText(tempTargetFile);

            C1File.Delete(tempTargetFile);

            searchDocumentBuilder.TextParts.Add(text);
            searchDocumentBuilder.Url = MediaUrls.BuildUrl(mediaFile);
        }

        internal static string GetTempFileName(string extension)
        {
            return Path.Combine(PathUtil.Resolve(GlobalSettingsFacade.TempDirectory),
                UrlUtils.CompressGuid(Guid.NewGuid()).Substring(0, 8)) + (extension ?? "");
        }
    }
}
