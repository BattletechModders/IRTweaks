

using System.Collections.Generic;

namespace IRTweaks {

    public static class ModStats {
        public const string CalledShowAlwaysAllow = "IRTCalledShotAlwaysAllow";
        public const string CalledShotMod = "IRTCalledShotMod";
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
    }

    public class FixesFlags {
        public bool ExtendedStats = true;
        public bool PreventCalledShots = true;
        public bool StoreQOL = true;
        public bool WeaponTooltip = true;
    }

    public class ModConfig {

        // If true, many logs will be printed
        public bool Debug = false;
        // If true, all logs will be printed
        public bool Trace = false;

        public StoreOpts Store = new StoreOpts();
        public Combat Combat = new Combat();
        public FixesFlags Fixes = new FixesFlags();

        public void LogConfig() {
            Mod.Log.Info("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info($"  DEBUG:{this.Debug} Trace:{this.Trace}");

            Mod.Log.Info("  -- Fixes --");
            Mod.Log.Info($"  ExtendedStats: {this.Fixes.ExtendedStats} PreventCalledShots: {this.Fixes.PreventCalledShots} " +
                $"StorePatches: {this.Fixes.StoreQOL} WeaponTooltips:{this.Fixes.WeaponTooltip}");

            Mod.Log.Info("  -- To Hit --");
            Mod.Log.Info($"   CalledShotDefaultMod:{Combat.CalledShot.Modifier}");
            foreach (KeyValuePair<string, int> kvp in Combat.CalledShot.PilotTags) {
                Mod.Log.Info($"   CalledShotPilotModifier - tag:{kvp.Key} modifier:{kvp.Value}");
            }

            Mod.Log.Info("=== MOD CONFIG END ===");
        }
    }
}
