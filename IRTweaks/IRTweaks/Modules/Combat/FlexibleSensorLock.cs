using BattleTech;
using BattleTech.UI;
using Harmony;

namespace IRTweaks.Modules.Combat {
    using System.Linq;
    using UnityEngine;

    public static class FlexibleSensorLock {

        public static void SelectionStateSensorLock_CanActorUseThisState_Postfix(SelectionStateSensorLock __instance, AbstractActor actor, ref bool __result) {
            Mod.Log.Trace?.Write("SSSL:CAUTS entered");

            if (ActorHasFreeSensorLock(actor)) {
                Pilot pilot = actor?.GetPilot();
                Ability activeAbility = pilot.GetActiveAbility(ActiveAbilityID.SensorLock);
                bool flag = (activeAbility != null && activeAbility.IsAvailable);
                Mod.Log.Debug?.Write($"  Pilot has sensorLock:{activeAbility} and abilityIsAvailable:{activeAbility.IsAvailable}");
                __result = flag;
            }
        }

        public static void SelectionStateSensorLock_CreateFiringOrders_Postfix(SelectionStateSensorLock __instance, string button) {
            Mod.Log.Trace?.Write("SSSL:CFO entered");

            if (button == "BTN_FireConfirm" && __instance.HasTarget) {
                ModState.SelectionStateSensorLock = __instance;
            }
        }

        public static bool SensorLockSequence_CompleteOrders_Prefix(SensorLockSequence __instance, AbstractActor ___owningActor) {
            Mod.Log.Trace?.Write("SLS:CO entered, aborting invocation");
            //Mod.Log.Trace?.Write($"  oa:{___owningActor.DisplayName}_{___owningActor.GetPilot().Name} hasFired:{___owningActor.HasFiredThisRound} hasMoved:{___owningActor.HasMovedThisRound} hasActivated:{___owningActor.HasActivatedThisRound}");

            // Force the ability to be on cooldown
            if (ActorHasFreeSensorLock(___owningActor)) {
                Pilot pilot = ___owningActor.GetPilot();
                Ability ability = pilot.GetActiveAbility(ActiveAbilityID.SensorLock);
                Mod.Log.Debug?.Write($"  On sensor lock complete, cooldown is:{ability.CurrentCooldown}");
                if (ability.CurrentCooldown < 1) {
                    ability.ActivateCooldown();
                }

                Mod.Log.Debug?.Write($"  Clearing all sequences");

                if (ModState.SelectionStateSensorLock != null) {
                    Mod.Log.Debug?.Write($"  Calling clearTargetedActor");
                    Traverse traverse = Traverse.Create(ModState.SelectionStateSensorLock).Method("ClearTargetedActor");
                    traverse.GetValue();

                    //State.SelectionStateSensorLock.BackOut();

                    ModState.SelectionStateSensorLock = null;
                }
            }

            return false;
        }

        public static bool OrderSequence_OnComplete_Prefix(OrderSequence __instance, AbstractActor ___owningActor) {
            if ((__instance is SensorLockSequence || (__instance is ActiveProbeSequence && Mod.Config.Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe) ) && ActorHasFreeSensorLock(___owningActor)) {
                Mod.Log.Trace?.Write($"OS:OC entered, cm:{__instance.ConsumesMovement} cf:{__instance.ConsumesFiring}");
                Mod.Log.Trace?.Write($"    oa:{___owningActor.DisplayName}_{___owningActor.GetPilot().Name} hasFired:{___owningActor.HasFiredThisRound} hasMoved:{___owningActor.HasMovedThisRound} hasActivated:{___owningActor.HasActivatedThisRound}");
                Mod.Log.Trace?.Write($"    ca:{__instance.ConsumesActivation} fae:{__instance.ForceActivationEnd}");

                Mod.Log.Trace?.Write(" SensorLockSequence, skipping.");

                Mod.Log.Trace?.Write(" Clearing shown list");
                Traverse.Create(__instance).Method("ClearShownList").GetValue();
                Mod.Log.Trace?.Write(" Clearing camera");
                Traverse.Create(__instance).Method("ClearCamera").GetValue();
                Mod.Log.Trace?.Write(" Clearing focal point");
                Traverse.Create(__instance).Method("ClearFocalPoint").GetValue();
                if (__instance.CompletedCallback != null) {
                    Mod.Log.Trace?.Write(" Getting SequenceFinished");
                    var seqFinished = Traverse.Create(__instance).Property("CompletedCallback").GetValue<SequenceFinished>();
                }

                return true;
            } else {
                //Mod.Log.Trace?.Write(" Not SensorLockSequence, continuing.");
                return true;
            }
        }

