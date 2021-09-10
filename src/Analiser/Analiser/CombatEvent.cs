using PUBG.Models.Telemetry;
using System;

namespace PUBG.Analiser
{
    public class CombatEvent
    {
        public CombatType Type { get; set; }

        public CombatDirection Direction { get; set; }

        public DateTime When { get; set; }

        public string Weapon { get; set; }

        public DamageReason? DamageReason { get; set; }

        public DamageTypeCategory? DamageType { get; set; }
    }
}
