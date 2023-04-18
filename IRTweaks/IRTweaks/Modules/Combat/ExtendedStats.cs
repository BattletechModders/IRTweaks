using UnityEngine;

namespace IRTweaks.Modules.Combat
{
    [HarmonyPatch(typeof(Pilot), "InitStatValidators")]
    static class ExtendedStats_Pilot_InitStatValidators
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.ExtendedStats;

        [HarmonyPrefix]
        static void Prefix(ref bool __runOriginal, Pilot __instance)
        {
            if (!__runOriginal) return;

            Mod.Log.Trace?.Write("P:ISV entered.");

            __instance.statCollection.SetValidator<int>("Piloting", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));
            __instance.statCollection.SetValidator<int>("Gunnery", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));
            __instance.statCollection.SetValidator<int>("Guts", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));
            __instance.statCollection.SetValidator<int>("Tactics", new Statistic.Validator<int>(CustomPilotAttributeValidator<int>));

            __instance.statCollection.SetValidator<int>("Injuries", new Statistic.Validator<int>(__instance.PilotInjuryValidator<int>));

            __runOriginal = false;
        }

        static bool CustomPilotAttributeValidator<T>(ref int newValue)
        {
            newValue = Mathf.Clamp(newValue, 1, Mod.Config.Combat.PilotAttributesMax);
            return true;
        }
    }
}
