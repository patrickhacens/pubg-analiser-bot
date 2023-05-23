using Azure;
using Azure.Data.Tables;
using System.Diagnostics;

namespace PUBG.Analiser.Functions.Models;

[DebuggerDisplay("Player {Name}")]
public class Member : ITableEntity
{
    public string Id { get => RowKey; set => RowKey = value; }
    public string Name { get; set; } = null!;
    public string DiscordId { get; set; } = null!;
    public string PartitionKey { get; set; } = null!;
    public string RowKey { get; set; } = null!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
