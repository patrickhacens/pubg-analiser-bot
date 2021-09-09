namespace PUBG.Models.Telemetry
{
    public class GameState
    {
        public int ElapsedTime { get; set; }
        public int NumStartTeams { get; set; }
        public int NumAliveTeams { get; set; }
        public int NumParticipatedTeams { get; set; }
        public int NumJoinPlayers { get; set; }
        public int NumStartPlayers { get; set; }
        public int NumAlivePlayers { get; set; }
        public int NumParticipatedPlayers { get; set; }
        public Location SafetyZonePosition { get; set; }
        public float SafetyZoneRadius { get; set; }
        public Location PoisonGasWarningPosition { get; set; }
        public float PoisonGasWarningRadius { get; set; }
        public Location RedZonePosition { get; set; }
        public int RedZoneRadius { get; set; }
        public Location BlackZonePosition { get; set; }
        public int BlackZoneRadius { get; set; }
    }
}
