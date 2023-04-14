using BattleTech.Save;

namespace IRTweaks.Modules.Misc
{

    // Resets all custom lances and units to allow a clean loading state
    [HarmonyPatch(typeof(CloudUserSettings), "PostDeserialize")]
    static class SkirmishReset
    {

        static bool Prepare() => Mod.Config.Fixes.SkirmishReset;

        static void Prefix(ref bool __runOriginal, ref LastUsedLances ___lastUsedLances, ref SkirmishUnitsAndLances ___customUnitsAndLances)
        {
            if (!__runOriginal) return;

            // Always reset the skirmish lances after serialization. This prevents the ever-spinny from missing mod pieces.
            Mod.Log.Info?.Write("Resetting used lances and custom units after PostDeserialize call.");
            ___lastUsedLances = new LastUsedLances();
            ___customUnitsAndLances = new SkirmishUnitsAndLances();
        }
    }
}
