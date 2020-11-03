using BattleTech;
using BattleTech.UI;
using Harmony;
using IRBTModUtils.Extension;
using IRTweaks.Helper;
using Localize;
using System;
using System.Collections.Generic;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(AbstractActor), "InitEffectStats")]
    static class AbstractActor_InitEffectStats
    {
        static bool Prepare => Mod.Config.Fixes.CalledShotTweaks;

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
        static bool Prepare => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(HUDMechArmorReadout __instance, ArmorLocation location, Mech ___displayedMech)
        {
            if (__instance == null || __instance.HUD == null ||
              __instance.HUD.SelectedActor == null || __instance.HUD.SelectedTarget == null)
                return; // nothing to do

            if (__instance.UseForCalledShots && location == ArmorLocation.Head)
            {
                Mod.Log.Trace?.Write("HUDMAR:SHA entered");

                bool canAlwaysCalledShot = false;
                List<Statistic> customStats = ActorHelper.FindCustomStatistic(ModStats.CalledShot_AlwaysAllow, __instance.HUD.SelectedActor);
                foreach (Statistic stat in customStats)
                {
                    if (stat.ValueType() == typeof(bool) && stat.Value<bool>())
                    {
                        canAlwaysCalledShot = true;
                    }
                }
                bool canBeTargeted = __instance.HUD.SelectedTarget.IsShutDown || __instance.HUD.SelectedTarget.IsProne || canAlwaysCalledShot;

                Mod.Log.Debug?.Write($"  Hover - target:({___displayedMech.DistinctId()}) canBeTargeted:{canBeTargeted} by attacker:({__instance.HUD.SelectedActor.DistinctId()})");
                Mod.Log.Debug?.Write($"      isShutdown:{___displayedMech.IsShutDown} isProne:{___displayedMech.IsProne} canAlwaysCalledShot:{canAlwaysCalledShot}");

                if (!canBeTargeted)
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
    static class SelectionStateFire_SetCalledShot
    {
        static bool Prepare => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(SelectionStateFire __instance, ArmorLocation location)
        {
            Mod.Log.Trace?.Write("SSF:SCS entered");

            if (location == ArmorLocation.Head)
            {
                Mod.Log.Debug?.Write("  SCS Checking if headshot should be prevented.");

                bool canAlwaysCalledShot = false;
                List<Statistic> customStats = ActorHelper.FindCustomStatistic(ModStats.CalledShot_AlwaysAllow, __instance.SelectedActor);
                foreach (Statistic stat in customStats)
                {
                    if (stat.ValueType() == typeof(bool) && stat.Value<bool>())
                    {
                        canAlwaysCalledShot = true;
                    }
                }

                bool canBeTargeted = __instance.TargetedCombatant.IsShutDown || __instance.TargetedCombatant.IsProne || canAlwaysCalledShot;
                Mod.Log.Debug?.Write($"  Select - target:{__instance.TargetedCombatant.DisplayName}_{__instance.TargetedCombatant.GetPilot()?.Name} canBeTargeted:{canBeTargeted} by attacker:{__instance.SelectedActor}");
                Mod.Log.Debug?.Write($"      isShutdown:{__instance.TargetedCombatant.IsShutDown} isProne:{__instance.TargetedCombatant.IsProne} canAlwaysCalledShot:{canAlwaysCalledShot}");

                if (!canBeTargeted)
                {
                    Mod.Log.Debug?.Write("  Disabling headshot.");
                    Traverse.Create(__instance).Method("ClearCalledShot").GetValue();
                }
            }
        }
    }

    // Override the default modifiers for called shot
    [HarmonyPatch(typeof(ToHit), "GetMoraleAttackModifer")]
    static class ToHit_GetMoraleAttackModifer
    {
        static bool Prepare => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(ref float __result)
        {
            __result = 0;
        }
    }

    [HarmonyPatch(typeof(ToHit), "GetAllModifiers")]
    static class ToHit_GetAllModifiers
    {
        static bool Prepare => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(ref float __result, bool isCalledShot, AbstractActor attacker)
        {
            if (isCalledShot)
            {
                Mod.Log.Trace?.Write("TH:GAM entered.");

                // Calculate called shot modifier
                int calledShotMod = ActorHelper.GetCalledShotModifier(attacker);
                __result += calledShotMod;
            }
        }
    }

    [HarmonyPatch(typeof(ToHit), "GetAllModifiersDescription")]
    static class ToHit_GetAllModifiersDescription
    {
        static bool Prepare => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(ref string __result, bool isCalledShot, AbstractActor attacker)
        {
            if (isCalledShot)
            {
                Mod.Log.Trace?.Write("TH:GAMD entered.");

                // Calculate called shot modifier
                int calledShotMod = ActorHelper.GetCalledShotModifier(attacker);
                if (calledShotMod != 0)
                {
                    // No need to localize, this is only printed in logs
                    __result = string.Format("{0}CALLED-SHOT {1:+#;-#}; ", __result, (int)calledShotMod);
                }
            }
        }
    }

    [HarmonyPatch(typeof(CombatHUDWeaponSlot), "UpdateToolTipsFiring")]
    static class CombatHUDWeaponSlot_UpdateToolTipsFiringr
    {
        static bool Prepare => Mod.Config.Fixes.CalledShotTweaks;

        static void Postfix(CombatHUDWeaponSlot __instance, CombatGameState ___Combat, CombatHUD ___HUD)
        {
            if (___HUD.SelectionHandler.ActiveState.SelectionType == SelectionType.FireMorale)
            {
                Mod.Log.Trace?.Write("CHUDWS:UTTF:Post entered.");

                AbstractActor attacker = ___HUD.SelectedActor;
                int calledShotMod = ActorHelper.GetCalledShotModifier(___HUD.SelectedActor);
                if (calledShotMod != 0)
                {
                    Text hoverText = new Text("{0} {1:+0;-#}", new object[] { Mod.LocalizedText.Modifiers[ModText.Mod_CalledShot], calledShotMod });

                    if (calledShotMod < 0)
                        __instance.ToolTipHoverElement.BuffStrings.Add(hoverText);
                    else if (calledShotMod > 0)
                        __instance.ToolTipHoverElement.DebuffStrings.Add(hoverText);

                }
            }
        }
    }

}
