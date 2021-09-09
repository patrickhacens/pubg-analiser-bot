namespace PUBG.Models.Telemetry
{
    public class ItemPickupFromLootBox : Event
    {
        public Character Character { get; set; }

        public Item Item { get; set; }

        public int OwnerTeamId { get; set; }

        public string CreatorAccountId { get; set; }
    }
}
