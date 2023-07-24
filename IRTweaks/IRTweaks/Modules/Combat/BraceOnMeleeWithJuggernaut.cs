using System;
using System.Collections.Generic;

namespace IRTweaks.Modules.Combat
{
    // why is this on a getter thats called frequently instead of OnComplete????????
    // Updated reference to https://github.com/RealityMachina/Better-Juggernaut, further refined by LadyAlekto
    //   as MightyJuggernaut. Applies braced on attack if your pilot has the Guts 8 ability ability. 

    [HarmonyPatch(typeof(MechMeleeSequence), "ConsumesFiring", MethodType.Getter)]
    static class MechMeleeSequence_ConsumesFiring_Getter
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.BraceOnMeleeWithJuggernaut && false;

        [HarmonyPrefix]
        static void Prefix(ref bool __runOriginal, MechMeleeSequence __instance)
        {

            if (!__runOriginal) return;

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
                    }
                }
            }

            return;
        }
    }

    [HarmonyPatch(typeof(MechDFASequence), "ConsumesFiring", MethodType.Getter)]
    static class MechDFASequence_ConsumesFiring_Getter
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.BraceOnMeleeWithJuggernaut && false;

        [HarmonyPrefix]
        static void Prefix(ref bool __runOriginal, MechDFASequence __instance)
        {
            if (!__runOriginal) return;

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
                    }
                }
            }

            return;
        }
    }



    [HarmonyPatch(typeof(OrderSequence), "OnComplete")]
    static class OrderSequence_OnComplete_Jugg
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.BraceOnMeleeWithJuggernaut;

        [HarmonyPrefix]
    [HarmonyPatch(typeof(OrderSequence), "OnComplete")]
        static void Postfix(OrderSequence __instance)
        {
            if (__instance is MechMeleeSequence || __instance is MechDFASequence)
            {
                if (__instance == null || __instance.owningActor == null || __instance.owningActor.GetPilot() == null)
                    return; // Nothing to do

                List<Ability> passives = __instance.owningActor.GetPilot().PassiveAbilities;
                if (passives.Count > 0)
                {
                    foreach (Ability ability in passives)
                    {
                        if (ability.Def.Description.Id.Equals(Mod.Config.Abilities.JuggernautId,
                                StringComparison.InvariantCultureIgnoreCase))
                        {
                            Mod.Log.Info?.Write(
                                "[OrderSequence_OnComplete_Jugg] Pilot has Juggernaut, bracing after melee or DFA attack using Mech.ApplyBraced.");
                            __instance.owningActor.ApplyBraced();
                        }
                    }
                }
            }
        }
    }
}
