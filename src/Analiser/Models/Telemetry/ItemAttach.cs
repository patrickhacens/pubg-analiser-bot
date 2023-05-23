namespace PUBG.Models.Telemetry;

public class ItemAttach : Event
{
    public Character Character { get; set; }

    public Item ParentItem { get; set; }

    public Item ChildItem { get; set; }
}
