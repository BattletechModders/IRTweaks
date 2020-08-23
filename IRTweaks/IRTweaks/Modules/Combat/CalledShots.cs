using BattleTech;
using BattleTech.UI;
using Harmony;
using IRBTModUtils.Extension;
using IRTweaks.Helper;
using Localize;
using System;
using System.Collections.Generic;

namespace IRTweaks.Modules.Combat {
    public static class CalledShots {

        public static void AbstractActor_InitEffectStats_Postfix(AbstractActor __instance) {
            Mod.Log.Trace?.Write("AA:IES entered.");
            __instance.StatCollection.AddStatistic<Int32>(ModStats.CalledShotMod, 0);
            __instance.StatCollection.AddStatistic<bool>(ModStats.CalledShowAlwaysAllow, false);
        }

        public static void HUDMechArmorReadout_SetHoveredArmor_Postfix(HUDMechArmorReadout __instance, ArmorLocation location, Mech ___displayedMech) {

            if (__instance == null || __instance.HUD == null || 
                __instance.HUD.SelectedActor == null || __instance.HUD.SelectedTarget == null) 
                return; // nothing to do

            if (__instance.UseForCalledShots && location == ArmorLocation.Head) {
                Mod.Log.Trace?.Write("HUDMAR:SHA entered");

                bool canAlwaysCalledShot = false;
                List<Statistic> customStats = ActorHelper.FindCustomStatistic(ModStats.CalledShowAlwaysAllow, __instance.HUD.SelectedActor);
                foreach (Statistic stat in customStats) {
                    if (stat.ValueType() == typeof(bool) && stat.Value<bool>()) {
                        canAlwaysCalledShot = true;
                    }
                }
                bool canBeTargeted = __instance.HUD.SelectedTarget.IsShutDown || __instance.HUD.SelectedTarget.IsProne || canAlwaysCalledShot;

                Mod.Log.Debug?.Write($"  Hover - target:({___displayedMech.DistinctId()}) canBeTargeted:{canBeTargeted} by attacker:({__instance.HUD.SelectedActor.DistinctId()})");
                Mod.Log.Debug?.Write($"      isShutdown:{___displayedMech.IsShutDown} isProne:{___displayedMech.IsProne} canAlwaysCalledShot:{canAlwaysCalledShot}");

                if (!canBeTargeted) {
                    Mod.Log.Debug?.Write("  preventing targeting of head.");
                    __instance.ClearHoveredArmor(ArmorLocation.Head);
                }
                else {
                    Mod.Log.Debug?.Write("  target head can be targeted.");
                }
            }
        }

        public static void SelectionStateFire_SetCalledShot_Postfix(SelectionStateFire __instance, ArmorLocation location) {
            Mod.Log.Trace?.Write("SSF:SCS entered");

            if (location == ArmorLocation.Head) {
                Mod.Log.Debug?.Write("  SCS Checking if headshot should be prevented.");

                bool canAlwaysCalledShot = false;
                List<Statistic> customStats = ActorHelper.FindCustomStatistic(ModStats.CalledShowAlwaysAllow, __instance.SelectedActor);
                foreach (Statistic stat in customStats) {
                    if (stat.ValueType() == typeof(bool) && stat.Value<bool>()) {
                        canAlwaysCalledShot = true;
                    }
                }

                bool canBeTargeted = __instance.TargetedCombatant.IsShutDown || __instance.TargetedCombatant.IsProne || canAlwaysCalledShot;
                Mod.Log.Debug?.Write($"  Select - target:{__instance.TargetedCombatant.DisplayName}_{__instance.TargetedCombatant.GetPilot()?.Name} canBeTargeted:{canBeTargeted} by attacker:{__instance.SelectedActor}");
                Mod.Log.Debug?.Write($"      isShutdown:{__instance.TargetedCombatant.IsShutDown} isProne:{__instance.TargetedCombatant.IsProne} canAlwaysCalledShot:{canAlwaysCalledShot}");

                if (!canBeTargeted) {
                    Mod.Log.Debug?.Write("  Disabling headshot.");
                    Traverse.Create(__instance).Method("ClearCalledShot").GetValue();
                }
            }
        }

