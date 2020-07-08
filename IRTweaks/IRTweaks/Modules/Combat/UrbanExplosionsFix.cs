using BattleTech;
using CustomAmmoCategoriesLog;
using Harmony;
using IRBTModUtils.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Mod.Log.Info($"ELECTRICAL EXPLOSION FOUND WITH SEQUENCE GUID {__instance.SequenceGUID}");
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


    // The HBS logic has a subtle bug here... it passes ICombatant but then tries a cast to AbstractActor. Which will 
    //   throw an NRE
    [HarmonyPatch(typeof(ArtilleryObjectiveSequence), "AttackNextTarget")]
    static class ArtilleryObjectiveSequence_AttackNextTarget
    {
        static bool Prepare() => Mod.Config.Fixes.UrbanExplosionsFix;

        // Replica of HBS logic to work around the errors in this
        static bool Prefix(ArtilleryObjectiveSequence __instance, float ___timeSinceLastAttack, float ___timeBetweenAttacks)
        {

            try
            {
                // If there's not 
                ___timeSinceLastAttack += Time.deltaTime;
                if (!(___timeSinceLastAttack > ___timeBetweenAttacks))
                {
                    return false;
                }

                Traverse allTargetsT = Traverse.Create(__instance).Property("AllTargets");
                List<ICombatant> allTargets = allTargetsT.GetValue<List<ICombatant>>();
                if (allTargets.Count > 0)
                {
                    ICombatant combatant = allTargets[0];
                    Mod.Log.Info($"ArtilleryObjectiveSequence_{__instance.SequenceGUID} attacking target: {combatant.DistinctId()}");
                    allTargets.Remove(combatant);

                    if (__instance.damageEachLocation > 0f)
                    {
                        Mod.Log.Info($" -- applying {__instance.damageEachLocation} damage to all target locations.");
                        DamageOrderUtility.ApplyDamageToAllLocations("ArtillerySequence", __instance.SequenceGUID, __instance.RootSequenceGUID,
                            combatant, (int)__instance.damageEachLocation, (int)__instance.damageEachLocation, AttackDirection.FromArtillery, DamageType.Artillery);
                    }

                    if (__instance.heatDamage != 0)
                    {
                        Mod.Log.Info($" -- applying {__instance.damageEachLocation} heat to all target locations.");
                        DamageOrderUtility.ApplyHeatDamage(__instance.SequenceGUID, combatant, __instance.heatDamage);
                    }

                    if (__instance.stabilityDamage > 0)
                    {
                        Mod.Log.Info($" -- applying {__instance.damageEachLocation} instability to all target locations.");
                        DamageOrderUtility.ApplyStabilityDamage(__instance.SequenceGUID, combatant, __instance.stabilityDamage);
                    }

                    if (__instance.applyDesignMaskOnExplosion != 0)
                    {
                        AbstractActor targetActor = combatant as AbstractActor;
                        if (targetActor != null)
                        {
                            Mod.Log.Debug($" -- re-applying design masks to actor.");
                            targetActor.ReapplyDesignMasks();
                        }
                        else
                        {
                            Mod.Log.Debug($" -- target ICombatant isn't an AbstractActor, skipping design-mask re-apply.");
                        }
                    }
                }

                List<ICombatant> deadCombatants = new List<ICombatant>();
                foreach (ICombatant combatant in allTargets)
                {
                    if (combatant.IsDead || combatant.IsFlaggedForDeath)
                    {
                        Mod.Log.Info($" -- target {combatant.DistinctId()} is already dead, removing them from AllTargets.");
                        deadCombatants.Add(combatant);
                    }
                }

                foreach (ICombatant combatant in deadCombatants)
                {
                    allTargets.Remove(combatant);
                }

                ___timeSinceLastAttack = 0f;

                return false;
            }
            catch (Exception e)
            {
                Mod.Log.Error("Failed to apply damage to targets from ArtilleryObjectiveSequence!", e);
                return false;
            }   
        }
    }

}
