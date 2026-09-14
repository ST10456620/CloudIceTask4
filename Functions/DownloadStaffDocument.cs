using System.Net;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using CoffeeNChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class DownloadStaffDocument
{
    private readonly ILogger<DownloadStaffDocument> _logger;
    private readonly FileShareService _fileShareService;

    public DownloadStaffDocument(ILogger<DownloadStaffDocument> logger, FileShareService fileShareService)
    {
        _logger = logger;
        _fileShareService = fileShareService;
    }

    [Function("DownloadStaffDocument")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "documents/download/{fileName}")] HttpRequest req, string fileName)
    {
       var download = await _fileShareService.DownloadFileAsync(fileName);

        if (download == null)
            return new NotFoundObjectResult($"'{fileName}' not found.");

        _logger.LogInformation("Downloading {FileName}", fileName);

        return new FileStreamResult(download.Content, download.ContentType ?? "application/octet-stream")
        { 
        FileDownloadName = fileName
        
        };
    }
}