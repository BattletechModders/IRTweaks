using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
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

        public static void Postfix(MechDef mechDef, MechLabPanel ___mechLab, HBS_InputField ___mechNickname)
        {
            if (MechNamesHelper.HasUiName(___mechLab.activeMechDef))
            {
                ___mechNickname.SetText(___mechLab.activeMechDef.Description.UIName);
            }
        }
    }

    [HarmonyPatch(typeof(MechBayMechInfoWidget), "SetDescriptions")]
    public static class MechBayMechInfoWidget_SetDescriptions
    {
        static bool Prepare() { return MechNamesHelper.IsEnabled(); }

        public static void Postfix(MechBayMechInfoWidget __instance, MechDef ___selectedMech, HBS_InputField ___mechNameInput)
        {
            if (__instance != null && MechNamesHelper.HasUiName(___selectedMech))
            {
                ___mechNameInput.SetText(___selectedMech.Description.UIName);
            }
        }
    }

    [HarmonyPatch(typeof(LanceLoadoutMechItem), "SetData")]
    public static class LanceLoadoutMechItem_SetData
    {
        static bool Prepare() { return MechNamesHelper.IsEnabled(); }

        public static void Postfix(LanceLoadoutMechItem __instance, MechDef mechDef, LocalizableText ___MechNameText)
        {
            if (__instance != null && MechNamesHelper.HasUiName(mechDef))
            {
                ___MechNameText.SetText(mechDef.Description.UIName);
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
        public static void Postfix(object data, LocalizableText ___NameField, LocalizableText ___VariantField)
        {
            MechDef mechDef = data as MechDef;
            if (MechNamesHelper.HasUiName(mechDef))
            {
                ___NameField.SetText(mechDef.Description.UIName, Array.Empty<object>());
            }
            else
            {
                ___NameField.SetText(mechDef.Description.Name, Array.Empty<object>());
                ___VariantField.SetText("( {0} {1} )", new object[]
            {
                mechDef.Chassis.Description.Name,
                mechDef.Chassis.VariantName
            });
            }
        }
    }
}
