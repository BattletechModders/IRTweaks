using HBS.Collections;

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
                    __result = ((Pilot)__instance.Context.GetObject(GameContextObjectTagEnum.SecondaryMechWarrior)).StatCollection;
                    return;
                case EventScope.SecondaryMech:
                    __result = ((MechDef)__instance.Context.GetObject(GameContextObjectTagEnum.TargetUnit)).Stats;
                    return;
                case EventScope.TertiaryMechWarrior:
                    __result = ((Pilot)__instance.Context.GetObject(GameContextObjectTagEnum.TertiaryMechWarrior)).StatCollection;
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
                    __result = ((Pilot)__instance.Context.GetObject(GameContextObjectTagEnum.SecondaryMechWarrior)).pilotDef.PilotTags;
                    return;
                case EventScope.SecondaryMech:
                    __result = ((MechDef)__instance.Context.GetObject(GameContextObjectTagEnum.TargetUnit)).MechTags;
                    return;
                case EventScope.TertiaryMechWarrior:
                    __result = ((Pilot)__instance.Context.GetObject(GameContextObjectTagEnum.TertiaryMechWarrior)).pilotDef.PilotTags;
                    return;
            }
        }
    }
}
