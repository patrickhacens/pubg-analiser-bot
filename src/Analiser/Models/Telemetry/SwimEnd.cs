namespace PUBG.Models.Telemetry;

public class SwimEnd : Event
{
    public Character Character { get; set; }
    public float SwimDistance { get; set; }
    public float MaxSwimDepthOfWater { get; set; }
}
