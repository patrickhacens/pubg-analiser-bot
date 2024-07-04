using Azure.Data.Tables;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PUBG;
using PUBG.Analiser.Functions;

var builder = new HostBuilder()
    .ConfigureAppConfiguration(confi => confi.AddJsonFile("local.settings.json", true, true))
    .ConfigureFunctionsWorkerDefaults();


builder.ConfigureServices((context, services) =>
{
    StorageOptions storageOptions = new();
    context.Configuration.Bind("StorageOptions", storageOptions);

    //#region Ensure services existance
    //var matchesTable = new TableClient(storageOptions.ConnectionString, storageOptions.MatchTableName);
    ////matchesTable.CreateIfNotExists();

    //var playersTable = new TableClient(storageOptions.ConnectionString, storageOptions.PlayerTableName);
    ////playersTable.CreateIfNotExists();

    //var matchesQueue = new QueueClient(storageOptions.ConnectionString, storageOptions.QueueName);
    ////matchesQueue.CreateIfNotExists();
    //#endregion

    services.AddSingleton<StorageOptions>(storageOptions);

    services.AddTransient<PUBGApi>(sp => new PUBGApi(context.Configuration.GetSection("PUBGApiOptions").Get<PUBGApiOptions>()));
});

var host = builder.Build();
host.Run();

