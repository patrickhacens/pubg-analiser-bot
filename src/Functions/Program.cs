using Azure.Data.Tables;
using Azure.Storage.Queues;
using Discord.Webhook;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace PUBG.Analiser.Functions
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((context, builder) => builder
                    .AddJsonFile("local.settings.json", true, true)
                    .AddEnvironmentVariables())
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((context, services) =>
                {
                    StorageOptions storageOptions = new StorageOptions();
                    context.Configuration.Bind("StorageOptions", storageOptions);

                    #region Ensure services existance
                    var matchesTable = new TableClient(storageOptions.ConnectionString, storageOptions.MatchTableName);
                    //matchesTable.CreateIfNotExists();

                    var playersTable = new TableClient(storageOptions.ConnectionString, storageOptions.PlayerTableName);
                    //playersTable.CreateIfNotExists();

                    var matchesQueue = new QueueClient(storageOptions.ConnectionString, storageOptions.QueueName);
                    //matchesQueue.CreateIfNotExists();
                    #endregion

                    services.AddSingleton<StorageOptions>(storageOptions);

                    services.AddTransient<PUBGApi>(sp => new PUBGApi(context.Configuration.GetSection("PUBGApiOptions").Get<PUBGApiOptions>()));
                })
                .Build();

            host.Run();
        }
    }
}