namespace PUBG.Models.Telemetry
{
    public class MatchStart : Event
    {
        public string MapName { get; set; }

        public string WeatherId { get; set; }

        public CharacterWrapper[] Characters { get; set; }

        public string CameraViewBehaviour { get; set; }

        public int TeamSize { get; set; }

        public bool IsCustomGame { get; set; }

        public bool IsEventMode { get; set; }

        public string BlueZoneCustomOptions { get; set; }
    }
}
