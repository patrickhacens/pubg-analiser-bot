namespace PUBG.Models.Telemetry
{
    public class Character
    {
        public string Name { get; set; }
        public int TeamId { get; set; }
        public float Health { get; set; }
        public Location Location { get; set; }
        public int Ranking { get; set; }
        public string AccountId { get; set; }
        public bool IsInBlueZone { get; set; }
        public bool IsInRedZone { get; set; }
        public string[] Zone { get; set; }
        public override string ToString() => $"{Name}";
    }
}
