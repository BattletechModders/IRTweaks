using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using Harmony;

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("ApplySimGameEventResult",
        new Type[] {typeof(SimGameEventResult), typeof(List<object>), typeof(SimGameEventTracker)})]
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

        public static void Postfix(SimGameDifficulty __instance, ref float ___curCareerModifier)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (!sim.CompanyStats.ContainsStatistic("IRTweaks_DiffMod")) return;
            
            var curMod = __instance.GetRawCareerModifier();
            var irMod = sim.CompanyStats.GetValue<float>("IRTweaks_DiffMod");

            var newMod = curMod + irMod;
            ___curCareerModifier = newMod;
            //Traverse.Create(__instance).Field("curCareerModifier").SetValue(newMod);
            Mod.Log.Info?.Write(
                $"DiffMods: Adding IRMod {irMod} to current career mod {curMod} for final career difficulty modifier {newMod}.");
        }
    }
}

