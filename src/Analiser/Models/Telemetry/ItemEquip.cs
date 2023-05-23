namespace PUBG.Models.Telemetry;

public class ItemEquip : Event
{
    public Character Character { get; set; }
    public Item Item { get; set; }
}
