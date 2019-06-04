using BattleTech;
using BattleTech.UI;
using Harmony;

namespace IRTweaks {

    [HarmonyPatch(typeof(SelectionStateSensorLock))]
    [HarmonyPatch("ConsumesFiring", MethodType.Getter)]
    public static class SelectionStateSensorLock_ConsumesFiring {

        public static void Postfix(SelectionStateSensorLock __instance, ref bool __result) {
            Mod.Log.Trace("SSSL:CF:GET entered");
            __result = false;
        }
    }

    [HarmonyPatch(typeof(SelectionStateSensorLock))]
    [HarmonyPatch("ConsumesMovement", MethodType.Getter)]
    public static class SelectionStateSensorLock_ConsumesMovement {

        public static void Postfix(SelectionStateSensorLock __instance, ref bool __result) {
            Mod.Log.Trace("SSSL:CM:GET entered");
            __result = false;
        }
    }

    [HarmonyPatch(typeof(SelectionStateSensorLock), "CanActorUseThisState")]
    public static class SelectionStateSensorLock_CanActorUseThisState {

        public static void Postfix(SelectionStateSensorLock __instance, AbstractActor actor, ref bool __result) {
            Mod.Log.Trace("SSSL:CAUTS entered");

            if (actor != null && actor.GetPilot() != null) {
                Pilot pilot = actor?.GetPilot();
                Ability activeAbility = pilot.GetActiveAbility(ActiveAbilityID.SensorLock);
                bool flag = (activeAbility != null && activeAbility.IsAvailable);
                Mod.Log.Debug($"  Pilot has sensorLock:{activeAbility} and abilityIsAvailable:{activeAbility.IsAvailable}");
                __result = flag;
            }
        }
    }

    [HarmonyPatch(typeof(SelectionStateSensorLock), "CreateFiringOrders")]
    public static class SelectionStateSensorLock_CreateFiringOrders {

        public static void Postfix(SelectionStateSensorLock __instance, string button) {
            Mod.Log.Trace("SSSL:CFO entered");

            if (button == "BTN_FireConfirm" && __instance.HasTarget) {
                State.SelectionStateSensorLock = __instance;
            }
        }
    }


    [HarmonyPatch(typeof(SelectionStateSensorLock), "OnInactivate")]
    public static class SelectionStateSensorLock_OnInactivate {

        public static void Postfix(SelectionStateSensorLock __instance) {
            Mod.Log.Trace("SSSL:OI entered");
        }
    }

    [HarmonyPatch(typeof(SensorLockSequence))]
    [HarmonyPatch("CompleteOrders")]
    public static class SensorLockSequence_CompleteOrders {

        public static bool Prefix(SensorLockSequence __instance, AbstractActor ___owningActor) {
            Mod.Log.Trace("SLS:CO entered, aborting invocation");
            //Mod.Log.Trace($"  oa:{___owningActor.DisplayName}_{___owningActor.GetPilot().Name} hasFired:{___owningActor.HasFiredThisRound} hasMoved:{___owningActor.HasMovedThisRound} hasActivated:{___owningActor.HasActivatedThisRound}");

            // Force the ability to be on cooldown
            if (___owningActor != null && ___owningActor.GetPilot() != null) {
                Pilot pilot = ___owningActor.GetPilot();
                Ability ability = pilot.GetActiveAbility(ActiveAbilityID.SensorLock);
                Mod.Log.Debug($"  On sensor lock complete, cooldown is:{ability.CurrentCooldown}");
                if (ability.CurrentCooldown < 1) {
                    ability.ActivateCooldown();
                }

                Mod.Log.Debug($"  Clearing all sequences");

                if (State.SelectionStateSensorLock != null) {
                    Mod.Log.Debug($"  Calling clearTargetedActor");
                    Traverse traverse = Traverse.Create(State.SelectionStateSensorLock).Method("ClearTargetedActor");
                    traverse.GetValue();

                    //State.SelectionStateSensorLock.BackOut();

                    State.SelectionStateSensorLock = null;

                }

            }

            return false;
        }
    }

    [HarmonyPatch(typeof(SensorLockSequence))]
    [HarmonyPatch("ConsumesFiring", MethodType.Getter)]
    public static class SensorLockSequence_ConsumesFiring {

