using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace IRTweaks.Modules.Combat
{
    // make buildings have blue damage number floaties to differentiate
    [HarmonyPatch(typeof(CombatHUDFloatie), "Init")]
    public static class CombatHUDFloatie_Init_Patch
    {

        public static bool Prepare() => Mod.Config.Fixes.BuildingDamageColorChange;

        public static void Postfix(CombatHUDFloatie __instance)
        {
            CombatHUDFloatieAnchor anchor = __instance.GetComponentInParent<CombatHUDFloatieAnchor>();
            if (anchor.DisplayedActor == null)
            {
                __instance.floatieText.color = Color.blue * Color.white;
            }
        }
    }
}
