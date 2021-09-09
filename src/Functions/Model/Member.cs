using Azure;
using Azure.Data.Tables;
using System;

namespace PUBG.Analiser.Functions.Model
{
    public class Member : ITableEntity
    {
        public string Id { get => RowKey; set => RowKey = value; }

        public string Name { get; set; }

        public string DiscordId { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
