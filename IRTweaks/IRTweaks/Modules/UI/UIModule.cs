using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using Harmony;
using IRTweaks.Modules.UI;
using System;
using System.Reflection;

namespace IRTweaks.Modules.Tooltip {
    public static class UIFixes {
        static bool Initialized = false;

        public static void InitModule(HarmonyInstance harmony) {
            if (!Initialized) {
                try {
                    // Updates the purchase and selling dialogs to allow multiple items to be purchased and sold at once
                    if (Mod.Config.Fixes.BulkPurchasing) {
                        Mod.Log.Info?.Write("Activating Fix: BulkPurchasing");
                        MethodInfo refreshMI = AccessTools.Method(typeof(SG_Stores_MultiPurchasePopup), "Refresh");
                        HarmonyMethod mpp_R_Post = new HarmonyMethod(typeof(StoreQuantities), "MultiPurchasePopup_Refresh_Postfix");
                        harmony.Patch(refreshMI, null, mpp_R_Post, null);

                        MethodInfo mpp_ReceiveButtonPress = AccessTools.Method(typeof(SG_Stores_MultiPurchasePopup), "ReceiveButtonPress");
                        HarmonyMethod mpp_RBP_Pre = new HarmonyMethod(typeof(StoreQuantities), "MultiPurchasePopup_ReceiveButtonPress_Prefix");
                        harmony.Patch(mpp_ReceiveButtonPress, mpp_RBP_Pre, null, null);

                        MethodInfo ss_ReceiveButtonPress = AccessTools.Method(typeof(SG_Shop_Screen), "ReceiveButtonPress");
                        HarmonyMethod ss_RBP_Pre = new HarmonyMethod(typeof(StoreQuantities), "Shop_Screen_ReceiveButtonPress_Prefix");
                        harmony.Patch(ss_ReceiveButtonPress, ss_RBP_Pre, null, null);
                    }

                    // Enable the CombatLog
                    if (Mod.Config.Fixes.CombatLog) {
                        Mod.Log.Info?.Write("Activating Fix: CombatLog");
                        MethodInfo combatHUD_Init_MI = AccessTools.Method(typeof(CombatHUD), "Init", new Type[] { typeof(CombatGameState) });
                        HarmonyMethod cl_chud_i_post = new HarmonyMethod(typeof(CombatLog), "CombatHUD_Init_Postfix");
                        harmony.Patch(combatHUD_Init_MI, null, cl_chud_i_post, null);

                        MethodInfo chud_ocgd_mi = AccessTools.Method(typeof(CombatHUD), "OnCombatGameDestroyed");
                        HarmonyMethod cl_chud_ocgd_post = new HarmonyMethod(typeof(CombatLog), "CombatHUD_OnCombatGameDestroyed_Postfix");
                        harmony.Patch(chud_ocgd_mi, null, cl_chud_ocgd_post, null);

                        MethodInfo ccm_i_mi = AccessTools.Method(typeof(CombatChatModule), "Init");
                        HarmonyMethod cl_i_post = new HarmonyMethod(typeof(CombatLog), "CombatChatModule_Init_Postfix");
                        harmony.Patch(ccm_i_mi, null, cl_i_post, null);

                        MethodInfo ccm_ci_mi = AccessTools.Method(typeof(CombatChatModule), "CombatInit");
                        HarmonyMethod cl_ci_post = new HarmonyMethod(typeof(CombatLog), "CombatChatModule_CombatInit_Postfix");
                        harmony.Patch(ccm_ci_mi, null, cl_ci_post, null);

                        MethodInfo ccm_a_oe = AccessTools.Method(typeof(CombatChatModule), "Active_OnEnter", new Type[] { });
                        HarmonyMethod cl_ccm_a_oe_post = new HarmonyMethod(typeof(CombatLog), "CombatChatModule_Active_OnEnter_Postfix");
                        harmony.Patch(ccm_a_oe, null, cl_ccm_a_oe_post, null);

                        MethodInfo chudiwem_afm_mi = AccessTools.Method(typeof(CombatHUDInWorldElementMgr), "AddFloatieMessage");
                        HarmonyMethod chudiwem_afm_pre = new HarmonyMethod(typeof(CombatLog), "CombatHUDInWorldElementMgr_AddFloatieMessage_Prefix");
                        harmony.Patch(chudiwem_afm_mi, chudiwem_afm_pre, null, null);

                        MethodInfo mc_rs_mi = AccessTools.Method(typeof(MessageCenter), "RemoveSubscriber");
                        HarmonyMethod cl_mc_rs_pre = new HarmonyMethod(typeof(CombatLog), "MessageCenter_RemoveSubscriber_Prefix");
                        harmony.Patch(mc_rs_mi, cl_mc_rs_pre, null, null);

                        // Initialize the helpers
                        CombatLog.InitModule();
                    }

                    if (Mod.Config.Fixes.SkirmishAlwaysUnlimited)
                        Mod.Log.Info?.Write("Activating Fix: SkirmishAlwaysUnlimited");

                    if (Mod.Config.Fixes.DisableCombatSaves)
                        Mod.Log.Info?.Write("Activating Fix: DisableCombatSaves");

                    if (Mod.Config.Fixes.DisableCombatRestarts)
                        Mod.Log.Info?.Write("Activating Fix: DisableCombatRestart");

                    if (Mod.Config.Fixes.WarnOnCombatRestart)
                        Mod.Log.Info?.Write("Activating Fix: EnableCombatRestartWarning");

                    // Makes the main menu a smoother as there are fewer
                    if (Mod.Config.Fixes.StreamlinedMainMenu) {
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

                    // Update the pilot stats to have a maximum greater than 10
                    if (Mod.Config.Fixes.WeaponTooltip) {
                        Mod.Log.Info?.Write("Activating Fix: WeaponTooltip");
                        MethodInfo tooltipPrefab_Weapon_SetData = AccessTools.Method(typeof(TooltipPrefab_Weapon), "SetData");
                        HarmonyMethod tm_tp_w_sd_post = new HarmonyMethod(typeof(WeaponTooltips), "TooltipPrefab_Weapon_SetData_Postfix");
                        harmony.Patch(tooltipPrefab_Weapon_SetData, null, tm_tp_w_sd_post, null);
                    }

                } catch (Exception e) {
                    Mod.Log.Error?.Write($"Failed to load patches due to: {e.Message}");
                    Mod.Log.Error?.Write(e.StackTrace);
                    Mod.Log.Error?.Write(e.ToString());
                }
            }
            Initialized = true;
        }

    }
}
