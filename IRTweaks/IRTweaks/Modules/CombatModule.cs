namespace IRTweaks.Modules.Combat
{

    public static class CombatFixes
    {
        static bool Initialized = false;
        public static class State
        {
            public static void Reset()
            {
            }
        }

        public static void InitModule()
        {
            if (!Initialized)
            {
                Mod.Log.Info?.Write("-- Initializing Combat Fixes --");

                if (Mod.Config.Fixes.BraceOnMeleeWithJuggernaut)
                    Mod.Log.Info?.Write("Activating Fix: BraceOnMeleeWithJuggernaut");

                if (Mod.Config.Fixes.BuildingDamageColorChange)
                    Mod.Log.Info?.Write("Activating Fix: BuildingDamageColorChange");

                if (Mod.Config.Fixes.CalledShotTweaks)
                    Mod.Log.Info?.Write("Activating Fix: CalledShotTweaks");

                if (Mod.Config.Fixes.ExtendedStats)
                    Mod.Log.Info?.Write("Activating Fix: ExtendedStats");

                if (Mod.Config.Fixes.FlexibleSensorLock)
                    Mod.Log.Info?.Write("Activating Fix: FlexibleSensorLock");

                if (Mod.Config.Fixes.PainTolerance)
                    Mod.Log.Info?.Write("Activating Fix: PainTolerance");

                if (Mod.Config.Fixes.PathfinderTeamFix)
                    Mod.Log.Info?.Write("Activating Fix: PathfinderTeamFix");

                if (Mod.Config.Fixes.ScaleObjectiveBuildingStructure)
                    Mod.Log.Info?.Write("Activating Fix: ScaleObjectiveBuildingStructure");

                if (Mod.Config.Fixes.SpawnProtection)
                    Mod.Log.Info?.Write("Activating Fix: SpawnProtection");

                if (Mod.Config.Fixes.UrbanExplosionsFix)
                    Mod.Log.Info?.Write("Activating Fix: UrbanExplosionsFix");

            }
            Initialized = true;
        }
    }
}
