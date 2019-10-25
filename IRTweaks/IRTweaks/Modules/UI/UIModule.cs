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
                        Mod.Log.Info("Activating Fix: BulkPurchasing");
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
                        Mod.Log.Info("Activating Fix: CombatLog");
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

                        MethodInfo ccm_ocm_mi = AccessTools.Method(typeof(CombatChatModule), "OnChatMessage", new Type[] { typeof(MessageCenterMessage) });
                        HarmonyMethod cl_ccm_ocm_pre = new HarmonyMethod(typeof(CombatLog), "CombatChatModule_OnChatMessage_Prefix");
                        harmony.Patch(ccm_ocm_mi, cl_ccm_ocm_pre, null, null);

                        MethodInfo ccm_u_mi = AccessTools.Method(typeof(CombatChatModule), "Update");
                        HarmonyMethod ccm_u_pre = new HarmonyMethod(typeof(CombatLog), "CombatChatModule_Update_Prefix");
                        harmony.Patch(ccm_u_mi, ccm_u_pre, null, null);

                        MethodInfo clvi_sd_mi = AccessTools.Method(typeof(ChatListViewItem), "SetData", new Type[] { typeof(ChatMessage), typeof(int?) });
                        HarmonyMethod cl_clvi_sd_pre = new HarmonyMethod(typeof(CombatLog), "ChatListViewItem_SetData_Prefix");
                        harmony.Patch(clvi_sd_mi, cl_clvi_sd_pre, null, null);

                        //MethodInfo chudai_stm_mi = AccessTools.Method(typeof(CombatHUDActorInfo), "SubscribeToMessages");
                        //HarmonyMethod cl_chudai_stm_post = new HarmonyMethod(typeof(CombatLog), "CombatHUDActorInfo_SubscribeToMessages_Postfix");
                        //harmony.Patch(chudai_stm_mi, null, cl_chudai_stm_post, null);

                        MethodInfo chudiwem_afm_mi = AccessTools.Method(typeof(CombatHUDInWorldElementMgr), "AddFloatieMessage");
                        HarmonyMethod chudiwem_afm_pre = new HarmonyMethod(typeof(CombatLog), "CombatHUDInWorldElementMgr_AddFloatieMessage_Prefix");
                        harmony.Patch(chudiwem_afm_mi, chudiwem_afm_pre, null, null);

                        MethodInfo mc_rs_mi = AccessTools.Method(typeof(MessageCenter), "RemoveSubscriber");
                        HarmonyMethod cl_mc_rs_pre = new HarmonyMethod(typeof(CombatLog), "MessageCenter_RemoveSubscriber_Prefix");
                        harmony.Patch(mc_rs_mi, cl_mc_rs_pre, null, null);

                        // Initialize the helpers
                        CombatLog.InitModule();
                    }

                    // Disables the ability to save in combat
                    if (Mod.Config.Fixes.DisableCombatSaves) {
                        Mod.Log.Info("Activating Fix: DisableCombatSaves");
                        MethodInfo sgom_cs = AccessTools.Method(typeof(SimGameOptionsMenu), "CanSave");
                        HarmonyMethod cs_sgom_cs_post = new HarmonyMethod(typeof(CombatSaves), "SimGameOptionsMenu_CanSave_Postfix");
                        harmony.Patch(sgom_cs, null, cs_sgom_cs_post, null);

                        MethodInfo sgom_sstt = AccessTools.Method(typeof(SimGameOptionsMenu), "SetSaveTooltip");
                        HarmonyMethod cs_sgom_sstt_postfix = new HarmonyMethod(typeof(CombatSaves), "SimGameOptionsMenu_SetSaveTooltip_Postfix");
                        harmony.Patch(sgom_sstt, null, cs_sgom_sstt_postfix, null);
                    }

                    // Update the pilot stats to have a maximum greater than 10
                    if (Mod.Config.Fixes.WeaponTooltip) {
                        Mod.Log.Info("Activating Fix: WeaponTooltip");
                        MethodInfo tooltipPrefab_Weapon_SetData = AccessTools.Method(typeof(TooltipPrefab_Weapon), "SetData");
                        HarmonyMethod tm_tp_w_sd_post = new HarmonyMethod(typeof(WeaponTooltips), "TooltipPrefab_Weapon_SetData_Postfix");
                        harmony.Patch(tooltipPrefab_Weapon_SetData, null, tm_tp_w_sd_post, null);
                    }

                } catch (Exception e) {
                    Mod.Log.Error($"Failed to load patches due to: {e.Message}");
                    Mod.Log.Error(e.StackTrace);
                    Mod.Log.Error(e.ToString());
                }
            }
            Initialized = true;
        }

    }
}
