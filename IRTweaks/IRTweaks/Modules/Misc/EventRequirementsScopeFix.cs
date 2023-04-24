using BattleTech;
using BattleTech.Data;
using HBS.Collections;
using System;
using System.Collections.Generic;

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(SimGameState), "GetStatsByScope")]
    public static class SimGameState_GetStatsByScope
    {
        static bool Prepare() => Mod.Config.Fixes.EventRequirementsScopeFix;
        public static void Postfix(SimGameState __instance, EventScope scope, ref StatCollection __result)
        {
            switch (scope)
            {
                case EventScope.SecondaryMechWarrior:
                    __result = ((Pilot)__instance.Context.GetObject(GameContextObjectTagEnum.SecondaryMechWarrior))?.StatCollection;
                    return;
                case EventScope.SecondaryMech:
                    __result = ((MechDef)__instance.Context.GetObject(GameContextObjectTagEnum.TargetUnit))?.Stats;
                    return;
                case EventScope.TertiaryMechWarrior:
                    __result = ((Pilot)__instance.Context.GetObject(GameContextObjectTagEnum.TertiaryMechWarrior))?.StatCollection;
                    return;
                case EventScope.DeadMechWarrior:
                    __result = ((Pilot)__instance.Context.GetObject(GameContextObjectTagEnum.DeadMechWarrior))?.StatCollection;
                    return;
            }
        }
    }
    [HarmonyPatch(typeof(SimGameState), "GetTagsByScope")]
    public static class SimGameState_GetTagsByScope
    {
        static bool Prepare() => Mod.Config.Fixes.EventRequirementsScopeFix;
        public static void Postfix(SimGameState __instance, EventScope scope, ref TagSet __result)
        {
            switch (scope)
            {
                case EventScope.SecondaryMechWarrior:
                    __result = ((Pilot)__instance.Context.GetObject(GameContextObjectTagEnum.SecondaryMechWarrior))?.pilotDef?.PilotTags;
                    return;
                case EventScope.SecondaryMech:
                    __result = ((MechDef)__instance.Context.GetObject(GameContextObjectTagEnum.TargetUnit))?.MechTags;
                    return;
                case EventScope.TertiaryMechWarrior:
                    __result = ((Pilot)__instance.Context.GetObject(GameContextObjectTagEnum.TertiaryMechWarrior))?.pilotDef?.PilotTags;
                    return;
                case EventScope.DeadMechWarrior:
                   __result = ((Pilot)__instance.Context.GetObject(GameContextObjectTagEnum.DeadMechWarrior))?.pilotDef?.PilotTags;
                    return;
            }
        }
    }

    [HarmonyPatch(typeof(SimGameState), "AttemptEvents")]
    public static class SimGameState_AttemptEvents
    {
        static bool Prepare() => Mod.Config.Fixes.EventRequirementsScopeFix;

        public static void Postfix(SimGameState __instance, bool incrementOnFailure = true, bool specialOnly = false)
        {
            if (__instance.CompanyTags.Contains(__instance.Constants.Story.SystemUseEventsTag) && !specialOnly)
            {

                __instance.deadEventTracker.baseProbability = __instance.Constants.Story.DeadEventStartingChance;
                __instance.deadEventTracker.chanceIncrement = __instance.Constants.Story.DeadEventIncreaseRate;
                Mod.Log.Trace?.Write($"DEBUG DSIABLE TBONE AttemptEvents : dead event starting chance in tracker after setting: {__instance.deadEventTracker.EventChance} should be {__instance.Constants.Story.DeadEventStartingChance}.");
                __instance.LogReport("% Chance for Dead MW Event: " + __instance.deadEventTracker.EventChance);
                __instance.LogReport("Dead MW Event Invoked: " + __instance.deadEventTracker.AttemptEvent(true).ToString());
            }
        }
    }

    [HarmonyPatch(typeof(SimGameEventTracker), "ActivateRandomEvent")]
    public static class SimGameEventTracker_ActivateRandomEvent
    {
        static bool Prepare() => Mod.Config.Fixes.EventRequirementsScopeFix;

        public static void Prefix(ref bool __runOriginal, SimGameEventTracker __instance)
        {
            if (!__runOriginal) return;
            SimGameEventTracker.PotentialEvent randomEvent = __instance.GetRandomEvent();
            if (randomEvent == null)
            {
                SimGameEventTracker.log.LogError(
                    $"Unable to find any events of type {__instance.eventType} that meet current requirements.");
                __runOriginal = true;
                return;
            }
            if (randomEvent.Pilot != null)
            {
                __instance.sim.Context.SetObject(
                    randomEvent.Def.Scope == EventScope.DeadMechWarrior
                        ? GameContextObjectTagEnum.DeadMechWarrior
                        : GameContextObjectTagEnum.TargetMechWarrior, randomEvent.Pilot);
            }
            if (randomEvent.SecondaryMech != null)
            {
                __instance.sim.Context.SetObject(GameContextObjectTagEnum.SecondaryUnit, randomEvent.SecondaryMech);
            }
            if (randomEvent.SecondaryPilot != null)
            {
                __instance.sim.Context.SetObject(GameContextObjectTagEnum.SecondaryMechWarrior, randomEvent.SecondaryPilot);
            }
            if (randomEvent.TertiaryPilot != null)
            {
                __instance.sim.Context.SetObject(GameContextObjectTagEnum.TertiaryMechWarrior, randomEvent.TertiaryPilot);
            }
            __instance.ActivateEvent(randomEvent.Def, randomEvent.Pilot);
            __runOriginal = false;
            return;
        }
    }


    [HarmonyPatch(typeof(SGDebugEventWidget), "CheckAdditionalObjects")]
    public static class SGDebugEventWidget_CheckAdditionalObjects
    {
        static bool Prepare() => Mod.Config.Fixes.EventRequirementsScopeFix;

        public static void Postfix(SGDebugEventWidget __instance, SimGameEventDef def, SimGameReport.ReportEntry entry, ref bool __result)
        {
            if (__result) return;
            var graveyard = __instance.Sim.Graveyard;
            foreach (var additionalObject in def.AdditionalObjects)
            {
                if (additionalObject.Scope != EventScope.DeadMechWarrior) continue;
                foreach (var deadPilot in graveyard)
                {
                    entry.Log($"/// Testing Tags/Stats on Pilot: {deadPilot.Name} ///");
                    var pilotTags = deadPilot.pilotDef.PilotTags;
                    var statCollection = deadPilot.StatCollection;
                    if (SimGameState.MeetsRequirements(additionalObject.Requirements, pilotTags, statCollection, entry))
                    {
                        __result = true;
                        return;
                    }
                }
            }
        }
    }
}
