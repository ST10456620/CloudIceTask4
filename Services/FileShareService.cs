using System;
using System.Collections.Generic;
using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CoffeeNChill.Services
{
   public class FileShareService
    {

        private readonly BlobContainerClient _containerClient;
        public FileShareService(string connectionString, string containerName) 
        {

            _containerClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<long> UploadFileAsync(string fileName, Stream content)
        {
            await _containerClient.CreateIfNotExistsAsync();
			var blobClient = _containerClient.GetBlobClient(fileName);

			await blobClient.UploadAsync(content, overwrite: true);

			return content.Length;
		}

        public async Task<List<object>> ListFilesAsync() 
        {

			var documents = new List<object>();

            if(!await _containerClient.ExistsAsync())
            return documents;


			await foreach (BlobItem blob in _containerClient.GetBlobsAsync())
			{

				documents.Add(new { FileName = blob.Name, SizeBytes = blob.Properties.ContentLength, LastModified = blob.Properties.LastModified });

			}

			return documents;
		}

		public async Task <BlobDownloadInfo?> DownloadFileAsync(string fileName)
		{

			var blobClient = _containerClient.GetBlobClient(fileName);

			if (!await blobClient.ExistsAsync())
				return null;

			return await blobClient.DownloadAsync();
		}

    }
}
