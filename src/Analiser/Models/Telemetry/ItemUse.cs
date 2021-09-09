namespace PUBG.Models.Telemetry
{
    public class ItemUse : Event
    {
        public Character Character { get; set; }

        public Item Item { get; set; }
    }
}
