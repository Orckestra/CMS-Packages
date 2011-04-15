using System.Collections.Generic;
using Microsoft.WindowsAzure.StorageClient;
using System.Diagnostics;
using System;


namespace Composite.Tools.AzureBlobVerifier
{
    internal static class AzureUtil
    {
        public static IEnumerable<CloudBlob> GetAllBlobsRecursively(this CloudBlobContainer container)
        {
            IEnumerable<IListBlobItem> blobItems = container.ListBlobs();

            foreach (IListBlobItem item in blobItems)
            {
                if (item is CloudBlob)
                {
                    yield return item as CloudBlob;
                }
                else
                {
                    foreach (CloudBlob blob in GetAllBlobsRecursively(item as CloudBlobDirectory))
                    {
                        yield return blob;
                    }
                }
            }
        }



        public static IEnumerable<CloudBlob> GetAllBlobsRecursively(this CloudBlobDirectory directory)
        {
            IEnumerable<IListBlobItem> blobItems = directory.ListBlobs();

            foreach (IListBlobItem item in blobItems)
            {
                if (item is CloudBlob)
                {
                    yield return item as CloudBlob;
                }
                else
                {
                    foreach (CloudBlob blob in GetAllBlobsRecursively(item as CloudBlobDirectory))
                    {
                        yield return blob;
                    }
                }
            }
        }     
    }
}
