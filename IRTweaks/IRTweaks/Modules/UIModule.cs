using BattleTech.UI;
using IRTweaks.Modules.UI;
using System;
using System.Reflection;

namespace IRTweaks.Modules.Tooltip
{
    public static class UIFixes
    {
        static bool Initialized = false;

        public static void InitModule(HarmonyInstance harmony)
        {
            if (!Initialized)
            {
                try
                {

                    // Enable the CombatLog
                    if (Mod.Config.Fixes.CombatLog)
                    {
                        Mod.Log.Info?.Write("Activating Fix: CombatLog");

                        // Initialize the helpers
                        CombatLog.InitModule();
                    }

                    if (Mod.Config.Fixes.SkirmishAlwaysUnlimited)
                        Mod.Log.Info?.Write("Activating Fix: SkirmishAlwaysUnlimited");

                    if (Mod.Config.Fixes.DisableCombatSaves)
                        Mod.Log.Info?.Write("Activating Fix: DisableCombatSaves");

                    if (Mod.Config.Fixes.DisableCombatRestarts)
                        Mod.Log.Info?.Write("Activating Fix: DisableCombatRestart");

                    // Makes the main menu a smoother as there are fewer
                    if (Mod.Config.Fixes.StreamlinedMainMenu)
                    {
                        Mod.Log.Info?.Write("Activating Fix: StreamlinedMainMenu");

                        MethodInfo sgnb_rftp_mi = AccessTools.Method(typeof(SGNavigationButton), "ResetFlyoutsToPrefab");
                        HarmonyMethod smm_sgnb_pftp = new HarmonyMethod(typeof(StreamlinedMainMenu), "SGNavigationButton_ResetFlyoutsToPrefab");
                        harmony.Patch(sgnb_rftp_mi, null, smm_sgnb_pftp, null);

                        MethodInfo sgnb_ssatsd_mi = AccessTools.Method(typeof(SGNavigationButton), "SetStateAccordingToSimDropship");
                        HarmonyMethod smm_sgnb_ssatsd = new HarmonyMethod(typeof(StreamlinedMainMenu), "SGNavigationButton_SetStateAccordingToSimDropship");
                        harmony.Patch(sgnb_ssatsd_mi, null, smm_sgnb_ssatsd, null);

                        MethodInfo sgnb_oc_mi = AccessTools.Method(typeof(SGNavigationButton), "OnClick");
                        HarmonyMethod smm_sgnb_oc = new HarmonyMethod(typeof(StreamlinedMainMenu), "SGNavigationButton_OnClick");
                        harmony.Patch(sgnb_oc_mi, smm_sgnb_oc, null, null);

                        MethodInfo sgnb_ope_mi = AccessTools.Method(typeof(SGNavigationButton), "OnPointerEnter");
                        HarmonyMethod smm_sgnb_ope = new HarmonyMethod(typeof(StreamlinedMainMenu), "SGNavigationButton_OnPointerEnter");
                        harmony.Patch(sgnb_ope_mi, null, smm_sgnb_ope, null);

                        MethodInfo sgnb_se_mi = AccessTools.Method(typeof(SGNavigationButton), "SetupElement");
                        HarmonyMethod smm_sgnb_se = new HarmonyMethod(typeof(StreamlinedMainMenu), "SGNavigationButton_SetupElement");
                        harmony.Patch(sgnb_se_mi, null, smm_sgnb_se, null);

                    }

                    // Update the weapon tooltip to support CAC behaviors
                    if (Mod.Config.Fixes.WeaponTooltip)
                        Mod.Log.Info?.Write("Activating Fix: WeaponTooltip");

                    if (Mod.Config.Fixes.DamageReductionInCombatHud)
                        Mod.Log.Info?.Write("Activating Fix: DamageReductionInCombatHud");
                }
                catch (Exception e)
                {
                    Mod.Log.Error?.Write($"Failed to load patches due to: {e.Message}");
                    Mod.Log.Error?.Write(e.StackTrace);
                    Mod.Log.Error?.Write(e.ToString());
                }
            }
            Initialized = true;
        }

    }
}
