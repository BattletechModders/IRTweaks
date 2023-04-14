using System;
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

    [HarmonyPatch(typeof(SimGameDifficulty))]
    [HarmonyPatch("RefreshCareerScoreMultiplier")]
    public static class SimGameDifficulty_RefreshCareerScoreMultiplier
    {
        static bool Prepare() => Mod.Config.Fixes.DifficultyModsFromStats;

        public static bool Prefix(SimGameDifficulty __instance, Dictionary<string, SimGameDifficulty.DifficultySetting> ___difficultyDict, Dictionary<string, int> ___difficultyIndices, ref float ___curCareerModifier)
        {

            float num = 0f;
            foreach (string key in ___difficultyDict.Keys)
            {
                SimGameDifficulty.DifficultySetting difficultySetting = ___difficultyDict[key];
                SimGameDifficulty.DifficultyOption difficultyOption = difficultySetting.Options[___difficultyIndices[key]];
                if (difficultySetting.Enabled)
                {
                    num += difficultyOption.CareerScoreModifier;
                }
            }

            ___curCareerModifier = num;

            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return false;
            if (!sim.CompanyStats.ContainsStatistic("IRTweaks_DiffMod")) return false;

            var curMod = __instance.GetRawCareerModifier();
            var irMod = sim.CompanyStats.GetValue<float>("IRTweaks_DiffMod");

            ___curCareerModifier += irMod;
            Mod.Log.Info?.Write(
                $"DiffMods: Adding IRMod {irMod} to current career mod {curMod} for final career difficulty modifier {___curCareerModifier}.");
            return false;
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

