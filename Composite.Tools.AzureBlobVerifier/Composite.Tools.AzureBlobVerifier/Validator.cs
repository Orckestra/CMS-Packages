using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Composite.AzureConnector;
using Composite.AzureConnector.IOProvider;
using Composite.Core;
using Composite.Core.Configuration;
using Composite.Core.IO;
using Composite.Core.IO.Plugins.IOProvider.Runtime;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.WindowsAzure.StorageClient;


namespace Composite.Tools.AzureBlobVerifier
{
    public class ValiddationResult
    {
        public List<FileChangeEntry> FileEntries { get; set; }
        public int FilesValidated { get; set; }
        public List<string> IgnoredFiles { get; set; }
        public List<string> PathsThatAreIgnored { get; set; }
    }




    public static class Validator
    {
        private static List<string> _ignorePaths = null;




        /// <summary>
        /// </summary>
        /// <param name="startPath">Root is '/'</param>
        /// <returns></returns>
        public static ValiddationResult Validate(string startPath)
        {
            ValiddationResult result = new ValiddationResult();
            result.FileEntries = new List<FileChangeEntry>();
            result.IgnoredFiles = new List<string>();
            result.PathsThatAreIgnored = new List<string>();

            PopulatePathsThatAreIgnored(result);

            /*  result.FileEntries.Add(new FileChangeEntry { Path = "/app_data/Composite/SomeFile.txt", Type = FileChangeEntryType.MissingLocally });
              result.FileEntries.Add(new FileChangeEntry { Path = "/app_data/Composite/SomeFile.txt", Type = FileChangeEntryType.MissingInBlob });
              result.FileEntries.Add(new FileChangeEntry { Path = "/app_data/Composite/SomeFile.txt", Type = FileChangeEntryType.Changed });

              result.IgnoredFiles.Add("/bin/msassembly.dll");
              result.IgnoredFiles.Add("/bin/msassembly.dll");

              return result;*/

            Log.LogVerbose("C1AzureBlobVerifier", "Comitting files to blob");
            ComitChangesToBlob();


            Log.LogVerbose("C1AzureBlobVerifier", "Building files entires");

            List<FileEntry> liveFiles = GetLiveFiles(startPath);
            List<FileEntry> storedFiles = GetStoredFiles(startPath);

            Log.LogVerbose("C1AzureBlobVerifier", "Done building files entires");


            /*Log.LogInformation("C1AzureBlobVerifier", "Listing all live files");
            foreach (FileEntry fileEntry in liveFiles)
            {
                Log.LogInformation("C1AzureBlobVerifier", fileEntry.Path);
            }*/

            /*Log.LogInformation("C1AzureBlobVerifier", "Listing all stored files");
            foreach (FileEntry fileEntry in storedChanges)
            {
                Log.LogInformation("C1AzureBlobVerifier", fileEntry.Path);
            }*/


            FindChangedAndBlobMissingFiles(result, liveFiles, storedFiles);

            FindLocalMissingFiles(result, liveFiles, storedFiles);

            return result;
        }



        private static void PopulatePathsThatAreIgnored(ValiddationResult result)
        {
            foreach (string path in IgnorePaths)
            {
                result.PathsThatAreIgnored.Add(path);
            }
        }


        
        private static IEnumerable<string> IgnorePaths
        {
            get
            {
                if (_ignorePaths == null)
                {
                    _ignorePaths = new List<string>();
                    _ignorePaths.Add("/bin/composite.azureconnector.dll");
                    _ignorePaths.Add("/bin/webrole.dll");
                    _ignorePaths.Add("/bin/webrole.pdb");
                    _ignorePaths.Add("/bin/microsoft.windowsazure.diagnostics.xml");
                    _ignorePaths.Add("/bin/microsoft.windowsazure.diagnostics.dll");
                    _ignorePaths.Add("/bin/microsoft.windowsazure.storageclient.xml");
                    _ignorePaths.Add("/bin/microsoft.windowsazure.storageclient.dll");

                    _ignorePaths.Add("/app_data/composite/azure/");
                    _ignorePaths.Add("/app_data/composite/cache/");
                    _ignorePaths.Add("/app_data/composite/logfiles/");

                    _ignorePaths.AddRange(GetConfigIgnoredPaths());
                }

                return _ignorePaths;
            }
        }



        private static IEnumerable<string> GetConfigIgnoredPaths()
        {
            IOProviderSettings settings = (IOProviderSettings)ConfigurationServices.ConfigurationSource.GetSection(IOProviderSettings.SectionName);

            AzureIOProviderData data = (AzureIOProviderData)settings.IOProviderPlugins.Where(f => f.GetType() == typeof(AzureIOProviderData)).FirstOrDefault();

            if (data != null)
            {
                foreach (string path in data.IgnorePaths)
                {
                    yield return path;
                }
            }
        }



