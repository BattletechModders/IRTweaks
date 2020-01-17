using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRTweaks.Modules.Misc {

    [HarmonyPatch(typeof(MainMenu), "Init")]
    public static class MainMenu_Init {

        static bool Prepare() { return Mod.Config.Fixes.DisableCampaign; }

        public static void Prefix(MainMenu __instance, HBSRadioSet ___topLevelMenu) {
            try {
                ___topLevelMenu.RadioButtons.Find((HBSButton x) => x.GetText() == "Campaign")
                    .gameObject.SetActive(false);
            } catch (Exception e) {
                Mod.Log.Error("Failed to disable the campaign!");
                Mod.Log.Error(e);
            }
        }
    }
}
