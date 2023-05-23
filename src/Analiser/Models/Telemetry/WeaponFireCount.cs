namespace PUBG.Models.Telemetry;

public class WeaponFireCount : Event
{
    public Character Character { get; set; }
    public string WeaponId { get; set; }
    public int FireCount { get; set; }
}
