using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace IRTweaks.Modules.UI
{
    [HarmonyPatch(typeof(MechLabPanel), "ToggleLayout")]
    static class MechLabPanel_ToggleLayout
    {
        static bool Prepare() => Mod.Config.Fixes.MechbayLayout;

        static bool HasRun = false;

        static void Postfix(MechLabPanel __instance, MechLabDismountWidget ___dismountWidget, HBSDOTweenButton ___btn_mechViewerButton)
        {
            if (__instance.IsSimGame && !HasRun)
            {
                // dismountWidget.gameObject.SetActive(IsSimGame); => change it's rectTransform.sizeDelta.y to 180
                if (___dismountWidget != null && ___dismountWidget.gameObject != null)
                {
                    RectTransform rt = ___dismountWidget.gameObject.GetComponent<RectTransform>();
                    Vector3 newDeltas = rt.sizeDelta;
                    newDeltas.y = 180;
                    rt.sizeDelta = newDeltas;
                }

                // btn_mechViewerButton.gameObject.SetActive(IsSimGame); => change it's pos_x from 1274 to 690
                if (___btn_mechViewerButton != null && ___btn_mechViewerButton.gameObject != null)
                {
                    RectTransform rt = ___btn_mechViewerButton.gameObject.GetComponent<RectTransform>();
                    Vector3 newPos = rt.position;
                    newPos.x -= 584;
                    rt.position = newPos;
                }

                HasRun = true;
            }
        }
    }
}
