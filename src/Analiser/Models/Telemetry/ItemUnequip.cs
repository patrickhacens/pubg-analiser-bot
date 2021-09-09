namespace PUBG.Models.Telemetry
{
    public class ItemUnequip :Event
    {
        public Character Character { get; set; }

        public Item Item { get; set; }
    }
}
