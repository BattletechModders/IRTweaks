
using BattleTech.UI;
using System.Collections.Generic;
using BattleTech;

namespace IRTweaks {

    public static class ModState {
        public static class InstantiatedDifficultySettings
        {
            public static List<SGDSToggle> instantiatedToggles = new List<SGDSToggle>();
            public static List<SGDSDropdown> instantiatedDropdowns = new List<SGDSDropdown>();
        }

        public static bool IsComponentValidForRefit = false;

        public static Dictionary<string, int> PilotCalledShotModifiers = new Dictionary<string, int>();

        public static SelectionStateSensorLock SelectionStateSensorLock;
        public static SelectionStateActiveProbe SelectionStateActiveProbe;

        // Tracks an attack sequence that creates an injury
        public static float InjuryResistPenalty = -1f;

        public static List<int> ExplosionSequences = new List<int>();
        public static List<BattleTech.Building> ExplosionBuildingTargets = new List<BattleTech.Building>();

        // Record objective buildings
        public static HashSet<string> ScaledObjectiveBuildings = new HashSet<string>();
        public static CombatOpts.StructureScale ActiveContractBuildingScaling = Mod.Config.Combat.ScaledStructure.DefaultScale;

        public static HashSet<string> SpawnProtectedUnits = new HashSet<string>();

        public static bool WasCTDestroyed = false;

        public static Dictionary<string, float> UnitStartingArmor = new Dictionary<string, float>();
        public static Dictionary<string, float> UnitCurrentArmor = new Dictionary<string, float>();
        public static Dictionary<string, int> UnitCurrentHeat = new Dictionary<string, int>();
        public static Dictionary<string, int> PilotCurrentFreeXP = new Dictionary<string, int>();
        public static Dictionary<string, int> PilotDefCurrentFreeXP = new Dictionary<string, int>();
        public static Dictionary<string, int> SimGameFunds = new Dictionary<string, int>();

        public static void Reset() {
            // Reinitialize state
            PilotCalledShotModifiers.Clear();
            InjuryResistPenalty = -1f;

            ExplosionSequences.Clear();
            ExplosionBuildingTargets.Clear();

            ScaledObjectiveBuildings.Clear();
            ActiveContractBuildingScaling = Mod.Config.Combat.ScaledStructure.DefaultScale;

            SpawnProtectedUnits.Clear();

            WasCTDestroyed = false;
        }
    }

}


