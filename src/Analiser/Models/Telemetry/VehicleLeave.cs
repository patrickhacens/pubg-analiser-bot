namespace PUBG.Models.Telemetry
{
    public class VehicleLeave : Event
    {
        public Character Character { get; set; }
        public Vehicle Vehicle { get; set; }
        public float RideDistance { get; set; }
        public int SeatIndex { get; set; }
        public float MaxSpeed { get; set; }
        public Character[] FellowPassengers { get; set; }
    }
}
