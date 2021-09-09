namespace PUBG.Models.Telemetry
{
    public class Item
    {
        public string ItemId { get; set; }
        public int StackCount { get; set; }
        public ItemCategory? Category { get; set; }
        public ItemSubCategory? SubCategory { get; set; }
        public string[] AttachedItems { get; set; }
    }
}
