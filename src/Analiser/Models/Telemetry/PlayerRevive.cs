namespace PUBG.Models.Telemetry
{
    public class PlayerRevive : Event
    {
        public Character Reviver { get; set; }
        public Character Victim { get; set; }
        public int DBNOId { get; set; }
    }
}
