namespace PUBG.Models.Telemetry
{
    public class CharacterWrapper
    {
        public Character Character { get; set; }
        public string PrimaryWeaponFirst { get; set; }
        public string PrimaryWeaponSecond { get; set; }
        public string SecondaryWeapon { get; set; }
        public int SpawnKitIndex { get; set; }
    }
}
