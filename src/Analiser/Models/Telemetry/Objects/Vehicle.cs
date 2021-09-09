namespace PUBG.Models.Telemetry
{
    public class Vehicle
    {
        public VehicleType? VehicleType { get; set; }
        public string VehicleId { get; set; }
        public int SeatIndex { get; set; }
        public float HealthPercent { get; set; }
        public float FeulPercent { get; set; }
        public int AltitudeAbs { get; set; }
        public int AltitudeRel { get; set; }
        public float Velocity { get; set; }
        public bool IsWheelsInAir { get; set; }
        public bool IsInWaterVolume { get; set; }
        public bool IsEngineOn { get; set; }
    }
}
