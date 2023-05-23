namespace PUBG.Models.Telemetry;

public class ItemDetach : Event
{
    public Character Character { get; set; }

    public Item ParentItem { get; set; }

    public Item ChildItem { get; set; }
}
