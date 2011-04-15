using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Composite.Core.IO;
using Microsoft.WindowsAzure.StorageClient;

namespace Composite.Tools.AzureBlobVerifier
{
	public static class RepairWorker
	{
		public static XElement Repair(string type, string path)
		{
			XElement resultHtml = new XElement("div", new XAttribute("style", "padding: 10px"), new XElement("h3", "Repair log"));

			if (type == FileChangeEntryType.MissingLocally.ToString())
			{
				RestoreLocalFile(path, resultHtml);
			}
			else if (type == FileChangeEntryType.MissingInBlob.ToString())
			{
				RestoreBlobFile(path, resultHtml);
			}
			else if (type == FileChangeEntryType.Changed.ToString() + "Local")
			{
				RestoreBlobFile(path, resultHtml);
			}
			else if (type == FileChangeEntryType.Changed.ToString() + "Blob")
			{
				RestoreLocalFile(path, resultHtml);
			}
			else if (type == "AllLocal")
			{
				ValiddationResult result = Validator.Validate(path);

				foreach (FileChangeEntry fileEntry in result.FileEntries)
				{
					switch (fileEntry.Type)
					{
						case FileChangeEntryType.MissingLocally:
							RestoreLocalFile(fileEntry.Path, resultHtml);
							break;

						case FileChangeEntryType.MissingInBlob:
							RestoreBlobFile(fileEntry.Path, resultHtml);
							break;

						case FileChangeEntryType.Changed:
							RestoreBlobFile(fileEntry.Path, resultHtml);
							break;
					}
				}
			}
			else if (type == "AllBlob")
			{
				ValiddationResult result = Validator.Validate(path);

				foreach (FileChangeEntry fileEntry in result.FileEntries)
				{
					switch (fileEntry.Type)
					{
						case FileChangeEntryType.MissingLocally:
							RestoreLocalFile(fileEntry.Path, resultHtml);
							break;

						case FileChangeEntryType.MissingInBlob:
							RestoreBlobFile(fileEntry.Path, resultHtml);
							break;

						case FileChangeEntryType.Changed:
							RestoreLocalFile(fileEntry.Path, resultHtml);
							break;
					}
				}
			}

			return resultHtml;
		}

		/// <summary>
		/// </summary>
		/// <param name="path">Prefixed with /</param>
		/// <param name="resultHtml"></param>
		private static void RestoreLocalFile(string path, XElement resultHtml)
		{
			string azurePath = path.Substring(1);

			CloudBlob blob = AzureFacade.C1WebsiteContainer.GetBlobReference(azurePath);
			blob.FetchAttributes();

			string localPath;
			string metaDataLocalPath = blob.Metadata["LocalPath"];
			if (metaDataLocalPath != null)
			{
				localPath = Path.Combine(PathUtil.BaseDirectory, metaDataLocalPath);
			}
			else
			{
				localPath = Path.Combine(PathUtil.BaseDirectory, path.Substring(1));
			}


			Exception lastException = null;
			for (int i = 0; i < 10; i++)
			{
				try
				{
					if (File.Exists(localPath))
					{
						FileAttributes fileAttributes = File.GetAttributes(localPath);

						if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
						{
							fileAttributes ^= FileAttributes.ReadOnly;
							File.SetAttributes(localPath, fileAttributes);
						}

						File.Delete(localPath);
					}

					blob.DownloadToFile(localPath);
				}
				catch (Exception ex)
				{
					lastException = ex;
					Thread.Sleep((i + 1) * 10);
				}
			}

			if (lastException == null)
			{
				resultHtml.Add(new XElement("div", "File copied from the blob (" + blob.Uri.LocalPath + ") to local disk (" + localPath + ")"));
			}
			else
			{
				resultHtml.Add(new XElement("div", "Error when trying to copy the file from the blob (" + blob.Uri.LocalPath + ") to local disk (" + localPath + ")"));
				resultHtml.Add(new XElement("div", lastException.ToString()));
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="path">Prefixed with /</param>
		/// <param name="resultHtml"></param>
		private static void RestoreBlobFile(string path, XElement resultHtml)
		{
			string localPath = GetCasedPath(Path.Combine(PathUtil.BaseDirectory, path.Substring(1)));
			string metaDataLocalPath = localPath.Substring(PathUtil.BaseDirectory.Length).Replace("\\", "/");

			CloudBlob blob = AzureFacade.C1WebsiteContainer.GetBlobReference(path.Substring(1).ToLower());

			Exception lastException = null;
			for (int i = 0; i < 10; i++)
			{
				try
				{
					blob.UploadFile(localPath);
					blob.Metadata["LocalPath"] = metaDataLocalPath;
					blob.SetMetadata();
				}
				catch (Exception ex)
				{
					lastException = ex;
					Thread.Sleep((i + 1) * 10);
				}
			}

			if (lastException == null)
			{
				resultHtml.Add(new XElement("div", "File copied from local disk (" + localPath + ") to blob (" + blob.Uri.LocalPath + ")"));
			}
			else
			{
				resultHtml.Add(new XElement("div", "Error when trying to copy the local file (" + localPath + ") to the blob (" + blob.Uri.LocalPath + ")"));
				resultHtml.Add(new XElement("div", lastException.ToString()));
			}
		}

		private static string GetCasedPath(string path)
		{
			string basePath = PathUtil.BaseDirectory;
			if ((basePath.EndsWith("\\")) || (basePath.EndsWith("/")))
			{
				basePath = basePath.Remove(basePath.Length - 1);
			}

			string casedDirectory = GetCasedDirectoryPath(Path.GetDirectoryName(path), basePath);
			string casedFilename = GetCasedFilename(path);

			return Path.Combine(casedDirectory, Path.GetFileName(casedFilename));
		}

		private static string GetCasedFilename(string path)
		{
			return Directory.GetFiles(Path.GetDirectoryName(path), Path.GetFileName(path)).Single();
		}

		private static string GetCasedDirectoryPath(string path, string basePath)
		{
			if (path.Equals(basePath, StringComparison.OrdinalIgnoreCase)) return basePath;

			string parentDirectory = Path.GetDirectoryName(path);
			string currentDirectory = Path.GetFileName(path);

			string casedPath = Directory.GetDirectories(parentDirectory, currentDirectory).Single();
			string casedCurrentDirectory = Path.GetFileName(casedPath);

			string totalCasedPath = GetCasedDirectoryPath(parentDirectory, basePath);

			return Path.Combine(totalCasedPath, casedCurrentDirectory);
		}
	}
}
