namespace PUBG.Models.Telemetry;

public class VehicleDamage : Event
{
    public int AttackId { get; set; }
    public Character Attacker { get; set; }
    public Vehicle Vehicle { get; set; }
    public DamageTypeCategory? DamageTypeCategory { get; set; }
    public string DamageCauserName { get; set; }
    public float Damage { get; set; }
    public float Distance { get; set; }
}
