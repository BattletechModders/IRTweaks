using BattleTech.UI;
using CustAmmoCategories;
using UnityEngine;

namespace IRTweaks.Modules.UI
{
    class TgtComputerTonnage
    {
        [HarmonyPatch(typeof(CombatHUDActorDetailsDisplay), "RefreshInfo")]
        public static class CombatHUDActorDetailsDisplay_RefreshInfo_Patch
        {
            static bool Prepare() => Mod.Config.Fixes.TgtComputerTonnageDisplay;

            [HarmonyPriority(Priority.Last)]

            public static void Postfix(CombatHUDActorDetailsDisplay __instance)
            {
                if (__instance.DisplayedActor is Mech mech)
                {

                    __instance.ActorWeightText.SetText("{0}: {1} ({2}T)", new object[]
                    {
                        mech.FakeVehicle() ? "VEHICLE" : "'MECH",
                        mech.weightClass.ToString(),
                        Mathf.RoundToInt(mech.tonnage)
                    });
                }
                else if (__instance.DisplayedActor is Vehicle vehicle)
                {
                    __instance.ActorWeightText.SetText("{0}: {1} ({2}T)", new object[]
                    {
                        "VEHICLE",
                        vehicle.weightClass.ToString(),
                        Mathf.RoundToInt(vehicle.tonnage)
                    });
                }
            }
        }
    }
}
