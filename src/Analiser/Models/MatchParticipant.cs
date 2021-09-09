using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUBG.Models
{
    public class MatchParticipant : PubgObject<MatchParticipantAttributes>
    {

    }

    public class MatchParticipantAttributes
    {
        public MatchParticipantStats Stats { get; set; }

        public string Actor { get; set; }

        public string ShardId { get; set; }
    }


    public class MatchParticipantStats
    {
        public int DBNOs { get; set; }
        public int Assists { get; set; }
        public int Boosts { get; set; }
        public float DamageDealt { get; set; }
        public string DeathType { get; set; }
        public int HeadshotKills { get; set; }
        public int Heals { get; set; }
        public int KillPlace { get; set; }
        public int KillStreaks { get; set; }
        public int Kills { get; set; }
        public int LongestKill { get; set; }
        public string Name { get; set; }
        public string PlayerId { get; set; }
        public int Revives { get; set; }
        public float RideDistance { get; set; }
        public int RoadKills { get; set; }
        public int SwimDistance { get; set; }
        public int TeamKills { get; set; }
        public int TimeSurvived { get; set; }
        public int VehicleDestroys { get; set; }
        public float WalkDistance { get; set; }
        public int WeaponsAcquired { get; set; }
        public int WinPlace { get; set; }
    }
}
