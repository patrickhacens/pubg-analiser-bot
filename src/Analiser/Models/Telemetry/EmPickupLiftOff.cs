namespace PUBG.Models.Telemetry;

public class EmPickupLiftOff : Event
{
    public Character Instigator { get; set; }

    public Character[] Riders { get; set; }
}
