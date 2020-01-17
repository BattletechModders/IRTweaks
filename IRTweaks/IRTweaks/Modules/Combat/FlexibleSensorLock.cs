using BattleTech;
using BattleTech.UI;
using Harmony;

namespace IRTweaks.Modules.Combat {
    public static class FlexibleSensorLock {

        public static void SelectionStateSensorLock_CanActorUseThisState_Postfix(SelectionStateSensorLock __instance, AbstractActor actor, ref bool __result) {
            Mod.Log.Trace("SSSL:CAUTS entered");

            if (actor != null && actor.GetPilot() != null) {
                Pilot pilot = actor?.GetPilot();
                Ability activeAbility = pilot.GetActiveAbility(ActiveAbilityID.SensorLock);
                bool flag = (activeAbility != null && activeAbility.IsAvailable);
                Mod.Log.Debug($"  Pilot has sensorLock:{activeAbility} and abilityIsAvailable:{activeAbility.IsAvailable}");
                __result = flag;
            }
        }

        public static void SelectionStateSensorLock_CreateFiringOrders_Postfix(SelectionStateSensorLock __instance, string button) {
            Mod.Log.Trace("SSSL:CFO entered");

            if (button == "BTN_FireConfirm" && __instance.HasTarget) {
                ModState.SelectionStateSensorLock = __instance;
            }
        }

        public static bool SensorLockSequence_CompleteOrders_Prefix(SensorLockSequence __instance, AbstractActor ___owningActor) {
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

                if (ModState.SelectionStateSensorLock != null) {
                    Mod.Log.Debug($"  Calling clearTargetedActor");
                    Traverse traverse = Traverse.Create(ModState.SelectionStateSensorLock).Method("ClearTargetedActor");
                    traverse.GetValue();

                    //State.SelectionStateSensorLock.BackOut();

                    ModState.SelectionStateSensorLock = null;
                }
            }

            return false;
        }

        public static bool OrderSequence_OnComplete_Prefix(OrderSequence __instance, AbstractActor ___owningActor) {
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
                    var seqFinished = Traverse.Create(__instance).Property("CompletedCallback").GetValue<SequenceFinished>();
                }

                return true;
            } else {
                //Mod.Log.Trace(" Not SensorLockSequence, continuing.");
                return true;
            }
        }

        public static void OrderSequence_ConsumesActivation_Postfix(OrderSequence __instance, ref bool __result, AbstractActor ___owningActor) {
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

        public static bool AIUtil_EvaluateSensorLockQuality_Prefix(ref bool __result, AbstractActor movingUnit, ICombatant target, out float quality) {
            AbstractActor abstractActor = target as AbstractActor;
            if (abstractActor == null || movingUnit.DynamicUnitRole == UnitRole.LastManStanding || !abstractActor.HasActivatedThisRound || abstractActor.IsDead || abstractActor.EvasivePipsTotal == 0) {
                quality = float.MinValue;
                __result = false;
                return false;
            }
            quality = float.MinValue;
            return true;
        }

        public static void Returns_False_Postfix(ref bool __result) {
            __result = false;
        }
    }
}
