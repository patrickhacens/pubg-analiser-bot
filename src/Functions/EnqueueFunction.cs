using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PUBG.Analiser.Functions.Model;

namespace PUBG.Analiser.Functions
{
    public static class EnqueueFunction
    {
        [Function("EnqueueFunction")]
        public static async Task Run(
#if DEBUG
            [TimerTrigger("0 * * * * *")] 
#else
            [TimerTrigger("0 */5 * * * *")] 
#endif
            MyInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger("EnqueueFunction");
            logger.LogInformation($"Enqueue function executed at: {DateTime.Now}");
            logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            
            StorageOptions storageOptions = context.InstanceServices.GetRequiredService<StorageOptions>();

            CancellationTokenSource tks = new CancellationTokenSource();

            var playerTable = new TableClient(storageOptions.ConnectionString, storageOptions.PlayerTableName);
            var matchesTable = new TableClient(storageOptions.ConnectionString, storageOptions.MatchTableName);
            var matchesQueue = new QueueClient(storageOptions.ConnectionString, storageOptions.QueueName);



            logger.LogInformation("Retrieving players ids");
            var players = await playerTable.QueryAsync<Member>().ToListAsync();
            var playersIds = players.Select(d => d.Id);

            PUBGApi pubg = context.InstanceServices.GetRequiredService<PUBGApi>();

            logger.LogInformation("Retrieving players matches");
            var playersResult = await pubg.PlayersByIds(playersIds, tks.Token);
            var matches = playersResult.Data.SelectMany(d => d.Matches.Select(d => d.Id)).Distinct().ToArray();

            logger.LogInformation("Retrieving previously processed matches");
            var processedMatches = await matchesTable.QueryAsync<TableEntity>(cancellationToken: tks.Token).Select(d => d.RowKey).ToListAsync(tks.Token);
            var newMatchesToQueue = matches.Where(d => !processedMatches.Any(f => f == d));
            if (newMatchesToQueue.Any())
            {
                logger.LogInformation($"{newMatchesToQueue.Count()} new matches to queue");

                var queueTasks = newMatchesToQueue.Select(matchToQ => matchesQueue.SendMessageAsync(matchToQ, tks.Token));
                logger.LogInformation("Queue new matches");
                await Task.WhenAll(queueTasks);

                var saveMatchesTask = newMatchesToQueue.Select(d => matchesTable.AddEntityAsync(new TableEntity("default", d)));
                logger.LogInformation("Saving matches on processed list");
                await Task.WhenAll(saveMatchesTask);

                logger.LogInformation("Queueing completed");
            }
            else
                logger.LogInformation("No new matches to queue");
        }

    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
