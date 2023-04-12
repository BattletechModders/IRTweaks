using BattleTech.Save;
using BattleTech.Save.Test;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(SimGameState), "Dehydrate")]
    public static class SimGameState_Dehydrate_Transpiler
    {
        static bool Prepare() => Mod.Config.Fixes.FactionReputationFixes;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            var codes = new List<CodeInstruction>(instructions);
            FieldInfo currentAlliedFactionSaveFieldInfo = typeof(SimGameSave).GetField("AlliedFactions");
            FieldInfo careerEndAlliedFactionSaveFieldInfo = typeof(SimGameSave).GetField("CareerModeEndAlliedFactions");
            //FieldInfo alliedFactionSaveFieldInfo = AccessTools.Field(typeof(SimGameSave), nameof(SimGameSave.AlliedFactions));
            //MethodInfo nameGetMethod = AccessTools.Property(typeof(BaseDescriptionDef), nameof(BaseDescriptionDef.Name)).GetGetMethod();
            int ldFldCount = 0;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand.Equals(currentAlliedFactionSaveFieldInfo))
                {
                    if (ldFldCount > 0) // change 2nd instance of save.AlliedFactions to save.CareerModeEndAlliedFactions to bake correct info into save
                    {
                        codes[i].operand = careerEndAlliedFactionSaveFieldInfo;
                        break;
                    }
                    ldFldCount++;
                }
            }
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(SimGameState), "Rehydrate")]
    public static class SimGameState_Rehydrate_Allies
    {
        static bool Prepare() => Mod.Config.Fixes.FactionReputationFixes;

        public static void Postfix(SimGameState __instance, GameInstanceSave gameInstanceSave)
        {
            //remove inappropriate allied factions from career end

            if (Mod.Log.Trace != null)
            {
                Mod.Log.Trace?.Write($"SimGameState_Rehydrate_Allies: spamming save contents:");

                SimGameSave save = gameInstanceSave.SimGameSave;
                SerializableReferenceContainer globalReferences = gameInstanceSave.GlobalReferences;
                globalReferences.ResetOperateOnAllForAll();
                save.SimGameContext.Rehydrate(__instance, save, globalReferences);
                foreach (var saveAlly in save.AlliedFactions)
                {
                    Mod.Log.Trace?.Write($"SimGameState_Rehydrate_Allies: {saveAlly} present in SAVE AlliedFactions");
                }
                foreach (var saveCareerEndAlly in save.CareerModeEndAlliedFactions)
                {
                    Mod.Log.Trace?.Write($"SimGameState_Rehydrate_Allies: {saveCareerEndAlly} present in SAVE CareerModeEndAlliedFactions");
                }
            }

            if (__instance.IsCareerModeComplete())
            {
                var toRemove = new List<string>();
                for (var index = __instance.AlliedFactions.Count - 1; index >= 0; index--)
                {
                    var alliedFaction = __instance.AlliedFactions[index];
                    var alliedFactionTag = $"ALLIED_FACTION_{alliedFaction}";

                    Mod.Log.Trace?.Write($"SimGameState_Rehydrate_Allies: {alliedFaction} present in AlliedFactions");
                    if (!__instance.CompanyTags.Contains(alliedFactionTag))
                    {
                        toRemove.Add(alliedFaction);
                        Mod.Log.Info?.Write($"SimGameState_Rehydrate_Allies: {alliedFaction} present in AlliedFactions but not Company Tags. probably from career end");
                    }
                }

                foreach (var careerEndAlly in __instance.CareerModeEndAlliedFactions)
                {
                    Mod.Log.Trace?.Write($"SimGameState_Rehydrate_Allies: {careerEndAlly} present in CareerModeEndAlliedFactions");
                }

                foreach (var removeAlly in toRemove)
                {
                    FactionValue factionByName = FactionEnumeration.GetFactionByName(removeAlly);
                    if (factionByName != null)
                    {
                        __instance.RemoveAllyFaction(factionByName);
                        Mod.Log.Info?.Write($"SimGameState_Rehydrate_Allies: Processed removal of {removeAlly} due to missing Company Tags");
                    }
                }
            }
        }
    }





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