// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using PUBG;
using PUBG.Models;
using PUBG.Models.Telemetry;
using System;

//const string playerPath = "C:\\Users\\patri\\Downloads\\players.json";
//const string mapPath = "C:\\Users\\patri\\Downloads\\match.json";

//string content = await File.ReadAllTextAsync(mapPath);

JsonSerializerSettings sOptions = new()
{

    Converters =
    {
        new PUBG.Converters.PubgEventConverter(),
        new PUBG.Converters.PubgObjectConverter()
    }
};

//var result = JsonConvert.DeserializeObject<PubgData<IEnumerable<Player>>>(content, sOptions);

//string matchId = "3f004790-d2bd-4b31-a235-9d498d2d5ccc";
PUBGApi pubg = new(new PUBGApiOptions()
{
    ApiKey = "",
    Shard = "steam"
});

var matchId = "e19f9367-50b5-4873-8226-61f15e831332";



var cachePath = $"{Path.GetTempPath()}\\{matchId}.tmp";
var cacheExists = File.Exists(cachePath);
var match = await pubg.Match(matchId);
Stream stream = File.Open(cachePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
if (!cacheExists)
    await pubg.CacheTelemetry(match, stream);

stream.Position = 0;
TextReader tr = new StreamReader(stream);
var str = await tr.ReadToEndAsync();

var result = JsonConvert.DeserializeObject<Telemetry>(str, sOptions);

Thread.Sleep(Timeout.Infinite);
