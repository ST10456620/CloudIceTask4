using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Files.Shares;
using CoffeeNChill.Services;

namespace CoffeeNChill.Functions;

public class UploadStaffDocument
{
    private readonly FileShareService _fileShareService;

    public UploadStaffDocument(FileShareService fileShareService)
    {
        _fileShareService = fileShareService;
    }

    [Function("UploadStaffDocument")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route ="documents/upload")] HttpRequest req)
    {
        if(!req.HasFormContentType || req.Form.Files.Count == 0)
        {
            return new BadRequestObjectResult("Send the file as mulitpart/form-data with at least one file field.");
        }

        var file = req.Form.Files[0];
		using var stream = file.OpenReadStream();
        var size = await _fileShareService.UploadFileAsync(file.FileName, stream);

        return new OkObjectResult($"Uploaded '{file.FileName}' ({size} bytes) to staff-docs.");
	}
}