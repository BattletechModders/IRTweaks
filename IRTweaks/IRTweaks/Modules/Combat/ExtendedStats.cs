using UnityEngine;

namespace IRTweaks.Modules.Combat
{
    [HarmonyPatch(typeof(Pilot), "InitStatValidators")]
    static class ExtendedStats_Pilot_InitStatValidators
    {
        static bool Prepare() => Mod.Config.Fixes.ExtendedStats;

        static bool Prefix(Pilot __instance, StatCollection ___statCollection)
        {
            Mod.Log.Trace?.Write("P:ISV entered.");

            ___statCollection.SetValidator<int>("Piloting", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));
            ___statCollection.SetValidator<int>("Gunnery", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));
            ___statCollection.SetValidator<int>("Guts", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));
            ___statCollection.SetValidator<int>("Tactics", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));

            ___statCollection.SetValidator<int>("Injuries", new Statistic.Validator<int>(__instance.PilotInjuryValidator<int>));

            return false;
        }

        static bool CustomPilotAttributeValidator<T>(ref int newValue)
        {
            newValue = Mathf.Clamp(newValue, 1, Mod.Config.Combat.PilotAttributesMax);
            return true;
        }
    }
}
