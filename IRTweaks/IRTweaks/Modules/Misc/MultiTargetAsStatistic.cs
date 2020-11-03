using BattleTech;
using Harmony;
using IRBTModUtils;
using IRBTModUtils.Extension;

namespace IRTweaks.Modules.Misc
{

    [HarmonyPatch(typeof(UnitSpawnPointGameLogic), "initializeActor")]
    public static class UnitSpawnPointGameLogic_initializeActor
    {
        static bool Prepare() => Mod.Config.Fixes.MultiTargetStat;

        static void Postfix(UnitSpawnPointGameLogic __instance, AbstractActor actor, Team team, Lance lance)
        {
            if (actor == null || actor.GetPilot() == null)
            {
                Mod.Log.Info?.Write("Actor or pilot is null, cannot check for multi-targeting, skipping.");
                return;
            }

            Statistic multiTargetStat = actor.StatCollection.GetStatistic(ModStats.EnableMultiTarget);
            if (multiTargetStat != null && multiTargetStat.Value<bool>())
            {
                bool hasMultiTargetAbility = false;
                foreach (Ability ability in actor.GetPilot().Abilities)
                {
                    if (ability.Def.Targeting == AbilityDef.TargetingType.MultiFire)
                    {
                        Mod.Log.Debug?.Write(" -- unit already has MultiFire ability, skipping.");
                        hasMultiTargetAbility = true;
                        break;
                    }
                }

                if (!hasMultiTargetAbility)
                {
                    Mod.Log.Debug?.Write("  -- adding multi-target ability to pilot.");
                    AbilityDef abilityDef = SharedState.Combat.DataManager.AbilityDefs.Get(Mod.Config.Abilities.MultiTargetId);
                    Ability ability = new Ability(abilityDef);
                    ability.Init(SharedState.Combat);

                    Pilot pilot = actor.GetPilot();
                    pilot.Abilities.Add(ability);
                    pilot.ActiveAbilities.Add(ability);
                    pilot.StatCollection.ModifyStat("", -1, ModStats.HBS_MaxTargets, StatCollection.StatOperation.Set, 3);
                    Mod.Log.Debug?.Write("  -- done.");
                }
            }

        }
    }

    //[HarmonyPatch(typeof(Pilot), "InitAbilities")]
    //public static class Pilot_InitAbilities
    //{
    //    static bool Prepare() { return Mod.Config.Fixes.MultiTargetStat; }

    //    static void Postfix(Pilot __instance)
    //    {
    //        if (SharedState.Combat == null) return; // Nothing to do

    //        if (__instance == null || __instance.ParentActor == null)
    //        {
    //            Mod.Log.Info?.Write("Pilot, ParentActor, or Combat is null! Cannot check for multi-targeting ability on unit, skipping!");
    //            return;
    //        }

    //        bool enableMultiTarget = __instance.ParentActor.StatCollection.GetValue<bool>(ModStats.EnableMultiTarget);
    //        Mod.Log.Debug?.Write($"Piloted unit {__instance.ParentActor.DistinctId()} has enableMultiTarget: {enableMultiTarget}");

    //        if (enableMultiTarget)
    //        {
    //            bool hasMultiTarget = false;
    //            foreach (Ability ability in __instance.Abilities)
    //            {
    //                if (ability.Def.Targeting == AbilityDef.TargetingType.MultiFire)
    //                {
    //                    Mod.Log.Debug?.Write(" -- unit already has MultiFire ability, skipping.");
    //                    hasMultiTarget = true;
    //                    break;
    //                }
    //            }

    //            if (!hasMultiTarget)
    //            {
    //                Mod.Log.Debug?.Write("  -- adding multi-target ability to pilot.");
    //                AbilityDef abilityDef = SharedState.Combat.DataManager.AbilityDefs.Get(Mod.Config.Abilities.MultiTargetId);
    //                Ability ability = new Ability(abilityDef);
    //                ability.Init(SharedState.Combat);
    //                __instance.Abilities.Add(ability);
    //                __instance.ActiveAbilities.Add(ability);
    //                __instance.StatCollection.ModifyStat("", -1, "MaxTargets", StatCollection.StatOperation.Set, 3);
    //                Mod.Log.Debug?.Write("  -- done.");
    //            }
    //        }
    //    }
    //}


    [HarmonyPatch(typeof(AbstractActor), "InitEffectStats")]
    public static class AbstractActor_InitEffectStats
    {
        static bool Prepare() => Mod.Config.Fixes.MultiTargetStat;

        static void Postfix(AbstractActor __instance)
        {
            Mod.Log.Debug?.Write($"Adding statistic: {ModStats.EnableMultiTarget} to unit: {__instance.DistinctId()}");
            __instance.StatCollection.AddStatistic<bool>(ModStats.EnableMultiTarget, false);
        }
    }
}
