using System.Linq;
using UnityEngine;

namespace IRTweaks.Modules.Combat
{
    // Classes related to Flexible Sensor Locks
    static class FlexibleSensorLockHelper
    {
        public static bool ActorHasFreeSensorLock(AbstractActor actor)
        {
            if (actor == null)
                return false;

            if (!Mod.Config.Combat.FlexibleSensorLock.FreeActionWithStat && 
                !Mod.Config.Combat.FlexibleSensorLock.FreeActionWithAbility)
                return true;

            if (Mod.Config.Combat.FlexibleSensorLock.FreeActionWithStat && actor
                .StatCollection.GetValue<bool>(Mod.Config.Combat.FlexibleSensorLock.FreeActionStatName))
                return true;

            Pilot pilot = actor.GetPilot();
            Mod.Log.Debug?.Write($"pilot = [{pilot}]\r\n"
                          + $"abilities = [{string.Join(",", pilot?.Abilities.Select(ability => ability.Def.Id))}]");
            if (Mod.Config.Combat.FlexibleSensorLock.FreeActionWithAbility && 
                (pilot?.Abilities?.Exists(ability => ability.Def.Id == Mod.Config.Abilities.FlexibleSensorLockId) ?? false))
                return true;

            return false;
        }
    }

    // --- SelectionStateSensorLock classes ---
    [HarmonyPatch(typeof(SelectionStateSensorLock), "ConsumesFiring", MethodType.Getter)]
    static class SelectionStateSensorLock_ConsumesFiring
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock;

