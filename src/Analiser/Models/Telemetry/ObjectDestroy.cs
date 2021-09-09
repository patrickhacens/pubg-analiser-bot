namespace PUBG.Models.Telemetry
{
    public class ObjectDestroy : Event
    {
        public Character Character { get; set; }
        public ObjectType? ObjectType { get; set; }
        public Location ObjectLocation { get; set; }
    }
}
