using BattleTech;
using Harmony;
using System.Collections.Generic;
using us.frostraptor.modUtils;

namespace IRTweaks.Modules.Misc {

    [HarmonyPatch(typeof(AbstractActor), "MaxTargets", MethodType.Getter)]
    public static class AbstractActor_MaxTargets_Getter {
        static bool Prepare() { return Mod.Config.Fixes.MultiTargetStat; }

        static void Postfix(AbstractActor __instance, ref int __result) {
            Mod.Log.Trace?.Write($"AA:MT:G - entered.");

            if (__instance != null && __instance.StatCollection.ContainsStatistic(ModStats.EnableMultiTarget)) {
                Mod.Log.Debug?.Write($"Multi-Target stat exists");
                if (__instance.StatCollection.GetStatistic(ModStats.EnableMultiTarget).Value<bool>()) {
                    Mod.Log.Debug?.Write($"Enabling multi-target for actor: {CombatantUtils.Label(__instance)}");
                    __result = 3;
                } else {
                    Mod.Log.Debug?.Write($"Actor: {CombatantUtils.Label(__instance)} has enableMultiTarget: false");
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pilot), "ActiveAbilities", MethodType.Getter)]
    public static class Pilot_ActiveAbilities_Getter {
        static bool Prepare() { return Mod.Config.Fixes.MultiTargetStat; }

        static void Postfix(Pilot __instance, ref List<Ability> __result) {
            Mod.Log.Trace?.Write($"AA:AA:G - entered.");

            if (__instance == null || __instance.ParentActor == null) { return; }

            if (__instance.ParentActor.StatCollection.GetStatistic(ModStats.EnableMultiTarget).Value<bool>()) {
                Mod.Log.Debug?.Write($"Pilot: {__instance} needs multi-target ability");

                bool hasMultiTarget = false;
                foreach (Ability ability in __result) {
                    if (ability.Def.Targeting == AbilityDef.TargetingType.MultiFire) {
                        hasMultiTarget = true;
                    }
                }

                if (!hasMultiTarget) {
                    Traverse combatT = Traverse.Create(__instance).Property("Combat");
                    CombatGameState combat = combatT.GetValue<CombatGameState>();

                    if (combat == null) { return; }

                    Mod.Log.Debug?.Write("  -- Adding multi-target ability to pilot.");
                    AbilityDef abilityDef = combat.DataManager.AbilityDefs.Get(Mod.Config.Abilities.MultiTargetId);
                    Ability ability = new Ability(abilityDef);
                    ability.Init(combat);
                    __result.Add(ability);
                }
                
            }
        }
    }

    [HarmonyPatch(typeof(AbstractActor), "InitEffectStats")]
    public static class AbstractActor_InitEffectStats {
        static bool Prepare() { return Mod.Config.Fixes.MultiTargetStat; }

        static void Postfix(AbstractActor __instance) {
            Mod.Log.Trace?.Write($"AA:MT:G - entered.");
            __instance.StatCollection.AddStatistic<bool>(ModStats.EnableMultiTarget, false);
        }
    }
}
