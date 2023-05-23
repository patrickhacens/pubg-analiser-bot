namespace PUBG.Analiser.Functions;

public class StorageOptions
{
    public string ConnectionString { get; set; } = null!; 

    public string PlayerTableName { get; set; } = null!;

    public string MatchTableName { get; set; } = null!;

    public string QueueName { get; set; } = null!;
}
