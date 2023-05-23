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

public class ProcessFunction
{
    private readonly ILogger logger;
    private readonly StorageOptions storageOptions;
    private readonly PUBGApi pubg;
    private readonly IConfiguration configuration;

    public ProcessFunction(ILoggerFactory loggerFactory,
        StorageOptions storageOptions,
        PUBGApi pubg,
        IConfiguration configuration
        )
    {
        logger = loggerFactory.CreateLogger<ProcessFunction>();
        this.storageOptions=storageOptions;
        this.pubg=pubg;
        this.configuration=configuration;
    }

    [Function("ProcessFunction")]
    public async Task Run(
#if DEBUG
            [TimerTrigger("0 * * * * *")] 
#else
            [TimerTrigger("0 */1 * * * *")] 
#endif
        MyInfo myTimer)
    {
        logger.LogInformation("Process function executed at: {time}", DateTime.Now);
        logger.LogInformation("Next timer schedule at: {time}", myTimer?.ScheduleStatus?.Next);

        CancellationTokenSource tks = new();

        var queue = new QueueClient(storageOptions.ConnectionString, storageOptions.QueueName);

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
            try
            {

                logger.LogInformation("Retrieving match to process");
                var message = await queue.ReceiveMessageAsync(TimeSpan.FromMinutes(5), tks.Token);
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
                    if (teams.Any())
                    {
                        logger.LogInformation("{count} analised in match, retrieving discord client", teams.Count);

                        var matchLogDiscordClient = new DiscordWebhookClient(configuration.GetValue<string>("MatchLogDiscordWebhook"));
                        var chickenDinnerDiscordClient = new DiscordWebhookClient(configuration.GetValue<string>("ChickenDinnerWebhook"));

                        foreach (var team in teams)
                        {
                            var embedBuilder = DiscordMessage.GetEmbedWithoutFields(team, matchData.Data);
                            var fields = DiscordMessage.GetFields(team).Select(d => (builder: d, field: d.Build()));


                            var fieldqueue = new Queue<(EmbedFieldBuilder builder, EmbedField field)>(fields);

                            static int getLength(EmbedField field) => field.Name.Length + field.Value.Length;

                            List<List<EmbedFieldBuilder>> groups = new();
                            List<EmbedFieldBuilder> group = new();
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
                                var embeds = embedsBuilders.Take(1).Concat(embedsBuilders.Skip(1).Select(d => d.WithTitle("continuação"))).Select(d => d.Build());
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
                        logger.LogInformation("No teams analised in match");
                    }
                    requests++;
                    logger.LogInformation("Removing message");
                    await queue.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, tks.Token);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error while processing match {matchId}", matchId);
            }
        }
    }
}
