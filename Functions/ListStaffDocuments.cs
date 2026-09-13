using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using CoffeeNChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace CoffeeNChill.Functions;

public class ListStaffDocuments
{
    private readonly ILogger<ListStaffDocuments> _logger;
    private readonly FileShareService _fileShareService;

    public ListStaffDocuments(ILogger<ListStaffDocuments> logger, FileShareService fileShareService)
    {
        _logger = logger;
        _fileShareService = fileShareService;
    }

    [Function("ListStaffDocuments")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "documents")] HttpRequest req)
    {

        var documents = await _fileShareService.ListFilesAsync();

        _logger.LogInformation("Listed {Count} staff documents", documents.Count);

        return new OkObjectResult(documents);

        //      var connectionString = Environment.GetEnvironmentVariable("AzureFilesConnection");
        //      var shareClient = new ShareClient(connectionString, "staff-docs");
        //      var directory = shareClient.GetRootDirectoryClient();

        //var documents = new List<object>();

        //if (await shareClient.ExistsAsync())
        //      {
        //	await foreach (ShareFileItem item in directory.GetFilesAndDirectoriesAsync())
        //	{
        //		if (item.IsDirectory) continue; // skip subfolders, only list files

        //		var fileClient = directory.GetFileClient(item.Name);
        //		ShareFileProperties props = await fileClient.GetPropertiesAsync();

        //		documents.Add(new { FileName = item.Name, SizeBytes = props.ContentLength, LastModified = props.LastModified });


    }

		}
        

        


//		return new OkObjectResult(documents);
//    }
//}