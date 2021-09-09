namespace PUBG.Models.Telemetry
{
    public class PlayerUseFlareGun : Event
    {

        public int AttackId { get; set; }
        public int FireWeaponStackCount { get; set; }
        public Character Attacker { get; set; }
        public AttackType? AttackType { get; set; }
        public Item Weapon { get; set; }
    }
}
