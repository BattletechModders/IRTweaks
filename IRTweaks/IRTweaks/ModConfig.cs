using System.Collections.Generic;

namespace IRTweaks {
    using IRTweaks.Modules.Combat;

    public static class ModStats {
        public const string CalledShowAlwaysAllow = "IRTCalledShotAlwaysAllow";
        public const string CalledShotMod = "IRTCalledShotMod";

        public const string EnableMultiTarget = "IRAllowMultiTarget";

        public const string RandomMechs = "StartingRandomMechLists";
        public const string FactionRep = "FactionReputation";
        public const string StrayShotValidTargets = "StrayShotValidTargets";
    }

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
        public bool BulkPurchasing = true;
        public bool CombatLog = true;
        public bool DisableCampaign = true;
        public bool DisableCombatSaves = true;
        public bool ExtendedStats = true;
        public bool FlexibleSensorLock = true;
        public bool MultiTargetStat = true;
        public bool PainTolerance = true;
        public bool PreventHeadShots = true;
        public bool RandomStartByDifficulty = true;
        public bool ReduceSaveCompression = true;
        public bool ShowAllArgoUpgrades = true;
        public bool SkirmishReset = true;
        public bool SkirmishAlwaysUnlimited = true;
        public bool SkipDeleteSavePopup = true;
        public bool SpawnProtection = true;
        public bool StreamlinedMainMenu = true;
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

        public void LogConfig() {
            Mod.Log.Info("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info($"  DEBUG:{this.Debug} Trace:{this.Trace}");

            Mod.Log.Info("  -- Fixes --");
            Mod.Log.Info($"  BulkPurchasing:{this.Fixes.BulkPurchasing}  CombatLog:{this.Fixes.CombatLog}  DisableCombatSaves:{this.Fixes.DisableCombatSaves}  " +
                         $"ExtendedStats: {this.Fixes.ExtendedStats}  FlexibleSensorLock:{this.Fixes.FlexibleSensorLock}  " +
                         $"PreventCalledShots: {this.Fixes.PreventHeadShots}  RandomStartByDifficulty:{this.Fixes.RandomStartByDifficulty}  ReduceSaveCompression:{this.Fixes.ReduceSaveCompression}  " +
                         $"SkirmishReset: {this.Fixes.SkirmishReset}  SkirmishAlwaysUnlimited:{this.Fixes.SkirmishAlwaysUnlimited}  " +
                         $"SkipDeleteSavePopup:{this.Fixes.SkipDeleteSavePopup} ShowAllArgoUpgrades:{this.Fixes.ShowAllArgoUpgrades}  " +
                         $"SpawnProtection:{this.Fixes.SpawnProtection}  WeaponTooltips:{this.Fixes.WeaponTooltip}");

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
