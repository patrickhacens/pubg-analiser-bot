namespace PUBG.Models.Telemetry
{
    public class ItemPickup : Event
    {
            public Character Character { get; set; }
            public Item Item { get; set; }
    }
}
