using System.Collections.Generic;

namespace IRTweaks {

    public class AbilityOpts {
        public string FlexibleSensorLockId = "AbilityDefT8A";
        public string JuggernautId = "AbilityDefGu8";
        public string MultiTargetId = "AbilityDefG5";
    }

    public class StoreOpts {
        public int QuantityOnShift = 5;
        public int QuantityOnControl = 20;
    }

    public class CombatOpts {
        public int PilotAttributesMax = 13;

        public CalledShotOpts CalledShot = new CalledShotOpts();
        public class CalledShotOpts {
            public int Modifier = 6;
            public Dictionary<string, int> PilotTags = new Dictionary<string, int>();
        }

        public FlexibleSensorLockOptions FlexibleSensorLock = new FlexibleSensorLockOptions();
        public class FlexibleSensorLockOptions {
            public bool FreeActionWithAbility = false;
        }

        public PainToleranceOpts PainTolerance = new PainToleranceOpts();
        public class PainToleranceOpts {
            public float ResistPerGuts = 10.0f;

            public float HeadDamageResistPenaltyPerArmorPoint = 5.0f;
            public float HeadHitArmorOnlyResistPenaltyMulti = 0.5f;

            // Reduces resist by this multiplied the capacity ratio of an ammo explosion
            public float AmmoExplosionResistPenaltyPerCapacityPercentile = 1.0f;
            // Reduces resist by this multiplied the capacity ratio of an head damage injury
            public float OverheatResistPenaltyPerHeatPercentile = 1.0f;
            
            public float KnockdownResistPenalty = 6f;
            public float SideLocationDestroyedResistPenalty = 10f;
            
        }

        public SpawnProtectionOpts SpawnProtection = new SpawnProtectionOpts();
        public class SpawnProtectionOpts {
            public bool ApplyGuard = true;

            public int EvasionPips = 6;

            public bool ApplyToEnemies = true;
            public bool ApplyToAllies = true;
            public bool ApplyToNeutrals = true;

            public bool ApplyToReinforcements = false;
        }

    }

    public class FixesFlags {
        public bool AlternateMechNamingStyle = true;
        public bool BuildingDamageColorChange = true;
        public bool BraceOnMeleeWithJuggernaut = true;
        public bool BulkPurchasing = true;
        public bool CombatLog = true;
        public bool DisableCampaign = true;
        public bool DisableCombatRestarts = true;
        public bool DisableCombatSaves = true;
        public bool DisableIronMan = true;
        public bool DisableMPHashCalculation = true;
        public bool ExtendedStats = true;
        public bool FlexibleSensorLock = true;
        public bool MechbayLayout = true;
        public bool MultiTargetStat = true;
        public bool PainTolerance = true;
        public bool PathfinderTeamFix = true;
        public bool PreventHeadShots = true;
        public bool RandomStartByDifficulty = true;
        public bool ReduceSaveCompression = true;
        public bool ShowAllArgoUpgrades = true;
        public bool SimGameDifficultyLabelsReplacer = true;
        public bool SkirmishReset = true;
        public bool SkirmishAlwaysUnlimited = true;
        public bool SkipDeleteSavePopup = true;
        public bool SpawnProtection = true;
        public bool StreamlinedMainMenu = true;
        public bool UrbanExplosionsFix = true;
        public bool WarnOnCombatRestart = false;
        public bool WeaponTooltip = true;
    }

    public class ModConfig {

        // If true, many logs will be printed
        public bool Debug = false;
        // If true, all logs will be printed
        public bool Trace = false;

        public FixesFlags Fixes = new FixesFlags();

        public AbilityOpts Abilities = new AbilityOpts();
        public CombatOpts Combat = new CombatOpts();
        public StoreOpts Store = new StoreOpts();

        public const string SimGameDifficultyString_Desc = "DESCRIPTION";
        public const string SimGameDifficultyString_Label = "LABEL";
        public Dictionary<string, string> SimGameDifficultyStrings = new Dictionary<string, string>()
        {
            { SimGameDifficultyString_Desc, "Overall Difficulty" },
            { SimGameDifficultyString_Label, "Difficulty" },
        };

