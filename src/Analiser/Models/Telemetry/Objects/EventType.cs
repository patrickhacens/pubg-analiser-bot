using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUBG.Models.Telemetry
{
    public enum EventType
    {
        LogMatchDefinition,
        LogPlayerLogin,
        LogPlayerCreate,
        LogPlayerPosition,
        LogPlayerAttack,
        LogVaultStart,
        LogSwimStart,
        LogPlayerLogout,
        LogObjectInteraction,
        LogVehicleRide,
        LogItemEquip,
        LogItemPickup,
        LogMatchStart,
        LogGameStatePeriodic,
        LogVehicleLeave,
        LogItemUnequip,
        LogParachuteLanding,
        LogItemDrop,
        LogObjectDestroy,
        LogItemAttach,
        LogPlayerTakeDamage,
        LogPlayerUseThrowable,
        LogPlayerMakeGroggy,
        LogPlayerKillV2,
        LogWeaponFireCount,
        LogItemDetach,
        LogItemPickupFromLootBox,
        LogPhaseChange,
        LogPlayerRevive,
        LogItemUse,
        LogHeal,
        LogSwimEnd,
        LogArmorDestroy,
        LogCarePackageSpawn,
        LogVehicleDamage,
        LogCarePackageLand,
        LogRedZoneEnded,
        LogEmPickupLiftOff,
        LogWheelDestroy,
        LogVehicleDestroy,
        LogItemPickupFromCarepackage,
        LogMatchEnd,
        LogPlayerUseFlareGun,
        LogPlayerRedeployBRStart,
        LogPlayerRedeploy,
        LogItemPickupFromCustomPackage,
        LogBlackZoneEnded,
        LogPlayerDestroyBreachableWall,
        //check exists
        LogItemPutToVehicleTrunk,
        LogItemPickupFromVehicleTrunk,
        LogCharacterCarry
    }
}
