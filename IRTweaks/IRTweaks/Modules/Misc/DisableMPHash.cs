﻿namespace IRTweaks.Modules.Misc
{

    // Disables the multiplayer hash calculation at startup. Saves 2-3s of time at the 'black screen' before the main menu.
    [HarmonyPatch(typeof(UnityGameInstance), "CalcGameHash")]
    public static class UnityGameInstance_CalcGameHash
    {

        static bool Prepare() { return Mod.Config.Fixes.DisableMPHashCalculation; }

        static void Prefix(ref bool __runOriginal)
        {
            if (!__runOriginal) return;

            __runOriginal = false;
        }
    }
}
