using BattleTech.UI;
using Harmony;

// ReSharper disable InconsistentNaming

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(SGLoadSavedGameScreen), "CloseIfEmpty")]
    public static class SGLoadSavedGameScreen_CloseIfEmpty_Patch
    {
        public static bool Prepare() => !Mod.Config.Fixes.SkipDeleteSavePopup;

        public static bool Prefix() => false;
    }
}
