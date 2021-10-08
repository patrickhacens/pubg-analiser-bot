using Discord;
using PUBG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace PUBG.Analiser
{
    public static class DiscordMessage
    {
        public static EmbedBuilder GetEmbedWithoutFields(TeamResult teamResult, Match match)
        => new EmbedBuilder()
            .WithAuthor(GetEmbedAuthor(teamResult, match))
            .WithTitle($"Rank: {teamResult.Rank} com {teamResult.OutgoingDamage:N0} de dano e {teamResult.Kills:N0} kills")
            .WithThumbnailUrl($"https://raw.githubusercontent.com/pubg/api-assets/master/Assets/MapSelection/{MapTranslation[teamResult.MapName]}.{MapExtension[teamResult.MapName]}")
            .WithUrl($"https://pubglookup.com/players/{match.Attributes.ShardId}/{teamResult.Members.FirstOrDefault()?.Name}/matches/{match.Id}")
            .WithFooter(GetEmbedFooter(teamResult));
            

        public static EmbedAuthorBuilder GetEmbedAuthor(TeamResult teamResult, Match match)
            => new EmbedAuthorBuilder()
            //.WithIconUrl($"https://raw.githubusercontent.com/pubg/api-assets/master/Assets/MapSelection/{MapTranslation[teamResult.MapName]}.{MapExtension[teamResult.MapName]}")
            .WithName($"{MapTranslation[teamResult.MapName]} - {TeamSizeTranslation(teamResult.TeamSize)}")
            .WithUrl($"https://pubglookup.com/players/{match.Attributes.ShardId}/{teamResult.Members.FirstOrDefault()?.Name}/matches/{match.Id}");

        public static EmbedFooterBuilder GetEmbedFooter(TeamResult teamResult)
            => new EmbedFooterBuilder()
                //.WithIconUrl("https://cdn.discordapp.com/icons/468173278952030209/ddf0d1f233b1f26dde17090a0224153f.png")
                .WithText($"{teamResult.Start:g} as {teamResult.End:g} - {Assembly.GetExecutingAssembly().GetName().Version}");

        public static EmbedFieldBuilder GetSummaryField(CharacterResult result)
        => new EmbedFieldBuilder()
            .WithName(result.Name)
            .WithValue($"{result.OutgoingDamage:N0}/{result.IncomingDamage:N0}/{result.Kills:N0}")
            .WithIsInline(true);

        public static IEnumerable<EmbedFieldBuilder> GetSummaryFields(TeamResult teamResult)
        {
            foreach (var characterResult in teamResult.Members)
            {
                yield return GetSummaryField(characterResult);
            }
        }

        public static IEnumerable<EmbedFieldBuilder> GetFields(TeamResult teamResult)
        {
            foreach (var characterResult in teamResult.Members)
            {
                yield return GetSummaryField(characterResult);
            }

            var evs = teamResult.Members
                .SelectMany(m => m.Combats
                    .SelectMany(c => c.Events
                        .Select(ev => (
                            Member: m.Name,
                            Action: CombatText(ev.Type, ev.Direction),
                            Enemy: c.Name,
                            When: ev.When,
                            combat: c,
                            _event: ev,
                            Weapon: WeaponNames[ev.Weapon]))))
                .OrderBy(d => d.When);

            List<string> texts = new List<string>();

            StringBuilder sb = new StringBuilder();
            foreach (var ev in evs)
            {
                bool isLastEventOfCombat = ev._event == ev.combat.Events.Last();
                var line = $"**{ev.Member}** {ev.Action} **{ev.Enemy}** com {ev.Weapon}";
                if (isLastEventOfCombat)
                    line += $"\n**{ev.Member}**: {ev.combat.TotalOutgoingDamage:N0} x {ev.combat.TotalIncomingDamage:N0} :**{ev.Enemy}**";
                if (sb.Length + line.Length > 900)
                {
                    texts.Add(sb.ToString());
                    sb.Clear();
                }
                sb.AppendLine(line);
            }
            if (sb.Length > 0 )
            {
                texts.Add(sb.ToString());
                sb.Clear();
            }
            for (int i = 0; i < texts.Count; i++)
            {
                string name = "Team log";
                if (texts.Count > 1)
                    name += $" ({(i + 1)}/{texts.Count})";
                yield return new EmbedFieldBuilder()
                    .WithName(name)
                    .WithIsInline(false)
                    .WithValue(texts[i]);
            }
        }


        static readonly Dictionary<CombatType, string> OutgoingCombatTypeTranslation = new Dictionary<CombatType, string>()
        {
            { CombatType.Dbno, "derrubou" },
            { CombatType.Kill, "matou" },
            { CombatType.LastTrade, "trocou com" }
        };

        static readonly Dictionary<CombatType, string> IncomingCombatTypeTranslation = new Dictionary<CombatType, string>()
        {
            { CombatType.Dbno, "caiu para" },
            { CombatType.Kill, "morreu para" },
            { CombatType.LastTrade, "trocou com" }
        };

        static string CombatText(CombatType type, CombatDirection direction)
        {
            if (direction == CombatDirection.Incoming)
                return IncomingCombatTypeTranslation[type];
            else
                return OutgoingCombatTypeTranslation[type];
        }

        static string TeamSizeTranslation(int teamSize)
        {
            if (teamSize == 1) return "Solo";
            else if (teamSize == 2) return "Duo";
            else if (teamSize == 4) return "Squad";
            else return "Custom Size";
        }

        static readonly Dictionary<string, string> MapTranslation = new Dictionary<string, string>()
        {
            { "Baltic_Main", "Erangel" },
            { "Chimera_Main", "Paramo" },
            { "Desert_Main", "Miramar" },
            { "DihorOtok_Main", "Vikendi" },
            { "Erangel_Main", "Erangel" },
            { "Heaven_Main", "Haven" },
            { "Range_Main", "Camp_Jackal" },
            { "Savage_Main", "Sanhok" },
            { "Summerland_Main", "Karakin" },
            { "Tiger_Main", "Taego" }
        };

        static readonly Dictionary<string, string> MapExtension = new Dictionary<string, string>()
        {
            { "Baltic_Main", "png" },
            { "Chimera_Main", "png" },
            { "Desert_Main", "png" },
            { "DihorOtok_Main", "png" },
            { "Erangel_Main", "png" },
            { "Heaven_Main", "png" },
            { "Range_Main", "png" },
            { "Savage_Main", "png" },
            { "Summerland_Main", "jpg" },
            { "Tiger_Main", "png" }
        };

        static readonly Dictionary<string, string> WeaponNames = new Dictionary<string, string>
        {
            {"AIPawn_Base_Female_C", "AI Player" },
            {"AIPawn_Base_Male_C", "AI Player" },
            {"AquaRail_A_01_C", "Aquarail" },
            {"AquaRail_A_02_C", "Aquarail"},
            {"AquaRail_A_03_C", "Aquarail"},
            {"BattleRoyaleModeController_Chimera_C", "Bluezone" },
            {"BattleRoyaleModeController_Def_C", "Bluezone" },
            {"BattleRoyaleModeController_Desert_C", "Bluezone" },
            {"BattleRoyaleModeController_DihorOtok_C", "Bluezone" },
            {"BattleRoyaleModeController_Heaven_C", "Bluezone" },
            {"BattleRoyaleModeController_Savage_C", "Bluezone"},
            {"BattleRoyaleModeController_Summerland_C", "Bluezone" },
            {"BattleRoyaleModeController_Tiger_C", "Bluezone" },
            {"BlackZoneController_Def_C", "Blackzone" },
            {"Bluezonebomb_EffectActor_C", "Bluezone Grenade" },
            {"Boat_PG117_C", "PG-117" },
            {"BP_ATV_C", "Quad" },
            {"BP_BRDM_C", "BRDM-2" },
            {"BP_CoupeRB_C", "Coupe RB" },
            {"BP_Dirtbike_C", "Dirt Bike" },
            {"BP_DO_Circle_Train_Merged_C", "Train" },
            {"BP_DO_Line_Train_Dino_Merged_C", "Train" },
            {"BP_DO_Line_Train_Merged_C", "Train" },
            {"BP_Eragel_CargoShip01_C", "Ferry Damage" },
            {"BP_FakeLootProj_AmmoBox_C", "Loot Truck" },
            {"BP_FakeLootProj_MilitaryCrate_C", "Loot Truck" },
            {"BP_FireEffectController_C", "Molotov Fire" },
            {"BP_FireEffectController_JerryCan_C", "Jerrycan Fire" },
            {"BP_Helicopter_C", "Pillar Scout Helicopter" },
            {"BP_IncendiaryDebuff_C", "Burn" },
            {"BP_JerryCanFireDebuff_C", "Burn" },
            {"BP_KillTruck_C", "Pillar Tactical" },
            {"BP_LootTruck_C", "Loot Truck" },
            {"BP_M_Rony_A_01_C", "Rony" },
            {"BP_M_Rony_A_02_C", "Rony"},
            {"BP_M_Rony_A_03_C", "Rony"},
            {"BP_Mirado_A_02_C", "Mirado" },
            {"BP_Mirado_A_03_Esports_C", "Mirado" },
            {"BP_Mirado_Open_03_C", "Mirado (open top)" },
            {"BP_Mirado_Open_04_C", "Mirado (open top)"},
            {"BP_Mirado_Open_05_C", "Mirado (open top)"},
            {"BP_MolotovFireDebuff_C", "Molotov Fire Damage" },
            {"BP_Motorbike_04_C", "Motorcycle" },
            {"BP_Motorbike_04_Desert_C", "Motorcycle" },
            {"BP_Motorbike_04_SideCar_C", "Motorcycle (w/ Sidecar)" },
            {"BP_Motorbike_04_SideCar_Desert_C", "Motorcycle (w/ Sidecar)" },
            {"BP_Motorbike_Solitario_C", "Motorcycle" },
            {"BP_Motorglider_C", "Motor Glider" },
            {"BP_Motorglider_Green_C", "Motor Glider" },
            {"BP_Niva_01_C", "Zima" },
            {"BP_Niva_02_C", "Zima"},
            {"BP_Niva_03_C", "Zima"},
            {"BP_Niva_04_C", "Zima"},
            {"BP_Niva_05_C", "Zima"},
            {"BP_Niva_06_C", "Zima"},
            {"BP_Niva_07_C", "Zima"},
            {"BP_PickupTruck_A_01_C", "Pickup Truck (closed top)" },
            {"BP_PickupTruck_A_02_C", "Pickup Truck (closed top)"},
            {"BP_PickupTruck_A_03_C", "Pickup Truck (closed top)"},
            {"BP_PickupTruck_A_04_C", "Pickup Truck (closed top)"},
            {"BP_PickupTruck_A_05_C", "Pickup Truck (closed top)"},
            {"BP_PickupTruck_A_esports_C", "Pickup Truck (closed top)" },
            {"BP_PickupTruck_B_01_C", "Pickup Truck (open top)" },
            {"BP_PickupTruck_B_02_C", "Pickup Truck (open top)"},
            {"BP_PickupTruck_B_03_C", "Pickup Truck (open top)"},
            {"BP_PickupTruck_B_04_C", "Pickup Truck (open top)"},
            {"BP_PickupTruck_B_05_C", "Pickup Truck (open top)"},
            {"BP_PonyCoupe_C", "Pony Coupe" },
            {"BP_Porter_C", "Porter" },
            {"BP_Scooter_01_A_C", "Scooter" },
            {"BP_Scooter_02_A_C", "Scooter"},
            {"BP_Scooter_03_A_C", "Scooter"},
            {"BP_Scooter_04_A_C", "Scooter"},
            {"BP_Snowbike_01_C", "Snowbike"},
            {"BP_Snowbike_02_C", "Snowbike"},
            {"BP_Snowmobile_01_C", "Snowmobile" },
            {"BP_Snowmobile_02_C", "Snowmobile"},
            {"BP_Snowmobile_03_C", "Snowmobile"},
            {"BP_Spiketrap_C", "Spike Trap" },
            {"BP_TukTukTuk_A_01_C", "Tukshai" },
            {"BP_TukTukTuk_A_02_C", "Tukshai"},
            {"BP_TukTukTuk_A_03_C", "Tukshai"},
            {"BP_Van_A_01_C", "Van" },
            {"BP_Van_A_02_C", "Van"},
            {"BP_Van_A_03_C", "Van"},
            {"Buff_DecreaseBreathInApnea_C", "Drowning" },
            {"Buggy_A_01_C", "Buggy" },
            {"Buggy_A_02_C", "Buggy"},
            {"Buggy_A_03_C", "Buggy"},
            {"Buggy_A_04_C", "Buggy"},
            {"Buggy_A_05_C", "Buggy"},
            {"Buggy_A_06_C", "Buggy"},
            {"Carepackage_Container_C", "Care Package" },
            {"Dacia_A_01_v2_C", "Dacia" },
            {"Dacia_A_01_v2_snow_C", "Dacia" },
            {"Dacia_A_02_v2_C", "Dacia" },
            {"Dacia_A_03_v2_C", "Dacia" },
            {"Dacia_A_03_v2_Esports_C", "Dacia" },
            {"Dacia_A_04_v2_C", "Dacia" },
            {"DroppedItemGroup", "Object Fragments" },
            {"EmergencyAircraft_Tiger_C", "Emergency Aircraft" },
            {"Jerrycan", "Jerrycan" },
            {"JerrycanFire", "Jerrycan Fire" },
            {"Lava", "Lava" },
            {"None", "None" },
            {"PanzerFaust100M_Projectile_C", "Panzerfaust Projectile" },
            {"PG117_A_01_C", "PG-117" },
            {"PlayerFemale_A_C", "Player" },
            {"PlayerMale_A_C", "Player" },
            {"ProjC4_C", "C4" },
            {"ProjGrenade_C", "Frag Grenade" },
            {"ProjIncendiary_C", "Incendiary" },
            {"ProjMolotov_C", "Molotov Cocktail" },
            {"ProjMolotov_DamageField_Direct_C", "Molotov Cocktail Fire Field" },
            {"ProjStickyGrenade_C", "Sticky Bomb" },
            {"RacingDestructiblePropaneTankActor_C", "Propane Tank" },
            {"RacingModeContorller_Desert_C", "Bluezone" },
            {"RedZoneBomb_C", "Redzone" },
            {"RedZoneBombingField", "Redzone" },
            {"RedZoneBombingField_Def_C", "Redzone" },
            {"TslDestructibleSurfaceManager", "Destructible Surface" },
            {"TslPainCausingVolume", "Lava" },
            {"Uaz_A_01_C", "UAZ (open top)" },
            {"Uaz_Armored_C", "UAZ (armored)" },
            {"Uaz_B_01_C", "UAZ (soft top)" },
            {"Uaz_B_01_esports_C", "UAZ (soft top)" },
            {"Uaz_C_01_C", "UAZ (hard top)" },
            {"UltAIPawn_Base_Female_C", "Player" },
            {"UltAIPawn_Base_Male_C", "Player" },
            {"WeapAK47_C", "AKM" },
            {"WeapAUG_C", "AUG A3" },
            {"WeapAWM_C", "AWM" },
            {"WeapBerreta686_C", "S686" },
            {"WeapBerylM762_C", "Beryl" },
            {"WeapBizonPP19_C", "Bizon" },
            {"WeapCowbar_C", "Crowbar" },
            {"WeapCowbarProjectile_C", "Crowbar Projectile" },
            {"WeapCrossbow_1_C", "Crossbow" },
            {"WeapDesertEagle_C", "Deagle" },
            {"WeapDP12_C", "DBS" },
            {"WeapDP28_C", "DP-28" },
            {"WeapDuncansHK416_C", "M416" },
            {"WeapFNFal_C", "SLR" },
            {"WeapG18_C", "P18C" },
            {"WeapG36C_C", "G36C" },
            {"WeapGroza_C", "Groza" },
            {"WeapHK416_C", "M416" },
            {"WeapJuliesKar98k_C", "Kar98k" },
            {"WeapMk12_C", "Mk12" },
            {"WeapK2_C", "K2" },
            {"WeapKar98k_C", "Kar98k" },
            {"WeapL6_C", "Lynx AMR" },
            {"WeapLunchmeatsAK47_C", "AKM" },
            {"WeapM16A4_C", "M16A4" },
            {"WeapM1911_C", "P1911" },
            {"WeapM249_C", "M249" },
            {"WeapM24_C", "M24" },
            {"WeapM9_C", "P92" },
            {"WeapMachete_C", "Machete" },
            {"WeapMacheteProjectile_C", "Machete Projectile" },
            {"WeapMadsQBU88_C", "QBU88" },
            {"WeapMG3_C", "MG3" },
            {"WeapMini14_C", "Mini 14" },
            {"WeapMk14_C", "Mk14 EBR" },
            {"WeapMk47Mutant_C", "Mk47 Mutant" },
            {"WeapMP5K_C", "MP5K" },
            {"WeapNagantM1895_C", "R1895" },
            {"WeapMosinNagant_C", "Mosin-Nagant" },
            {"WeapP90_C", "P90" },
            {"WeapPan_C", "Pan" },
            {"WeapPanProjectile_C", "Pan Projectile" },
            {"WeapPanzerFaust100M1_C", "Panzerfaust" },
            {"WeapQBU88_C", "QBU88" },
            {"WeapQBZ95_C", "QBZ95" },
            {"WeapRhino_C", "R45" },
            {"WeapSaiga12_C", "S12K" },
            {"WeapSawnoff_C", "Sawed-off" },
            {"WeapSCAR-L_C", "SCAR-L" },
            {"WeapSickle_C", "Sickle" },
            {"WeapSickleProjectile_C", "Sickle Projectile" },
            {"WeapSKS_C", "SKS" },
            {"WeapThompson_C", "Tommy Gun" },
            {"WeapTurret_KillTruck_Main_C", "Kill Truck Turret" },
            {"WeapUMP_C", "UMP9" },
            {"WeapUZI_C", "Micro Uzi" },
            {"WeapVector_C", "Vector" },
            {"WeapVSS_C", "VSS" },
            {"Weapvz61Skorpion_C", "Skorpion" },
            {"WeapWin94_C", "Win94" },
            {"WeapWinchester_C", "S1897" }
        };
    }
}
