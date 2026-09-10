using System.Net;
using System.Text.Json;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class ListStaffDocuments
{
    private readonly ILogger<ListStaffDocuments> _logger;

    public ListStaffDocuments(ILogger<ListStaffDocuments> logger)
    {
        _logger = logger;
    }

    [Function("ListStaffDocuments")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "documents")] HttpRequest req)
    {

        var connectionString = Environment.GetEnvironmentVariable("AzureFilesConnection");
        var shareClient = new ShareClient(connectionString, "staff-docs");
        var directory = shareClient.GetRootDirectoryClient();

		var documents = new List<object>();

		if (await shareClient.ExistsAsync())
        {
			await foreach (ShareFileItem item in directory.GetFilesAndDirectoriesAsync())
			{
				if (item.IsDirectory) continue; // skip subfolders, only list files

				var fileClient = directory.GetFileClient(item.Name);
				ShareFileProperties props = await fileClient.GetPropertiesAsync();

				documents.Add(new { FileName = item.Name, SizeBytes = props.ContentLength, LastModified = props.LastModified });


			}

		}
        

        


		return new OkObjectResult(documents);
    }
}