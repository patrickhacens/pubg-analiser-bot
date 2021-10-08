using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PUBG.Models.Telemetry;
using System;

namespace PUBG.Converters
{
    public class PubgEventConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return typeof(Event).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var j = JObject.Load(reader);

            if (Enum.TryParse(typeof(EventType), j["_T"].ToString(), true, out var type))
            {
                return type switch
                {
                    EventType.LogMatchDefinition => j.ToObject<MatchDefinition>(),
                    EventType.LogPlayerLogin => j.ToObject<PlayerLogin>(),
                    EventType.LogPlayerCreate => j.ToObject<PlayerCreate>(),
                    EventType.LogPlayerPosition => j.ToObject<PlayerPosition>(),
                    EventType.LogPlayerAttack => j.ToObject<PlayerAttack>(),
                    EventType.LogVaultStart => j.ToObject<VaultStart>(),
                    EventType.LogSwimStart => j.ToObject<SwimStart>(),
                    EventType.LogPlayerLogout => j.ToObject<PlayerLogout>(),
                    EventType.LogObjectInteraction => j.ToObject<ObjectInteraction>(),
                    EventType.LogVehicleRide => j.ToObject<VehicleRide>(),
                    EventType.LogItemEquip => j.ToObject<ItemEquip>(),
                    EventType.LogItemPickup => j.ToObject<ItemPickup>(),
                    EventType.LogMatchStart => j.ToObject<MatchStart>(),
                    EventType.LogGameStatePeriodic => j.ToObject<GameStatePeriodic>(),
                    EventType.LogVehicleLeave => j.ToObject<VehicleLeave>(),
                    EventType.LogItemUnequip => j.ToObject<ItemUnequip>(),
                    EventType.LogParachuteLanding => j.ToObject<ParachuteLanding>(),
                    EventType.LogItemDrop => j.ToObject<ItemDrop>(),
                    EventType.LogObjectDestroy => j.ToObject<ObjectDestroy>(),
                    EventType.LogItemAttach => j.ToObject<ItemAttach>(),
                    EventType.LogPlayerTakeDamage => j.ToObject<PlayerTakeDamage>(),
                    EventType.LogPlayerUseThrowable => j.ToObject<PlayerUseThrowable>(),
                    EventType.LogPlayerMakeGroggy => j.ToObject<PlayerMakeGroggy>(),
                    EventType.LogPlayerKillV2 => j.ToObject<PlayerKill>(),
                    EventType.LogWeaponFireCount => j.ToObject<WeaponFireCount>(),
                    EventType.LogItemDetach => j.ToObject<ItemDetach>(),
                    EventType.LogItemPickupFromLootBox => j.ToObject<ItemPickupFromLootBox>(),
                    EventType.LogPhaseChange => j.ToObject<PhaseChange>(),
                    EventType.LogPlayerRevive => j.ToObject<PlayerRevive>(),
                    EventType.LogItemUse => j.ToObject<ItemUse>(),
                    EventType.LogHeal => j.ToObject<Heal>(),
                    EventType.LogSwimEnd => j.ToObject<SwimEnd>(),
                    EventType.LogArmorDestroy => j.ToObject<ArmorDestroy>(),
                    EventType.LogCarePackageSpawn => j.ToObject<CarePackageSpawn>(),
                    EventType.LogVehicleDamage => j.ToObject<VehicleDamage>(),
                    EventType.LogCarePackageLand => j.ToObject<CarePackageLanded>(),
                    EventType.LogRedZoneEnded => j.ToObject<RedZoneEnded>(),
                    EventType.LogEmPickupLiftOff => j.ToObject<EmPickupLiftOff>(),
                    EventType.LogWheelDestroy => j.ToObject<WheelDestroy>(),
                    EventType.LogVehicleDestroy => j.ToObject<VehicleDestroy>(),
                    EventType.LogItemPickupFromCarepackage => j.ToObject<ItemPickupFromCarePackage>(),
                    EventType.LogMatchEnd => j.ToObject<MatchEnd>(),
                    EventType.LogPlayerUseFlareGun => j.ToObject<PlayerUseFlareGun>(),
                    EventType.LogPlayerRedeployBRStart => j.ToObject<PlayerRedeployBRStart>(),
                    EventType.LogPlayerRedeploy => j.ToObject<PlayerRedeploy>(),
                    EventType.LogItemPickupFromCustomPackage => j.ToObject<ItemPickupFromCustomPackage>(),
                    EventType.LogBlackZoneEnded => j.ToObject<BlackZoneEnded>(),
                    EventType.LogPlayerDestroyBreachableWall => j.ToObject<PlayerDestroyBreachableWall>(),
                    _ => j.ToObject<Event>(),
                };
            }
            return new Event();
            throw new NotImplementedException($"no enum for EventType = {j["_T"]}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
