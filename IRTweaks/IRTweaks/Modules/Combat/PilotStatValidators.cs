using BattleTech;
using UnityEngine;

namespace IRTweaks.Modules.Combat {
    public static class PilotStatValidators {
        public static bool Pilot_InitStatValidators_Prefix(Pilot __instance, StatCollection ___statCollection) {
            Mod.Log.Trace?.Write("P:ISV entered.");

            ___statCollection.SetValidator<int>("Piloting", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));
            ___statCollection.SetValidator<int>("Gunnery", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));
            ___statCollection.SetValidator<int>("Guts", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));
            ___statCollection.SetValidator<int>("Tactics", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));

            ___statCollection.SetValidator<int>("Injuries", new Statistic.Validator<int>(__instance.PilotInjuryValidator<int>));

            return false;
        }
        static bool CustomPilotAttributeValidator<T>(ref int newValue) {
            newValue = Mathf.Clamp(newValue, 1, Mod.Config.Combat.PilotAttributesMax);
            return true;
        }
    }
}
