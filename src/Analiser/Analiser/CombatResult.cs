using System.Collections.ObjectModel;

namespace PUBG.Analiser
{
    public class CombatResult
    {
        public string EnemyId { get; set; }

        public float TotalIncomingDamage { get; set; }

        public float TotalOutgoingDamage { get; set; }

        public string Name { get; set; }

        public ReadOnlyCollection<CombatEvent> Events { get; set; }
    }
}
