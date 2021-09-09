using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUBG.Models
{
    public class MatchRoster : PubgObject<MatchRosterAttributes>
    {
        public IEnumerable<PubgObject> Team => Relationships?["team"]?.Data;

        public IEnumerable<PubgObject> Participants => Relationships?["participants"]?.Data;
    }

    public class MatchRosterAttributes
    {
        public bool Won { get; set; }

        public string SharId { get; set; }

        public MatchRosterStats Stats { get; set; }
    }

    public class MatchRosterStats
    {
        public int Rank { get; set; }
        public int TeamId { get; set; }
    }
}
