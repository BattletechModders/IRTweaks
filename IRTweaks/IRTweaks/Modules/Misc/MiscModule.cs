using BattleTech;
using BattleTech.Save;
using BattleTech.UI;
using Harmony;
using System;
using System.Reflection;

namespace IRTweaks.Modules.Misc {
    public static class MiscFixes {
        static bool Initialized = false;

        public static void InitModule(HarmonyInstance harmony) {
            if (!Initialized) {
                try {
                    // Update the pilot stats to have a maximum greater than 10
                    if (Mod.Config.Fixes.RandomStartByDifficulty) {
                        Mod.Log.Info?.Write("Activating Fix: RandomStartByDifficulty");
                        MethodInfo sgs_arsm = AccessTools.Method(typeof(SimGameState), "AddRandomStartingMechs");
                        HarmonyMethod rsbd_sgs_arsm_pre = new HarmonyMethod(typeof(RandomStartByDifficulty), "SimGameState_AddRandomStartingMechs_Prefix");
                        harmony.Patch(sgs_arsm, rsbd_sgs_arsm_pre, null, null);

                        MethodInfo sgco_ao = AccessTools.Method(typeof(SimGameConstantOverride), "ApplyOverride");
                        HarmonyMethod rsbd_sgco_ao_post = new HarmonyMethod(typeof(RandomStartByDifficulty), "SimGameConstantOverride_ApplyOverride_Postfix");
                        harmony.Patch(sgco_ao, null, rsbd_sgco_ao_post, null);

                        MethodInfo sgdso_ccm = AccessTools.Method(typeof(SGDifficultySettingObject), "CurCareerModifier");
                        HarmonyMethod rsbd_sgdso_ccm_pre = new HarmonyMethod(typeof(RandomStartByDifficulty), "SGDifficultySettingObject_CurCareerModifier_Prefix");
                        harmony.Patch(sgdso_ccm, rsbd_sgdso_ccm_pre, null, null);
                    }

                    // Makes the main menu a smoother as there are fewer
                    if (Mod.Config.Fixes.MultiTargetStat) {
                        Mod.Log.Info?.Write("Activating Fix: MultiTargetStat");
                    }

                } catch (Exception e) {
                    Mod.Log.Error?.Write($"Failed to load patches due to: {e.Message}");
                    Mod.Log.Error?.Write(e);
                }
            }
            Initialized = true;
        }
    }
}
