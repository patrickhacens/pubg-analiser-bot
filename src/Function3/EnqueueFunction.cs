using System;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PUBG;
using PUBG.Analiser.Functions.Models;

namespace PUBG.Analiser.Functions
{
    public class EnqueueFunction
    {
        private readonly ILogger logger;
        private readonly StorageOptions storageOptions;
        private readonly PUBGApi pubg;

        public EnqueueFunction(ILoggerFactory loggerFactory, 
            StorageOptions storageOptions,
            PUBGApi pubg)
        {
            logger = loggerFactory.CreateLogger<EnqueueFunction>();
            this.storageOptions=storageOptions;
            this.pubg=pubg;
        }

        [Function("EnqueueFunction")]
        public async Task Run(
#if DEBUG
            [TimerTrigger("0 * * * * *")] 
#else
            [TimerTrigger("0 */5 * * * *")] 
#endif
            MyInfo myTimer)
        {
            logger.LogInformation("Enqueue function executed at: {time}", DateTime.Now);
            logger.LogInformation("Next timer schedule at: {time}", myTimer.ScheduleStatus.Next);

            CancellationTokenSource tks = new();

            var playerTable = new TableClient(storageOptions.ConnectionString, storageOptions.PlayerTableName);
            var matchesTable = new TableClient(storageOptions.ConnectionString, storageOptions.MatchTableName);
            var matchesQueue = new QueueClient(storageOptions.ConnectionString, storageOptions.QueueName);



            logger.LogInformation("Retrieving players ids");
            var players = await playerTable.QueryAsync<Member>().ToListAsync();
            var playersIds = players.Select(d => d.Id);


            logger.LogInformation("Retrieving players matches");
            var playersResult = await pubg.PlayersByIds(playersIds, tks.Token);
            var matches = playersResult.Data.SelectMany(d => d.Matches.Select(d => d.Id)).Distinct().ToArray();

            logger.LogInformation("Retrieving previously processed matches");
            var processedMatches = await matchesTable.QueryAsync<TableEntity>(cancellationToken: tks.Token).Select(d => d.RowKey).ToListAsync(tks.Token);
            var newMatchesToQueue = matches.Where(d => !processedMatches.Any(f => f == d));
            if (newMatchesToQueue.Any())
            {
                logger.LogInformation("{count} new matches to queue", newMatchesToQueue.Count());

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
}
