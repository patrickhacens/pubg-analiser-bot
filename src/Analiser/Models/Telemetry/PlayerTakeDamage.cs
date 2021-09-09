namespace PUBG.Models.Telemetry
{
    public class PlayerTakeDamage : Event
    {
        public int AttackId { get; set; }
        public Character Attacker { get; set; }
        public Character Victim { get; set; }
        public DamageTypeCategory? DamageTypeCategory { get; set; }
        public DamageReason? DamageReason { get; set; }
        /// <summary>
        /// 1.0 damage = 1.0 health
        /// Net damage after armor; damage to health
        /// </summary>
        public float Damage { get; set; }
        public string DamageCauserName { get; set; }
        public bool IsThroughPenetrableWall { get; set; }

        public override string ToString()
        {
            return $"{Attacker} damages {Victim} with {Damage}";
        }
    }
}
