using Azure.Data.Tables;
using Azure.Storage.Queues;
using Discord.Webhook;
using Discord;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PUBG.Models;
using PUBG.Analiser.Functions.Models;

namespace PUBG.Analiser.Functions;

public class ProcessFunction(ILoggerFactory loggerFactory,
	StorageOptions storageOptions,
	PUBGApi pubg,
	IConfiguration configuration)
{
    private readonly ILogger logger = loggerFactory.CreateLogger<ProcessFunction>();
    private readonly StorageOptions storageOptions = storageOptions;
    private readonly PUBGApi pubg = pubg;
    private readonly IConfiguration configuration = configuration;

	[Function("ProcessFunction")]
    public async Task Run(
#if DEBUG
            [TimerTrigger("0 * * * * *")] 
#else
            [TimerTrigger("0 */1 * * * *")] 
#endif
        MyInfo myTimer)
    {
        logger.LogInformation("Process function executed at: {time} next at {nextTime}", DateTime.Now, myTimer?.ScheduleStatus?.Next);

        CancellationTokenSource tks = new();

        var queue = new QueueClient(storageOptions.ConnectionString, storageOptions.QueueName);
        var errorQueue = new QueueClient(storageOptions.ConnectionString, storageOptions.ErrorQueueName);

        var peek = await queue.PeekMessageAsync(tks.Token);
        if (peek.Value == null)
        {
            logger.LogInformation("No messages on queue. sleeping...");
            return;
        }

        var playersTable = new TableClient(storageOptions.ConnectionString, storageOptions.PlayerTableName);

        logger.LogInformation("Acquiring players");
        var players = await playersTable.QueryAsync<Member>(cancellationToken: tks.Token).ToListAsync(cancellationToken: tks.Token);
        var playersIds = players.Select(d => d.Id).ToArray();

        int requests = 0;

        while (requests < 10)
        {
            string matchId = String.Empty;
            logger.LogInformation("Retrieving match to process");
            var message = await queue.ReceiveMessageAsync(TimeSpan.FromMinutes(5), tks.Token);
            try
            {

                if (message.Value == null)
                {
                    logger.LogInformation("No more message on queue");
                    requests = 10;
                }
                else
                {


                    matchId = message.Value.Body.ToString();

                    logger.LogInformation("Retrieving match data {matchId}", matchId);
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
                    logger.LogInformation("Telemetry url {url}", telemetryUrl);

                    logger.LogInformation("Retrieving telemetry");
                    var telemetry = await pubg.Telemetry(matchData, tks.Token);

                    var analiser = new TelemetryAnaliser(telemetry);

                    var teams = rosters.Select(d => analiser.AnaliseTeam(d.Id, d.Participants.Select(d => matchData.Included.OfType<MatchParticipant>().First(p => p.Id == d.Id).Attributes.Stats.PlayerId).ToArray())).ToList();
                    if (teams.Count > 0)
                    {
                        logger.LogInformation("{count} analysed in match, retrieving discord client", teams.Count);

                        var matchLogDiscordClient = new DiscordWebhookClient(configuration.GetValue<string>("MatchLogDiscordWebhook"));
                        var chickenDinnerDiscordClient = new DiscordWebhookClient(configuration.GetValue<string>("ChickenDinnerWebhook"));

                        foreach (var team in teams)
                        {
                            var embedBuilder = DiscordMessage.GetEmbedWithoutFields(team, matchData.Data);
                            var fields = DiscordMessage.GetFields(team).Select(d => (builder: d, field: d.Build()));


                            var fieldqueue = new Queue<(EmbedFieldBuilder builder, EmbedField field)>(fields);

                            static int getLength(EmbedField field) => field.Name.Length + field.Value.Length;

                            List<List<EmbedFieldBuilder>> groups = [];
                            List<EmbedFieldBuilder> group = [];
                            int currentGroupLength = 0;

                            while (fieldqueue.Count > 0)
                            {
                                var fb = fieldqueue.Dequeue();
                                var targetLength = getLength(fb.field);
                                if (currentGroupLength + targetLength >= 5000)
                                {
                                    groups.Add(group);
                                    group = [fb.builder];
                                    currentGroupLength = targetLength;
                                }
                                else
                                {
                                    group.Add(fb.builder);
                                    currentGroupLength += targetLength;
                                }
                            }
                            if (group.Count > 0)
                            {
                                groups.Add(group);
                            }

                            ulong messageId = 0;
                            if (groups.Count > 1)
                            {
                                var embedsBuilders = groups.Select(d => DiscordMessage.GetEmbedWithoutFields(team, matchData.Data).WithFields(d));
                                var embeds = embedsBuilders.Take(1).Concat(embedsBuilders.Skip(1).Select(d => d.WithTitle("continuação"))).Select(d => d.Build());
                                foreach (var embed in embeds)
                                {
                                    var mId = await matchLogDiscordClient.SendMessageAsync(
                                            text: String.Join("; ", team.Members.Select(d => players.FirstOrDefault(f => f.Id == d.Id)?.DiscordId).Where(d => !String.IsNullOrWhiteSpace(d))),
                                            embeds: [embed]);
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
                                    embeds: [embed]);


                            }
                            if (team.Rank == 1)
                            {
                                var winBuilder = DiscordMessage.GetEmbedWithoutFields(team, matchData.Data)
                                    .WithFields(DiscordMessage.GetSummaryFields(team));

                                var winembed = winBuilder
                                    .WithFooter("Clique no link para ver o match log")
                                    .WithUrl($"https://discord.com/channels/1174151137180528682/1174152555383758930/{messageId}")
									.Build();


                                await chickenDinnerDiscordClient.SendMessageAsync(embeds: [winembed]);
                            }

                        }
                    }
                    else
                    {
                        logger.LogInformation("No teams analysed in match");
                    }
                    requests++;
                    logger.LogInformation("Removing message");
                    await queue.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, tks.Token);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error while processing match {matchId} moving to another queue", matchId);
                await errorQueue.CreateIfNotExistsAsync(null, CancellationToken.None);
                await errorQueue.SendMessageAsync(matchId, CancellationToken.None);
                await queue.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, CancellationToken.None);
            }
        }
    }
}
