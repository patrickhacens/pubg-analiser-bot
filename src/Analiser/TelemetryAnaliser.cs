using PUBG.Analiser;
using PUBG.Models.Telemetry;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PUBG
{
    public class TelemetryAnaliser
    {
        public MatchStart MatchStart { get; }

        public ReadOnlyDictionary<string, Character> Players { get; }

        public ReadOnlyDictionary<string, Character> Winners { get; }

        public ReadOnlyCollection<PlayerTakeDamage> Damages { get; }

        public ReadOnlyCollection<PlayerTakeDamage> Dbnos { get; }

        public ReadOnlyCollection<PlayerKill> Kills { get; }

        public MatchEnd MatchEnd { get; }

        public TelemetryAnaliser(Telemetry telemetry)
        {
            List<Character> characters = new List<Character>();
            List<PlayerKill> kills = new List<PlayerKill>();
            List<PlayerTakeDamage> damages = new List<PlayerTakeDamage>();
            List<PlayerTakeDamage> dbnos = new List<PlayerTakeDamage>();

            foreach (var ev in telemetry)
            {
                if (ev is MatchStart matchStart)
                {
                    MatchStart = matchStart;
                    if (Players is null)
                    {
                        Players = new ReadOnlyDictionary<string, Character>(matchStart
                            .Characters
                            .GroupBy(d => d.Character.AccountId)
                            .ToDictionary(d => d.Key, d => d.First().Character));
                    }
                }
                else if (ev is MatchEnd matchEnd)
                {
                    MatchEnd = matchEnd;
                    if (Winners is null)
                    {
                        //if (Players is null)
                        //{
                        Players = new ReadOnlyDictionary<string, Character>(matchEnd
                            .Characters
                            .GroupBy(d => d.Character.AccountId)
                            .ToDictionary(d => d.Key, d => d.First().Character));
                        //}

                        Winners = new ReadOnlyDictionary<string, Character>(matchEnd
                            .GameResultOnFinished
                            .Results
                            .Select(d => Players[d.AccountId])
                            .GroupBy(d => d.AccountId)
                            .ToDictionary(d => d.Key, d => d.First()));
                    }
                }
                else if (ev is PlayerTakeDamage dmg)
                {
                    damages.Add(dmg);
                    if (dmg.Damage >= dmg.Victim.Health && dmg.Damage > 0)
                        dbnos.Add(dmg);
                }
                else if (ev is PlayerKill kill)
                {
                    kills.Add(kill);
                }
            }

            Damages = new ReadOnlyCollection<PlayerTakeDamage>(damages);
            Kills = new ReadOnlyCollection<PlayerKill>(kills);
            Dbnos = new ReadOnlyCollection<PlayerTakeDamage>(dbnos);
        }

        public CharacterResult AnalisePlayer(string playerId)
        {
            if (!Players.ContainsKey(playerId))
                throw new Exception("Player not in match");

            var playerDamages = Damages.Where(d => d.Attacker?.AccountId == playerId || d.Victim?.AccountId == playerId).ToArray();
            var playerDbnos = Dbnos.Where(d => d.Attacker?.AccountId == playerId || d.Victim?.AccountId == playerId).ToArray();
            var playerKills = Kills.Where(d => d.Killer?.AccountId == playerId || d.Victim?.AccountId == playerId).ToArray();
            var deaths = playerKills.Where(d => d.Victim?.AccountId == playerId).ToArray();

            var enemyIds = GetEnemies(playerKills, playerDamages, playerId);
            CharacterResult result = default;
            try
            {

                result = new CharacterResult()
                {
                    Id = playerId,
                    IncomingDamage = playerDamages.Where(d => d.Victim?.AccountId == playerId).Sum(d => d.Damage),
                    OutgoingDamage = playerDamages.Where(d => d.Attacker?.AccountId == playerId).Sum(d => d.Damage),
                    Kills = playerKills.Where(d => d.Killer?.AccountId == playerId).Count(),
                    Name = Players[playerId].Name,
                    Survived = deaths.Any(),
                    SurvivalTime = deaths.Any() ? deaths.Last()._D - MatchStart._D : MatchEnd._D - MatchStart._D,
                    Combats = new ReadOnlyCollection<CombatResult>(enemyIds
                       .Select(enemyId => new CombatResult
                       {
                           EnemyId = enemyId,
                           Name = Players[enemyId].Name,
                           TotalIncomingDamage = playerDamages
                               .Where(f => f.Attacker?.AccountId == enemyId)
                               .Where(f => f.Victim?.AccountId == playerId)
                               .Sum(f => f.Damage),
                           TotalOutgoingDamage = playerDamages
                               .Where(f => f.Attacker?.AccountId == playerId)
                               .Where(f => f.Victim?.AccountId == enemyId)
                               .Sum(f => f.Damage),
                           Events = new ReadOnlyCollection<CombatEvent>(GetEventsBetween(playerDbnos, playerKills, playerId, enemyId).ToList())
                       }).ToList())
                };
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public TeamResult AnaliseTeam(string id, params string[] playersIds)
        {
            var result = new TeamResult
            {
                Id = id,
                MapName = MatchStart.MapName,
                TeamSize = MatchStart.TeamSize,
                Start = MatchStart._D,
                End = MatchEnd._D,

                Members = new ReadOnlyCollection<CharacterResult>(playersIds.Select(d => AnalisePlayer(d)).ToList())
            };

            result.Rank = result.Members.Select(m => Players[m.Id].Ranking).Min();
            result.IncomingDamage = result.Members.Sum(d => d.IncomingDamage);
            result.OutgoingDamage = result.Members.Sum(d => d.OutgoingDamage);
            result.Kills = result.Members.Sum(d => d.Kills);

            return result;
        }

        private string[] GetEnemies(IEnumerable<PlayerKill> kills, IEnumerable<PlayerTakeDamage> damages, string playerId)
            => damages.Select(d => new string[] { d.Victim?.AccountId, d.Attacker?.AccountId })
                .Concat(kills.Select(d => new string[] { d.Victim?.AccountId, d.Killer?.AccountId }))
                .SelectMany(d => d)
                .Where(d => d != playerId)
                .Where(d => d != null)
                .Distinct()
                .ToArray();

        private static IEnumerable<CombatEvent> GetEventsBetween(IEnumerable<PlayerTakeDamage> dbos, IEnumerable<PlayerKill> kills, string playerId, string oponnentId)
        {

            var dbosBetween = dbos
                .Where(d => d.Attacker?.AccountId == oponnentId || d.Victim?.AccountId == oponnentId)
                .Select(d => new CombatEvent
                {
                    Type = d.Attacker.AccountId == playerId ? CombatType.OutgoingDbno : CombatType.IncomingDbno,
                    When = d._D,
                    Weapon = d.DamageCauserName,
                    DamageReason = d.DamageReason,
                    DamageType = d.DamageTypeCategory
                });

            var kill = kills
                .Where(d => d.Killer?.AccountId == oponnentId || d.Victim?.AccountId == oponnentId)
                .Select(d => new CombatEvent
                {
                    When = d._D,
                    Type = d.Killer.AccountId == playerId ? CombatType.OutgoingKill : CombatType.IncomingKill,
                    DamageType = d.KillerDamageInfo.DamageTypeCategory,
                    DamageReason = d.KillerDamageInfo.DamageReason,
                    Weapon = d.KillerDamageInfo.DamageCauserName
                });
            return dbosBetween.Concat(kill).OrderBy(d => d.When);
        }
    }
}
