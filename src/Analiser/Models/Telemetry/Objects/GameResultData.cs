namespace PUBG.Models.Telemetry
{
    public class GameResultData
    {
        public int Rank { get; set; }
        public string GameResult { get; set; }
        public int TeamId { get; set; }
        public Stats Stats { get; set; }
        public string AccountId { get; set; }
    }
}
