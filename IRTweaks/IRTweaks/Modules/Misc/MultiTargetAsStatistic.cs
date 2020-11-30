using BattleTech;
using Harmony;
using IRBTModUtils;
using IRBTModUtils.Extension;

namespace IRTweaks.Modules.Misc
{

    [HarmonyPatch(typeof(UnitSpawnPointGameLogic), "initializeActor")]
    [HarmonyAfter("ca.jwolf.MechAffinity")]
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
