using System.Linq;
using UnityEngine;

namespace IRTweaks.Modules.Combat
{

    class MechLocationDestructFixes
    {
        //can be disabled when ME fixes it
        [HarmonyPatch(typeof(MechStructureRules), "GetPassthroughLocation")]
        public static class MechStructureRules_GetPassthroughLocation
        {
            static bool Prepare() => !string.IsNullOrEmpty(Mod.Config.Combat.TorsoMountStatName);

            public static void Postfix(ArmorLocation location, AttackDirection attackDirection,
                ref ArmorLocation __result)
            {
                if ((location & ArmorLocation.Head) != 0) __result = ArmorLocation.CenterTorso;
            }
        }

        //can be disabled when ME fixes it
        [HarmonyPatch(typeof(Mech))]
        [HarmonyPatch("IsDead", MethodType.Getter)]
        public static class Mech_IsDead
        {
            static bool Prepare() => !string.IsNullOrEmpty(Mod.Config.Combat.TorsoMountStatName);
            public static void Postfix(Mech __instance, ref bool __result)
            {
                if (__instance.StatCollection.ContainsStatistic(Mod.Config.Combat.TorsoMountStatName))
                {
                    if (__instance.StatCollection.GetValue<bool>(Mod.Config.Combat.TorsoMountStatName))
                    {
                        __result = __instance.pilot.IsIncapacitated || __instance.pilot.HasEjected || __instance.CenterTorsoStructure <= 0f || (__instance.LeftLegStructure <= 0f && __instance.RightLegStructure <= 0f) || __instance.HasHandledDeath;
                    }
                }
            }
        }

        //can be disabled when ME fixes it
        [HarmonyPatch(typeof(AbstractActor), "FlagForDeath")]
        static class Turret_FlagForDeath
        {
            [HarmonyPrepare]
            static bool Prepare() => !string.IsNullOrEmpty(Mod.Config.Combat.TorsoMountStatName);

            [HarmonyPrefix]
            static void Prefix(ref bool __runOriginal, AbstractActor __instance, string reason, DeathMethod deathMethod, DamageType damageType, int location, int stackItemID, string attackerID, bool isSilent)
            {
                if (!__runOriginal) return;

                if (__instance is Turret turret)
                {
                    if (deathMethod == DeathMethod.HeadDestruction && turret.StatCollection.ContainsStatistic(Mod.Config.Combat.TorsoMountStatName))
                    {
                        if (turret.StatCollection.GetValue<bool>(Mod.Config.Combat.TorsoMountStatName))
                        {
                            Mod.Log.Info?.Write($"Head destroyed, but unit is Torso Mount! Not flagging for death!");
                            __runOriginal = false;
                        }
                    }
                }
            }
        }

        //keep this always
        [HarmonyPatch(typeof(Mech), "OnLocationDestroyed")]
        [HarmonyBefore(new string[] { "io.mission.customunits" })]
        public static class Mech_OnLocationDestroyed_Patch
        {
            [HarmonyPrepare]
            static bool Prepare() => Mod.Config.Fixes.CTDestructInjuryFix;

            [HarmonyPrefix]
            static void Prefix(ref bool __runOriginal, Mech __instance, ChassisLocations location, Vector3 attackDirection, WeaponHitInfo hitInfo, DamageType damageType)
            {
                if (!__runOriginal) return;

                if (location == ChassisLocations.CenterTorso)
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
