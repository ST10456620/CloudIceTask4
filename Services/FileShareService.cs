using System;
using System.Collections.Generic;
using System.Text;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace CoffeeNChill.Services
{
   public class FileShareService
    {

        private readonly ShareClient _shareClient;
        public FileShareService(string connectionString, string shareName) 
        {

            _shareClient = new ShareClient(connectionString, shareName);
        }

        public async Task<long> UploadFileAsync(string fileName, Stream content)
        {
            await _shareClient.CreateIfNotExistsAsync();
			var directory = _shareClient.GetRootDirectoryClient();
			var fileClient = directory.GetFileClient(fileName);

			await fileClient.CreateAsync(content.Length);
			await fileClient.UploadRangeAsync(new Azure.HttpRange(0, content.Length), content);

            return content.Length;
		}

        public async Task<List<object>> ListFilesAsync() 
        {

			var documents = new List<object>();

            if(!await _shareClient.ExistsAsync())
            return documents;

			var directory = _shareClient.GetRootDirectoryClient();

			await foreach (ShareFileItem item in directory.GetFilesAndDirectoriesAsync())
			{
				if (item.IsDirectory) continue; // skip subfolders, only list files

				var fileClient = directory.GetFileClient(item.Name);
				ShareFileProperties props = await fileClient.GetPropertiesAsync();

				documents.Add(new { FileName = item.Name, SizeBytes = props.ContentLength, LastModified = props.LastModified });


			}

			return documents;
		}

		public async Task <ShareFileDownloadInfo?>DownloadFileAsync(string fileName)
		{

			var directory = _shareClient.GetRootDirectoryClient();
			var fileClient = directory.GetFileClient(fileName);


			if (!await fileClient.ExistsAsync())
				return null;

			return await fileClient.DownloadAsync();
		}

    }
}
