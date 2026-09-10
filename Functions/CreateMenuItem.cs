// HTTP trigger structure (attribute usage, HttpRequestData/HttpResponseData
// pattern) adapted from the Azure Functions HTTP trigger documentation
// for .NET isolated worker functions (Microsoft, 2025).
using System.Net;
using System.Text.Json;
using CoffeeNChill.Models;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions
{
    /// <summary>
    /// POST /api/menu — accepts a JSON body and inserts a new menu item.
    /// </summary>
    public class CreateMenuItem
    {
        private readonly ILogger _logger;
        private readonly TableStorageService _tableService;

        public CreateMenuItem(ILoggerFactory loggerFactory, TableStorageService tableService)
        {
            _logger = loggerFactory.CreateLogger<CreateMenuItem>();
            _tableService = tableService;
        }

        [Function("CreateMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")] HttpRequestData req)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            MenuItem? item;

            // Validate JSON before attempting to use it
            try
            {
                item = JsonSerializer.Deserialize<MenuItem>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid JSON body.");
                return badResponse;
            }

            // PartitionKey/RowKey are required for Azure Table Storage inserts
            if (item is null || string.IsNullOrWhiteSpace(item.PartitionKey) || string.IsNullOrWhiteSpace(item.RowKey))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("PartitionKey (Category) and RowKey (SKU) are required.");
                return badResponse;
            }

            var created = await _tableService.CreateMenuItemAsync(item);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }
    }
}