using BattleTech;
using BattleTech.Framework;
using Harmony;
using IRBTModUtils;
using IRBTModUtils.Extension;
using System;
using System.Collections.Generic;

namespace IRTweaks.Modules.Combat
{
    [HarmonyPatch(typeof(CombatGameState), "_Init")]
    static class CombatGameState__Init
    {
        static bool Prepare() => Mod.Config.Fixes.ScaleObjectiveBuildingStructure;

        static void Postfix(CombatGameState __instance, Contract contract)
        {
            ModState.ScaledObjectiveBuildings.Clear();

            // Determine scale, if any
            Mod.Log.Info?.Write("Checking contract for objective building scaling:");
            Mod.Log.Info?.Write($"  -- contract has " +
                $"difficulty: {SharedState.Combat.ActiveContract.Override.difficulty} and " +
                $"finalDifficulty: {SharedState.Combat.ActiveContract.Override.finalDifficulty}");

            if (SharedState.Combat.ActiveContract.Override.finalDifficulty < Mod.Config.Combat.ScaledStructure.MinDifficulty)
            {
                ModState.ActiveContractBuildingScaling = Mod.Config.Combat.ScaledStructure.DifficultyScaling[Mod.Config.Combat.ScaledStructure.MinDifficulty];
            }
            else if (SharedState.Combat.ActiveContract.Override.finalDifficulty > Mod.Config.Combat.ScaledStructure.MaxDifficulty)
            {
                ModState.ActiveContractBuildingScaling = Mod.Config.Combat.ScaledStructure.DifficultyScaling[Mod.Config.Combat.ScaledStructure.MaxDifficulty];
            }
            else
            {
                CombatOpts.StructureScale scale = Mod.Config.Combat.ScaledStructure.DefaultScale;
                bool foundScaling = Mod.Config.Combat.ScaledStructure.DifficultyScaling.TryGetValue(SharedState.Combat.ActiveContract.Override.finalDifficulty, out scale);
                if (!foundScaling)
                {
                    Mod.Log.Warn?.Write("Difficulty not found, but within the bounds of min and max. This is a configuration error, using default value!");
                    scale = Mod.Config.Combat.ScaledStructure.DefaultScale;
                }
                ModState.ActiveContractBuildingScaling = scale;
            }
            Mod.Log.Info?.Write($"Building scaling set to: {ModState.ActiveContractBuildingScaling}");

        }
    }

    [HarmonyPatch(typeof(BattleTech.Building), "AttachBuildingToObjective")]
    static class Building_AttachBuildingToObjective
    {
        static bool Prepare() => Mod.Config.Fixes.ScaleObjectiveBuildingStructure;

        static void Postfix(BattleTech.Building __instance, ObjectiveGameLogic objective)
        {
            if (!ModState.ScaledObjectiveBuildings.Contains(__instance.DistinctId()))
            {
                __instance.ScaleHealth();
                ModState.ScaledObjectiveBuildings.Add(__instance.DistinctId());
            }
            else
            {
                Mod.Log.Info?.Write($" -- building: {__instance.DistinctId()} was already scaled, skipping");
            }
        }
    }

    [HarmonyPatch(typeof(TurnDirector), "OnInitializeContractComplete")]
    static class ScaleObjectiveBuildingStructure_TurnDirector_OnInitializeContractComplete
    {
        static bool Prepare() => Mod.Config.Fixes.ScaleObjectiveBuildingStructure;

        static void Postfix(TurnDirector __instance, MessageCenterMessage message)
        {
            Mod.Log.Trace?.Write("TD:OICC - entered.");

            // Iterate contract objectives
            List<ITaggedItem> objectsOfType = SharedState.Combat.ItemRegistry.GetObjectsOfType(TaggedObjectType.Objective);
            List<ObjectiveGameLogic> objectives = objectsOfType.ConvertAll((ITaggedItem x) => x as ObjectiveGameLogic);
            foreach (ObjectiveGameLogic ogl in objectives)
            {
                Mod.Log.Debug?.Write($" -- objective: {ogl.DisplayName} has: {ogl.GetTargetUnits().Count} targets");
                foreach (ICombatant combatant in ogl.GetTargetUnits())
                {
                    if (combatant is BattleTech.Building building)
                    {
                        BattleTech.Building scaledBuilding = building;
                        if (!ModState.ScaledObjectiveBuildings.Contains(building.DistinctId()))
                        {
                            building.ScaleHealth();
                            ModState.ScaledObjectiveBuildings.Add(building.DistinctId());
                        }
                        else
                        {
                            Mod.Log.Debug?.Write($" -- building: {building.DistinctId()} was already scaled, skipping");
                        }
                    }
                }
            }

        }
    }

    static class BuildingHelper
    {
        public static void ScaleHealth(this BattleTech.Building building)
        {
            float adjustedStruct = (float)Math.Floor((building.CurrentStructure * ModState.ActiveContractBuildingScaling.Multi) + ModState.ActiveContractBuildingScaling.Mod);

            // Update Structure stat and StartingStructure value
            building.StatCollection.ModifyStat("IRTweaks", -1, ModStats.HBS_Building_Structure, StatCollection.StatOperation.Set, adjustedStruct);
            Traverse startingStructT = Traverse.Create(building).Property("StartingStructure");
            startingStructT.SetValue(adjustedStruct);

            Mod.Log.Info?.Write($"Scaling health for building: {building.DistinctId()}" +
                $" =>  currentStructure: {building.CurrentStructure}  startingStructure: {building.StartingStructure}  ratio: {building.HealthAsRatio}");

            Mod.Log.Info?.Write($" -- adjustedStructure: {adjustedStruct} = ( currentStruct: {building.CurrentStructure} " +
                $"x scaleMulti: {ModState.ActiveContractBuildingScaling.Multi} ) + scaleMod: {ModState.ActiveContractBuildingScaling.Mod}");

            // Update the destructable group
            if (building.DestructibleObjectGroup != null)
            {
                building.DestructibleObjectGroup.health = adjustedStruct;
                building.DestructibleObjectGroup.fullHealth = adjustedStruct;

                Mod.Log.Debug?.Write($"  -- new stats for destructibleObjectGroup =>  health: {building.DestructibleObjectGroup.health}  fullHealth: {building.DestructibleObjectGroup.fullHealth}");
            }
        }
    }

}
