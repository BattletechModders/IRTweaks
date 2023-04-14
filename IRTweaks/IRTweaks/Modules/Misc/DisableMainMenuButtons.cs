using System;
using UnityEngine;

namespace IRTweaks.Modules.Misc
{

    [HarmonyPatch(typeof(MainMenu), "Init")]
    public static class MainMenu_Init
    {

        static bool Prepare() => Mod.Config.Fixes.DisableCampaign || Mod.Config.Fixes.DisableDebug;

        public static void Prefix(MainMenu __instance)
        {
            if (Mod.Config.Fixes.DisableCampaign)
            {
                Mod.Log.Info?.Write($"Disabling the campaign button on the main menu.");
                try
                {
                    foreach (HBSButton button in __instance.topLevelMenu.RadioButtons)
                    {
                        if (button.gameObject != null && button.gameObject.name == "button-CAMPAIGN")
                        {
                            button.gameObject.SetActive(false);
                        }
                    }
                }
                catch (Exception e)
                {
                    Mod.Log.Error?.Write("Failed to disable the campaign!");
                    Mod.Log.Error?.Write(e);
                }
            }
        }

        public static void Postfix(MainMenu __instance)
        {
            if (Mod.Config.Fixes.DisableDebug)
            {
                Mod.Log.Info?.Write($"Disabling the debug buttons in the main menu.");
                try
                {
                    GameObject[] array = __instance.enableInDevelopmentbuildsOnly;
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].SetActive(false);
                    }
                }
                catch (Exception e)
                {
                    Mod.Log.Error?.Write("Failed to disable debug buttons!");
                    Mod.Log.Error?.Write(e);
                }
            }
        }
    }
}
