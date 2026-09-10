// HTTP trigger structure adapted from Microsoft (2025) — see CreateMenuItem.cs
// header for full citation.
using System.Net;
using System.Text.Json;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions
{
    /// <summary>
    /// PUT /api/menu/{category}/{id} — updates the price and/or availability of a menu item.
    /// </summary>
    public class UpdateMenuItem
    {
        private readonly ILogger _logger;
        private readonly TableStorageService _tableService;

        public UpdateMenuItem(ILoggerFactory loggerFactory, TableStorageService tableService)
        {
            _logger = loggerFactory.CreateLogger<UpdateMenuItem>();
            _tableService = tableService;
        }

        /// <summary>Shape of the JSON body expected for an update request.</summary>
        private class UpdateRequest
        {
            public double? Price { get; set; }
            public bool? IsAvailable { get; set; }
        }

        [Function("UpdateMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "menu/{category}/{id}")] HttpRequestData req,
            string category, string id)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            var update = JsonSerializer.Deserialize<UpdateRequest>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var updated = await _tableService.UpdateMenuItemAsync(category, id, update?.Price, update?.IsAvailable);

            if (updated is null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync($"Menu item {id} in category {category} not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(updated);
            return response;
        }
    }
}