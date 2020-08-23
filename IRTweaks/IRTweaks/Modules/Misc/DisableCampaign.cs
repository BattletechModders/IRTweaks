using BattleTech.UI;
using Harmony;
using System;

namespace IRTweaks.Modules.Misc {

    [HarmonyPatch(typeof(MainMenu), "Init")]
    public static class MainMenu_Init {

        static bool Prepare() => Mod.Config.Fixes.DisableCampaign;

        public static void Prefix(HBSRadioSet ___topLevelMenu) {
            Mod.Log.Info?.Write($"Disabling the campaign button on the main menu.");
            try {
                foreach (HBSButton button in ___topLevelMenu.RadioButtons)
                {
                    if (button.gameObject != null && button.gameObject.name == "button-CAMPAIGN")
                    {
                        button.gameObject.SetActive(false);
                    }
                }
            } catch (Exception e) {
                Mod.Log.Error?.Write("Failed to disable the campaign!");
                Mod.Log.Error?.Write(e);
            }
        }
    }
}
