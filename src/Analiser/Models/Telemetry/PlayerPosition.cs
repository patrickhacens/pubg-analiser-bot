namespace PUBG.Models.Telemetry
{
    public class PlayerPosition : Event
    {
        public Character Character { get; set; }
        public Vehicle Vehicle { get; set; }
        public float ElapsedTime { get; set; }
        public int NumAlivePlayers { get; set; }
    }
}
