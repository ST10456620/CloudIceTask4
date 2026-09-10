using System.Net;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class DownloadStaffDocument
{
    private readonly ILogger<DownloadStaffDocument> _logger;

    public DownloadStaffDocument(ILogger<DownloadStaffDocument> logger)
    {
        _logger = logger;
    }

    [Function("DownloadStaffDocument")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "documents/download/{fileName}")] HttpRequest req, string fileName)
    {
        var connectionString = Environment.GetEnvironmentVariable("AzureFilesConnection");
        var shareClient = new ShareClient(connectionString, "staff-docs");
        var directory = shareClient.GetRootDirectoryClient();
        var fileClient = directory.GetFileClient(fileName);

        if (!await fileClient.ExistsAsync()) 
        {

            
            return new NotFoundObjectResult($"'{fileName}' not found in staff-doc.");
        }

		_logger.LogInformation("Downloading document {documentName}", fileName);

		ShareFileDownloadInfo download = await fileClient.DownloadAsync();

        return new FileStreamResult(download.Content, download.ContentType ?? "application/octet-stream")
        { 
        FileDownloadName = fileName
        
        };
    }
}