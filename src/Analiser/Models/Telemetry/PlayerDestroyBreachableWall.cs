namespace PUBG.Models.Telemetry
{
    public class PlayerDestroyBreachableWall : Event
    {
        public Character Attacker { get; set; }
        public Item Weapon { get; set; }
    }
}