        public void LogConfig() {
            Mod.Log.Info?.Write("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info?.Write($"  DEBUG: {this.Debug} Trace: {this.Trace}");

            Mod.Log.Info?.Write("  -- Fixes --");
            Mod.Log.Info?.Write($"  AlternateMechNamingStyle:           {this.Fixes.AlternateMechNamingStyle}");
            Mod.Log.Info?.Write($"  BuildingDamageColorChange:          {this.Fixes.BuildingDamageColorChange}");
            Mod.Log.Info?.Write($"  BraceOnMeleeWithJuggernaut:         {this.Fixes.BraceOnMeleeWithJuggernaut}");
            Mod.Log.Info?.Write($"  BulkPurchasing:                     {this.Fixes.BulkPurchasing}");
            Mod.Log.Info?.Write($"  CombatLog:                          {this.Fixes.CombatLog}");
            Mod.Log.Info?.Write($"  DisableCampaign:                    {this.Fixes.DisableCampaign}");
            Mod.Log.Info?.Write($"  DisableCombatSaves:                 {this.Fixes.DisableCombatSaves}");
            Mod.Log.Info?.Write($"  DisableMPHashCalculation:           {this.Fixes.DisableMPHashCalculation}");
            Mod.Log.Info?.Write($"  ExtendedStats:                      {this.Fixes.ExtendedStats}");
            Mod.Log.Info?.Write($"  FlexibleSensorLock:                 {this.Fixes.FlexibleSensorLock}");
            Mod.Log.Info?.Write($"  MechbayLayoutFix:                   {this.Fixes.MechbayLayout}");
            Mod.Log.Info?.Write($"  PainTolerance:                      {this.Fixes.PainTolerance}");
            Mod.Log.Info?.Write($"  PathfinderTeamFix:                  {this.Fixes.PathfinderTeamFix}");
            Mod.Log.Info?.Write($"  PreventHeadShots:                   {this.Fixes.PreventHeadShots}");
            Mod.Log.Info?.Write($"  RandomStartByDifficulty:            {this.Fixes.RandomStartByDifficulty}");
            Mod.Log.Info?.Write($"  ReduceSaveCompression:              {this.Fixes.ReduceSaveCompression}");
            Mod.Log.Info?.Write($"  ShowAllArgoUpgrades:                {this.Fixes.ShowAllArgoUpgrades}");
            Mod.Log.Info?.Write($"  SkipDeleteSavePopup:                {this.Fixes.SkipDeleteSavePopup}");
            Mod.Log.Info?.Write($"  SkirmishAlwaysUnlimited:            {this.Fixes.SkirmishAlwaysUnlimited}");
            Mod.Log.Info?.Write($"  SkirmishReset:                      {this.Fixes.SkirmishReset}");
            Mod.Log.Info?.Write($"  SimGameDifficultyLabelsReplacer:    {this.Fixes.SimGameDifficultyLabelsReplacer}");
            Mod.Log.Info?.Write($"  SpawnProtection:                    {this.Fixes.SpawnProtection}");
            Mod.Log.Info?.Write($"  StreamlinedMainMenu:                {this.Fixes.StreamlinedMainMenu}");
            Mod.Log.Info?.Write($"  UrbanExplosionsFix:                 {this.Fixes.UrbanExplosionsFix}");
            Mod.Log.Info?.Write($"  WeaponTooltips:                     {this.Fixes.WeaponTooltip}");

            Mod.Log.Info?.Write("  -- Called Shot --");
            Mod.Log.Info?.Write($"   CalledShotDefaultMod:{Combat.CalledShot.Modifier}");
            foreach (KeyValuePair<string, int> kvp in Combat.CalledShot.PilotTags) {
                Mod.Log.Info?.Write($"   CalledShotPilotModifier - tag:{kvp.Key} modifier:{kvp.Value}");
            }
            Mod.Log.Info?.Write($"   CalledShotDefaultMod:{Combat.CalledShot.Modifier}");

            Mod.Log.Info?.Write("  -- Spawn Protection --");
            Mod.Log.Info?.Write($"   ApplyGuard:{Combat.SpawnProtection.ApplyGuard}  EvasionPips:{Combat.SpawnProtection.EvasionPips}");
            Mod.Log.Info?.Write($"   ApplyToEnemies:{Combat.SpawnProtection.ApplyToEnemies}  ApplyToAllies:{Combat.SpawnProtection.ApplyToAllies}  ApplyToNeutrals:{Combat.SpawnProtection.ApplyToNeutrals}  ");
            Mod.Log.Info?.Write($"   ApplyToReinforcements:{Combat.SpawnProtection.ApplyToReinforcements}");

            Mod.Log.Info?.Write("  -- Store --");
            Mod.Log.Info?.Write($"   QuantityOnShift:{Store.QuantityOnShift}  QuantityOnControl:{Store.QuantityOnControl}");

            Mod.Log.Info?.Write("  -- Flexible Sensor Lock Options --");
            Mod.Log.Info?.Write($"   FreeActionWithAbility:{this.Combat.FlexibleSensorLock.FreeActionWithAbility}  AbilityId:{this.Abilities.FlexibleSensorLockId}");

            Mod.Log.Info?.Write("=== MOD CONFIG END ===");
        }
    }
}
