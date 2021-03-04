using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using System;
using System.Collections.Generic;
using HBS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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

    [HarmonyPatch(typeof(MechLabPanel), "OnMaxArmor")]
    static class MechLabPanel_OnMaxArmor_Patch
    {
        static bool Prepare() => Mod.Config.Fixes.MaxArmorMaxesArmor;
        static bool Prefix(MechLabPanel __instance, MechLabMechInfoWidget ___mechInfoWidget, MechLabItemSlotElement ___dragItem)
        {
            if (!__instance.Initialized)
            {
                return false;
            }
            if (___dragItem != null)
            {
                return false;
            }
            if (__instance.headWidget.IsDestroyed || __instance.centerTorsoWidget.IsDestroyed || __instance.leftTorsoWidget.IsDestroyed || __instance.rightTorsoWidget.IsDestroyed || __instance.leftArmWidget.IsDestroyed || __instance.rightArmWidget.IsDestroyed || __instance.leftLegWidget.IsDestroyed || __instance.rightLegWidget.IsDestroyed)
            {
                __instance.modifiedDialogShowing = true;
                GenericPopupBuilder.Create("'Mech Location Destroyed", "You cannot auto-assign armor while a 'Mech location is Destroyed.").AddButton("Okay", null, true, null).CancelOnEscape().AddFader(new UIColorRef?(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill), 0f, true).SetAlwaysOnTop().SetOnClose(delegate
                {
                    __instance.modifiedDialogShowing = false;
                }).Render();
                return false;
            }

            __instance.headWidget.ModifyArmor(false, __instance.headWidget.maxArmor, true);
			__instance.centerTorsoWidget.ModifyArmor(false, __instance.centerTorsoWidget.maxArmor, true);
            __instance.centerTorsoWidget.ModifyArmor(true, __instance.centerTorsoWidget.maxRearArmor, true);
            __instance.leftTorsoWidget.ModifyArmor(false, __instance.leftTorsoWidget.maxArmor, true);
            __instance.leftTorsoWidget.ModifyArmor(true, __instance.leftTorsoWidget.maxRearArmor, true);
            __instance.rightTorsoWidget.ModifyArmor(false, __instance.rightTorsoWidget.maxArmor, true);
            __instance.rightTorsoWidget.ModifyArmor(true, __instance.rightTorsoWidget.maxRearArmor, true);
            __instance.leftArmWidget.ModifyArmor(false, __instance.leftArmWidget.maxArmor, true);
            __instance.rightArmWidget.ModifyArmor(false, __instance.rightArmWidget.maxArmor, true);
            __instance.leftLegWidget.ModifyArmor(false, __instance.leftLegWidget.maxArmor, true);
            __instance.rightLegWidget.ModifyArmor(false, __instance.rightLegWidget.maxArmor, true);
            ___mechInfoWidget.RefreshInfo(false);

            __instance.FlagAsModified();
            __instance.ValidateLoadout(false);
            return false;
        }
    }

    [HarmonyPatch(typeof(MechLabPanel), "OnStripEquipment")]
    static class MechLabPanel_OnStripEquipment_Patch
    {
        static bool Prepare() => Mod.Config.Fixes.MechbayAdvancedStripping;

        static bool Prefix(MechLabPanel __instance, bool ___batchActionInProgress, MechLabItemSlotElement ___dragItem)
        {
            var hk = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (!hk) return true;

            if (!__instance.Initialized)
            {
                return false;
            }
            if (___dragItem != null)
            {
                return false;
            }
            ___batchActionInProgress = true;
            __instance.headWidget.AdvancedStripping(__instance);
            __instance.centerTorsoWidget.AdvancedStripping(__instance);
            __instance.leftTorsoWidget.AdvancedStripping(__instance);
            __instance.rightTorsoWidget.AdvancedStripping(__instance);
            __instance.leftArmWidget.AdvancedStripping(__instance);
            __instance.rightArmWidget.AdvancedStripping(__instance);
            __instance.leftLegWidget.AdvancedStripping(__instance);
            __instance.rightLegWidget.AdvancedStripping(__instance);
            __instance.FlagAsModified();
            __instance.ValidateLoadout(false);
            ___batchActionInProgress = false;
            return false;
        }

        internal static void AdvancedStripping(this MechLabLocationWidget widget, MechLabPanel panel)
        {
            if (!panel.Initialized)
            {
                return;
            }

            var loclInv = Traverse.Create(widget).Field("localInventory").GetValue<List<MechLabItemSlotElement>>();

            for (int i = loclInv.Count - 1; i >= 0; i--)
            {
                MechLabItemSlotElement mechLabItemSlotElement = loclInv[i];
                if (!mechLabItemSlotElement.ComponentRef.IsFixed && (mechLabItemSlotElement.ComponentRef.Def is WeaponDef || mechLabItemSlotElement.ComponentRef.Def is AmmunitionBoxDef))
                {
                    widget.OnRemoveItem(mechLabItemSlotElement, true);
                    panel.ForceItemDrop(mechLabItemSlotElement);
                }
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
