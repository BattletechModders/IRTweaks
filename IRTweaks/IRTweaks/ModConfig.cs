using System.Collections.Generic;

namespace IRTweaks {

    public static class ModStats {
        public const string CalledShowAlwaysAllow = "IRTCalledShotAlwaysAllow";
        public const string CalledShotMod = "IRTCalledShotMod";

        public const string RandomMechs = "StartingRandomMechLists";
        public const string FactionRep = "FactionReputation";
        public const string StrayShotValidTargets = "StrayShotValidTargets";
    }

    public class StoreOpts {
        public int QuantityOnShift = 5;
        public int QuantityOnControl = 20;
    }

    public class Combat {
        public int PilotAttributesMax = 13;

        public CalledShotOpts CalledShot = new CalledShotOpts();
        public class CalledShotOpts {
            public int Modifier = 6;
            public Dictionary<string, int> PilotTags = new Dictionary<string, int>();
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
        public bool PreventHeadShots = true;
        public bool RandomStartByDifficulty = true;
        public bool SkirmishReset = true;
        public bool SpawnProtection = true;
        public bool StreamlinedMainMenu = true;
        public bool WeaponTooltip = true;
        public bool SkipDeleteSavePopup = true;
    }

    public class ModConfig {

        // If true, many logs will be printed
        public bool Debug = false;
        // If true, all logs will be printed
        public bool Trace = false;

        public FixesFlags Fixes = new FixesFlags();

        public Combat Combat = new Combat();
        public StoreOpts Store = new StoreOpts();

        public void LogConfig() {
            Mod.Log.Info("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info($"  DEBUG:{this.Debug} Trace:{this.Trace}");

            Mod.Log.Info("  -- Fixes --");
            Mod.Log.Info($"  BulkPurchasing:{this.Fixes.BulkPurchasing}  CombatLog:{this.Fixes.CombatLog}  DisableCombatSaves:{this.Fixes.DisableCombatSaves}  " +
                $"ExtendedStats: {this.Fixes.ExtendedStats}  FlexibleSensorLock:{this.Fixes.FlexibleSensorLock}  " +
                $"PreventCalledShots: {this.Fixes.PreventHeadShots}  RandomStartByDifficulty:{this.Fixes.RandomStartByDifficulty}  " +
                $"SkirmishReset: {this.Fixes.SkirmishReset}  SpawnProtection:{this.Fixes.SpawnProtection}" +
                $"WeaponTooltips:{this.Fixes.WeaponTooltip}" +
                $"SkipDeleteSavePopup:{this.Fixes.SkipDeleteSavePopup}");

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

            Mod.Log.Info("=== MOD CONFIG END ===");
        }
    }
}
