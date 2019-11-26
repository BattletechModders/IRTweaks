using BattleTech;
using BattleTech.UI;
using Harmony;
using System;

namespace IRTweaks.Modules.Misc {
    public static class RandomStartByDifficulty {

        public static void SimGameState_AddRandomStartingMechs_Prefix(SimGameState __instance) {
            Mod.Log.Trace("SGS:ARSM entered.");
            SimGameConstantOverride sgco = __instance.ConstantOverrides;

            if (sgco.ConstantOverrides.ContainsKey("CareerMode")) {
                // Patch starting mechs
                if (sgco.ConstantOverrides["CareerMode"].ContainsKey(ModStats.RandomMechs)) {
                    string startingMechsS = sgco.ConstantOverrides["CareerMode"][ModStats.RandomMechs];
                    Mod.Log.Info($"Replacing starting random mechs with:{startingMechsS}");
                    string[] startingMechs = startingMechsS.Split(',');
                    __instance.Constants.CareerMode.StartingRandomMechLists = startingMechs;
                } else {
                    Mod.Log.Debug($"key: {ModStats.RandomMechs} not found");
                }

                // Patch faction reputation
                if (sgco.ConstantOverrides["CareerMode"].ContainsKey(ModStats.FactionRep)) {
                    string factionRepS = sgco.ConstantOverrides["CareerMode"][ModStats.FactionRep];
                    string[] factions = factionRepS.Split(',');
                    foreach (string factionToken in factions) {
                        string[] factionSplit = factionToken.Split(':');
                        string factionId = factionSplit[0];
                        int factionRep = int.Parse(factionSplit[1]);
                        Mod.Log.Info($"Applying rep: {factionRep} to faction: ({factionId})");
                        FactionDef factionDef = FactionDef.GetFactionDefByEnum(__instance.DataManager, factionId);
                        __instance.AddReputation(factionDef.FactionValue, factionRep, false);
                    }
                } else {
                    Mod.Log.Debug($"key: {ModStats.RandomMechs} not found");
                }

            } else if (!sgco.ConstantOverrides.ContainsKey("CareerMode")) {
                Mod.Log.Debug("key:CareerMode not found");
            }

        }

        public static void SimGameConstantOverride_ApplyOverride_Postfix(SimGameConstantOverride __instance, string constantType, string constantName, SimGameState ___simState) {
            Mod.Log.Trace("SGCO:AO entered.");

            if (constantName != null && constantName.ToLower().Equals(ModStats.StrayShotValidTargets.ToLower())) {
                string value = __instance.ConstantOverrides[constantType][constantName];
                Mod.Log.Debug($" Setting StrayShotValidTargets to {value} ");
                ToHitConstantsDef thcd = ___simState.CombatConstants.ToHit;
                thcd.StrayShotValidTargets = (StrayShotValidTargets)Enum.Parse(typeof(StrayShotValidTargets), value);

                Traverse traverse = Traverse.Create(___simState.CombatConstants).Property("ToHit");
                traverse.SetValue(thcd);
                Mod.Log.Debug($" Replaced ToHit");
            }

        }

        public static bool SGDifficultySettingObject_CurCareerModifier_Prefix(SGDifficultySettingObject __instance, ref float __result, int ___curIdx) {
            Mod.Log.Trace("SGDSO:CCM entered.");

            float careerScoreModifier = 0f;
            if (__instance != null && __instance.Setting != null && __instance.Setting.Options != null && __instance.Setting.Options.Count > ___curIdx) {
                careerScoreModifier = __instance.Setting.Options[___curIdx].CareerScoreModifier;
            }

            __result = (careerScoreModifier <= -1f) ? 0f : careerScoreModifier;

            return false;
        }

    }
}
