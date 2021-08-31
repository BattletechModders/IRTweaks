using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using Harmony;
using UnityEngine;

namespace IRTweaks.Modules.Combat
{
    class CTDestructInjuryFix
    {
        [HarmonyPatch(typeof(Mech), "OnLocationDestroyed")]
        public static class Mech_OnLocationDestroyed_Patch
        {
            static bool Prepare() => Mod.Config.Fixes.CTDestructInjuryFix;
            [HarmonyBefore(new string[] {"io.mission.customunits"})]

            public static void Prefix(Mech __instance, ChassisLocations location, Vector3 attackDirection, WeaponHitInfo hitInfo, DamageType damageType)
            {
                if (location==ChassisLocations.CenterTorso)
                {
                    if (__instance.MechDef.MechTags.Any(x => Mod.Config.Combat.DisableCTMaxInjureTags.Contains(x)))
                        return;
                    var pilot = __instance.GetPilot();
                    pilot.MaxInjurePilot(__instance.Combat.Constants, hitInfo.attackerId, hitInfo.stackItemUID, damageType, null, __instance.Combat.FindActorByGUID(hitInfo.attackerId));
                    Mod.Log.Info?.Write($"CT Destroyed! Calling MaxInjurePilot on {pilot.Name}");
                }
            }
        }
    }
}
