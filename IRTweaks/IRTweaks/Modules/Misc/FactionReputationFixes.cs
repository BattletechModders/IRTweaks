using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.Save;
using BattleTech.UI;
using Harmony;

namespace IRTweaks.Modules.Misc
{
    //remove duplicates
    [HarmonyPatch(typeof(SimGameState), "AddAllyFaction")]
    public static class SimGameState_AddAllyFaction
    {
        static bool Prepare() => Mod.Config.Fixes.FactionReputationFixes;

        public static void Postfix(SimGameState __instance)
        {
            var alliedFactions = new List<string>(__instance.AlliedFactions.Distinct());
            __instance.AlliedFactions = alliedFactions.ToList();
        }
    }

    //update stat validators to not be dumb
    [HarmonyPatch(typeof(SimGameState), "RemoveAllyFaction")]
    public static class SimGameState_RemoveAllyFaction
    {
        static bool Prepare() => Mod.Config.Fixes.FactionReputationFixes;

        public static void Postfix(SimGameState __instance, FactionValue faction)
        {
            var alliedFactions = new List<string>(__instance.AlliedFactions.Distinct());
            __instance.AlliedFactions = alliedFactions.ToList();
            __instance.AlliedFactions.Remove(faction.Name);

            FactionDef factionDef = __instance.GetFactionDef(faction.Name);
            foreach (var enemyFactionName in factionDef.Enemies)
            {
                FactionValue enemyFactionValue = FactionEnumeration.GetFactionByName(enemyFactionName);
                string repID = __instance.GetRepID("Reputation", enemyFactionValue);
                Statistic statistic = __instance.CompanyStats.GetStatistic(repID);
                bool flag = false;
                bool flag2 = false;
                if (enemyFactionValue.DoesGainReputation)
                {
                    if (__instance.IsFactionAlly(enemyFactionValue))
                    {
                        statistic.SetValidator<int>(new Statistic.Validator<int>(__instance.ReputationAllyValidator<int>));
                        flag = true;
                    }
                    else if (__instance.IsFactionEnemy(enemyFactionValue))
                    {
                        statistic.SetValidator<int>(new Statistic.Validator<int>(__instance.ReputationEnemyValidator<int>));
                        flag2 = true;
                    }
                    else
                    {
                        statistic.SetValidator<int>(new Statistic.Validator<int>(__instance.ReputationNormalValidator<int>));
                    }

                    statistic.SetValue<int>(statistic.Value<int>(), true);
                    string text2 = enemyFactionName.ToString();
                    string text3 = "ALLIED_FACTION_" + text2;
                    string text4 = "ENEMY_FACTION_" + text2;
                    if (flag && !__instance.companyTags.Contains(text3))
                    {
                        __instance.companyTags.Add(text3);
                    }
                    else if (!flag && __instance.companyTags.Contains(text3))
                    {
                        __instance.companyTags.Remove(text3);
                    }
                    if (flag2 && !__instance.companyTags.Contains(text4))
                    {
                        __instance.companyTags.Add(text4);
                    }
                    else if (!flag2 && __instance.companyTags.Contains(text4))
                    {
                        __instance.companyTags.Remove(text4);
                    }
                }
            }
        }
    }
}