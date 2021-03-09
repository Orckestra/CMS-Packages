using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        private const string BaseDir = "~/App_Data/Search/MediaContentIndexing";
        private const string IFilterExecutableRelativePath = BaseDir + "/filtdump.exe";
        private const string CacheDirectory = "~/App_Data/Composite/Cache/MediaContentIndexing";

        private const int MaxMissingFilesLogMessages = 100;
        private const int MaxInvalidStreamsLogMessages = 100;
        private static int _missingFilesLogged;
        private static int _invalidStreamsLogged;


        private const string LogTitle = nameof(MediaContentSearchExtension);

        private static readonly string[] ApplicationMimeTypesToIgnore = 
        {
            "application/octet-stream",
            "application/zip",
            "application/x-shockwave-flash"
        };

        private bool IsIndexableMimeType(string mimeType)
        {
            return mimeType.StartsWith("text/") 
                || (mimeType.StartsWith("application/")
                    && !ApplicationMimeTypesToIgnore.Contains(mimeType));
        }

        public void Populate(SearchDocumentBuilder searchDocumentBuilder, IData data)
        {
            if (!(data is IMediaFile mediaFile)) return;

            var text = GetTextToIndex(mediaFile);

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            searchDocumentBuilder.TextParts.Add(text);
            searchDocumentBuilder.Url = MediaUrls.BuildUrl(mediaFile, UrlKind.Internal);
        }

        private string GetTextToIndex(IMediaFile mediaFile)
        {
            var mimeType = MimeTypeInfo.GetCanonical(mediaFile.MimeType);
            if (!IsIndexableMimeType(mimeType))
            {
                return null;
            }

            // Checking if the parsing results are preserved in the cache
            var outputFilePath = GetTextOutputFilePath(mediaFile);
            var lastWriteTime = mediaFile.LastWriteTime ?? mediaFile.CreationTime;

            if (File.Exists(outputFilePath)
                && lastWriteTime != null
                && File.GetCreationTime(outputFilePath) == lastWriteTime)
            {
                string text = File.ReadAllText(outputFilePath);
                if (!string.IsNullOrEmpty(text))
                {
                    return text;
                }
            }

            string extension = GetExtension(mediaFile);
            if (string.IsNullOrEmpty(extension))
            {
                Log.LogWarning(LogTitle, $"Failed to extract extension from file: '{mediaFile.FileName}', mime type: '{mediaFile.MimeType}' ");
                return null;
            }

            // Saving the file to a temp directory
            var tempSourceFile = GetTempFileName(extension);

            try
            {
                using (var mediaStream = mediaFile.GetReadStream())
                using (var file = File.Create(tempSourceFile))
                {
                    mediaStream.CopyTo(file);
                }
            }
            catch (InvalidOperationException ex)
            {
                if (Interlocked.Increment(ref _invalidStreamsLogged) <= MaxInvalidStreamsLogMessages)
                {
                    Log.LogWarning(LogTitle, $"Failed to read the underlying stream for media file '{mediaFile.KeyPath}', error message: {ex.Message}");
                }
                return null;
            }
            catch (FileNotFoundException)
            {
                if (Interlocked.Increment(ref _missingFilesLogged) <= MaxMissingFilesLogMessages)
                {
                    Log.LogWarning(LogTitle, $"Missing an underlying content file for the media file '{mediaFile.KeyPath}'");
                }
                return null;
            }

            bool success = ExtractText(tempSourceFile, outputFilePath, mediaFile);

            C1File.Delete(tempSourceFile);

            if (!success)
            {
                return null;
            }

            if (lastWriteTime != null)
            {
                File.SetLastWriteTime(outputFilePath, lastWriteTime.Value);
            }

            var result = File.ReadAllText(outputFilePath);

            if (result.Length == 0)
            {
                Log.LogWarning(LogTitle, $"Failed to extract text from media file '{mediaFile.FileName}', mime type: '{mediaFile.MimeType}', extension '{extension}'");
            }

            return result;
        }


        static bool ExtractText(string sourceFile, string targetFile, IMediaFile mediaFile)
        {
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
                    Arguments = $"\"{sourceFile}\" \"{targetFile}\"",
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

            if (exitCode != 0)
            {
                var msg = $"Failed to parse the content of the media file '{Path.GetFileName(mediaFile.FileName)}'.";

                if ((uint)exitCode == 0x80004005 /*E_FAIL*/)
                {
                    msg += " Unspecified error.";
                }
                else if ((uint) exitCode == 0x80004002 /*E_NOINTERFACE*/)
                {
                    msg += " IFilter not found for the given file extension.";
                }
                else
                {
                    msg += $"\r\nExit Code: {exitCode}\r\nOutput: {stdout}"
                           + (!string.IsNullOrEmpty(stderr) ? $"\r\nError: {stderr}" : "");
                }

                Log.LogWarning(LogTitle, msg);
                return false;
            }

            return File.Exists(targetFile);
        }

        static string GetExtension(IMediaFile mediaFile)
        {
            string extension;
            try
            {
                var fileName = mediaFile.FileName;
                foreach (var ch in Path.GetInvalidFileNameChars())
                {
                    fileName = fileName.Replace(ch, '_');
                }
                extension = Path.GetExtension(fileName);
            }
            catch (ArgumentException)
            {
                return null;
            }

            if (string.IsNullOrEmpty(extension))
            {
                extension = MimeTypeInfo.GetExtensionFromMimeType(mediaFile.MimeType);
            }

            return extension;
        }

        internal static string GetTextOutputFilePath(IMediaFile mediaFile)
        {
            var cacheDir = PathUtil.Resolve(CacheDirectory);

            Directory.CreateDirectory(cacheDir);

            string storeKey = mediaFile.StoreId == "MediaArchive" ? "" : mediaFile.StoreId + "_";
            return $"{cacheDir}/{storeKey}{mediaFile.Id}.txt";
        }


        internal static string GetTempFileName(string extension)
        {
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            return Path.Combine(PathUtil.Resolve(GlobalSettingsFacade.TempDirectory),
                UrlUtils.CompressGuid(Guid.NewGuid()).Substring(0, 8)) + (extension ?? "");
        }
    }
}
