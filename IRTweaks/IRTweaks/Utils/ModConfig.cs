

namespace IRTweaks {

    public class ModConfig {

        // If true, many logs will be printed
        public bool Debug = false;

        public void LogConfig() {
            Mod.Log.Info("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info($"  DEBUG: {this.Debug}");
            Mod.Log.Info("=== MOD CONFIG END ===");
        }
    }
}
