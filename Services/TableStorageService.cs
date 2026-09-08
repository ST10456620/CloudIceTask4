using System;
using System.Collections.Generic;
using System.Text;

// CRUD implementation adapted from the query and entity-operation patterns
// shown in the Azure.Data.Tables client library documentation (Microsoft, 2025).
using Azure;
using Azure.Data.Tables;
using CoffeeNChill.Models;

namespace CoffeeNChill.Services
{
    /// <summary>
    /// Wraps Azure Table Storage operations for the MenuItems table,
    /// isolating the rest of the app from the Azure.Data.Tables SDK directly.
    /// </summary>
    public class TableStorageService
    {
        private readonly TableClient _tableClient;

        /// <param name="connectionString">Storage connection string (Azurite: "UseDevelopmentStorage=true").</param>
        /// <param name="tableName">Name of the table to operate on. Defaults to "MenuItems".</param>
        public TableStorageService(string connectionString, string tableName = "MenuItems")
        {
            _tableClient = new TableClient(connectionString, tableName);
            _tableClient.CreateIfNotExists(); // (Microsoft, 2025)
        }

        /// <summary>Inserts a new menu item entity into the table.</summary>
        public async Task<MenuItem> CreateMenuItemAsync(MenuItem item)
        {
            await _tableClient.AddEntityAsync(item);
            return item;
        }

        /// <summary>Returns every menu item across all categories.</summary>
        public async Task<List<MenuItem>> GetAllMenuItemsAsync()
        {
            var items = new List<MenuItem>();
            await foreach (var item in _tableClient.QueryAsync<MenuItem>())
            {
                items.Add(item);
            }
            return items;
        }

        /// <summary>Returns menu items filtered by PartitionKey (category).</summary>
        public async Task<List<MenuItem>> GetMenuItemsByCategoryAsync(string category)
        {
            var items = new List<MenuItem>();
            await foreach (var item in _tableClient.QueryAsync<MenuItem>(
                x => x.PartitionKey == category))
            {
                items.Add(item);
            }
            return items;
        }

        /// <summary>Updates the price and/or availability of an existing menu item.</summary>
        /// <returns>The updated item, or null if it doesn't exist.</returns>
        public async Task<MenuItem?> UpdateMenuItemAsync(string category, string id, double? price, bool? isAvailable)
        {
            try
            {
                var existing = await _tableClient.GetEntityAsync<MenuItem>(category, id);
                var item = existing.Value;

                if (price.HasValue) item.Price = price.Value;
                if (isAvailable.HasValue) item.IsAvailable = isAvailable.Value;

                // Optimistic concurrency via ETag (Microsoft, 2025)
                await _tableClient.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
                return item;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        /// <summary>Deletes a menu item. Returns false if it doesn't exist.</summary>
        public async Task<bool> DeleteMenuItemAsync(string category, string id)
        {
            try
            {
                await _tableClient.DeleteEntityAsync(category, id);
                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return false;
            }
        }
    }
}