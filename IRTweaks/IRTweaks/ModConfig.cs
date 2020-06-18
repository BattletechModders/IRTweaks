using System.Collections.Generic;

namespace IRTweaks {

    public class AbilityOpts {
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

            // Default to Master Tactician
            public string AbilityId = "AbilityDefT8A";
        }

        public PainToleranceOpts PainTolerance = new PainToleranceOpts();
        public class PainToleranceOpts {
            public float ResistPerGuts = 10.0f;
            public float PenaltyPerHeadDamage = 5.0f;

            // Reduces resist by this multiplied the capacity ratio of an ammo explosion
            public float PenaltyPerAmmoExplosionRatio = 1.0f;

            // Reduces resist by this multiplied the capacity ratio of an head damage injury
            public float PenaltyPerHeatDamageInjuryRatio = 1.0f;
            
            public float KnockdownDamage = 6f;
            public float TorsoDestroyedDamage = 10f;
            public float HeadHitArmorOnlyMulti = 0.5f;
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
        public bool BraceOnMeleeWithJuggernaut = true;
        public bool BulkPurchasing = true;
        public bool CombatLog = true;
        public bool DisableCampaign = true;
        public bool DisableCombatSaves = true;
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

        public string JuggernautAbilityId = "AbilityDefGu8";

        public const string SimGameDifficultyString_Desc = "DESCRIPTION";
        public const string SimGameDifficultyString_Label = "LABEL";
        public Dictionary<string, string> SimGameDifficultyStrings = new Dictionary<string, string>()
        {
            { SimGameDifficultyString_Desc, "Overall Difficulty" },
            { SimGameDifficultyString_Label, "Difficulty" },
        };

        public void LogConfig() {
            Mod.Log.Info("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info($"  DEBUG:{this.Debug} Trace:{this.Trace}");

            Mod.Log.Info("  -- Fixes --");
            Mod.Log.Info($"  AlternateMechNamingStyle:{this.Fixes.AlternateMechNamingStyle}");
            Mod.Log.Info($"  BraceOnMeleeWithJuggernaut:{this.Fixes.BraceOnMeleeWithJuggernaut}");
            Mod.Log.Info($"  BulkPurchasing:{this.Fixes.BulkPurchasing}");
            Mod.Log.Info($"  CombatLog:{this.Fixes.CombatLog}");
            Mod.Log.Info($"  DisableCampaign:{this.Fixes.DisableCampaign}");
            Mod.Log.Info($"  DisableCombatSaves:{this.Fixes.DisableCombatSaves}");
            Mod.Log.Info($"  DisableMPHashCalculation:{this.Fixes.DisableMPHashCalculation}");
            Mod.Log.Info($"  ExtendedStats: {this.Fixes.ExtendedStats}");
            Mod.Log.Info($"  FlexibleSensorLock:{this.Fixes.FlexibleSensorLock}");
            Mod.Log.Info($"  MechbayLayoutFix:{this.Fixes.MechbayLayout}");
            Mod.Log.Info($"  PathfinderTeamFix:{this.Fixes.PathfinderTeamFix}");
            Mod.Log.Info($"  PreventHeadShots: {this.Fixes.PreventHeadShots}");
            Mod.Log.Info($"  RandomStartByDifficulty:{this.Fixes.RandomStartByDifficulty}");
            Mod.Log.Info($"  ReduceSaveCompression:{this.Fixes.ReduceSaveCompression}");
            Mod.Log.Info($"  ShowAllArgoUpgrades:{this.Fixes.ShowAllArgoUpgrades}");
            Mod.Log.Info($"  SkirmishReset: {this.Fixes.SkirmishReset}");
            Mod.Log.Info($"  SkirmishAlwaysUnlimited:{this.Fixes.SkirmishAlwaysUnlimited}");
            Mod.Log.Info($"  SkipDeleteSavePopup:{this.Fixes.SkipDeleteSavePopup}");
            Mod.Log.Info($"  SpawnProtection:{this.Fixes.SpawnProtection}");
            Mod.Log.Info($"  StreamlinedMainMenu:{this.Fixes.StreamlinedMainMenu}");
            Mod.Log.Info($"  UrbanExplosionsFix:{this.Fixes.UrbanExplosionsFix}");
            Mod.Log.Info($"  WeaponTooltips:{this.Fixes.WeaponTooltip}");

            Mod.Log.Info("  -- Called Shot --");
            Mod.Log.Info($"   CalledShotDefaultMod:{Combat.CalledShot.Modifier}");
            foreach (KeyValuePair<string, int> kvp in Combat.CalledShot.PilotTags) {
                Mod.Log.Info($"   CalledShotPilotModifier - tag:{kvp.Key} modifier:{kvp.Value}");
            }
            Mod.Log.Info($"   CalledShotDefaultMod:{Combat.CalledShot.Modifier}");

            Mod.Log.Info("  -- Spawn Protection --");
            Mod.Log.Info($"   ApplyGuard:{Combat.SpawnProtection.ApplyGuard}  EvasionPips:{Combat.SpawnProtection.EvasionPips}");
            Mod.Log.Info($"   ApplyToEnemies:{Combat.SpawnProtection.ApplyToEnemies}  ApplyToAllies:{Combat.SpawnProtection.ApplyToAllies}  ApplyToNeutrals:{Combat.SpawnProtection.ApplyToNeutrals}  ");
            Mod.Log.Info($"   ApplyToReinforcements:{Combat.SpawnProtection.ApplyToReinforcements}");

            Mod.Log.Info("  -- Store --");
            Mod.Log.Info($"   QuantityOnShift:{Store.QuantityOnShift}  QuantityOnControl:{Store.QuantityOnControl}");

            Mod.Log.Info("  -- Flexible Sensor Lock Options --");
            Mod.Log.Info($"   FreeActionWithAbility:{this.Combat.FlexibleSensorLock.FreeActionWithAbility}  AbilityId:{this.Combat.FlexibleSensorLock.AbilityId}");

            Mod.Log.Info("=== MOD CONFIG END ===");
        }
    }
}
