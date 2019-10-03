

using System.Collections.Generic;

namespace IRTweaks {

    public static class ModStats {
        public const string CalledShowAlwaysAllow = "IRTCalledShotAlwaysAllow";
        public const string CalledShotMod = "IRTCalledShotMod";
    }

    public class StoreQOL {
        public bool Enabled = false;
        public int QuantityOnShift = 5;
        public int QuantityOnControl = 20;
    }

    public class ToHitCfg {

        public int CalledShotDefaultMod = 6;

        public Dictionary<string, int> CalledShotPilotTags = new Dictionary<string, int>();
    }

    public class ModConfig {

        // If true, many logs will be printed
        public bool Debug = false;
        // If true, all logs will be printed
        public bool Trace = false;

        public ToHitCfg ToHitCfg = new ToHitCfg();
        public StoreQOL StoreQOL = new StoreQOL();

        public void LogConfig() {
            Mod.Log.Info("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info($"  DEBUG:{this.Debug} Trace:{this.Trace}");

            Mod.Log.Info($"  StorePatches enabled: {this.StoreQOL.Enabled}");

            Mod.Log.Info("  -- To Hit --");
            Mod.Log.Info($"   CalledShotDefaultMod:{ToHitCfg.CalledShotDefaultMod}");
            foreach (KeyValuePair<string, int> kvp in ToHitCfg.CalledShotPilotTags) {
                Mod.Log.Info($"   CalledShotPilotModifier - tag:{kvp.Key} modifier:{kvp.Value}");
            }

            Mod.Log.Info("=== MOD CONFIG END ===");
        }
    }
}
