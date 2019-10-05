using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using Harmony;
using IRTweaks.Modules.UI;
using System;
using System.Reflection;
using static IRTweaks.Combat;

namespace IRTweaks.Modules.Tooltip {
    public static class UIFixes {
        static bool Initialized = false;

        public static void InitModule(HarmonyInstance harmony) {
            if (!Initialized) {
                try {
                    // Update the pilot stats to have a maximum greater than 10
                    if (Mod.Config.Fixes.WeaponTooltip) {
                        Mod.Log.Info("Activating WeaponTooltip fix");
                        MethodInfo tooltipPrefab_Weapon_SetData = AccessTools.Method(typeof(TooltipPrefab_Weapon), "SetData");
                        HarmonyMethod tm_tp_w_sd_post = new HarmonyMethod(typeof(WeaponTooltips), "TooltipPrefab_Weapon_SetData_Postfix");
                        harmony.Patch(tooltipPrefab_Weapon_SetData, null, tm_tp_w_sd_post, null);
                    }

                    // Updates the purchase and selling dialogs to allow multiple items to be purchased and sold at once
                    if (Mod.Config.Fixes.StoreQOL) {
                        Mod.Log.Info("Activating StoreQOL fix");
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

                    // Disables the ability to save in combat
                    if (Mod.Config.Fixes.DisableCombatSaves) {
                        Mod.Log.Info("Activating DisableCombatSaves fix");
                        MethodInfo sgom_cs = AccessTools.Method(typeof(SimGameOptionsMenu), "CanSave");
                        HarmonyMethod cs_sgom_cs_post = new HarmonyMethod(typeof(CombatSaves), "SimGameOptionsMenu_CanSave_Postfix");
                        harmony.Patch(sgom_cs, null, cs_sgom_cs_post, null);

                        MethodInfo sgom_sstt = AccessTools.Method(typeof(SimGameOptionsMenu), "SetSaveTooltip");
                        HarmonyMethod cs_sgom_sstt_postfix = new HarmonyMethod(typeof(CombatSaves), "SimGameOptionsMenu_SetSaveTooltip_Postfix");
                        harmony.Patch(sgom_sstt, null, cs_sgom_sstt_postfix, null);
                    }

                    if (Mod.Config.Fixes.SpawnProtection) {
                        Mod.Log.Info("Activating SpawnProtection fix");
                        MethodInfo t_au = AccessTools.Method(typeof(Team), "AddUnit");
                        HarmonyMethod sp_t_au_post = new HarmonyMethod(typeof(SpawnProtectionOpts), "Team_AddUnit_Postfix");
                        harmony.Patch(t_au, null, sp_t_au_post, null);

                        MethodInfo td_bnr = AccessTools.Method(typeof(TurnDirector), "BeginNewRound");
                        HarmonyMethod sp_td_bnr_post = new HarmonyMethod(typeof(SpawnProtectionOpts), "TurnDirector_BeginNewRound_Postfix");
                        harmony.Patch(td_bnr, null, sp_td_bnr_post, null);
                    }

                } catch (Exception e) {
                    Mod.Log.Error($"Failed to load patches due to: {e.Message}");
                    Mod.Log.Error(e);
                }
            }
            Initialized = true;
        }

    }
}