        public static void OrderSequence_ConsumesActivation_Postfix(OrderSequence __instance, ref bool __result, AbstractActor ___owningActor) {
            if (__instance is SensorLockSequence || (__instance is ActiveProbeSequence && Mod.Config.Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe))
            {
                __result = true;
                if (ActorHasFreeSensorLock(___owningActor))
                {
                    __result = false;
                }
                Mod.Log.Debug?.Write($" OrderSequence_ConsumesActivation_Postfix - returning [{__result}].");
            }
        }

        private static bool ActorHasFreeSensorLock(AbstractActor actor)
        {
            if (actor == null)
                return false;

            if (!Mod.Config.Combat.FlexibleSensorLock.FreeActionWithStat && !Mod.Config.Combat.FlexibleSensorLock.FreeActionWithAbility)
                return true;

            if (Mod.Config.Combat.FlexibleSensorLock.FreeActionWithStat && actor.StatCollection.GetValue<bool>(Mod.Config.Combat.FlexibleSensorLock.FreeActionStatName))
                return true;

            Pilot pilot = actor.GetPilot();
            Mod.Log.Debug?.Write($"pilot = [{pilot}]\r\n"
                          + $"abilities = [{string.Join(",", pilot?.Abilities.Select(ability => ability.Def.Id))}]");
            if (Mod.Config.Combat.FlexibleSensorLock.FreeActionWithAbility && (pilot?.Abilities?.Exists(ability => ability.Def.Id == Mod.Config.Abilities.FlexibleSensorLockId) ?? false))
                return true;
            return false;
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

        public static void AbstractActor_InitStats_Prefix(AbstractActor __instance)
        {
            if (!__instance.Combat.IsLoadingFromSave && Mod.Config.Combat.FlexibleSensorLock.FreeActionWithStat)
            {
                __instance.StatCollection.AddStatistic(Mod.Config.Combat.FlexibleSensorLock.FreeActionStatName, false);
            }
        }

        public static void SelectionStateActiveProbe_CanActorUseThisState_Postfix(SelectionStateActiveProbe __instance, AbstractActor actor, ref bool __result)
        {
            Mod.Log.Trace?.Write("SSAP:CAUTS entered");

            if (ActorHasFreeSensorLock(actor))
            {
                Ability activeAbility = actor.ComponentAbilities.Find((Ability x) => x.Def.Targeting == AbilityDef.TargetingType.ActiveProbe);
                bool flag = (activeAbility != null && activeAbility.IsAvailable);
                Mod.Log.Debug?.Write($"  Pilot has sensorLock:{activeAbility} and abilityIsAvailable:{activeAbility.IsAvailable}");
                __result = flag;
            }
        }

        public static void SelectionStateActiveProbe_CreateFiringOrders_Postfix(SelectionStateActiveProbe __instance, string button)
        {
            Mod.Log.Trace?.Write("SSSL:CFO entered");

            if (button == "BTN_FireConfirm" && __instance.HasTarget)
            {
                ModState.SelectionStateActiveProbe = __instance;
            }
        }

        public static bool ActiveProbeSequence_CompleteOrders_Prefix(ActiveProbeSequence __instance, AbstractActor ___owningActor, ParticleSystem ___probeParticles)
        {
            Mod.Log.Trace?.Write("SLS:CO entered, aborting invocation");
            //Mod.Log.Trace?.Write($"  oa:{___owningActor.DisplayName}_{___owningActor.GetPilot().Name} hasFired:{___owningActor.HasFiredThisRound} hasMoved:{___owningActor.HasMovedThisRound} hasActivated:{___owningActor.HasActivatedThisRound}");

            // Force the ability to be on cooldown
            if (ActorHasFreeSensorLock(___owningActor))
            {
                CombatGameState ___Combat = Traverse.Create(__instance).Property("Combat").GetValue<CombatGameState>();
                if (___probeParticles != null)
                {
                    ___probeParticles.Stop(true);
                    ___Combat.DataManager.PoolGameObject(___Combat.Constants.VFXNames.active_probe_effect, ___probeParticles.gameObject);
                }
                WwiseManager.PostEvent(AudioEventList_activeProbe.activeProbe_stop, WwiseManager.GlobalAudioObject, null, null);

                Mod.Log.Debug?.Write($"  Clearing all sequences");

                if (ModState.SelectionStateActiveProbe != null)
                {
                    Mod.Log.Debug?.Write($"  Calling clearTargetedActor");
                    Traverse traverse = Traverse.Create(ModState.SelectionStateActiveProbe).Method("RefreshPossibleTargets");
                    traverse.GetValue();

                    //State.SelectionStateSensorLock.BackOut();

                    ModState.SelectionStateActiveProbe = null;
                }
                return false;
            }

            return true;
        }
    }
}
