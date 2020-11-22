
using BattleTech.UI;
using System.Collections.Generic;

namespace IRTweaks {

    public static class ModState {

        public static Dictionary<string, int> PilotCalledShotModifiers = new Dictionary<string, int>();

        public static SelectionStateSensorLock SelectionStateSensorLock;

        // Tracks an attack sequence that creates an injury
        public static float InjuryResistPenalty = -1f;

        public static List<int> ExplosionSequences = new List<int>();
        public static List<BattleTech.Building> ExplosionBuildingTargets = new List<BattleTech.Building>();

        // Record objective buildings
        public static HashSet<string> ScaledObjectiveBuildings = new HashSet<string>();


        public static void Reset() {
            // Reinitialize state
            PilotCalledShotModifiers.Clear();
            InjuryResistPenalty = -1f;

            ExplosionSequences.Clear();
            ExplosionBuildingTargets.Clear();

            ScaledObjectiveBuildings.Clear();
        }
    }

}


