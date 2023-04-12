using BattleTech.UI;
using IRBTModUtils.Extension;
using IRTweaks.Helper;
using Localize;
using System;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(AbstractActor), "InitEffectStats")]
    static class AbstractActor_InitEffectStats
    {
        static bool Prepare() => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(AbstractActor __instance)
        {
            Mod.Log.Trace?.Write("AA:IES entered.");
            __instance.StatCollection.AddStatistic<Int32>(ModStats.CalledShot_AttackMod, 0);
            __instance.StatCollection.AddStatistic<bool>(ModStats.CalledShot_AlwaysAllow, false);
        }
    }

    [HarmonyPatch(typeof(HUDMechArmorReadout), "SetHoveredArmor")]
    static class HUDMechArmorReadout_SetHoveredAmor
    {
        static bool Prepare() => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(HUDMechArmorReadout __instance, ArmorLocation location, Mech ___displayedMech)
        {
            if (__instance == null || __instance.HUD == null ||
              __instance.HUD.SelectedActor == null || __instance.HUD.SelectedTarget == null)
                return; // nothing to do

            if (__instance.UseForCalledShots && location == ArmorLocation.Head)
            {
                Mod.Log.Trace?.Write("HUDMAR:SHA entered");

                bool attackerCanAlwaysMakeCalledShot = __instance.HUD.SelectedActor.CanAlwaysUseCalledShot();
                bool targetCanBeCalledShot = __instance.HUD.SelectedTarget.IsShutDown || __instance.HUD.SelectedTarget.IsProne || attackerCanAlwaysMakeCalledShot;

                Mod.Log.Debug?.Write($"  Hover - target:({___displayedMech.DistinctId()}) canBeTargeted:{targetCanBeCalledShot} by attacker:({__instance.HUD.SelectedActor.DistinctId()})");
                Mod.Log.Debug?.Write($"      isShutdown:{___displayedMech.IsShutDown} isProne:{___displayedMech.IsProne} canAlwaysCalledShot:{attackerCanAlwaysMakeCalledShot}");

                if (!targetCanBeCalledShot)
                {
                    Mod.Log.Debug?.Write("  preventing targeting of head.");
                    __instance.ClearHoveredArmor(ArmorLocation.Head);
                }
                else
                {
                    Mod.Log.Debug?.Write("  target head can be targeted.");
                }
            }
        }
    }

    [HarmonyPatch(typeof(SelectionStateFire), "SetCalledShot")]
    [HarmonyPatch(new Type[] { typeof(ArmorLocation) })]
    static class SelectionStateFire_SetCalledShot_AL
    {
        static bool Prepare() => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(SelectionStateFire __instance, ArmorLocation location)
        {
            Mod.Log.Trace?.Write("SSF:SCS entered");

            bool attackerCanAlwaysMakeCalledShot = __instance.SelectedActor.CanAlwaysUseCalledShot();
            bool targetCanBeCalledShot = __instance.TargetedCombatant.IsShutDown || __instance.TargetedCombatant.IsProne || attackerCanAlwaysMakeCalledShot;

            Mod.Log.Debug?.Write($"  Select - target:{__instance.TargetedCombatant.DistinctId()} canBeTargeted:{targetCanBeCalledShot} by attacker:{__instance.SelectedActor.DistinctId()}");
            Mod.Log.Debug?.Write($"      isShutdown:{__instance.TargetedCombatant.IsShutDown} isProne:{__instance.TargetedCombatant.IsProne} canAlwaysCalledShot:{attackerCanAlwaysMakeCalledShot}");

            if (!targetCanBeCalledShot)
            {
                if (Mod.Config.Combat.CalledShot.DisableAllLocations)
                {
                    Mod.Log.Info?.Write($"  Disabling called shot from attacker: {__instance.SelectedActor.DistinctId()} against target: {__instance.TargetedCombatant.DistinctId()}");
                    //Traverse.Create(__instance).Method("ClearCalledShot").GetValue();
                    __instance.ClearCalledShot();
                }
                else if (Mod.Config.Combat.CalledShot.DisableHeadshots && location == ArmorLocation.Head)
                {
                    Mod.Log.Info?.Write($"  Disabling headshot from attacker: {__instance.SelectedActor.DistinctId()} against target mech: {__instance.TargetedCombatant.DistinctId()}");
                    //Traverse.Create(__instance).Method("ClearCalledShot").GetValue();
                    __instance.ClearCalledShot();
                }
            }
        }
    }

    [HarmonyPatch(typeof(SelectionStateFire), "SetCalledShot")]
    [HarmonyPatch(new Type[] { typeof(VehicleChassisLocations) })]
    static class SelectionStateFire_SetCalledShot_VCL
    {
        static bool Prepare() => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(SelectionStateFire __instance)
        {
            Mod.Log.Trace?.Write("SSF:SCS entered");

            bool attackerCanAlwaysMakeCalledShot = __instance.SelectedActor.CanAlwaysUseCalledShot();
            bool targetCanBeCalledShot = __instance.TargetedCombatant.IsShutDown || __instance.TargetedCombatant.IsProne || attackerCanAlwaysMakeCalledShot;
            if (!targetCanBeCalledShot && Mod.Config.Combat.CalledShot.DisableAllLocations)
            {
                Mod.Log.Info?.Write($"  Disabling called shot from attacker: {__instance.SelectedActor.DistinctId()} against target vehicle: {__instance.TargetedCombatant.DistinctId()}");
                //Traverse.Create(__instance).Method("ClearCalledShot").GetValue();
                __instance.ClearCalledShot();
            }
        }
    }

    [HarmonyPatch(typeof(SelectionStateMoraleAttack), "NeedsCalledShot", MethodType.Getter)]
    static class SelectionStateMoraleAttack_NeedsCalledShot
    {
        static bool Prepare() => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(SelectionStateFire __instance, ref bool __result)
        {
            Mod.Log.Trace?.Write("SSF:NCS:GET entered");

            if (__result == true)
            {
                bool attackerCanAlwaysMakeCalledShot = __instance.SelectedActor.CanAlwaysUseCalledShot();
                bool targetCanBeCalledShot = __instance.TargetedCombatant.IsShutDown || __instance.TargetedCombatant.IsProne || attackerCanAlwaysMakeCalledShot;
                if (!targetCanBeCalledShot && Mod.Config.Combat.CalledShot.DisableAllLocations)
                {
                    Mod.Log.Debug?.Write($"  Disabling NeedsCalledShot from attacker: {__instance.SelectedActor.DistinctId()} against target vehicle: {__instance.TargetedCombatant.DistinctId()}");
                    __result = false;

                }
            }
        }
    }

    // Override the default modifiers for called shot
    [HarmonyPatch(typeof(ToHit), "GetMoraleAttackModifier")]
    static class ToHit_GetMoraleAttackModifier
    {
        static bool Prepare() => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(ref float __result)
        {
            __result = 0;
        }
    }

    [HarmonyPatch(typeof(ToHit), "GetAllModifiers")]
    static class ToHit_GetAllModifiers
    {
        static bool Prepare() => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(ref float __result, bool isCalledShot, AbstractActor attacker)
        {
            if (isCalledShot)
            {
                Mod.Log.Trace?.Write("TH:GAM entered.");

                // Calculate called shot modifier
                int calledShotMod = ActorHelper.CalledShotModifier(attacker);
                Mod.Log.Debug?.Write($"Actor: {attacker.DistinctId()} has calledShotMod: {calledShotMod}");
                __result += calledShotMod;
            }
        }
    }

    [HarmonyPatch(typeof(ToHit), "GetAllModifiersDescription")]
    static class ToHit_GetAllModifiersDescription
    {
        static bool Prepare() => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(ref string __result, bool isCalledShot, AbstractActor attacker)
        {
            if (isCalledShot)
            {
                Mod.Log.Trace?.Write("TH:GAMD entered.");

                // Calculate called shot modifier
                int calledShotMod = ActorHelper.CalledShotModifier(attacker);
                if (calledShotMod != 0)
                {
                    // No need to localize, this is only printed in logs
                    __result = string.Format("{0}CALLED-SHOT {1:+#;-#}; ", __result, (int)calledShotMod);
                }
            }
        }
    }

    //Update the hover text in the case of a modifier
    [HarmonyPatch(typeof(CombatHUDWeaponSlot), "SetHitChance", new Type[] { typeof(ICombatant) })]
    static class CombatHUDWeaponSlot_SetHitChance
    {
        static bool Prepare() => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(CombatHUDWeaponSlot __instance, ICombatant target, Weapon ___displayedWeapon, CombatHUD ___HUD)
        {
            if (__instance == null || ___displayedWeapon == null || ___HUD.SelectedActor == null || target == null) return;

            Mod.Log.Trace?.Write("CHUDWS:SHC entered");

            if (___HUD.SelectionHandler.ActiveState.SelectionType == SelectionType.FireMorale)
            {
                int calledShotMod = ActorHelper.CalledShotModifier(___HUD.SelectedActor);
                if (calledShotMod != 0)
                {

                    //Traverse addToolTipDetailT = Traverse.Create(__instance).Method("AddToolTipDetail", new Type[] { typeof(string), typeof(int) });

                    string localText = new Text(Mod.LocalizedText.Modifiers[ModText.Mod_CalledShot]).ToString();
                    //addToolTipDetailT.GetValue(new object[] { localText, calledShotMod });
                    __instance.AddToolTipDetail(localText, calledShotMod);
                    Mod.Log.Debug?.Write($"Adding calledShot tooltip with text: {localText} and mod: {calledShotMod}");
                }
                Mod.Log.Debug?.Write($"Updated TooltipsForFiring for actor: {___HUD.SelectedActor} with mod: {calledShotMod}");
            }
            else
            {
                Mod.Log.Trace?.Write("Not FireMorale, skipping!");
            }
        }
    }

    // Hide the popup if we've disabled it intentionally
    [HarmonyPatch(typeof(CombatHUDCalledShotPopUp), "Update")]
    static class CombatHUDCalledShotPopUp_Update
    {
        static bool Prepare() => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(CombatHUDCalledShotPopUp __instance, CombatHUD ___HUD)
        {
            if (__instance.Visible && ___HUD.SelectionHandler.ActiveState is SelectionStateFire selectionStateFire &&
                selectionStateFire.SelectionType == SelectionType.FireMorale)
            {
                bool attackerCanAlwaysMakeCalledShot = ___HUD.SelectedActor.CanAlwaysUseCalledShot();
                bool targetCanBeCalledShot = __instance.DisplayedActor.IsShutDown || __instance.DisplayedActor.IsProne || attackerCanAlwaysMakeCalledShot;
                if (!targetCanBeCalledShot && Mod.Config.Combat.CalledShot.DisableAllLocations)
                {
                    Mod.Log.Info?.Write($"  Disabling called shot popup from attacker: {___HUD.SelectedActor.DistinctId()} against target vehicle: {__instance.DisplayedActor.DistinctId()}");
                    __instance.Visible = false;
                }
            }
        }
    }


}
