using BattleTech;
using IRBTModUtils.Extension;
using System.Collections.Generic;
using us.frostraptor.modUtils;

namespace IRTweaks.Helper {

    public static class ActorHelper {

        // Looks in the actor and their components for a statistic
        public static List<Statistic> FindCustomStatistic(string statName, AbstractActor actor) {
            List<Statistic> statistics = new List<Statistic>();

            Mod.Log.Debug?.Write($" == BEGIN Searching actor:{actor.DisplayName} and components for stat:{statName}");

            // Walk the actor's statCollection
            if (actor.StatCollection.ContainsStatistic(statName)) {
                Mod.Log.Debug?.Write($"  actor has stat: {statName}");
                statistics.Add(actor.StatCollection.GetStatistic(statName));
            }

            Mod.Log.Debug?.Write($" == DONE Searching actor:{actor.DisplayName} and components for stat:{statName}");

            return statistics;
        }

        public static int GetCalledShotModifier(AbstractActor actor)
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
            }

            return mod;
        }

        public static string CacheKey(this Pilot pilot)
        {
            return $"{pilot?.Name}_{pilot?.ParentActor?.DisplayName}_{pilot?.GUID}_{pilot?.ParentActor?.GUID}";
        }
    }
}