        public static void ToHit_GetMoraleAttackModifier_Postfix(ToHit __instance, ICombatant target, bool isMoraleAttack, ref float __result) {
            __result = 0;
        }

        public static void ToHit_GetAllModifiers_Postfix(ToHit __instance, ref float __result, bool isCalledShot, AbstractActor attacker, Weapon weapon, ICombatant target) {
            if (isCalledShot) {
                Mod.Log.Trace?.Write("TH:GAM entered.");

                // Calculate called shot modifier
                int pilotValue = CalledShotHelper.GetCalledShotModifier(attacker.GetPilot());

                int unitMod = 0;
                if (attacker.StatCollection.ContainsStatistic(ModStats.CalledShotMod)) {
                    unitMod = attacker.StatCollection.GetStatistic(ModStats.CalledShotMod).Value<int>();
                }

                int calledShotMod = pilotValue + unitMod;
                Mod.Log.Debug?.Write($"   Called Shot from pilot:{attacker.GetPilot().Name} => pilotValue:{pilotValue} + unitMod:{unitMod} = calledShotMod:{calledShotMod}");

                __result = __result + calledShotMod;
            }
        }

        public static void ToHit_GetAllModifiersDescription_Postfix(ToHit __instance, ref string __result, bool isCalledShot, AbstractActor attacker, Weapon weapon, ICombatant target) {
            if (isCalledShot) {
                Mod.Log.Trace?.Write("TH:GAMD entered.");

                // Calculate called shot modifier
                int pilotValue = CalledShotHelper.GetCalledShotModifier(attacker.GetPilot());

                int unitMod = 0;
                if (attacker.StatCollection.ContainsStatistic(ModStats.CalledShotMod)) {
                    unitMod = attacker.StatCollection.GetStatistic(ModStats.CalledShotMod).Value<int>();
                }

                int calledShotMod = pilotValue + unitMod;
                Mod.Log.Debug?.Write($"   Called Shot from pilot:{attacker.GetPilot().Name} => pilotValue:{pilotValue} + unitMod:{unitMod} = calledShotMod:{calledShotMod}");

                if (calledShotMod != 0) {
                    __result = string.Format("{0}CALLED-SHOT {1:+#;-#}; ", __result, (int)calledShotMod);
                }
            }
        }

        public static void CombatHUDWeaponSlot_UpdateToolTipsFiring_Postfix(CombatHUDWeaponSlot __instance, ICombatant target, CombatGameState ___Combat, CombatHUD ___HUD, int ___modifier) {
            if (___HUD.SelectionHandler.ActiveState.SelectionType == SelectionType.FireMorale) {
                Mod.Log.Trace?.Write("CHUDWS:UTTF:Post entered.");

                AbstractActor attacker = ___HUD.SelectedActor;

                // Calculate called shot modifier
                int pilotValue = CalledShotHelper.GetCalledShotModifier(___HUD.SelectedActor.GetPilot());

                int unitMod = 0;
                if (attacker.StatCollection.ContainsStatistic(ModStats.CalledShotMod)) {
                    unitMod = attacker.StatCollection.GetStatistic(ModStats.CalledShotMod).Value<int>();
                }

                int calledShotMod = pilotValue + unitMod;
                Mod.Log.Debug?.Write($"   Called Shot from pilot:{attacker.GetPilot().Name} => pilotValue:{pilotValue} + unitMod:0 = calledShotMod:{calledShotMod}");

                if (calledShotMod != 0) {
                    AddMoraleToolTip(__instance, ___Combat.Constants.CombatUIConstants.MoraleAttackDescription.Name, calledShotMod);
                }
            }
        }

        private static void AddMoraleToolTip(CombatHUDWeaponSlot instance, string description, int modifier) {
            Mod.Log.Trace?.Write($"CHUDWS:UTTF:AMTT - adding desc:{description} with modifier:{modifier}.");
            if (modifier < 0) {
                instance.ToolTipHoverElement.BuffStrings.Add(new Text("{0} {1:+0;-#}", new object[] { description, modifier }));
            }
            else if (modifier > 0) {
                instance.ToolTipHoverElement.DebuffStrings.Add(new Text("{0} {1:+0;-#}", new object[] { description, modifier }));
            }
        }
    }
}
