using BattleTech;
using IRBTModUtils.Extension;
using us.frostraptor.modUtils;

namespace IRTweaks.Helper {

    public static class ActorHelper {
        public static bool CanAlwaysUseCalledShot(this AbstractActor actor)
        {
            Statistic stat = actor.StatCollection.GetStatistic(ModStats.CalledShot_AlwaysAllow);
            return stat != null && stat.Value<bool>();
        }

        public static bool ImmuneToHeadInjuries(this AbstractActor actor)
        {
            Statistic stat = actor.StatCollection.GetStatistic(ModStats.IgnoreHeadInjuries);
            return stat != null && stat.Value<bool>();
        }

        public static int CalledShotModifier(this AbstractActor actor)
        {
            int mod = 0;

            // Calculate the pilot mod, if any
            if (actor.GetPilot() != null)
            {
                Pilot pilot = actor.GetPilot();
                string pilotKey = pilot.CacheKey();
                bool hasPilot = ModState.PilotCalledShotModifiers.TryGetValue(pilot.CacheKey(), out mod);
                if (!hasPilot)
                {
                    // Calculate the modifiers for the pilot
                    Mod.Log.Info?.Write($" Calculating calledShotModifier for actor: {actor.DistinctId()}");
                    int baseMod = Mod.Config.Combat.CalledShot.BaseModifier;
                    int tacticsMod = Mod.Config.Combat.CalledShot.EnableTacticsModifier ?
                        (-1 * SkillUtils.GetTacticsModifier(pilot)) : 0;
                    int tagsCSMod = SkillUtils.GetTagsModifier(pilot, Mod.Config.Combat.CalledShot.PilotTags);

                    // Calculate the actor mod, if any
                    int actorMod = actor.StatCollection.GetValue<int>(ModStats.CalledShot_AttackMod);

                    int calledShotMod = baseMod + tacticsMod + tagsCSMod + actorMod;
                    Mod.Log.Info?.Write($" -- calledShotMod: {calledShotMod} = defaultMod: {baseMod} + tacticsMod: {tacticsMod} + tagsCSMod: {tagsCSMod} + actorMod: {actorMod}");

                    ModState.PilotCalledShotModifiers[pilotKey] = calledShotMod;
                }
                else
                {
                    Mod.Log.Debug?.Write($"CalledShotModifier: {mod} for actor: {actor.DistinctId()}");
                }
            }

            return mod;
        }

        public static string CacheKey(this Pilot pilot)
        {
            return $"{pilot?.Name}_{pilot?.ParentActor?.DisplayName}_{pilot?.GUID}_{pilot?.ParentActor?.GUID}";
        }
    }
}
