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
    ApiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiJiNmViNTg2MC1lZjMyLTAxMzktNTliZS0wNjcxMjU5ZjdmMjgiLCJpc3MiOiJnYW1lbG9ja2VyIiwiaWF0IjoxNjMwNzA3NDc2LCJwdWIiOiJibHVlaG9sZSIsInRpdGxlIjoicHViZyIsImFwcCI6IjEwMHNfYm90In0.lI3xmD2QrznuCxRCT-djKNj8wYlUSj7-RanLouHiGfQ",
    Shard = "steam"
});

var matchId = "23a1d4fa-3551-410a-9a65-c03515b57194";


var cachePath = "C:\\Users\\patri\\AppData\\Local\\Temp\\tmp1a.tmp";
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
