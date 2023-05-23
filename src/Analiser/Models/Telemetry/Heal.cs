namespace PUBG.Models.Telemetry;

public class Heal : Event
{
    public Character Character { get; set; }

    public Item Item { get; set; }

    public float HealAmount { get; set; }
}
