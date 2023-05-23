namespace PUBG.Models.Telemetry;

public class VehicleRide : Event
{
    public Character Character { get; set; }
    public Vehicle Vehicle { get; set; }
    public int SeatIndex { get; set; }
    public Character[] FellowPassengers { get; set; }
}
