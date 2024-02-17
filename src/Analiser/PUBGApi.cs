using Newtonsoft.Json;
using PUBG.Models;
using PUBG.Models.Telemetry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PUBG

{
    public class PUBGApi
    {
        private readonly HttpClient client;
        public JsonSerializerSettings SerializerSettings { get; }

        public PUBGApi(PUBGApiOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (options.ApiKey is null)
                throw new ArgumentNullException($"{nameof(options)}.{nameof(options.ApiKey)}");

            SerializerSettings = new JsonSerializerSettings()
            {
                Converters =
                {
                    new Converters.PubgEventConverter(),
                    new Converters.PubgObjectConverter()
                }
            };

            client = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })
            {
                BaseAddress = new Uri($"https://api.pubg.com/shards/{options.Shard}/"),
            };
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.ApiKey);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.api+json"));
        }


        public Task<PubgData<IEnumerable<Player>>> PlayerByName(string name, CancellationToken cancellation = default)
            => PlayersByName(new string[] { name }, cancellation);

        public Task<PubgData<IEnumerable<Player>>> PlayersByName(IEnumerable<string> names, CancellationToken cancellation = default)
            => GetAs<PubgData<IEnumerable<Player>>>($"players?filter[playerNames]={String.Join(",", names)}", cancellation);

        public Task<PubgData<IEnumerable<Player>>> PlayersByIds(IEnumerable<string> ids, CancellationToken cancellation = default)
            => GetAs<PubgData<IEnumerable<Player>>>($"players?filter[playerIds]={String.Join(",", ids)}", cancellation);

        public Task<PubgData<Player>> Player(string id, CancellationToken cancellation = default)
            => GetAs<PubgData<Player>>($"players/{id}", cancellation);

        public Task<PubgData<Match>> Match(string id, CancellationToken cancellation = default)
            => GetAs<PubgData<Match>>($"matches/{id}", cancellation);

        public Task<Telemetry> Telemetry(PubgData<Match> match, CancellationToken cancellation = default)
            => GetAs<Telemetry>(GetTelemetryUrl(match), cancellation);

        private string GetTelemetryUrl(PubgData<Match> match) => match
                .Included
                .OfType<Asset>()
                .First()
                .Attributes
                .Url;


		public async Task CacheTelemetry(PubgData<Match> match, Stream stream, CancellationToken cancellation = default)
        {
            var response = await client.GetAsync(GetTelemetryUrl(match), cancellation);
            using (var readStream = await response.Content.ReadAsStreamAsync())
                await readStream.CopyToAsync(stream);
        }

        private async Task<T> GetAs<T>(string url, CancellationToken cancellation)
        {
            var response = await client.GetAsync(url, cancellation);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new PUBGApiException()
                {
                    StatusCode = response.StatusCode,
                    Content = content
                };

            return JsonConvert.DeserializeObject<T>(content, SerializerSettings);
        }
    }

    public class PUBGApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Content { get; set; }
    }

}
