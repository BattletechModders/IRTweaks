using BattleTech;
using Harmony;
using IRBTModUtils.Extension;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IRTweaks.Modules.Combat
{
    [HarmonyPatch(typeof(ArtilleryObjectiveSequence))]
    [HarmonyPatch(new Type[] { typeof(CombatGameState), typeof(List<Vector3>), typeof(ArtilleryVFXType), typeof(List<ICombatant>), typeof(float), typeof(int), typeof(int), typeof(TerrainMaskFlags) })]
    [HarmonyPatch(MethodType.Constructor)]
    static class ArtilleryObjectiveSequence_Ctor
    {
        static bool Prepare() => Mod.Config.Fixes.UrbanExplosionsFix;

        static void Postfix(ArtilleryObjectiveSequence __instance, ArtilleryVFXType ___artilleryVFXType)
        {

            if (__instance != null &&
                (___artilleryVFXType == ArtilleryVFXType.ElectricTransformerExplosion || ___artilleryVFXType == ArtilleryVFXType.CoolantExplosion)
                )
            {
                Mod.Log.Info("ELECTRICAL EXPLOSION FOUND!");
                Mod.Log.Info($"  - damageEachLocation: {__instance.damageEachLocation}  heat: {__instance.heatDamage}  stability: {__instance.stabilityDamage}");

                foreach (Vector3 targetPos in __instance.TargetPositions)
                {
                    Mod.Log.Info($"  -- target position: {targetPos}");
                }

                Traverse allTargetsT = Traverse.Create(__instance).Property("AllTargets");
                List<ICombatant> allTargets = allTargetsT.GetValue<List<ICombatant>>();

                foreach (ICombatant target in allTargets)
                {
                    Mod.Log.Info($"  -- target: {target.DistinctId()}");
                }

                ModState.ExplosionSequences.Add(__instance.SequenceGUID);
            }

        }
    }

    [HarmonyPatch(typeof(ArtilleryObjectiveSequence), "AttackNextTarget")]
    static class ArtilleryObjectiveSequence_AttackNextTarget
    {
        static bool Prepare() => Mod.Config.Fixes.UrbanExplosionsFix;

        static void Prefix(ArtilleryObjectiveSequence __instance)
        {
            if (__instance != null && ModState.ExplosionSequences.Contains(__instance.SequenceGUID))
            {
                Traverse allTargetsT = Traverse.Create(__instance).Property("AllTargets");
                List<ICombatant> allTargets = allTargetsT.GetValue<List<ICombatant>>();
                if (allTargets != null && allTargets.Count > 0)
                {
                    Mod.Log.Info($" Attacking target: {allTargets[0].DistinctId()}");
                }
            }
        }
    }

}
