using System;
using System.Collections.Generic;

namespace IRTweaks.Modules.Combat
{
    // Updated reference to https://github.com/RealityMachina/Better-Juggernaut, further refined by LadyAlekto
    //   as MightyJuggernaut. Applies braced on attack if your pilot has the Guts 8 ability ability. 

    [HarmonyPatch(typeof(MechMeleeSequence), "ConsumesFiring", MethodType.Getter)]
    static class MechMeleeSequence_ConsumesFiring_Getter
    {
        static bool Prepare() => Mod.Config.Fixes.BraceOnMeleeWithJuggernaut;

        static void Prefix(MechMeleeSequence __instance)
        {
            if (__instance == null || __instance.OwningMech == null || __instance.OwningMech.GetPilot() == null)
                return; // Nothing to do

            List<Ability> passives = __instance.OwningMech.GetPilot().PassiveAbilities;
            if (passives.Count > 0)
            {
                foreach (Ability ability in passives)
                {
                    if (ability.Def.Description.Id.Equals(Mod.Config.Abilities.JuggernautId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Mod.Log.Info?.Write("Pilot has Juggernaut, bracing after melee attack using Mech.ApplyBraced.");
                        __instance.OwningMech.ApplyBraced();
                        //__instance.OwningMech.BracedLastRound = true;
                        //__instance.OwningMech.ApplyInstabilityReduction(StabilityChangeSource.Bracing);
                    }
                }
            }

            return;
        }
    }

    [HarmonyPatch(typeof(MechDFASequence), "ConsumesFiring", MethodType.Getter)]
    static class MechDFASequence_ConsumesFiring_Getter
    {
        static bool Prepare() => Mod.Config.Fixes.BraceOnMeleeWithJuggernaut;

        static void Prefix(MechDFASequence __instance)
        {
            if (__instance == null || __instance.OwningMech == null || __instance.OwningMech.GetPilot() == null)
                return; // Nothing to do

            List<Ability> passives = __instance.OwningMech.GetPilot().PassiveAbilities;
            if (passives.Count > 0)
            {
                foreach (Ability ability in passives)
                {
                    if (ability.Def.Description.Id.Equals(Mod.Config.Abilities.JuggernautId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Mod.Log.Info?.Write("Pilot has Juggernaut, bracing after DFA attack using Mech.ApplyBraced.");
                        __instance.OwningMech.ApplyBraced();
                        //__instance.OwningMech.BracedLastRound = true;
                        //__instance.OwningMech.ApplyInstabilityReduction(StabilityChangeSource.Bracing);
                    }
                }
            }

            return;
        }
    }
}
