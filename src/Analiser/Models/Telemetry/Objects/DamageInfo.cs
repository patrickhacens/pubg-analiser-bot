namespace PUBG.Models.Telemetry
{
    public class DamageInfo
    {
        public DamageReason? DamageReason { get; set; }
        public DamageTypeCategory? DamageTypeCategory { get; set; }
        public string DamageCauserName { get; set; }
        public string[] AdditionalInfo { get; set; }
        public float Distance { get; set; }
        public bool IsThroughPenetrableWall { get; set; }
    }
}
