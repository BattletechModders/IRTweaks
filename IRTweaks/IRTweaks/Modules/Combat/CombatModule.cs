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
                        Mod.Log.Info("Activating ExtendedStats fix");
                        MethodInfo pilot_ISV_MI = AccessTools.Method(typeof(Pilot), "InitStatValidators");
                        HarmonyMethod psv_ISV_HM = new HarmonyMethod(typeof(PilotStatValidators), "Pilot_InitStatValidators_Prefix");
                        harmony.Patch(pilot_ISV_MI, null, psv_ISV_HM, null);
                    }

                    // Headshot patches - prevent headshot without appropriate equipment
                    if (Mod.Config.Fixes.PreventCalledShots) {
                        Mod.Log.Info("Activating PreventCalledShots fix");
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

                    if (Mod.Config.Fixes.FlexibleSensorLock) {
                        Mod.Log.Info("Activating FlexibleSensorLock fix");
                        // TODO: Add in sensor probe sequence. Limit to once per turn.
                        HarmonyMethod slc_r_f_post = new HarmonyMethod(typeof(FlexibleSensorLock), "Returns_False_Postfix");

                        PropertyInfo sssl_cf = AccessTools.Property(typeof(SelectionStateSensorLock), "ConsumesFiring");
                        harmony.Patch(sssl_cf.GetGetMethod(false), null, slc_r_f_post, null);

                        PropertyInfo sssl_cm = AccessTools.Property(typeof(SelectionStateSensorLock), "ConsumesMovement");
                        harmony.Patch(sssl_cf.GetGetMethod(false), null, slc_r_f_post, null);

                        MethodInfo sssl_cauts = AccessTools.Method(typeof(SelectionStateSensorLock), "CanActorUseThisState");
                        HarmonyMethod fsl_sssl_cauts_post = new HarmonyMethod(typeof(FlexibleSensorLock), "SelectionStateSensorLock_CanActorUseThisState_Postfix");
                        harmony.Patch(sssl_cauts, null, fsl_sssl_cauts_post, null);

                        MethodInfo sssl_cfo = AccessTools.Method(typeof(SelectionStateSensorLock), "CreateFiringOrders");
                        HarmonyMethod fsl_sssl_cfo_post = new HarmonyMethod(typeof(FlexibleSensorLock), "SelectionStateSensorLock_CreateFiringOrders_Postfix");
                        harmony.Patch(sssl_cfo, null, fsl_sssl_cfo_post, null);

                        MethodInfo sls_co = AccessTools.Method(typeof(SensorLockSequence), "CompleteOrders");
                        HarmonyMethod fsl_sls_co_pre = new HarmonyMethod(typeof(FlexibleSensorLock), "SensorLockSequence_CompleteOrders_Prefix");
                        harmony.Patch(sls_co, fsl_sls_co_pre, null, null);

                        PropertyInfo sls_cf = AccessTools.Property(typeof(SensorLockSequence), "ConsumesFiring");
                        harmony.Patch(sls_cf.GetGetMethod(false), null, slc_r_f_post, null);

                        PropertyInfo sls_cm = AccessTools.Property(typeof(SensorLockSequence), "ConsumesMovement");
                        harmony.Patch(sls_cm.GetGetMethod(false), null, slc_r_f_post, null);

                        MethodInfo os_oc = AccessTools.Method(typeof(OrderSequence), "OnComplete");
                        HarmonyMethod fsl_oc_pre = new HarmonyMethod(typeof(FlexibleSensorLock), "OrderSequence_OnComplete_Prefix");
                        harmony.Patch(os_oc, fsl_oc_pre, null, null);

                        PropertyInfo os_ca = AccessTools.Property(typeof(OrderSequence), "ConsumesActivation");
                        HarmonyMethod fsl_os_ca_post = new HarmonyMethod(typeof(FlexibleSensorLock), "OrderSequence_ConsumesActivation_Postfix");
                        harmony.Patch(os_ca.GetGetMethod(false), null, fsl_os_ca_post, null);

                        MethodInfo aiu_eslq = AccessTools.Method(typeof(AIUtil), "EvaluateSensorLockQuality");
                        HarmonyMethod fsl_aiu_eslq_pre = new HarmonyMethod(typeof(FlexibleSensorLock), "AIUtil_EvaluateSensorLockQuality_Prefix");
                        harmony.Patch(aiu_eslq, fsl_aiu_eslq_pre, null, null);
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
