using IRTweaks.Modules.UI;

namespace IRTweaks.Modules.Tooltip
{
    public static class UIFixes
    {
        static bool Initialized = false;

        public static void InitModule()
        {
            if (!Initialized)
            {
                Mod.Log.Info?.Write("-- Initializing UI Fixes --");

                // Enable the CombatLog
                if (Mod.Config.Fixes.CombatLog)
                {
                    Mod.Log.Info?.Write("Activating Fix: CombatLog");

                    // Initialize the helpers
                    CombatLog.InitModule();
                }

                if (Mod.Config.Fixes.SkirmishAlwaysUnlimited)
                    Mod.Log.Info?.Write("Activating Fix: SkirmishAlwaysUnlimited");

                if (Mod.Config.Fixes.DisableCombatSaves)
                    Mod.Log.Info?.Write("Activating Fix: DisableCombatSaves");

                if (Mod.Config.Fixes.DisableCombatRestarts)
                    Mod.Log.Info?.Write("Activating Fix: DisableCombatRestart");

                // Makes the main menu a smoother as there are fewer
                if (Mod.Config.Fixes.StreamlinedMainMenu)
                    Mod.Log.Info?.Write("Activating Fix: StreamlinedMainMenu");

                // Update the weapon tooltip to support CAC behaviors
                if (Mod.Config.Fixes.WeaponTooltip)
                    Mod.Log.Info?.Write("Activating Fix: WeaponTooltip");

                if (Mod.Config.Fixes.DamageReductionInCombatHud)
                    Mod.Log.Info?.Write("Activating Fix: DamageReductionInCombatHud");
            }
            Initialized = true;
        }

    }
}
