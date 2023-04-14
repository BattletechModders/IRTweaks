// ReSharper disable InconsistentNaming

namespace IRTweaks.Modules.Misc
{

    // Disables the confirmation on deleting save and immediately deletes it
    [HarmonyPatch(typeof(SGLoadSavedGameScreen), "CloseIfEmpty")]
    public static class SGLoadSavedGameScreen_CloseIfEmpty_Patch
    {
        public static bool Prepare() => !Mod.Config.Fixes.SkipDeleteSavePopup;

        public static bool Prefix() => false;
    }
}
