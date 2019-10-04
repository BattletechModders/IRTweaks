using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Reflection;

namespace IRTweaks.Modules.Combat {

    public static class CombatFixes {
        static bool Initialized = false;
        public static class State {
            public static void Reset() {
            }
        }

        public static void InitModule(HarmonyInstance harmony) {
            if (!Initialized) {

                try {
                    // Update the pilot stats to have a maximum greater than 10
                    if (Mod.Config.Fixes.ExtendedStats) {
                        MethodInfo pilot_ISV_MI = AccessTools.Method(typeof(Pilot), "InitStatValidators");
                        HarmonyMethod psv_ISV_HM = new HarmonyMethod(typeof(PilotStatValidators), "Pilot_InitStatValidators_Prefix");
                        harmony.Patch(pilot_ISV_MI, null, psv_ISV_HM, null);
                    }

                    // Headshot patches - prevent headshot without appropriate equipment
                    if (Mod.Config.Fixes.PreventCalledShots) {
                        MethodInfo aa_IES_MI = AccessTools.Method(typeof(AbstractActor), "InitEffectStats");
                        HarmonyMethod cs_aa_IES_Post = new HarmonyMethod(typeof(CalledShots), "AbstractActor_InitEffectStats_Postfix");
                        harmony.Patch(aa_IES_MI, null, cs_aa_IES_Post, null);

                        MethodInfo hudMAR_SHA_MI = AccessTools.Method(typeof(HUDMechArmorReadout), "SetHoveredArmor");
                        HarmonyMethod cs_hudMAR_SHA_Post = new HarmonyMethod(typeof(CalledShots), "HUDMechArmorReadout_SetHoveredArmor_Postfix");
                        harmony.Patch(hudMAR_SHA_MI, null, cs_hudMAR_SHA_Post, null);

                        MethodInfo ssf_SCS_MI = AccessTools.Method(typeof(SelectionStateFire), "SetCalledShot", new Type[] { typeof(ArmorLocation) });
                        HarmonyMethod cs_ssf_scs_Post = new HarmonyMethod(typeof(CalledShots), "SelectionStateFire_SetCalledShot_Postfix");
                        harmony.Patch(ssf_SCS_MI, null, cs_ssf_scs_Post, null);

                        MethodInfo th_GMAM = AccessTools.Method(typeof(ToHit), "GetMoraleAttackModifier");
                        HarmonyMethod cs_gmam_post = new HarmonyMethod(typeof(CalledShots), "ToHit_GetMoraleAttackModifier_Postfix");
                        harmony.Patch(th_GMAM, null, cs_gmam_post, null);

                        MethodInfo th_GAM = AccessTools.Method(typeof(ToHit), "GetAllModifiers");
                        HarmonyMethod cs_th_gam_post = new HarmonyMethod(typeof(CalledShots), "ToHit_GetAllModifiers_Postfix");
                        harmony.Patch(th_GAM, null, cs_th_gam_post, null);

                        MethodInfo th_GAMD = AccessTools.Method(typeof(ToHit), "GetAllModifiersDescription");
                        HarmonyMethod cs_th_gamd_post = new HarmonyMethod(typeof(CalledShots), "ToHit_GetAllModifiersDescription_Postfix");
                        harmony.Patch(th_GAMD, null, cs_th_gamd_post, null);

                        MethodInfo chudws_UTTF = AccessTools.Method(typeof(CombatHUDWeaponSlot), "UpdateToolTipsFiring");
                        HarmonyMethod cs_chudws_uttf_post = new HarmonyMethod(typeof(CalledShots), "CombatHUDWeaponSlot_UpdateToolTipsFiring_Postfix");
                        harmony.Patch(chudws_UTTF, null, cs_chudws_uttf_post, null);
                    }


                }
                catch (Exception e) {
                    Mod.Log.Error($"Failed to load patches due to: {e.Message}");
                    Mod.Log.Error(e);
                }
            }
            Initialized = true;
        }


    }
}
