using BattleTech;
using BattleTech.Save;

namespace IRTweaks.Modules.Misc {
    public static class SkirmishReset {
        public static void CloudUserSettings_PostDeserialize_Prefix(ref LastUsedLances ___lastUsedLances,
            ref SkirmishUnitsAndLances ___customUnitsAndLances) {
            // Always reset the skirmish lances after serialization. This prevents the ever-spinny from missing mod pieces.
            ___lastUsedLances = new LastUsedLances();
            ___customUnitsAndLances = new SkirmishUnitsAndLances();
        }
    }
}
