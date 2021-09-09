namespace PUBG.Models.Telemetry
{
    public class PlayerDestroyProp : Event
    {
        public Character Attacker { get; set; }
        public ObjectType? ObjectType { get; set; }
        public Location ObjectLocation { get; set; }
    }
}