        private static void FindChangedAndBlobMissingFiles(ValiddationResult result, List<FileEntry> liveFiles, List<FileEntry> storedFiles)
        {
            foreach (FileEntry fileEntry in liveFiles)
            {
                result.FilesValidated++;
                if (DoIgnoreFile(fileEntry))
                {
                    result.IgnoredFiles.Add(fileEntry.Path);
                    continue;
                }

                FileEntry foundEntry = storedFiles.Where(f => f.Path == fileEntry.Path).FirstOrDefault();

                bool isMissing = foundEntry == null;
                if (isMissing)
                {
                    result.FileEntries.Add(
                        new FileChangeEntry
                        {
                            Path = fileEntry.Path,
                            Type = FileChangeEntryType.MissingInBlob
                        });
                }
                else
                {
                    bool isChanged = !Equals(foundEntry.Hash, fileEntry.Hash);
                    if (isChanged)
                    {
                        result.FileEntries.Add(
                            new FileChangeEntry
                            {
                                Path = fileEntry.Path,
                                Type = FileChangeEntryType.Changed
                            });
                    }
                }
            }
        }



        private static void FindLocalMissingFiles(ValiddationResult result, List<FileEntry> liveFiles, List<FileEntry> storedFiles)
        {
            foreach (FileEntry fileEntry in storedFiles)
            {
                if (DoIgnoreFile(fileEntry)) continue;

                bool isExisting = liveFiles.Where(f => f.Path == fileEntry.Path).Any();
                if (!isExisting)
                {
                    result.FileEntries.Add(
                        new FileChangeEntry
                        {
                            Path = fileEntry.Path,
                            Type = FileChangeEntryType.MissingLocally
                        });
                }
            }
        }



        private static void ComitChangesToBlob()
        {
            Log.LogVerbose("C1AzureBlobVerifier", "Comitting file changes to blob");

            AzureFileCommitController.Commit();
        }



        private static bool DoIgnoreFile(FileEntry fileEntry)
        {
            foreach (string path in IgnorePaths)
            {
                if (fileEntry.Path.StartsWith(path)) return true;
            }

            return false;
        }



        private static List<FileEntry> GetLiveFiles(string startPath)
        {
            return GetAllWebsiteFiles(startPath);
        }



        private static List<FileEntry> GetStoredFiles(string startPath)
        {
            List<FileEntry> storedChanges = GetAllBlobFiles(startPath);

            /*string containerName = ContainerPrefix + "installfiles";
            CloudBlobContainer container = CloudClient.GetContainerReference(containerName);
            CloudBlob zipBlob = container.GetBlobReference("Website.zip");

            Log.LogVerbose("C1AzureBlobVerifier", "Reading files from: " + zipBlob.Uri);
            string commonPrefix;
            using (Stream zipFile = zipBlob.OpenRead())
            {
                commonPrefix = FindCommonPrefix(zipFile);
            }

            using (Stream zipFile = zipBlob.OpenRead())
            {
                foreach (FileEntry entry in GetAllZipFiles(zipFile, commonPrefix, startPath))
                {
                    if (!storedChanges.Where(f => f.Path == entry.Path).Any())
                    {
                        storedChanges.Add(entry);
                    }
                }
            }*/

            return storedChanges;
        }



        private static List<FileEntry> GetAllWebsiteFiles(string startPath)
        {
            List<FileEntry> allWebsiteFiles = new List<FileEntry>();

            if (startPath.StartsWith("/")) startPath = startPath.Remove(0, 1);

            string resultPath = Path.Combine(PathUtil.BaseDirectory, startPath);

            Log.LogVerbose("C1AzureBlobVerifier", "Reading files from: " + resultPath);
            FindAllWebsiteFiles(resultPath, allWebsiteFiles);

            allWebsiteFiles.Sort();

            return allWebsiteFiles;
        }



