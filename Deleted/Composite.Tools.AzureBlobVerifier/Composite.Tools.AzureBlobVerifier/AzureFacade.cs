using System;
using System.Linq;
using Composite.AzureConnector.IOProvider;
using Composite.Core;
using Composite.Core.Configuration;
using Composite.Core.IO.Plugins.IOProvider.Runtime;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Composite.Tools.AzureBlobVerifier
{
	internal static class AzureFacade
	{
		private static CloudStorageAccount _account;
		private static CloudBlobClient _client;
		private static CloudBlobContainer _websiteContainer;

		public static CloudBlobContainer C1WebsiteContainer
		{
			get
			{
				if (_websiteContainer == null)
				{
					_websiteContainer = CloudClient.GetContainerReference(C1WebsiteContainerName);
				}

				return _websiteContainer;
			}
		}

		public static string C1WebsiteContainerName
		{
			get
			{
				return ContainerPrefix + "website";
			}
		}

		public static CloudBlobClient CloudClient
		{
			get
			{
				if (_account == null)
				{
					StorageCredentialsAccountAndKey credentials = new StorageCredentialsAccountAndKey(AccountName, AccountKey);

					_account = new CloudStorageAccount(credentials, false);
					_client = _account.CreateCloudBlobClient();
				}

				return _client;
			}
		}

		public static string AccountName
		{
			get
			{
				IOProviderSettings settings = (IOProviderSettings)ConfigurationServices.ConfigurationSource.GetSection(IOProviderSettings.SectionName);
				AzureIOProviderData data = (AzureIOProviderData)settings.IOProviderPlugins.Where(f => f.GetType() == typeof(AzureIOProviderData)).FirstOrDefault();

				string connectionString = data.BlobConnectionString;

				int startIndex = connectionString.IndexOf("AccountName=");
				if (startIndex == -1)
				{
					Log.LogError("AzureBlobVerifier", "Blob connection string is malformed");
					throw new InvalidOperationException("Blob connection string is malformed");
				}
				startIndex += "AccountName=".Length;

				int endIndex = connectionString.IndexOf(';', startIndex);
				endIndex = endIndex != -1 ? endIndex : connectionString.Length;

				string accountName = connectionString.Substring(startIndex, endIndex - startIndex);

				return accountName;
			}
		}

		public static string AccountKey
		{
			get
			{
				IOProviderSettings settings = (IOProviderSettings)ConfigurationServices.ConfigurationSource.GetSection(IOProviderSettings.SectionName);
				AzureIOProviderData data = (AzureIOProviderData)settings.IOProviderPlugins.Where(f => f.GetType() == typeof(AzureIOProviderData)).FirstOrDefault();

				string connectionString = data.BlobConnectionString;

				int startIndex = connectionString.IndexOf("AccountKey=");
				if (startIndex == -1)
				{
					Log.LogError("AzureBlobVerifier", "Blob connection string is malformed");
					throw new InvalidOperationException("Blob connection string is malformed");
				}
				startIndex += "AccountKey=".Length;

				int endIndex = connectionString.IndexOf(';', startIndex);
				endIndex = endIndex != -1 ? endIndex : connectionString.Length;

				string accountKey = connectionString.Substring(startIndex, endIndex - startIndex);

				return accountKey;
			}
		}

		public static string ContainerPrefix
		{
			get
			{
				IOProviderSettings settings = (IOProviderSettings)ConfigurationServices.ConfigurationSource.GetSection(IOProviderSettings.SectionName);

				AzureIOProviderData data = (AzureIOProviderData)settings.IOProviderPlugins.Where(f => f.GetType() == typeof(AzureIOProviderData)).FirstOrDefault();

				return data.BlobContainerPrefix;
			}
		}
	}
}
