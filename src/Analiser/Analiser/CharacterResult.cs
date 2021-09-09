using System;
using System.Collections.ObjectModel;

namespace PUBG.Analiser
{
    public class CharacterResult
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public float OutgoingDamage { get; set; }

        public float IncomingDamage { get; set; }

        public float Kills { get; set; }

        public bool Survived { get; set; }

        public TimeSpan SurvivalTime { get; set; }

        public ReadOnlyCollection<CombatResult> Combats { get; set; }

    }
}
