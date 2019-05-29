using BattleTech;
using BattleTech.UI;
using Harmony;
using IRTweaks.Helper;
using System;
using System.Collections.Generic;

namespace IRTweaks {

    [HarmonyPatch(typeof(AbstractActor), "InitEffectStats")]
    public static class AbstractActor_InitEffectStats {

        public static void Postfix(AbstractActor __instance) {
            Mod.Log.Trace("AA:IES entered.");
            __instance.StatCollection.AddStatistic<bool>(ModStats.CalledShotMod, false);
            __instance.StatCollection.AddStatistic<bool>(ModStats.CalledShowAlwaysAllow, false);
        }
    }

    [HarmonyPatch(typeof(HUDMechArmorReadout), "SetHoveredArmor")]
    public static class HUDMechArmorReadout_SetHoveredArmor {

        public static void Postfix(HUDMechArmorReadout __instance, ArmorLocation location, Mech ___displayedMech) {
            if (__instance.UseForCalledShots && location == ArmorLocation.Head) {
                Mod.Log.Trace("HUDMAR:SHA entered");

                bool canAlwaysCalledShot = false;
                List<Statistic> customStats = ActorHelper.FindCustomStatistic(ModStats.CalledShowAlwaysAllow, __instance.HUD.SelectedActor);
                foreach (Statistic stat in customStats) {
                    if (stat.ValueType() == typeof(bool) && stat.Value<bool>()) {
                        canAlwaysCalledShot = true;
                    }
                }
                bool canBeTargeted = __instance.HUD.SelectedTarget.IsShutDown || __instance.HUD.SelectedTarget.IsProne || canAlwaysCalledShot;

                Mod.Log.Debug($"  Hover - target:({___displayedMech.DisplayName}_{___displayedMech.GetPilot()?.Name}) canBeTargeted:{canBeTargeted} by attacker:({__instance.HUD.SelectedActor.DisplayName})");
                Mod.Log.Debug($"      isShutdown:{___displayedMech.IsShutDown} isProne:{___displayedMech.IsProne} canAlwaysCalledShot:{canAlwaysCalledShot}");

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
                Mod.Log.Debug("  SCS Checking if headshot should be prevented.");

                bool canAlwaysCalledShot = false;
                List<Statistic> customStats = ActorHelper.FindCustomStatistic(ModStats.CalledShowAlwaysAllow, __instance.SelectedActor);
                foreach (Statistic stat in customStats) {
                    if (stat.ValueType() == typeof(bool) && stat.Value<bool>()) {
                        canAlwaysCalledShot = true;
                    }
                }

                bool canBeTargeted = __instance.TargetedCombatant.IsShutDown || __instance.TargetedCombatant.IsProne || canAlwaysCalledShot;
                Mod.Log.Debug($"  Select - target:{__instance.TargetedCombatant.DisplayName}_{__instance.TargetedCombatant.GetPilot()?.Name} canBeTargeted:{canBeTargeted} by attacker:{__instance.SelectedActor}");
                Mod.Log.Debug($"      isShutdown:{__instance.TargetedCombatant.IsShutDown} isProne:{__instance.TargetedCombatant.IsProne} canAlwaysCalledShot:{canAlwaysCalledShot}");

                if (!canBeTargeted) {
                    Mod.Log.Debug("  Disabling headshot.");
                    Traverse.Create(__instance).Method("ClearCalledShot").GetValue();
                }

            }



        }
    }

}