        [HarmonyPostfix]
        static void Postfix(ref bool __result)
        {
            __result = false;
        }

    }

    [HarmonyPatch(typeof(SelectionStateSensorLock), "ConsumesMovement", MethodType.Getter)]
    static class SelectionStateSensorLock_ConsumesMovement
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock;

        [HarmonyPostfix]
        static void Postfix(ref bool __result)
        {
            __result = false;
        }

    }

    [HarmonyPatch(typeof(SelectionStateSensorLock), "CanActorUseThisState")]
    static class SelectionStateSensorLock_CanActorUseThisState
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock;

        [HarmonyPostfix]
        static void Postfix(SelectionStateSensorLock __instance, AbstractActor actor, ref bool __result)
        {
            Mod.Log.Trace?.Write("SSSL:CAUTS entered");

            if (FlexibleSensorLockHelper.ActorHasFreeSensorLock(actor))
            {
                Pilot pilot = actor?.GetPilot();
                Ability activeAbility = pilot.GetActiveAbility(ActiveAbilityID.SensorLock);
                bool flag = (activeAbility != null && activeAbility.IsAvailable);
                Mod.Log.Debug?.Write($"  Pilot has sensorLock:{activeAbility} and abilityIsAvailable:{activeAbility.IsAvailable}");
                __result = flag;
            }
        }

    }

    [HarmonyPatch(typeof(SelectionStateSensorLock), "CreateFiringOrders")]
    static class SelectionStateSensorLock_CreateFiringOrder
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock;

        [HarmonyPostfix]
        static void Postfix(SelectionStateSensorLock __instance, string button)
        {
            Mod.Log.Trace?.Write("SSSL:CFO entered");

            if (button == "BTN_FireConfirm" && __instance.HasTarget)
            {
                ModState.SelectionStateSensorLock = __instance;
            }
        }

    }

    // --- SensorLockSequence classes ---
    [HarmonyPatch(typeof(SensorLockSequence), "ConsumesFiring", MethodType.Getter)]
    static class SensorLockSequence_ConsumesFiring
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock;

        [HarmonyPostfix]
        static void Postfix(ref bool __result)
        {
            __result = false;
        }

    }

    [HarmonyPatch(typeof(SensorLockSequence), "ConsumesMovement", MethodType.Getter)]
    static class SensorLockSequence_ConsumesMovement
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock;

        [HarmonyPostfix]
        static void Postfix(ref bool __result)
        {
            __result = false;
        }

    }

    [HarmonyPatch(typeof(SensorLockSequence), "CompleteOrders")]
    static class SensorLockSequence_CompleteOrders
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock;

        [HarmonyPrefix]
        static void Prefix(AbstractActor ___owningActor, ref bool __runOriginal)
        {
            if (!__runOriginal) return;

            Mod.Log.Trace?.Write("SLS:CO entered, aborting invocation");

            // Force the ability to be on cooldown
            if (FlexibleSensorLockHelper.ActorHasFreeSensorLock(___owningActor))
            {
                Pilot pilot = ___owningActor.GetPilot();
                Ability ability = pilot.GetActiveAbility(ActiveAbilityID.SensorLock);
                Mod.Log.Debug?.Write($"  On sensor lock complete, cooldown is:{ability.CurrentCooldown}");
                if (ability.CurrentCooldown < 1)
                {
                    ability.ActivateCooldown();
                }

                Mod.Log.Debug?.Write($"  Clearing all sequences");

                if (ModState.SelectionStateSensorLock != null)
                {
                    Mod.Log.Debug?.Write($"  Calling clearTargetedActor");
                    ModState.SelectionStateSensorLock.ClearTargetedActor();
                    //State.SelectionStateSensorLock.BackOut();
                    ModState.SelectionStateSensorLock = null;
                }
            }

            __runOriginal = false;
        }

    }

    // --- OrderSequence classes ---
    [HarmonyPatch(typeof(OrderSequence), "OnComplete")]
    static class OrderSequence_OnComplete
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock;

        [HarmonyPrefix]
        static void Prefix(OrderSequence __instance, AbstractActor ___owningActor, ref bool __runOriginal)
        {
            if (!__runOriginal) return;

            if ((__instance is SensorLockSequence || 
                (__instance is ActiveProbeSequence && Mod.Config.Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe))
                && FlexibleSensorLockHelper.ActorHasFreeSensorLock(___owningActor))
            {
                Mod.Log.Trace?.Write($"OS:OC entered, cm:{__instance.ConsumesMovement} cf:{__instance.ConsumesFiring}");
                Mod.Log.Trace?.Write($"    oa:{___owningActor.DisplayName}_{___owningActor.GetPilot().Name} hasFired:{___owningActor.HasFiredThisRound} " +
                    $"hasMoved:{___owningActor.HasMovedThisRound} hasActivated:{___owningActor.HasActivatedThisRound}");
                Mod.Log.Trace?.Write($"    ca:{__instance.ConsumesActivation} fae:{__instance.ForceActivationEnd}");

                Mod.Log.Trace?.Write(" SensorLockSequence, skipping.");

                __instance.ClearShownList();
                __instance.ClearCamera();
                __instance.ClearFocalPoint();
                if (__instance.CompletedCallback != null)
                {
                    Mod.Log.Trace?.Write(" Getting SequenceFinished");
                    __instance.CompletedCallback.Invoke();
                }
            }
            __runOriginal = true;
        }

    }

    [HarmonyPatch(typeof(OrderSequence), "ConsumesActivation", MethodType.Getter)]
    static class OrderSequence_ConsumesActivation
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock;

        [HarmonyPostfix]
        static void Postfix(OrderSequence __instance, ref bool __result, AbstractActor ___owningActor)
        {
            if (__instance is SensorLockSequence || (__instance is ActiveProbeSequence && Mod.Config.Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe))
            {
                __result = true;
                if (FlexibleSensorLockHelper.ActorHasFreeSensorLock(___owningActor))
                {
                    __result = false;
                }
                Mod.Log.Debug?.Write($" OrderSequence_ConsumesActivation_Postfix - returning [{__result}].");
            }
        }

    }

    // --- AIUtil classes ---
    [HarmonyPatch(typeof(AIUtil), "EvaluateSensorLockQuality")]
    static class AIUtil_EvaluateSensorLockQuality
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock;

        [HarmonyPrefix]
        static void Prefix(ref bool __result, AbstractActor movingUnit, ICombatant target, out float quality, ref bool __runOriginal)
        {
            if (!__runOriginal)
            {
                quality = float.MinValue;
                return;
            }

            AbstractActor abstractActor = target as AbstractActor;
            if (abstractActor == null || movingUnit.DynamicUnitRole == UnitRole.LastManStanding || !abstractActor.HasActivatedThisRound || abstractActor.IsDead || abstractActor.EvasivePipsTotal == 0)
            {
                __result = false;
                __runOriginal = false;
            }
            else
            {
                __runOriginal = true;
            }
            quality = float.MinValue;
        }

    }

    // --- AIUtil classes ---
    [HarmonyPatch(typeof(Mech), "InitStats")]
    static class Mech_InitStats
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock;

        [HarmonyPrefix]
        static void Prefix(AbstractActor __instance, ref bool __runOriginal)
        {
            if (!__runOriginal) return;

            if (!__instance.Combat.IsLoadingFromSave && Mod.Config.Combat.FlexibleSensorLock.FreeActionWithStat)
            {
                __instance.StatCollection.AddStatistic(Mod.Config.Combat.FlexibleSensorLock.FreeActionStatName, false);
            }
        }
    }

    // --- SelectionStateActiveProbe classes ---
    [HarmonyPatch(typeof(SelectionStateActiveProbe), "ConsumesFiring", MethodType.Getter)]
    static class SelectionStateActiveProbe_ConsumesFiring
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock && Mod.Config.Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe;

        [HarmonyPostfix]
        static void Postfix(ref bool __result)
        {
            __result = false;
        }

    }

    [HarmonyPatch(typeof(SelectionStateActiveProbe), "ConsumesMovement", MethodType.Getter)]
    static class SelectionStateActiveProbe_ConsumesMovement
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock && Mod.Config.Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe;

        [HarmonyPostfix]
        static void Postfix(ref bool __result)
        {
            __result = false;
        }

    }

    [HarmonyPatch(typeof(SelectionStateActiveProbe), "CanActorUseThisState")]
    static class SelectionStateActiveProbe_CanActorUseThisState
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock && Mod.Config.Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe;

        [HarmonyPostfix]
        static void Postfix(SelectionStateActiveProbe __instance, AbstractActor actor, ref bool __result)
        {
            Mod.Log.Trace?.Write("SSAP:CAUTS entered");

            if (FlexibleSensorLockHelper.ActorHasFreeSensorLock(actor))
            {
                Ability activeAbility = actor.ComponentAbilities.Find((Ability x) => x.Def.Targeting == AbilityDef.TargetingType.ActiveProbe);
                bool flag = (activeAbility != null && activeAbility.IsAvailable);
                Mod.Log.Debug?.Write($"  Pilot has sensorLock:{activeAbility} and abilityIsAvailable:{activeAbility.IsAvailable}");
                __result = flag;
            }
        }

    }

    [HarmonyPatch(typeof(SelectionStateActiveProbe), "CreateFiringOrders")]
    static class SelectionStateActiveProbe_CreateFiringOrder
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock && Mod.Config.Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe;


        [HarmonyPostfix]
        static void Postfix(SelectionStateActiveProbe __instance, string button)
        {
            Mod.Log.Trace?.Write("SSSL:CFO entered");

            if (button == "BTN_FireConfirm" && __instance.HasTarget)
            {
                ModState.SelectionStateActiveProbe = __instance;
            }
        }

    }

    // --- ActiveProbeSequence classes ---
    [HarmonyPatch(typeof(ActiveProbeSequence), "ConsumesFiring", MethodType.Getter)]
    static class ActiveProbeSequence_ConsumesFiring
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock && Mod.Config.Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe;


        [HarmonyPostfix]
        static void Postfix(ref bool __result)
        {
            __result = false;
        }

    }

    [HarmonyPatch(typeof(ActiveProbeSequence), "ConsumesMovement", MethodType.Getter)]
    static class ActiveProbeSequence_ConsumesMovement
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock && Mod.Config.Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe;


        [HarmonyPostfix]
        static void Postfix(ref bool __result)
        {
            __result = false;
        }

    }

    [HarmonyPatch(typeof(ActiveProbeSequence), "CompleteOrders")]
    static class ActiveProbeSequence_CompleteOrders
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.FlexibleSensorLock && Mod.Config.Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe;


        [HarmonyPrefix]
        static void Prefix(ActiveProbeSequence __instance, AbstractActor ___owningActor, ParticleSystem ___probeParticles, ref bool __runOriginal)
        {
            if (!__runOriginal) return;

            Mod.Log.Trace?.Write("SLS:CO entered, aborting invocation");

            // Force the ability to be on cooldown
            if (FlexibleSensorLockHelper.ActorHasFreeSensorLock(___owningActor))
            {
                if (___probeParticles != null)
                {
                    ___probeParticles.Stop(true);
                    __instance.Combat.DataManager.PoolGameObject(__instance.Combat.Constants.VFXNames.active_probe_effect, ___probeParticles.gameObject);
                }
                WwiseManager.PostEvent(AudioEventList_activeProbe.activeProbe_stop, WwiseManager.GlobalAudioObject, null, null);

                Mod.Log.Debug?.Write($"  Clearing all sequences");

                if (ModState.SelectionStateActiveProbe != null)
                {
                    Mod.Log.Debug?.Write($"  Calling clearTargetedActor");
                    ModState.SelectionStateActiveProbe.RefreshPossibleTargets();

                    ModState.SelectionStateActiveProbe = null;
                }

                __runOriginal = false;
            }
        }
    }
}
