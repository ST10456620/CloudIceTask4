using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Files.Shares;

namespace CoffeeNChill.Functions;

public class UploadStaffDocument
{
    private readonly ILogger<UploadStaffDocument> _logger;

    public UploadStaffDocument(ILogger<UploadStaffDocument> logger)
    {
        _logger = logger;
    }

    [Function("UploadStaffDocument")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route ="documents/upload")] HttpRequest req)
    {
        if(!req.HasFormContentType || req.Form.Files.Count == 0)
        {
            return new BadRequestObjectResult("Send the file as mulitpart/form-data with at least one file field.");
        }

        var file = req.Form.Files[0];

		var connectionString = Environment.GetEnvironmentVariable("AzureFilesConnection");
		var shareClient = new ShareClient(connectionString, "staff-docs");
		await shareClient.CreateIfNotExistsAsync();

		var directory = shareClient.GetRootDirectoryClient();
		var fileClient = directory.GetFileClient(file.FileName);

        using var stream = file.OpenReadStream();
        await fileClient.CreateAsync(stream.Length);
        await fileClient.UploadRangeAsync(new Azure.HttpRange(0, stream.Length), stream);

        _logger.LogInformation("Uploaded {FileName} ({Size} bytes) to staff-docs", file.FileName, stream.Length);
        return new OkObjectResult($"Uploaded '{file.FileName}' ({stream.Length} bytes) to staff-docs.");
	}
}