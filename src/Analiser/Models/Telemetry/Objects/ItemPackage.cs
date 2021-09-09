namespace PUBG.Models.Telemetry
{
    public class ItemPackage
    {
        public string ItemPackageId { get; set; }
        public Location Location { get; set; }
        public Item[] Items { get; set; }
    }
}
