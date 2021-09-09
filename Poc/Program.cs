using PUBG;
using PUBG.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Poc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string key = "";

            PUBGApi api = new PUBGApi(new PUBGApiOptions
            {
                ApiKey = key
            });

            var matchData = await api.Match("c53e7830-a4ee-4791-8b67-ba5b05b29d35");

            var telemetryUrl = matchData.Included.OfType<Asset>()
                .First()
                .Attributes
                .Url;
            var telemetry = await api.Telemetry(matchData);
            var analiser = new TelemetryAnaliser(telemetry);


            Discord.Webhook.DiscordWebhookClient client = new Discord.Webhook.DiscordWebhookClient("");
            
        }
    }
}
