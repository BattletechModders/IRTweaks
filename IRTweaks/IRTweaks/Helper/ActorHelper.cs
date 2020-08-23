using BattleTech;
using System.Collections.Generic;

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
    }
}