        private static void FindAllWebsiteFiles(string currentPath, List<FileEntry> foundFiles)
        {
            foreach (string directory in Directory.GetDirectories(currentPath))
            {
                FindAllWebsiteFiles(directory, foundFiles);
            }

            foreach (string file in Directory.GetFiles(currentPath))
            {
                string filePath = file.Remove(0, PathUtil.BaseDirectory.Length - 1);

                FileEntry fileEntry = new FileEntry()
                {
                    Path = filePath.Replace("\\", "/").ToLower(),
                    Hash = new FileEntryHash()
                };

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        MD5 md5 = new MD5CryptoServiceProvider();

                        int totalRead = 0;
                        using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            const int bufferSize = 2048;
                            byte[] buffer = new byte[2048];
                            while (true)
                            {
                                int read = fileStream.Read(buffer, 0, bufferSize);
                                totalRead += read;
                                if (read == 0) break;

                                md5.ComputeHash(buffer, 0, read);
                            }
                        }

                        fileEntry.Hash.HashBytes = totalRead == 0 ? new byte[16] : md5.Hash;

                        break;

                    }
                    catch (Exception)
                    {
                        Thread.Sleep((i + 1) * 10);
                    }
                }

                foundFiles.Add(fileEntry);
            }
        }




        private static List<FileEntry> GetAllZipFiles(Stream compressedStream, string commonPrefix, string startPath)
        {
            List<FileEntry> allZipFiles = new List<FileEntry>();

            int removeCount = commonPrefix != null ? commonPrefix.Length : 0;
            if (commonPrefix != null && commonPrefix.EndsWith("/")) removeCount--;


            using (ZipInputStream zipStream = new ZipInputStream(compressedStream))
            {
                ZipEntry entry;
                while ((entry = zipStream.GetNextEntry()) != null)
                {
                    if (!entry.IsFile) continue;

                    string filePath = entry.Name.Remove(0, removeCount);

                    FileEntry fileEntry = new FileEntry()
                    {
                        Path = filePath.Replace("\\", "/").ToLower(),
                        Hash = new FileEntryHash()
                    };

                    if (!fileEntry.Path.ToLower().StartsWith(startPath.ToLower())) continue;

                    MD5 md5 = new MD5CryptoServiceProvider();

                    int totalRead = 0;

                    const int bufferSize = 2048;
                    byte[] buffer = new byte[2048];
                    while (true)
                    {
                        int read = zipStream.Read(buffer, 0, bufferSize);
                        totalRead += read;

                        if (read == 0) break;

                        md5.ComputeHash(buffer, 0, read);
                    }

                    fileEntry.Hash.HashBytes = totalRead == 0 ? new byte[16] : md5.Hash;

                    allZipFiles.Add(fileEntry);
                }
            }

            allZipFiles.Sort();

            return allZipFiles;
        }



        private static List<FileEntry> GetAllBlobFiles(string startPath)
        {
            List<FileEntry> allBlobFiles = new List<FileEntry>();

            Log.LogVerbose("C1AzureBlobVerifier", "Reading files from: " + AzureFacade.C1WebsiteContainer.Uri);
            foreach (CloudBlob blob in AzureFacade.C1WebsiteContainer.GetAllBlobsRecursively())
            {
                string filePath = blob.Uri.LocalPath.Remove(0, AzureFacade.C1WebsiteContainerName.Length + 1).Replace('\\', '/');

                if (!filePath.StartsWith(startPath.ToLower())) continue;

                FileEntry fileEntry = new FileEntry
                {
                    Path = filePath,
                    Hash = new FileEntryHash()
                };

                using (Stream blobStream = blob.OpenRead())
                {
                    MD5 md5 = new MD5CryptoServiceProvider();

                    int totalRead = 0;

                    const int bufferSize = 2048;
                    byte[] buffer = new byte[2048];
                    while (true)
                    {
                        int read = blobStream.Read(buffer, 0, bufferSize);
                        totalRead += read;

                        if (read == 0) break;

                        md5.ComputeHash(buffer, 0, read);
                    }

                    fileEntry.Hash.HashBytes = totalRead == 0 ? new byte[16] : md5.Hash;
                }

                allBlobFiles.Add(fileEntry);
            }

            return allBlobFiles;
        }



        public static string FindCommonPrefix(Stream compressedStream)
        {
            using (ZipInputStream zipStream = new ZipInputStream(compressedStream))
            {
                string foundPrefix = null;
                bool allStartsWithSamePrefix = true;

                bool isFirst = true;
                ZipEntry entry;
                while ((entry = zipStream.GetNextEntry()) != null)
                {
                    if (isFirst && entry.IsDirectory)
                    {
                        foundPrefix = entry.Name.ToLower();
                        isFirst = false;
                    }
                    else
                    {
                        if ((foundPrefix != null) && (!entry.Name.ToLower().StartsWith(foundPrefix)))
                        {
                            allStartsWithSamePrefix = false;
                            break;
                        }
                    }
                }

                if (allStartsWithSamePrefix)
                {
                    return foundPrefix;
                }

                return null;
            }
        }
    }
}
