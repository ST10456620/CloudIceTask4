// HTTP trigger structure adapted from Microsoft (2025) — see CreateMenuItem.cs
// header for full citation. Route parameter binding also follows this source.
using System.Net;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions
{
    /// <summary>
    /// GET /api/menu/category/{category} — returns menu items filtered by category (PartitionKey).
    /// </summary>
    public class GetMenuItemsByCategory
    {
        private readonly ILogger _logger;
        private readonly TableStorageService _tableService;

        public GetMenuItemsByCategory(ILoggerFactory loggerFactory, TableStorageService tableService)
        {
            _logger = loggerFactory.CreateLogger<GetMenuItemsByCategory>();
            _tableService = tableService;
        }

        [Function("GetMenuItemsByCategory")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu/category/{category}")] HttpRequestData req,
            string category)
        {
            var items = await _tableService.GetMenuItemsByCategoryAsync(category);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(items);
            return response;
        }
    }
}