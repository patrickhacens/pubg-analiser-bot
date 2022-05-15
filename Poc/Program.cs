using PUBG;
using PUBG.Models;
using PUBG.Models.Telemetry;
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

            var matchData = await api.Match("e934f864-ec7e-46f6-b6d0-a67a7f772a50");

            var telemetryUrl = matchData.Included.OfType<Asset>()
                .First()
                .Attributes
                .Url;
            var telemetry = await api.Telemetry(matchData);
            var types = telemetry
                .Select(d => d.GetType())
                .Distinct()
                .ToArray();

        }
    }
}
