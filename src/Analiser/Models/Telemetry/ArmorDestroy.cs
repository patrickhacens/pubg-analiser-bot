namespace PUBG.Models.Telemetry
{
    public class ArmorDestroy : Event
    {
        public int AttackId { get; set; }

        public Character Attacker { get; set; }

        public Character Victim { get; set; }

        public DamageTypeCategory? DamageTypeCategory { get; set; }

        public DamageReason? DamageReason { get; set; }

        public string DamageCauserName { get; set; }

        public Item Item { get; set; }

        public float Distance { get; set; }
    }
}
