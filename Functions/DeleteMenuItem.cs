// HTTP trigger structure adapted from Microsoft (2025) — see CreateMenuItem.cs
using System.Net;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions
{
    /// <summary>
    /// DELETE /api/menu/{category}/{id} — removes a menu item.
    /// </summary>
    public class DeleteMenuItem
    {
        private readonly ILogger _logger;
        private readonly TableStorageService _tableService;

        public DeleteMenuItem(ILoggerFactory loggerFactory, TableStorageService tableService)
        {
            _logger = loggerFactory.CreateLogger<DeleteMenuItem>();
            _tableService = tableService;
        }

        [Function("DeleteMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "menu/{category}/{id}")] HttpRequestData req,
            string category, string id)
        {
            bool deleted = await _tableService.DeleteMenuItemAsync(category, id);

            if (!deleted)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync($"Menu item {id} in category {category} not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
    }
}