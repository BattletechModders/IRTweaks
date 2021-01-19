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

    public class ChassisCount
    {
        public ChassisDef ChassisDef;
        public int PartsCount;
        public int PartsMax;
        public int ChassisQty;
    }

    public static class ScrapHelper
    {
        public static void BuildScrapAllDialog(List<ChassisCount> filteredChassis,
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

        public static void ScrapChassis(List<ChassisCount> filteredChassis)
        {
            MechBayPanel mechBayPanel = LazySingletonBehavior<UIManager>.Instance.GetOrCreateUIModule<MechBayPanel>();
            
            foreach (ChassisCount item in filteredChassis)
            {

                Mod.Log.Debug?.Write($"Scrapping chassis: {item.ChassisDef.Description.Name}");
                for (int i = 0; i < item.ChassisQty ; i++)
                {
                    mechBayPanel.Sim.ScrapInactiveMech(item.ChassisDef.Description.Id, pay: true);
                    Mod.Log.Debug?.Write($"  -- scrapped one full mech");
                }

                for (int i = 0; i < item.PartsCount ; i++)
                {
                    Mod.Log.Debug?.Write($"  -- scrapped one mech part");
                    mechBayPanel.Sim.ScrapMechPart(item.ChassisDef.Description.Id, 1f, item.ChassisDef.MechPartMax, pay: true);
                }

                mechBayPanel.RefreshData(resetFilters: true);
                mechBayPanel.SelectChassis(null);
            }
        }

        public static int CalculateTotalScrap(List<ChassisCount> filteredChassis, float scrapPartModifier)
        {
            float total = 0;
            foreach (ChassisCount item in filteredChassis)
            {

                Mod.Log.Debug?.Write($"Calculating scrap for chassis: {item.ChassisDef.Description.Name}");
                float scrapMulti = 0;

                Mod.Log.Debug?.Write($"  -- adding {item.ChassisQty} full chassis.");
                scrapMulti += item.ChassisQty;

                if (item.PartsCount > 0)
                {
                    float partsFrac = (float)item.PartsCount / (float)item.PartsMax;
                    scrapMulti += partsFrac;
                    Mod.Log.Debug?.Write($"  -- adding {partsFrac} fraction for {item.PartsCount} / {item.PartsMax} parts");
                }

                float scrapPricePerChassis = Mathf.RoundToInt((float)item.ChassisDef.Description.Cost * scrapPartModifier);
                Mod.Log.Debug?.Write($" -- scrapPricePerChassis: {scrapPricePerChassis} = chassisCost: {item.ChassisDef.Description.Cost} x scrapPartModifier: {scrapPartModifier}");

                int scrapValue = (int)Math.Floor(scrapMulti * scrapPricePerChassis);
                Mod.Log.Debug?.Write($" -- scrapPricePerChassis: {scrapPricePerChassis} x scrapMulti: {scrapMulti} => scrapValue: {scrapValue}");

                total += scrapValue;
            }

            Mod.Log.Info?.Write($"Calculated totalScrap value: {total}");
            int cbills = (int)Math.Ceiling(total);

            return cbills;
        }

        public static ChassisCount MapChassisUnitElement(MechBayChassisUnitElement mbcue, SimGameState sgs)
        {
            int chassisQty = sgs.GetItemCount(mbcue.ChassisDef.Description.Id, typeof(MechDef), SimGameState.ItemCountType.UNDAMAGED_ONLY);
            Mod.Log.Info?.Write($"Part : {mbcue.ChassisDef.Description.Name} has chassisQty: {chassisQty}  " +
                $"partsCount: {mbcue.PartsCount} partsMax: {mbcue.PartsMax}");

            return new ChassisCount()
            {
                ChassisDef = mbcue.ChassisDef,
                PartsCount = mbcue.PartsCount,
                PartsMax = mbcue.PartsMax,
                ChassisQty = chassisQty
            };
        }
    }

    [HarmonyPatch(typeof(MechBayMechStorageWidget), "Filter_WeightAll")]
    static class BulkScrapping_MechBayMechStorageWidget_Filter_WeightAll
    {
        static bool Prepare() => Mod.Config.Fixes.BulkScrapping;

        static void Postfix(MechBayMechStorageWidget __instance, List<IMechLabDraggableItem> ___inventory, IMechLabDropTarget ___parentDropTarget)
        {
            // Not MechBay, skip
            if (___parentDropTarget == null || !(___parentDropTarget is MechBayPanel mechBayPanel))
                return;

            // Components tab isn't selected, skip
            if (!__instance.gameObject.activeInHierarchy) return;

            Mod.Log.Info?.Write("MBMSW:F_WA FIRED");
            if (___inventory != null && ___inventory.Count > 0 && 
                Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                string titleLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Title_ScrapAll]).ToString();

                List<ChassisCount> chassisCounts = ___inventory
                    .OfType<MechBayChassisUnitElement>()
                    .Select(x => ScrapHelper.MapChassisUnitElement(x, __instance.Sim))
                    .ToList();

                ScrapHelper.BuildScrapAllDialog(chassisCounts, __instance.Sim.Constants.Finances.MechScrapModifier, titleLT, 
                    delegate { ScrapHelper.ScrapChassis(chassisCounts); });
            }
        }
    }

    [HarmonyPatch(typeof(MechBayMechStorageWidget), "Filter_WeightAssault")]
    static class BulkScrapping_MechBayMechStorageWidget_Filter_WeightAssault
    {
        static bool Prepare() => Mod.Config.Fixes.BulkScrapping;

        static void Postfix(MechBayMechStorageWidget __instance, List<IMechLabDraggableItem> ___inventory, IMechLabDropTarget ___parentDropTarget)
        {
            // Not MechBay, skip
            if (___parentDropTarget == null || !(___parentDropTarget is MechBayPanel mechBayPanel))
                return;

            // Components tab isn't selected, skip
            if (!__instance.gameObject.activeInHierarchy) return;



            if (___inventory != null && ___inventory.Count > 0 &&
                Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                string titleLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Title_ScrapAssaults]).ToString();

                List<ChassisCount> chassisCounts = ___inventory
                    .OfType<MechBayChassisUnitElement>()
                    .Where(x => x.ChassisDef.weightClass == WeightClass.ASSAULT)
                    .Select(x => ScrapHelper.MapChassisUnitElement(x, __instance.Sim))
                    .ToList();

                ScrapHelper.BuildScrapAllDialog(chassisCounts, __instance.Sim.Constants.Finances.MechScrapModifier, titleLT,
                    delegate { ScrapHelper.ScrapChassis(chassisCounts); });
            }
        }
    }

    [HarmonyPatch(typeof(MechBayMechStorageWidget), "Filter_WeightHeavy")]
    static class BulkScrapping_MechBayMechStorageWidget_Filter_WeightHeavy
    {
        static bool Prepare() => Mod.Config.Fixes.BulkScrapping;

        static void Postfix(MechBayMechStorageWidget __instance, List<IMechLabDraggableItem> ___inventory, IMechLabDropTarget ___parentDropTarget)
        {
            // Not MechBay, skip
            if (___parentDropTarget == null || !(___parentDropTarget is MechBayPanel mechBayPanel))
                return;

            // Components tab isn't selected, skip
            if (!__instance.gameObject.activeInHierarchy) return;


            if (___inventory != null && ___inventory.Count > 0 &&
                Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                string titleLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Title_ScrapHeavies]).ToString();

                List<ChassisCount> chassisCounts = ___inventory
                    .OfType<MechBayChassisUnitElement>()
                    .Where(x => x.ChassisDef.weightClass == WeightClass.HEAVY)
                    .Select(x => ScrapHelper.MapChassisUnitElement(x, __instance.Sim))
                    .ToList();

                ScrapHelper.BuildScrapAllDialog(chassisCounts, __instance.Sim.Constants.Finances.MechScrapModifier, titleLT,
                    delegate { ScrapHelper.ScrapChassis(chassisCounts); });
            }
        }
    }

    [HarmonyPatch(typeof(MechBayMechStorageWidget), "Filter_WeightLight")]
    static class BulkScrapping_MechBayMechStorageWidget_Filter_WeightLight
    {
        static bool Prepare() => Mod.Config.Fixes.BulkScrapping;

        static void Postfix(MechBayMechStorageWidget __instance, List<IMechLabDraggableItem> ___inventory, IMechLabDropTarget ___parentDropTarget)
        {
            // Not MechBay, skip
            if (___parentDropTarget == null || !(___parentDropTarget is MechBayPanel mechBayPanel))
                return;

            // Components tab isn't selected, skip
            if (!__instance.gameObject.activeInHierarchy) return;


            if (___inventory != null && ___inventory.Count > 0 &&
                Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                string titleLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Title_ScrapLights]).ToString();

                List<ChassisCount> chassisCounts = ___inventory
                    .OfType<MechBayChassisUnitElement>()
                    .Where(x => x.ChassisDef.weightClass == WeightClass.LIGHT)
                    .Select(x => ScrapHelper.MapChassisUnitElement(x, __instance.Sim))
                    .ToList();

                ScrapHelper.BuildScrapAllDialog(chassisCounts, __instance.Sim.Constants.Finances.MechScrapModifier, titleLT,
                    delegate { ScrapHelper.ScrapChassis(chassisCounts); });
            }
        }
    }

    [HarmonyPatch(typeof(MechBayMechStorageWidget), "Filter_WeightMedium")]
    static class BulkScrapping_MechBayMechStorageWidget_Filter_WeightMedium
    {
        static bool Prepare() => Mod.Config.Fixes.BulkScrapping;

        static void Postfix(MechBayMechStorageWidget __instance, List<IMechLabDraggableItem> ___inventory, IMechLabDropTarget ___parentDropTarget)
        {
            // Not MechBay, skip
            if (___parentDropTarget == null || !(___parentDropTarget is MechBayPanel mechBayPanel))
                return;

            // Components tab isn't selected, skip
            if (!__instance.gameObject.activeInHierarchy) return;


            if (___inventory != null && ___inventory.Count > 0 &&
                Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                string titleLT = new Text(Mod.LocalizedText.Dialog[ModText.DT_Title_ScrapMediums]).ToString();

                List<ChassisCount> chassisCounts = ___inventory
                    .OfType<MechBayChassisUnitElement>()
                    .Where(x => x.ChassisDef.weightClass == WeightClass.MEDIUM)
                    .Select(x => ScrapHelper.MapChassisUnitElement(x, __instance.Sim))
                    .ToList();

                ScrapHelper.BuildScrapAllDialog(chassisCounts, __instance.Sim.Constants.Finances.MechScrapModifier, titleLT,
                    delegate { ScrapHelper.ScrapChassis(chassisCounts); });
            }
        }
    }
}
