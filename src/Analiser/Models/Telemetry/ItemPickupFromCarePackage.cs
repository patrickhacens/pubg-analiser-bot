namespace PUBG.Models.Telemetry
{
    public class ItemPickupFromCarePackage : Event
    {
        public Character Character { get; set; }

        public Item Item { get; set; }

        public float CarePackageUniqueId { get; set; }
    }
}
