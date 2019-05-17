using BattleTech;
using BattleTech.UI;
using Harmony;

namespace IRTweaks {

    [HarmonyPatch(typeof(ToHit))]
    [HarmonyPatch("GetMoraleAttackModifier")]
    public static class ToHit_GetMoraleAttackModifier {

        public static void Postfix(ToHit __instance, ICombatant target, bool isMoraleAttack, ref float __result) {
            Mod.Log.Trace("TH:GMAM entered");

            if (isMoraleAttack) {
                int defaultValue = Mod.Config.ToHitCfg.CalledShotDefaultMod;
                int pilotValue = PilotHelper.GetCurrentAttackerCalledShotModifier();

                Statistic calledShotModStat = State.CurrentAttacker?.StatCollection.GetStatistic("IRTCalledShotMod");
                int unitMod = calledShotModStat != null ? calledShotModStat.Value<int>() : 0;

                __result = defaultValue + pilotValue + unitMod;
                Mod.Log.Debug($" Called Shot from pilot:{State.CurrentAttacker?.GetPilot()?.Name} => defaultMod:{defaultValue} " +
                    $"+ pilotValue:{pilotValue} + unitMod:{unitMod} = result:{__result}");
            }
            
        }
    }

    // Wrappers to pull information out of the higher context
    [HarmonyPatch(typeof(ToHit), "GetAllModifiers")]
    public static class ToHit_GetAllModifiers {

        public static void Prefix(bool __state, ToHit __instance, AbstractActor attacker, Weapon weapon, ICombatant target) {
            Mod.Log.Trace("TH:GAM:Pre entered.");
            if (State.CurrentAttacker == null) {
                State.CurrentAttacker = attacker;
                State.CurrentWeapon = weapon;
                State.CurrentTarget = target;

                if (State.CurrentAttacker?.GetPilot() != null) { PilotHelper.CachePilot(State.CurrentAttacker.GetPilot()); }
                __state = true;
            } else {
                __state = false;
            }
        }

        public static void Postfix(bool __state, ToHit __instance) {
            Mod.Log.Trace("TH:GAM:Post entered.");
            if (__state) {
                State.CurrentAttacker = null;
                State.CurrentWeapon = null;
                State.CurrentTarget = null;
            }
        }
    }

    [HarmonyPatch(typeof(ToHit), "GetAllModifiersDescription")]
    public static class ToHit_GetAllModifiersDescription {

        public static void Prefix(bool __state, ToHit __instance, AbstractActor attacker, Weapon weapon, ICombatant target) {
            Mod.Log.Trace("TH:GAMD:Pre entered.");
            if (State.CurrentAttacker == null) {
                State.CurrentAttacker = attacker;
                State.CurrentWeapon = weapon;
                State.CurrentTarget = target;

                if (State.CurrentAttacker?.GetPilot() != null) { PilotHelper.CachePilot(State.CurrentAttacker.GetPilot()); }

                __state = true;
            } else {
                __state = false;
            }
        }

        public static void Postfix(bool __state, ToHit __instance) {
            Mod.Log.Trace("TH:GAMD:Post entered.");
            if (__state) {
                State.CurrentAttacker = null;
                State.CurrentWeapon = null;
                State.CurrentTarget = null;
            }
        }
    }

    [HarmonyPatch(typeof(CombatHUDWeaponSlot), "UpdateToolTipsFiring")]
    public static class CombatHUDWeaponSlot_UpdateToolTipsFiring {

        public static void Prefix(CombatHUDWeaponSlot __instance, ICombatant target, CombatHUD ___HUD, Weapon ___displayedWeapon) {
            Mod.Log.Trace("CHUDWS:UTTF:Pre entered.");
            State.CurrentAttacker = ___HUD.SelectedActor;
            State.CurrentWeapon = ___displayedWeapon;
            State.CurrentTarget = target;

            if (State.CurrentAttacker?.GetPilot() != null) { PilotHelper.CachePilot(State.CurrentAttacker.GetPilot()); }
        }

        public static void Postfix(CombatHUDWeaponSlot __instance) {
            Mod.Log.Trace("CHUDWS:UTTF:Post entered.");
            State.CurrentAttacker = null;
            State.CurrentWeapon = null;
            State.CurrentTarget = null;
        }
    }

    // Cleanup at the end of combat
    [HarmonyPatch(typeof(TurnDirector), "OnCombatGameDestroyed")]
    public static class TurnDirector_OnCombatGameDestroyed {

        public static void Postfix(TurnDirector __instance) {
            Mod.Log.Trace("TD:OCGD entered");

            State.Reset();
        }
    }
}
