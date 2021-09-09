namespace PUBG.Models.Telemetry
{
    public class MatchDefinition : Event
    {
        public string MatchId { get; set; }

        public string PingQuality { get; set; }

        public string SeasonState { get; set; }
    }
}
