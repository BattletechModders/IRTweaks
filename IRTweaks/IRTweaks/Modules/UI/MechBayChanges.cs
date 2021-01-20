using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    [HarmonyPatch(typeof(MechLabPanel), "SetData")]
    [HarmonyPatch(new Type[] { typeof(MechDef), typeof(MechBayPanel), typeof(SimGameState), typeof(List<MechComponentRef>), typeof(WorkOrderEntry_MechLab), typeof(UnityAction<List<WorkOrderEntry>, string, int>), typeof(UnityAction) })]
    static class MechLabPanel_SetData_1
    {
        static bool Prepare() => Mod.Config.Fixes.MechbayLayout;

        static void Postfix(MechLabPanel __instance)
        {
            try
            {
                // Change the label on the confirm button
                Mod.Log.Info?.Write($"MLP:SD - walking transforms of GO: {__instance.gameObject.name}");

                Transform cancelConfirmT = __instance.gameObject.transform.Find("Representation/OBJ_cancelconfirm");
                if (cancelConfirmT == null) Mod.Log.Error?.Write("Failed to find OBJ_cancelconfirm!");
                
                Transform readyTextT = cancelConfirmT.Find("uixPrfBttn_BASE_button2-MANAGED-confirm/bttn2_contentLayout/ready_Text-optional");
                if (readyTextT == null) Mod.Log.Error?.Write("Failed to find ready_Text-optional!");

                LocalizableText buttonText = readyTextT.gameObject.GetComponent<LocalizableText>();
                if (buttonText == null) Mod.Log.Error?.Write("Failed to find LocalizableText!");
                buttonText.SetText("VALIDATE");

                // Disable the store button
                if (Mod.Config.Fixes.MechbayLayoutDisableStore)
                {
                    Transform storeButtonT = __instance.gameObject.transform.Find("Representation/OBJGROUP_LEFT/OBJ_inventoryLong/OBJ_storeButton");
                    if (storeButtonT == null) Mod.Log.Error?.Write("Failed to find OBJ_storeButton!");
                    storeButtonT.gameObject.SetActive(false);
                }
            }
            catch (Exception e)
            {
                Mod.Log.Warn?.Write(e, $"Failed to set MechLab confirm button to new text");
            }


        }
    }

    //[HarmonyPatch(typeof(MechLabPanel), "SetData")]
    //[HarmonyPatch(new Type[] { typeof(MechDef), typeof(DataManager), typeof(UnityAction), typeof(UnityAction), typeof(bool)})]
    //static class MechLabPanel_SetData_2
    //{
    //    static bool Prepare() => Mod.Config.Fixes.MechbayLayout;

    //    static void Postfix(MechLabPanel __instance)
    //    {
    //        try
    //        {
    //            Transform cancelConfirmT = __instance.gameObject.transform.Find("uixPrfBttn_BASE_button2-MANAGED-confirm");
    //            Transform readyTextT = cancelConfirmT.Find("ready_Text-optional");
    //            LocalizableText buttonText = readyTextT.gameObject.GetComponent<LocalizableText>();
    //            buttonText.SetText("VALIDATE");
    //        }
    //        catch (Exception e)
    //        {
    //            Mod.Log.Warn?.Write(e, $"Failed to set MechLab confirm button to new text");
    //        }
    //    }
    //}

}
