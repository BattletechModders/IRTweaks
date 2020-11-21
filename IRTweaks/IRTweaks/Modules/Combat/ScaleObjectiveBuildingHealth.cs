using BattleTech;
using BattleTech.Framework;
using Harmony;
using IRBTModUtils;
using IRBTModUtils.Extension;
using System;
using System.Collections.Generic;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(TurnDirector), "OnInitializeContractComplete")]
    public static class ScaleObjectiveBuildingStructure_TurnDirector_OnInitializeContractComplete
    {
        static bool Prepare() => Mod.Config.Fixes.ScaleObjectiveBuildingStructure;

        public static void Postfix(TurnDirector __instance, MessageCenterMessage message)
        {
            Mod.Log.Trace?.Write("TD:OICC - entered.");

            Mod.Log.Info?.Write("CONTRACT INIT COMPLETE");

            // Determine scale, if any
            Mod.Log.Info?.Write("Checking contract for objective building scaling:");
            Mod.Log.Info?.Write($"  -- contract has " +
                $"difficulty: {SharedState.Combat.ActiveContract.Override.difficulty} and " +
                $"finalDifficulty: {SharedState.Combat.ActiveContract.Override.finalDifficulty}");

            CombatOpts.StructureScale scale;
            if (SharedState.Combat.ActiveContract.Override.finalDifficulty < Mod.Config.Combat.ScaledStructure.MinDifficulty)
            {
                scale = Mod.Config.Combat.ScaledStructure.DifficultyScaling[Mod.Config.Combat.ScaledStructure.MinDifficulty];
            }
            else if (SharedState.Combat.ActiveContract.Override.finalDifficulty > Mod.Config.Combat.ScaledStructure.MaxDifficulty)
            {
                scale = Mod.Config.Combat.ScaledStructure.DifficultyScaling[Mod.Config.Combat.ScaledStructure.MaxDifficulty];
            }
            else
            {
                bool foundScaling = Mod.Config.Combat.ScaledStructure.DifficultyScaling.TryGetValue(SharedState.Combat.ActiveContract.Override.finalDifficulty, out scale);
                if (!foundScaling)
                {
                    Mod.Log.Warn?.Write("Difficulty not found, but within the bounds of min and max. This is a configuration error, using default value!");
                    scale = Mod.Config.Combat.ScaledStructure.DefaultScale;
                }
            }

            // Iterate contract objectives
            List<ITaggedItem> objectsOfType = SharedState.Combat.ItemRegistry.GetObjectsOfType(TaggedObjectType.Objective);
            List<ObjectiveGameLogic> objectives = objectsOfType.ConvertAll((ITaggedItem x) => x as ObjectiveGameLogic);
            Mod.Log.Info?.Write("Iterating objectives!");
            foreach (ObjectiveGameLogic ogl in objectives)
            {
                Mod.Log.Info?.Write($" -- objective: {ogl.DisplayName} has: {ogl.GetTargetUnits().Count} targets");
                foreach (ICombatant combatant in ogl.GetTargetUnits())
                {
                    if (combatant is BattleTech.Building building)
                    {
                        Mod.Log.Info?.Write($" -- building: {building.DistinctId()} is an objective target, it should be scaled!");

                        float adjustedStruct = (float)Math.Floor((building.CurrentStructure * scale.Multi) + scale.Mod);
                        Mod.Log.Info?.Write($" -- adjustedStructure: {adjustedStruct} = ( currentStruct: {building.CurrentStructure} x scaleMulti: {scale.Multi} ) + scaleMod: {scale.Mod}");
                        
                        building.StatCollection.ModifyStat("IRTweaks", -1, ModStats.HBS_Building_Structure, StatCollection.StatOperation.Set, adjustedStruct);
                        Traverse startingStructT = Traverse.Create(__instance).Property("StartingStructure");
                        startingStructT.SetValue(adjustedStruct);
                    }
                }
            }

            Mod.Log.Info?.Write("ON INIT CONTRACT COMPLETE DONE");
        }
    }
}
