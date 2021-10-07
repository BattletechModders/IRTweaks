using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace IRTweaks.Modules.UI
{
    class TgtComputerTonnage
    {
        [HarmonyPatch(typeof(CombatHUDActorDetailsDisplay), "RefreshInfo")]
        public static class CombatHUDActorDetailsDisplay_RefreshInfo_Patch
        {
            static bool Prepare() => Mod.Config.Fixes.TgtComputerTonnageDisplay;

            [HarmonyBefore(new string[] {"io.mission.customunits"})]

            public static void Postfix(CombatHUDActorDetailsDisplay __instance)
            {
                if (__instance.DisplayedActor is Mech mech)
                {
                    __instance.ActorWeightText.SetText("{0}: {1} ({2}T)", new object[]
                    {
                        "'MECH",
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
