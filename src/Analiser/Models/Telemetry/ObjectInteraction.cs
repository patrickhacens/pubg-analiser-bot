namespace PUBG.Models.Telemetry
{
    public class ObjectInteraction : Event
    {
        public Character Character { get; set; }
        public ObjectType? ObjectType { get; set; }
        public ObjectTypeStatus? ObjectTypeStatus { get; set; }
        public object ObjectTypeAdditionalInfo { get; set; }
    }
}
