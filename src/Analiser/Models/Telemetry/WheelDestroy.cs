namespace PUBG.Models.Telemetry
{
    public class WheelDestroy : Event
    {
        public int AttackId { get; set; }
        public Character Attacker { get; set; }
        public Vehicle Vehicle { get; set; }
        public DamageTypeCategory? DamageTypeCategory { get; set; }
        public string DamageCauserName { get; set; }
    }
}
