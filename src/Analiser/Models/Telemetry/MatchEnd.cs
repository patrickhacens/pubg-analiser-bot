namespace PUBG.Models.Telemetry
{
    public class MatchEnd : Event
    {
        public CharacterWrapper[] Characters { get; set; }
        public GameResultOnFinished GameResultOnFinished { get; set; }
    }
}
