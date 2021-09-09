using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PUBG.Analiser
{
    public class TeamResult
    {
        public string Id { get; set; }

        public string MapName { get; set; }

        public int TeamSize { get; set; }

        public int Rank { get; set; }

        public float OutgoingDamage { get; set; }

        public float IncomingDamage { get; set; }

        public float Kills { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public ReadOnlyCollection<CharacterResult> Members { get; set; }
    }
}
