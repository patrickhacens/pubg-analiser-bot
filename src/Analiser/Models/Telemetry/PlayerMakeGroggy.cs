namespace PUBG.Models.Telemetry
{
    public class PlayerMakeGroggy : Event
    {
        public int AttackId { get; set; }
        public Character Attacker { get; set; }
        public Character Victim { get; set; }
        public DamageReason? DamageReason { get; set; }
        public DamageTypeCategory? DamageTypeCategory { get; set; }
        public string DamageCauserName { get; set; }
        public string[] DamageCauserAdditionalInfo { get; set; }
        public string VictimWeapon { get; set; }
        public string[] VictimWeaponadditionalInfo { get; set; }
        public float Distance { get; set; }
        public bool IsAttackerInVehicle { get; set; }
        public int DBNOId { get; set; }
        public bool IsThroughPenetrableWall { get; set; }
    }
}
