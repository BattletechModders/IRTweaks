using BattleTech;
using Harmony;
using UnityEngine;

namespace IRTweaks.Patches {

    [HarmonyPatch(typeof(Pilot), "InitStatValidators")]
    public static class Pilot_InitStatValidators {

        public static bool Prefix(Pilot __instance, StatCollection ___statCollection) {
            Mod.Log.Debug("P:ISV entered.");

            ___statCollection.SetValidator<int>("Piloting", new Statistic.Validator<int>(Pilot_InitStatValidators.CustomPilotAttributeValidator<int>));
            ___statCollection.SetValidator<int>("Gunnery", new Statistic.Validator<int>(Pilot_InitStatValidators.CustomPilotAttributeValidator<int>));
            ___statCollection.SetValidator<int>("Guts", new Statistic.Validator<int>(Pilot_InitStatValidators.CustomPilotAttributeValidator<int>));
            ___statCollection.SetValidator<int>("Tactics", new Statistic.Validator<int>(Pilot_InitStatValidators.CustomPilotAttributeValidator<int>));

            ___statCollection.SetValidator<int>("Injuries", new Statistic.Validator<int>(__instance.PilotInjuryValidator<int>));

            return false;
        }

        static bool CustomPilotAttributeValidator<T>(ref int newValue) {
            newValue = Mathf.Clamp(newValue, 1, 13);
            return true;
        }
    }
}
