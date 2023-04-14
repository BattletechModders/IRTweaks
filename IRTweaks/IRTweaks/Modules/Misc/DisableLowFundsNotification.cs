namespace IRTweaks.Modules.Misc
{

    [HarmonyPatch(typeof(SimGameState), "ShowLowFundsWarning")]
    public static class SimGameState_ShowLowFundsWarning
    {
        static bool Prepare() => Mod.Config.Fixes.DisableLowFundsNotification;

        static void Prefix(ref bool __runOriginal)
        {
            if (!__runOriginal) return;

            Mod.Log.Info?.Write($"Skipping Low Funds warning.");
            __runOriginal = false;
        }
    }
}
