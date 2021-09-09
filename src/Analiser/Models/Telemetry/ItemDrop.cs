namespace PUBG.Models.Telemetry
{
    public class ItemDrop : Event
    {
        public Character Character { get; set; }

        public Item Item { get; set; }
    }
}
