using BattleTech;
using BattleTech.UI;
using Harmony;

namespace IRTweaks {

    [HarmonyPatch(typeof(AAR_SalvageScreen), "CalculateAndAddAvailableSalvage")]
    public static class AAR_SalvageScreen_CalculateAndAddAvailableSalvage {

        public static void Postfix(AAR_SalvageScreen __instance, Contract ___contract) {
            Mod.Log.Debug("AAR_SS:CAAAS entered.");

        }
    }


}
