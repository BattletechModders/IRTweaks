using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using CustAmmoCategories;
using Harmony;
using HBS.Extensions;
using Localize;
using UnityEngine;

namespace IRTweaks.Modules.UI
{
    [HarmonyPatch(typeof(SimGameDifficultySettingsModule), "InitSettings")]
    static class SimGameDifficultySettingsModule_InitSettings
    {
        static bool Prepare() => Mod.Config.Fixes.SimGameDifficultyLabelsReplacer;

        static void Postfix(SimGameDifficultySettingsModule __instance, PreGameCareerModeSettingsTotalScoreDescAndBar ___difficultyBarAndMod)
        {
            if (__instance != null && ___difficultyBarAndMod != null)
            {
                ___difficultyBarAndMod.TotalScoreModifierLabel.SetText(
                    new Text(Mod.Config.SimGameDifficultyStrings[ModConfig.SimGameDifficultyString_Desc], new object[] { })
                    );

                GameObject difficultyTotalGO = __instance.gameObject.FindFirstChildNamed("OBJ_difficultyTotal");
                if (difficultyTotalGO != null)
                {
                    // Find difTotal-text
                    GameObject diffTotalGO = difficultyTotalGO.FindFirstChildNamed("difTotal-text");
                    if (diffTotalGO != null)
                    {
                        // Use the label here
                        LocalizableText localText = diffTotalGO.GetComponent<LocalizableText>();
                        if (localText != null)
                        {
                            Mod.Log.Info?.Write("UPDATED LABEL");
                            localText.SetText(Mod.Config.SimGameDifficultyStrings[ModConfig.SimGameDifficultyString_Label]);
                        }
                        else
                        {
                            Mod.Log.Warn?.Write("FAILED TO FIND LocalizableText COMP FOR `diffTotal-text`");
                        }

                    }
                    else
                    {
                        Mod.Log.Warn?.Write("FAILED TO FIND `diffTotal-text` CHILD OF `OBJ_difficultyTotal`!");
                    }
                }
            }
        }
    }

}
