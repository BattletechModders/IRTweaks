using BattleTech;
using Harmony;

namespace IRTweaks.Modules.Misc {

    [HarmonyPatch(typeof(UnityGameInstance), "CalcGameHash")]
    public static class UnityGameInstance_CalcGameHash {

        static bool Prepare() { return Mod.Config.Fixes.DisableMPHashCalculation; }

        public static bool Prefix() {
            return false;
        }
    }
}
