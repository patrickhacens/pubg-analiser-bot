namespace PUBG.Models.Telemetry
{
    public class ParachuteLanding : Event
    {
        public Character Character { get; set; }
        public float Distance { get; set; }
    }
}
