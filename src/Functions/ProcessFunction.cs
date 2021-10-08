using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Discord;
using Discord.Webhook;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PUBG.Analiser.Functions.Model;
using PUBG.Models;

namespace PUBG.Analiser.Functions
{
    public static class ProcessFunction
    {
        [Function("ProcessFunction")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var log = context.GetLogger("ProcessFunction");
            log.LogInformation($"Process function executed at: {DateTime.Now}");
            log.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");

            CancellationTokenSource tks = new CancellationTokenSource();

            var storageOptions = context.InstanceServices.GetRequiredService<StorageOptions>();
            var queue = new QueueClient(storageOptions.ConnectionString, storageOptions.QueueName);

            var peek = await queue.PeekMessageAsync(tks.Token);
            if (peek.Value == null)
            {
                log.LogInformation("No messages on queue. sleeping...");
                return;
            }

            var playersTable = new TableClient(storageOptions.ConnectionString, storageOptions.PlayerTableName);

            var pubg = context.InstanceServices.GetRequiredService<PUBGApi>();

            log.LogInformation("Acquiring players");
            var players = await playersTable.QueryAsync<Member>(cancellationToken: tks.Token).ToListAsync(cancellationToken: tks.Token);
            var playersIds = players.Select(d => d.Id).ToArray();

            int requests = 0;

            while (requests < 10)
            {
                string matchId = String.Empty;
                try
                {

                    log.LogInformation("Retrieving match to process");
                    var message = await queue.ReceiveMessageAsync(TimeSpan.FromMinutes(5), tks.Token);
                    if (message.Value == null)
                    {
                        log.LogInformation("No more message on queue");
                        requests = 10;
                    }
                    else
                    {


                        matchId = message.Value.Body.ToString();

                        log.LogInformation($"Retrieving match data {matchId}");
                        var matchData = await pubg.Match(matchId, tks.Token);

                        var playerRoostersId = matchData.Included
                            .OfType<MatchParticipant>()
                            .Where(d => playersIds.Contains(d.Attributes.Stats.PlayerId))
                            .Select(d => d.Id);

                        var rosters = matchData.Included
                                .OfType<MatchRoster>()
                                .Where(d => d.Participants.Any(p => playerRoostersId.Contains(p.Id)));

                        var telemetryUrl = matchData.Included.OfType<Asset>()
                            .First()
                            .Attributes
                            .Url;
                        log.LogInformation("Telemetry url {url}", telemetryUrl);

                        log.LogInformation("Retrieving telemetry");
                        var telemetry = await pubg.Telemetry(matchData, tks.Token);

                        var analiser = new TelemetryAnaliser(telemetry);

                        var teams = rosters.Select(d => analiser.AnaliseTeam(d.Id, d.Participants.Select(d => matchData.Included.OfType<MatchParticipant>().First(p => p.Id == d.Id).Attributes.Stats.PlayerId).ToArray())).ToList();
                        if (teams.Any())
                        {
                            log.LogInformation($"{teams.Count()} analised in match, retrieving discord client");
                            var configuration = context.InstanceServices.GetRequiredService<IConfiguration>();
                            var matchLogDiscordClient = new DiscordWebhookClient(configuration.GetValue<string>("MatchLogDiscordWebhook"));
                            var chickenDinnerDiscordClient = new DiscordWebhookClient(configuration.GetValue<string>("ChickenDinnerWebhook"));

                            foreach (var team in teams)
                            {
                                var embedBuilder = DiscordMessage.GetEmbedWithoutFields(team, matchData.Data);
                                var fields = DiscordMessage.GetFields(team).Select(d => (builder: d, field: d.Build()));


                                var fieldqueue = new Queue<(EmbedFieldBuilder builder, EmbedField field)>(fields);

                                int getLength(EmbedField field) => field.Name.Length + field.Value.Length;

                                List<List<EmbedFieldBuilder>> groups = new List<List<EmbedFieldBuilder>>();
                                List<EmbedFieldBuilder> group = new List<EmbedFieldBuilder>();
                                int currentGroupLength = 0;

                                while (fieldqueue.Count > 0)
                                {
                                    var fb = fieldqueue.Dequeue();
                                    var targetLength = getLength(fb.field);
                                    if (currentGroupLength + targetLength >= 5000)
                                    {
                                        groups.Add(group);
                                        group = new List<EmbedFieldBuilder>() { fb.builder };
                                        currentGroupLength = targetLength;
                                    }
                                    else
                                    {
                                        group.Add(fb.builder);
                                        currentGroupLength += targetLength;
                                    }
                                }
                                if (group.Any())
                                {
                                    groups.Add(group);
                                }

                                ulong messageId = 0;
                                if (groups.Count > 1)
                                {
                                    var embedsBuilders = groups.Select(d => DiscordMessage.GetEmbedWithoutFields(team, matchData.Data).WithFields(d));
                                    var embeds = embedsBuilders.Take(1).Concat(embedsBuilders.Skip(1).Select(d => d.WithTitle("continuação"))).Select(d=>d.Build());
                                    foreach (var embed in embeds)
                                    {
                                        var mId = await matchLogDiscordClient.SendMessageAsync(
                                                text: String.Join("; ", team.Members.Select(d => players.FirstOrDefault(f => f.Id == d.Id)?.DiscordId).Where(d => !String.IsNullOrWhiteSpace(d))),
                                                embeds: new Embed[] { embed });
                                        if (messageId == 0)
                                            messageId = mId;
                                    }
                                }
                                else
                                {
                                    var embed = DiscordMessage.GetEmbedWithoutFields(team, matchData.Data)
                                        .WithFields(group)
                                        .Build();
                                    messageId = await matchLogDiscordClient.SendMessageAsync(
                                        text: String.Join("; ", team.Members.Select(d => players.FirstOrDefault(f => f.Id == d.Id)?.DiscordId).Where(d => !String.IsNullOrWhiteSpace(d))),
                                        embeds: new Embed[] { embed });


                                }
                                if (team.Rank == 1)
                                {
                                    var winBuilder = DiscordMessage.GetEmbedWithoutFields(team, matchData.Data)
                                        .WithFields(DiscordMessage.GetSummaryFields(team));

                                    var winembed = winBuilder
                                        .WithFooter("Clique no link para ver o match log")
                                        .WithUrl($"https://discord.com/channels/468173278952030209/882740719624814602/{messageId}")
                                        .Build();

                                        
                                    await chickenDinnerDiscordClient.SendMessageAsync(embeds: new Embed[] { winembed });
                                }

                            }
                        }
                        else
                        {
                            log.LogInformation("No teams analised in match");
                        }
                        requests++;
                        log.LogInformation("Removing message");
                        await queue.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, tks.Token);
                    }
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "error while processing match {matchId}", matchId);
                }
            }

        }
    }
}
