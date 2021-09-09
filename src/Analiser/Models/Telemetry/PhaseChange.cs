namespace PUBG.Models.Telemetry
{
    public class PhaseChange : Event
    {
        public int Phase { get; set; }
        public int ElapsedTime { get; set; }
    }
}
