using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IRTweaks.Modules.UI
{
    public static class ScrapHelper
    {
        public static void BuildScrapAllDialog(List<MechBayChassisUnitElement> filteredChassis,
            float scrapPartModifier, string title, Action confirmAction)
        {
            int cbills = ScrapHelper.CalculateTotalScrap(filteredChassis, scrapPartModifier);
            string cbillStr = SimGameState.GetCBillString(cbills);

            string descLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Desc_ScrapAll], new object[] { cbillStr }).ToString();
            string cancelLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Button_Cancel]).ToString();
            string scrapLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Button_Scrap]).ToString();
            GenericPopupBuilder.Create(title, descLT)
                .AddButton(cancelLT)
                .AddButton(scrapLT, confirmAction)
                .CancelOnEscape()
                .AddFader(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill)
                .Render();
        }

        public static void ScrapChassis(List<MechBayChassisUnitElement> filteredChassis)
        {
            MechBayPanel mechBayPanel = LazySingletonBehavior<UIManager>.Instance.GetOrCreateUIModule<MechBayPanel>();
            
            foreach (MechBayChassisUnitElement item in filteredChassis)
            {
                // parts 0 + max 0 = one unit
                int fullMechs = (item.PartsCount == 0 && item.PartsMax == 0) ?
                    1 : (int)Math.Floor((double)item.PartsCount / item.PartsMax);
                int partsCount = item.PartsCount - fullMechs;
                Mod.Log.Info?.Write($"Scrapping chassis: {item.ChassisDef.Description.Name}" +
                    $" with {fullMechs} complete mechs and {partsCount} parts.");

                for (int i = 0; i < fullMechs; i++)
                {
                    mechBayPanel.Sim.ScrapInactiveMech(item.ChassisDef.Description.Id, pay: true);
                }

                for (int i = 0; i < partsCount; i++)
                {
                    mechBayPanel.Sim.ScrapMechPart(item.ChassisDef.Description.Id, 1f, item.ChassisDef.MechPartMax, pay: true);
                }

                mechBayPanel.RefreshData(resetFilters: true);
                mechBayPanel.SelectChassis(null);
            }
        }

        public static int CalculateTotalScrap(List<MechBayChassisUnitElement> filteredChassis,  float scrapPartModifier)
        {
            float total = 0;
            foreach (MechBayChassisUnitElement item in filteredChassis)
            {
                Mod.Log.Info?.Write($"Part : {item.ChassisDef.Description.Name} {item.PartsCount} partsCounts with {item.PartsMax} partsMax");
                // parts 0 + max 0 = one unit
                int fullMechs = (item.PartsCount == 0 && item.PartsMax == 0) ?
                    1 : (int)Math.Floor((double)item.PartsCount / item.PartsMax);
                int partsCount = item.PartsCount - fullMechs;
                float partsFrac = item.PartsMax != 0 ? partsCount * (1f / item.PartsMax) : 0;
                float partsMulti = fullMechs + partsFrac;
                Mod.Log.Info?.Write($" -- {fullMechs} fullMechs, {partsCount} parts => {partsFrac} partsFrac, for partsMulti: {partsMulti}");

                int perPartScrap = Mathf.RoundToInt((float)item.ChassisDef.Description.Cost * scrapPartModifier);
                int totalScrapForChassis = (int)Math.Floor(perPartScrap * partsMulti);
                Mod.Log.Info?.Write($" -- perPartScrap: {perPartScrap} x partsMulti: {partsMulti} => totalScrapForChassis: {totalScrapForChassis}");

                total += totalScrapForChassis;
            }
            
            int cbills = (int)Math.Ceiling(total);

            return cbills;
        }
    }

    [HarmonyPatch(typeof(MechBayMechStorageWidget), "Filter_WeightAll")]
    static class BulkScrapping_MechBayMechStorageWidget_Filter_WeightAll
    {
        static bool Prepare => Mod.Config.Fixes.BulkScrapping;

        static void Postfix(MechBayMechStorageWidget __instance, List<IMechLabDraggableItem> ___inventory)
        {
            if (___inventory != null && ___inventory.Count > 0 && 
                Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                string titleLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Title_ScrapAll]).ToString();

                List<MechBayChassisUnitElement> filteredItems = ___inventory
                    .OfType<MechBayChassisUnitElement>()
                    .ToList();

                ScrapHelper.BuildScrapAllDialog(filteredItems, __instance.Sim.Constants.Finances.MechScrapModifier, titleLT, 
                    delegate { ScrapHelper.ScrapChassis(filteredItems); });
            }
        }
    }

    [HarmonyPatch(typeof(MechBayMechStorageWidget), "Filter_WeightAssault")]
    static class BulkScrapping_MechBayMechStorageWidget_Filter_WeightAssault
    {
        static bool Prepare => Mod.Config.Fixes.BulkScrapping;

        static void Postfix(MechBayMechStorageWidget __instance, List<IMechLabDraggableItem> ___inventory)
        {
            if (___inventory != null && ___inventory.Count > 0 &&
                Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                string titleLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Title_ScrapAssaults]).ToString();

                List<MechBayChassisUnitElement> filteredItems = ___inventory
                    .OfType<MechBayChassisUnitElement>()
                    .Where(x => x.ChassisDef.weightClass == WeightClass.ASSAULT)
                    .ToList();

                ScrapHelper.BuildScrapAllDialog(filteredItems, __instance.Sim.Constants.Finances.MechScrapModifier, titleLT,
                    delegate { ScrapHelper.ScrapChassis(filteredItems); });
            }
        }
    }

    [HarmonyPatch(typeof(MechBayMechStorageWidget), "Filter_WeightHeavy")]
    static class BulkScrapping_MechBayMechStorageWidget_Filter_WeightHeavy
    {
        static bool Prepare => Mod.Config.Fixes.BulkScrapping;

        static void Postfix(MechBayMechStorageWidget __instance, List<IMechLabDraggableItem> ___inventory)
        {
            if (___inventory != null && ___inventory.Count > 0 &&
                Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                string titleLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Title_ScrapHeavies]).ToString();

                List<MechBayChassisUnitElement> filteredItems = ___inventory
                    .OfType<MechBayChassisUnitElement>()
                    .Where(x => x.ChassisDef.weightClass == WeightClass.HEAVY)
                    .ToList();

                ScrapHelper.BuildScrapAllDialog(filteredItems, __instance.Sim.Constants.Finances.MechScrapModifier, titleLT,
                    delegate { ScrapHelper.ScrapChassis(filteredItems); });
            }
        }
    }

    [HarmonyPatch(typeof(MechBayMechStorageWidget), "Filter_WeightLight")]
    static class BulkScrapping_MechBayMechStorageWidget_Filter_WeightLight
    {
        static bool Prepare => Mod.Config.Fixes.BulkScrapping;

        static void Postfix(MechBayMechStorageWidget __instance, List<IMechLabDraggableItem> ___inventory)
        {
            if (___inventory != null && ___inventory.Count > 0 &&
                Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                string titleLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Title_ScrapLights]).ToString();

                List<MechBayChassisUnitElement> filteredItems = ___inventory
                    .OfType<MechBayChassisUnitElement>()
                    .Where(x => x.ChassisDef.weightClass == WeightClass.LIGHT)
                    .ToList();

                ScrapHelper.BuildScrapAllDialog(filteredItems, __instance.Sim.Constants.Finances.MechScrapModifier, titleLT,
                    delegate { ScrapHelper.ScrapChassis(filteredItems); });
            }
        }
    }

    [HarmonyPatch(typeof(MechBayMechStorageWidget), "Filter_WeightMedium")]
    static class BulkScrapping_MechBayMechStorageWidget_Filter_WeightMedium
    {
        static bool Prepare => Mod.Config.Fixes.BulkScrapping;

        static void Postfix(MechBayMechStorageWidget __instance, List<IMechLabDraggableItem> ___inventory)
        {
            if (___inventory != null && ___inventory.Count > 0 &&
                Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                string titleLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Title_ScrapMediums]).ToString();

                List<MechBayChassisUnitElement> filteredItems = ___inventory
                    .OfType<MechBayChassisUnitElement>()
                    .Where(x => x.ChassisDef.weightClass == WeightClass.MEDIUM)
                    .ToList();

                ScrapHelper.BuildScrapAllDialog(filteredItems, __instance.Sim.Constants.Finances.MechScrapModifier, titleLT,
                    delegate { ScrapHelper.ScrapChassis(filteredItems); });
            }
        }
    }
}
