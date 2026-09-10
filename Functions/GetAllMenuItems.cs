// HTTP trigger structure adapted from Microsoft (2025) — see CreateMenuItem.cs
// header for full citation.
using System.Net;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions
{
    /// <summary>
    /// GET /api/menu — returns every menu item across all categories.
    /// </summary>
    public class GetAllMenuItems
    {
        private readonly ILogger _logger;
        private readonly TableStorageService _tableService;

        public GetAllMenuItems(ILoggerFactory loggerFactory, TableStorageService tableService)
        {
            _logger = loggerFactory.CreateLogger<GetAllMenuItems>();
            _tableService = tableService;
        }

        [Function("GetAllMenuItems")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu")] HttpRequestData req)
        {
            var items = await _tableService.GetAllMenuItemsAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(items);
            return response;
        }
    }
}