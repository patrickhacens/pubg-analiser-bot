namespace PUBG.Models.Telemetry
{
    public class PlayerKill_TournamentMatch : Event
    {
        public int AttackId { get; set; }
        public Character Killer { get; set; }
        public Character Victim { get; set; }
        public Character Assistant { get; set; }
        public int DBNOId { get; set; }
        public DamageReason? DamageReason { get; set; }
        public DamageTypeCategory? DamageTypeCategory { get; set; }
        public string DamageCauserName { get; set; }
        public string[] DamageCauserAdditionalInfo { get; set; }
        public string VictimWeapon { get; set; }
        public string[] VictimWeaponAdditionalInfo { get; set; }
        public float Distance { get; set; }
        public GameResultData VictimGameResult { get; set; }
        public bool IsThroughPenetrableWall { get; set; }
    }
}
