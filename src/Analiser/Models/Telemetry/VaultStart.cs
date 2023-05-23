namespace PUBG.Models.Telemetry;

public class VaultStart : Event
{
    public Character Character { get; set; }
    public bool IsLedgeGrab { get; set; }
}
