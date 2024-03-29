﻿using BattleTech.UI.Tooltips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace IRTweaks.Modules.UI
{

    static class MechNamesHelper
    {
        public static bool HasUiName(MechDef mechDef)
        {
            if (mechDef == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(mechDef.Description.UIName))
            {
                return false;
            }
            return true;
        }

        public static bool IsEnabled()
        {
            return Mod.Config.Fixes.AlternateMechNamingStyle;
        }
    }

    [HarmonyPatch(typeof(MechLabMechInfoWidget), "SetData")]
    [HarmonyPatch(new Type[] { typeof(MechDef) })]
    public static class MechLabMechInfoWidget_SetData
    {

        static bool Prepare() { return MechNamesHelper.IsEnabled(); }

        public static void Postfix(MechDef mechDef, MechLabMechInfoWidget __instance)
        {
            if (MechNamesHelper.HasUiName(__instance.mechLab.activeMechDef))
            {
                __instance.mechNickname.SetText(__instance.mechLab.activeMechDef.Description.UIName);
            }
        }
    }

    [HarmonyPatch(typeof(MechBayMechInfoWidget), "SetDescriptions")]
    public static class MechBayMechInfoWidget_SetDescriptions
    {
        static bool Prepare() { return MechNamesHelper.IsEnabled(); }

        public static void Postfix(MechBayMechInfoWidget __instance)
        {
            if (__instance != null && MechNamesHelper.HasUiName(__instance.selectedMech))
            {
                __instance.mechNameInput.SetText(__instance.selectedMech.Description.UIName);
            }
        }
    }

    [HarmonyPatch(typeof(LanceLoadoutMechItem), "SetData")]
    public static class LanceLoadoutMechItem_SetData
    {
        static bool Prepare() { return MechNamesHelper.IsEnabled(); }

        public static void Postfix(LanceLoadoutMechItem __instance, MechDef mechDef)
        {
            if (__instance != null && MechNamesHelper.HasUiName(mechDef))
            {
                __instance.MechNameText.SetText(mechDef.Description.UIName);
            }
        }
    }

    [HarmonyPatch(typeof(SkirmishMechBayPanel), "OnCopyMechClicked")]
    public static class SkirmishMechBayPanel_OnCopyMechClicked
    {
        static bool Prepare() { return MechNamesHelper.IsEnabled(); }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            var codes = new List<CodeInstruction>(instructions);
            MethodInfo nameGetMethod = AccessTools.Property(typeof(BaseDescriptionDef), nameof(BaseDescriptionDef.Name)).GetGetMethod();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand.Equals(nameGetMethod))
                {
                    MethodInfo uiNameGetMethod = AccessTools.Property(typeof(DescriptionDef), nameof(DescriptionDef.UIName)).GetGetMethod();
                    codes[i].operand = uiNameGetMethod;
                    break;
                }
            }

            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(TooltipPrefab_Mech), "SetData")]
    public static class TooltipPrefab_Mech_SetData
    {
        static bool Prepare() { return MechNamesHelper.IsEnabled(); }

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(TooltipPrefab_Mech __instance, object data)
        {
            MechDef mechDef = data as MechDef;
            if (MechNamesHelper.HasUiName(mechDef))
            {
                __instance.NameField.SetText(mechDef.Description.UIName, Array.Empty<object>());
            }
            else
            {
                __instance.NameField.SetText(mechDef.Description.Name, Array.Empty<object>());
                __instance.VariantField.SetText("( {0} {1} )", new object[]
            {
                mechDef.Chassis.Description.Name,
                mechDef.Chassis.VariantName
            });
            }
        }
    }
}
