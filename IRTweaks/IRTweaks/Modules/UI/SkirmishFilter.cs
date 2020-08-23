using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;

namespace IRTweaks.Modules.UI {

    [HarmonyPatch(typeof(SkirmishSettings_Beta), "OnLoadComplete")]
    public static class SkirmishSettings_Beta_OnLoadComplete {

        static bool Prepare() { return Mod.Config.Fixes.SkirmishAlwaysUnlimited; }

        public static void Postfix(SkirmishSettings_Beta __instance, HBS_Dropdown ___lanceBudgetDropdown) {
            Mod.Log.Trace?.Write("SS_B:OLC - ENTERED!");

            Traverse initLanceModuleT = Traverse.Create(__instance).Method("InitializeLanceModules", new object[] { 3 });
            initLanceModuleT.GetValue();

            Mod.Log.Info?.Write("Set battlevalue to 3");
        }
    }
}
