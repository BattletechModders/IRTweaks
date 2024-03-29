﻿using System;
using System.Collections.Generic;

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("ApplySimGameEventResult",
        new Type[] { typeof(SimGameEventResult), typeof(List<object>), typeof(SimGameEventTracker) })]
    public static class SimGameState_ApplySimGameEventResult
    {
        static bool Prepare() => Mod.Config.Fixes.DifficultyModsFromStats;

        public static void Postfix(SimGameState __instance, SimGameEventResult result, List<object> objects,
            SimGameEventTracker tracker = null)
        {
            if (result.Scope == EventScope.Company)
            {
                if (result.Stats != null)
                {
                    foreach (var simGameStat in result.Stats)
                    {
                        if (simGameStat.name == "IRTweaks_DiffMod")
                        {
                            var sim = UnityGameInstance.BattleTechGame.Simulation;
                            if (!sim.CompanyStats.ContainsStatistic(simGameStat.name))
                            {
                                Mod.Log.Info?.Write(
                                    $"DiffMods: Something wrong here, we should have added the stat 'IRTweaks_DiffMod' to company stats already.");
                            }
                            else
                            {
                                sim.DifficultySettings.RefreshCareerScoreMultiplier();
                                Mod.Log.Info?.Write(
                                    $"DiffMods: Found IRTweaks_DiffMod in stat results, refreshing difficulty modifier.");
                            }
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(SimGameDifficulty), "RefreshCareerScoreMultiplier")]
    static class SimGameDifficulty_RefreshCareerScoreMultiplier
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.DifficultyModsFromStats;

        static void Prefix(ref bool __runOriginal, SimGameDifficulty __instance)
        {
            if (!__runOriginal) return;

            float num = 0f;
            foreach (string key in __instance.difficultyDict.Keys)
            {
                SimGameDifficulty.DifficultySetting difficultySetting = __instance.difficultyDict[key];
                SimGameDifficulty.DifficultyOption difficultyOption = difficultySetting.Options[__instance.difficultyIndices[key]];
                if (difficultySetting.Enabled)
                {
                    num += difficultyOption.CareerScoreModifier;
                }
            }

            __instance.curCareerModifier = num;

            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null)
            {
                __runOriginal = false;
                return;
            }
            if (!sim.CompanyStats.ContainsStatistic("IRTweaks_DiffMod"))
            {
                __runOriginal = false;
                return;
            }

            var curMod = __instance.GetRawCareerModifier();
            var irMod = sim.CompanyStats.GetValue<float>("IRTweaks_DiffMod");

            __instance.curCareerModifier += irMod;
            Mod.Log.Info?.Write(
                $"DiffMods: Adding IRMod {irMod} to current career mod {curMod} for final career difficulty modifier {__instance.curCareerModifier}.");

            __runOriginal = false;
        }
    }

    [HarmonyPatch(typeof(SimGameDifficultySettingsModule))]
    [HarmonyPatch("CalculateRawScoreMod")]
    public static class SimGameDifficultySettingsModule_CalculateRawScoreMod
    {
        static bool Prepare() => Mod.Config.Fixes.DifficultyModsFromStats;

        public static void Postfix(SimGameDifficultySettingsModule __instance, ref float __result)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (!sim.CompanyStats.ContainsStatistic("IRTweaks_DiffMod")) return;
            var irMod = sim.CompanyStats.GetValue<float>("IRTweaks_DiffMod");
            __result += irMod;
        }
    }
}

