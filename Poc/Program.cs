// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using PUBG;
using PUBG.Models;
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


var match = await pubg.Match("628eea0c-e6d5-4508-9780-7ae7c76dc9af");



////var match = await pubg.Match(matchId);

//var result = JsonConvert.DeserializeObject<PubgData<Match>>(content, sOptions);

Thread.Sleep(Timeout.Infinite);
