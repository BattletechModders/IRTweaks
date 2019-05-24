using BattleTech;
using BattleTech.UI;
using Harmony;
using System;

namespace IRTweaks {

    [HarmonyPatch(typeof(HUDMechArmorReadout), "SetHoveredArmor")]
    public static class HUDMechArmorReadout_SetHoveredArmor {

        public static void Postfix(HUDMechArmorReadout __instance, ArmorLocation location, Mech ___displayedMech) {
            Mod.Log.Trace("HUDMAR:SHA entered");

            if (__instance.UseForCalledShots && location == ArmorLocation.Head) {
                Mod.Log.Trace("  Checking if headshot should be prevented.");
                Statistic allowHeadshotStat = State.CurrentAttacker?.StatCollection.GetStatistic(ModStats.CalledShowAlwaysAllow);
                bool allowHeadShot = allowHeadshotStat != null ? allowHeadshotStat.Value<bool>() : false;

                bool canBeTargeted = ___displayedMech.IsShutDown || ___displayedMech.IsProne || allowHeadShot;
                Mod.Log.Trace($"  canBeTargeted:{canBeTargeted} isShutdown:{___displayedMech.IsShutDown} isProne:{___displayedMech.IsProne} allowHeadshot:{allowHeadShot}");

                if (!canBeTargeted) {
                    Mod.Log.Debug("  preventing targeting of head.");
                    __instance.ClearHoveredArmor(ArmorLocation.Head);
                } else {
                    Mod.Log.Debug("  target head can be targeted.");
                }
            }
        }
    }

    [HarmonyPatch(typeof(SelectionStateFire), "SetCalledShot")]
    [HarmonyPatch(new Type[] {  typeof(ArmorLocation) })]
    public static class SelectionStateFire_SetCalledShot {

        public static void Postfix(SelectionStateFire __instance, ArmorLocation location) {
            Mod.Log.Trace("SSF:SCS entered");

            if (location == ArmorLocation.Head) {
                Mod.Log.Debug("  Checking if headshot should be prevented.");

                Statistic allowHeadshotStat = __instance.SelectedActor?.StatCollection.GetStatistic(ModStats.CalledShowAlwaysAllow);
                bool allowHeadShot = allowHeadshotStat != null ? allowHeadshotStat.Value<bool>() : false;

                bool canBeTargeted = __instance.TargetedCombatant.IsShutDown || __instance.TargetedCombatant.IsProne || allowHeadShot;
                Mod.Log.Trace($"  canBeTargeted:{canBeTargeted} isShutdown:{__instance.TargetedCombatant.IsShutDown} isProne:{__instance.TargetedCombatant.IsProne} allowHeadshot:{allowHeadShot}");

                if (!allowHeadShot) {
                    Mod.Log.Debug("  Disabling headshot.");
                    Traverse.Create(__instance).Method("ClearCalledShot").GetValue();
                }

            }



        }
    }

}
