// This entity model is based on the ITableEntity implementation pattern
// described in the Azure.Data.Tables client library documentation
// (Microsoft, 2025).
using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeNChill.Models
{
    /// <summary>
    /// Represents a single menu item stored in the Azure Table "MenuItems".
    /// PartitionKey groups items by category; RowKey is the unique item SKU.
    /// </summary>
    public class MenuItem : ITableEntity
    {
        /// <summary>Category the item belongs to, e.g. "Hot Drinks", "Pastries".</summary>
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>Unique item SKU/ID, e.g. "COF-001".</summary>
        public string RowKey { get; set; } = string.Empty;

        /// <summary>Display name of the item, e.g. "Espresso".</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Short description shown on the digital menu.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Price of the item in ZAR.</summary>
        public double Price { get; set; }

        /// <summary>Whether the item is currently available for sale.</summary>
        public bool IsAvailable { get; set; }

        // Required by ITableEntity (Microsoft, 2025)
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}