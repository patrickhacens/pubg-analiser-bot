using System.Collections.ObjectModel;
using System.Linq;

namespace PUBG.Analiser
{
    public class MatchResult
    {
        public string Id { get; set; }

        public string MapName { get; set; }

        public bool Win => Teams.Any(d => d.Rank == 1);

        public ReadOnlyCollection<TeamResult> Teams { get; set; }
    }
}
