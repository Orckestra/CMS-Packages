using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Composite.Core;
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
        private const string LogTitle = nameof(MediaContentSearchExtension);

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

            string stdout, stderr;
            int exitCode;

            using (var process = new Process
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
            })
            {
                process.Start();

                stdout = process.StandardOutput.ReadToEnd();
                stderr = process.StandardError.ReadToEnd();

                process.WaitForExit();

                exitCode = process.ExitCode;
            }

            C1File.Delete(tempSourceFile);

            if (exitCode != 0)
            {
                var msg = $"Failed to parse the content of the media file '{Path.GetFileName(mediaFile.FileName)}'.";

                if ((uint) exitCode == 0x80004005 /*E_FAIL*/)
                {
                    Log.LogVerbose(LogTitle, msg + " Unspecified error.");
                    return;
                }

                if ((uint) exitCode == 0x80004002 /*E_NOINTERFACE*/)
                {
                    Log.LogWarning(LogTitle, msg + " IFilter not found for the given file extension.");
                    return;
                }

                Log.LogWarning(LogTitle,
                    msg +
                    $"\r\nExit Code: {exitCode}\r\nOutput: {stdout}" 
                    + (!string.IsNullOrEmpty(stderr) ? $"\r\nError: {stderr}" : ""));
                return;
            }

            if (!File.Exists(tempTargetFile)) return;
            
            var text = File.ReadAllText(tempTargetFile);

            C1File.Delete(tempTargetFile);

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

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
