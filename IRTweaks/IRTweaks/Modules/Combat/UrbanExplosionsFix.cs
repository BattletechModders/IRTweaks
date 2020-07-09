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
                Mod.Log.Info($"ELECTRICAL EXPLOSION FOUND WITH SEQUENCE GUID {__instance.SequenceGUID}");
                Mod.Log.Info($"  - damageEachLocation: {__instance.damageEachLocation}  heat: {__instance.heatDamage}  stability: {__instance.stabilityDamage}");

                foreach (Vector3 targetPos in __instance.TargetPositions)
                {
                    Mod.Log.Info($"  -- target position: {targetPos}");
                }

                Traverse allTargetsT = Traverse.Create(__instance).Property("AllTargets");
                List<ICombatant> allTargets = allTargetsT.GetValue<List<ICombatant>>();

                // Find all buildings that were included as targets. HBS code can't handle these, so cut them out 
                //  of AllTargets and we'll destroy them independently in AttackNextTarget.
                List<AbstractActor> actorTargets = new List<AbstractActor>();
                foreach (ICombatant target in allTargets)
                {
                    Mod.Log.Info($"  -- target: {target.DistinctId()}");
                    if (target is BattleTech.Building building)
                    {
                        ModState.ExplosionBuildingTargets.Add(building);
                    }
                    else if (target is AbstractActor actor) 
                    {
                        // TODO: Check if the actor is already dead, and don't attack them. If they can be attacked, salvage can be impacted.
                        //   But is that desirable?
                        actorTargets.Add(actor);
                    }
                }

                allTargets.Clear();
                allTargets.AddRange(actorTargets);

                ModState.ExplosionSequences.Add(__instance.SequenceGUID);
            }

        }
    }


    // The HBS logic has a subtle bug here... it passes ICombatant but then tries a cast to AbstractActor 
    //   when it invokes ReapplyDesignMasks(). This will cause an NRE that allows the sequence to 'shortcircuit'.
    //   We fix this by eliminating any non-AbstractActors in the constructor, and damaging the buildings directly.
    [HarmonyPatch(typeof(ArtilleryObjectiveSequence), "AttackNextTarget")]
    static class ArtilleryObjectiveSequence_AttackNextTarget
    {
        static bool Prepare() => Mod.Config.Fixes.UrbanExplosionsFix;

        // Replica of HBS logic to work around the errors in this
        static void Prefix(ArtilleryObjectiveSequence __instance, float ___timeSinceLastAttack, float ___timeBetweenAttacks)
        {
            // If there are buildings to destroy, destroy them in one attack. This will use the camera of the first actual target
            if (ModState.ExplosionBuildingTargets.Count > 0)
            {
                // Destroy the buildings immediately
                foreach (BattleTech.Building building in ModState.ExplosionBuildingTargets)
                {
                    Mod.Log.Info($"Applying damage to target building: {building.DistinctId()}");
                    if (__instance.damageEachLocation > 0f)
                    {
                        Mod.Log.Info($" -- applying {__instance.damageEachLocation} damage to all locations.");
                        DamageOrderUtility.ApplyDamageToAllLocations("ArtillerySequence", __instance.SequenceGUID, __instance.RootSequenceGUID,
                            building, (int)__instance.damageEachLocation, (int)__instance.damageEachLocation, 
                            AttackDirection.FromArtillery, DamageType.Artillery);
                    }

                    if (__instance.heatDamage != 0)
                    {
                        Mod.Log.Info($" -- applying {__instance.damageEachLocation} heat to all locations.");
                        DamageOrderUtility.ApplyHeatDamage(__instance.SequenceGUID, building, __instance.heatDamage);
                    }

                    if (__instance.stabilityDamage > 0)
                    {
                        Mod.Log.Info($" -- applying {__instance.damageEachLocation} instability to all locations.");
                        DamageOrderUtility.ApplyStabilityDamage(__instance.SequenceGUID, building, __instance.stabilityDamage);
                    }

                }

                ModState.ExplosionBuildingTargets.Clear();
            }

            // One minor problem this can cause is that AttackNextTarget may have nothing to attack - which should work in theory, 
            //   but it's difficult to test.
            
        }
    }

    [HarmonyPatch(typeof(AbstractActor), "OnRecomputePathing")]
    static class AbstractActor_OnRecomputePathing
    {
        static void Prefix(AbstractActor __instance)
        {
            Mod.Log.Info($"Recomputing pathing for actor: {__instance.DistinctId()}");
        }
    }
}
