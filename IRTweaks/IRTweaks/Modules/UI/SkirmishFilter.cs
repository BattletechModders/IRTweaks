using BattleTech.UI;
using BattleTech.UI.TMProWrapper;

namespace IRTweaks.Modules.UI
{

    [HarmonyPatch(typeof(SkirmishSettings_Beta), "OnLoadComplete")]
    public static class SkirmishSettings_Beta_OnLoadComplete
    {

        static bool Prepare() { return Mod.Config.Fixes.SkirmishAlwaysUnlimited; }

        public static void Postfix(SkirmishSettings_Beta __instance, HBS_Dropdown ___lanceBudgetDropdown)
        {
            Mod.Log.Trace?.Write("SS_B:OLC - ENTERED!");

            __instance.InitializeLanceModules(3);
            Mod.Log.Info?.Write("Set battlevalue to 3");
        }
    }
}
