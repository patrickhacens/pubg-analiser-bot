namespace PUBG.Models.Telemetry
{
    public class PlayerKill : Event
    {
        public int AttackId { get; set; }
        public int DBNOId { get; set; }
        public GameResultData VictimGameResult { get; set; }
        public Character Victim { get; set; }
        public string VictimWeapon { get; set; }
        public string[] VictimWeaponAdditionalInfo { get; set; }
        public Character DBNOMaker { get; set; }
        public DamageInfo DBNODamageInfo { get; set; }
        public Character Finisher { get; set; }
        public DamageInfo FinishDamageInfo { get; set; }
        public Character Killer { get; set; }
        public DamageInfo KillerDamageInfo { get; set; }
        public string[] Assists_AccountId { get; set; }
        public string[] TeamKillers_AccountId { get; set; }
        public bool IsSuicide { get; set; }

        public override string ToString()
        {
            if (IsSuicide)
            {
                return $"{Victim.Name} suicided";
            }
            else if (Killer?.AccountId == Finisher?.AccountId)
            {
                return $"{Killer} killed {Victim} from {KillerDamageInfo.Distance}";
            }
            else
            {
                return $"{Finisher} finished {Victim} for {Killer}";
            }
        }
    }
}
