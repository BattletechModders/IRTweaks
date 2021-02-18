using BattleTech;
using Harmony;
using System;

namespace IRTweaks.Modules.Misc {

    [HarmonyPatch(typeof(SimGameState), "ShowLowFundsWarning")]
    public static class SimGameState_ShowLowFundsWarning {
        static bool Prepare() => Mod.Config.Fixes.DisableLowFundsNotification;

        public static bool Prefix() {
            Mod.Log.Info?.Write($"Skipping Low Funds warning.");
            return false;
        }
    }
}