        public static void Postfix(SensorLockSequence __instance, ref bool __result, AbstractActor ___owningActor) {
            Mod.Log.Trace("SLS:CF:GET entered.");
            Mod.Log.Trace($"    oa:{___owningActor.DisplayName}_{___owningActor.GetPilot().Name} hasFired:{___owningActor.HasFiredThisRound} hasMoved:{___owningActor.HasMovedThisRound} hasActivated:{___owningActor.HasActivatedThisRound}");
            __result = false;
        }
    }

    [HarmonyPatch(typeof(SensorLockSequence))]
    [HarmonyPatch("ConsumesMovement", MethodType.Getter)]
    public static class SensorLockSequence_ConsumesMovement {

        public static void Postfix(SensorLockSequence __instance, ref bool __result, AbstractActor ___owningActor) {
            Mod.Log.Trace("SLS:CM:GET entered.");
            Mod.Log.Trace($"    oa:{___owningActor.DisplayName}_{___owningActor.GetPilot().Name} hasFired:{___owningActor.HasFiredThisRound} hasMoved:{___owningActor.HasMovedThisRound} hasActivated:{___owningActor.HasActivatedThisRound}");
            __result = false;
        }
    }

    [HarmonyPatch(typeof(OrderSequence))]
    [HarmonyPatch("OnComplete")]
    public static class OrderSequence_OnComplete {

        public static bool Prefix(OrderSequence __instance, AbstractActor ___owningActor) {

            if (__instance is SensorLockSequence) {
                Mod.Log.Trace($"OS:OC entered, cm:{__instance.ConsumesMovement} cf:{__instance.ConsumesFiring}");
                Mod.Log.Trace($"    oa:{___owningActor.DisplayName}_{___owningActor.GetPilot().Name} hasFired:{___owningActor.HasFiredThisRound} hasMoved:{___owningActor.HasMovedThisRound} hasActivated:{___owningActor.HasActivatedThisRound}");
                Mod.Log.Trace($"    ca:{__instance.ConsumesActivation} fae:{__instance.ForceActivationEnd}");

                Mod.Log.Trace(" SensorLockSequence, skipping.");

                Mod.Log.Trace(" Clearing shown list");
                Traverse.Create(__instance).Method("ClearShownList").GetValue();
                Mod.Log.Trace(" Clearing camera");
                Traverse.Create(__instance).Method("ClearCamera").GetValue();
                Mod.Log.Trace(" Clearing focal point");
                Traverse.Create(__instance).Method("ClearFocalPoint").GetValue();
                if (__instance.CompletedCallback != null) {
                    Mod.Log.Trace(" Getting SequenceFinished");
                    var seqFinished  = Traverse.Create(__instance).Property("CompletedCallback").GetValue<SequenceFinished>();
                }

                return true;
            } else {
                //Mod.Log.Trace(" Not SensorLockSequence, continuing.");
                return true;
            }

        }
    }

    [HarmonyPatch(typeof(OrderSequence))]
    [HarmonyPatch("ConsumesActivation", MethodType.Getter)]
    public static class OrderSequence_ConsumesActivation {

        public static void Postfix(OrderSequence __instance, ref bool __result, AbstractActor ___owningActor) {

            if (__instance is SensorLockSequence) {
                //Mod.Log.Trace($"SLS:CA entered, cm:{__instance.ConsumesMovement} cf:{__instance.ConsumesFiring}");
                //Mod.Log.Trace($"    oa:{___owningActor.DisplayName}_{___owningActor.GetPilot().Name} hasFired:{___owningActor.HasFiredThisRound} hasMoved:{___owningActor.HasMovedThisRound} hasActivated:{___owningActor.HasActivatedThisRound}");
                if (___owningActor.HasFiredThisRound && ___owningActor.HasMovedThisRound) {
                    Mod.Log.Debug(" Owner has moved and fired, returning true.");
                    __result = false;
                } else {
                    //Mod.Log.Trace(" Returning false");
                    __result = false;
                }
            } 

        }
    }

    [HarmonyPatch(typeof(AIUtil), "EvaluateSensorLockQuality")]
    public static class AIUtil_EvaluateSensorLockQuality {

        public static bool Prefix(ref bool __result, AbstractActor movingUnit, ICombatant target, out float quality) {

            AbstractActor abstractActor = target as AbstractActor;
            if (abstractActor == null || movingUnit.DynamicUnitRole == UnitRole.LastManStanding || !abstractActor.HasActivatedThisRound || abstractActor.IsDead || abstractActor.EvasivePipsTotal == 0) {
                quality = float.MinValue;
                __result = false;
                return false;
            }
            quality = float.MinValue;
            return true;
        }
    }

}
