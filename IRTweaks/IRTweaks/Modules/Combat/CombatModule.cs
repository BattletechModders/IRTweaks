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
                        Mod.Log.Info?.Write("Activating Fix: ExtendedStats");
                        MethodInfo pilot_ISV_MI = AccessTools.Method(typeof(Pilot), "InitStatValidators");
                        HarmonyMethod psv_ISV_HM = new HarmonyMethod(typeof(PilotStatValidators), "Pilot_InitStatValidators_Prefix");
                        harmony.Patch(pilot_ISV_MI, psv_ISV_HM, null, null);
                    }

                    if (Mod.Config.Fixes.CalledShotTweaks)
                        Mod.Log.Info?.Write("Activating Fix: CalledShotTweaks");

                    if (Mod.Config.Fixes.FlexibleSensorLock) {
                        Mod.Log.Info?.Write("Activating Fix: FlexibleSensorLock");
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

                    if (Mod.Config.Fixes.SpawnProtection) {
                        Mod.Log.Info?.Write("Activating Fix: SpawnProtection");

                        MethodInfo t_au = AccessTools.Method(typeof(Team), "AddUnit");
                        HarmonyMethod sp_t_au_post = new HarmonyMethod(typeof(SpawnProtection), "Team_AddUnit_Postfix");
                        harmony.Patch(t_au, null, sp_t_au_post, null);

                        MethodInfo td_bnr = AccessTools.Method(typeof(TurnDirector), "BeginNewRound");
                        HarmonyMethod sp_td_bnr_post = new HarmonyMethod(typeof(SpawnProtection), "TurnDirector_BeginNewRound_Postfix");
                        harmony.Patch(td_bnr, null, sp_td_bnr_post, null);
                    }

                }
                catch (Exception e) {
                    Mod.Log.Error?.Write($"Failed to load patches due to: {e.Message}");
                    Mod.Log.Error?.Write(e);
                }
            }
            Initialized = true;
        }


    }
}
