
using BattleTech.UI;
using System.Collections.Generic;
using BattleTech;

namespace IRTweaks {

    public static class ModState
    {
        public static Dictionary<string, List<ActiveDmgMod>> StabDmgMods = new Dictionary<string, List<ActiveDmgMod>>();
        public static Dictionary<string, List<ActiveDmgMod>> HeatDmgMods = new Dictionary<string, List<ActiveDmgMod>>();
        public static Dictionary<string, List<ActiveDmgMod>> APDmgMods = new Dictionary<string, List<ActiveDmgMod>>();
        public static Dictionary<string, List<ActiveDmgMod>> StdDmgMods = new Dictionary<string, List<ActiveDmgMod>>();

        public static List<SGDSToggle> InstantiatedToggles = new List<SGDSToggle>();
        public static List<SGDSDropdown> InstantiatedDropdowns = new List<SGDSDropdown>();
        

        public static float MinDiffModifier = 0f;
        public static float MaxDiffModifier = 0f;
        public static bool HaveDiffSettingsInitiated = false;

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
        public static BraceEffects BraceEffectsInit = new BraceEffects();

        public static bool ShouldGetReducedDamageIgnoreDR = false;
        public static bool AttackShouldCheckForKnockDown = false;
        public static List<string> AttackShouldCheckActorsForShutdown = new List<string>();

        public static Dictionary<string, bool> DidActorBraceLastRoundBeforeFiring = new Dictionary<string, bool>();

        public static Dictionary<CombatHUDEvasiveBarPips, AbstractActor> DamageReductionInCombatHud = new Dictionary<CombatHUDEvasiveBarPips, AbstractActor>();
        public static Dictionary<AbstractActor, CombatHUDEvasiveBarPips> DamageReductionInCombatHudActors = new Dictionary<AbstractActor, CombatHUDEvasiveBarPips>();

        public static void OnSimInit()
        {
            HaveDiffSettingsInitiated = false;
            InstantiatedDropdowns = new List<SGDSDropdown>();
            InstantiatedToggles = new List<SGDSToggle>();
        }

        public static void InitializeEffects()
        {
            BraceEffectsInit = new BraceEffects();

            Mod.Log.Info?.Write($"[InitializeEffects] Initializing effects for Brace Effects");
            foreach (var jObject in Mod.Config.Abilities.BraceEffectConfig.effectDataJO)
            {
                var effectData = new EffectData();
                effectData.FromJSON(jObject.ToString());
                BraceEffectsInit.effects.Add(effectData);
                Mod.Log.Info?.Write($"EffectData statname: {effectData?.statisticData?.statName}");
            }
        }

        public static void Reset() {
            // Reinitialize state
            PilotCalledShotModifiers.Clear();
            InjuryResistPenalty = -1f;

            ExplosionSequences.Clear();
            ExplosionBuildingTargets.Clear();

            ScaledObjectiveBuildings.Clear();
            ActiveContractBuildingScaling = Mod.Config.Combat.ScaledStructure.DefaultScale;

            SpawnProtectedUnits.Clear();
            DamageReductionInCombatHud.Clear();
            DamageReductionInCombatHudActors.Clear();

            WasCTDestroyed = false;
            AttackShouldCheckActorsForShutdown = new List<string>();
        }
    }

}


