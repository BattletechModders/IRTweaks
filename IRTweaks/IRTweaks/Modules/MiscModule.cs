using System;

namespace IRTweaks.Modules.Misc
{
    public static class MiscFixes
    {
        static bool Initialized = false;

        public static void InitModule(HarmonyInstance harmony)
        {
            if (!Initialized)
            {
                try
                {
                    // Update the pilot stats to have a maximum greater than 10
                    if (Mod.Config.Fixes.RandomStartByDifficulty)
                        Mod.Log.Info?.Write("Activating Fix: RandomStartByDifficulty");

                    if (Mod.Config.Fixes.DisableCampaign)
                        Mod.Log.Info?.Write("Activating Fix: DisableCampaign");

                    if (Mod.Config.Fixes.DisableLowFundsNotification)
                        Mod.Log.Info?.Write("Activating Fix: DisableLowFundsNotification");

                    if (Mod.Config.Fixes.DisableMPHashCalculation)
                        Mod.Log.Info?.Write("Activating Fix: DisableMPHashCalculation");

                    if (Mod.Config.Fixes.MultiTargetStat)
                        Mod.Log.Info?.Write("Activating Fix: MultiTargetStat");

                    if (Mod.Config.Fixes.ReduceSaveCompression)
                        Mod.Log.Info?.Write("Activating fix: ReduceSaveCompression.");

                    if (Mod.Config.Fixes.ShowAllArgoUpgrades)
                        Mod.Log.Info?.Write("Activating fix: ShowAllArgoUpgrades.");

                    if (Mod.Config.Fixes.SkipDeleteSavePopup)
                        Mod.Log.Info?.Write("Activating fix: SkipDeleteSavePopup.");

                    if (Mod.Config.Fixes.SkirmishReset)
                        Mod.Log.Info?.Write("Activating fix: SkirmishReset.");


                }
                catch (Exception e)
                {
                    Mod.Log.Error?.Write($"Failed to load patches due to: {e.Message}");
                    Mod.Log.Error?.Write(e);
                }
            }
            Initialized = true;
        }
    }
}
