using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(CombatHUDActionButton), "ActivateAbility", new Type[]
    {
        typeof(string),
        typeof(string)
    })]
    public static class CombatHUDActionButton_ActivateAbility_Confirmed
    {
        public static bool Prepare()
        {
            return Mod.Config.Fixes.AbilityResourceFix;
        }

        public static void Postfix(CombatHUDActionButton __instance, string creatorGUID, string targetGUID)
        {
            var HUD = Traverse.Create((object)__instance).Property("HUD", (object[])null).GetValue<CombatHUD>();
            var selectedActor = HUD.SelectedActor;
            if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesFiring)
            {
                //                selectedActor.HasFiredThisRound = true;
                // this doesnt work to disable firing.
            }
            else if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesMovement)
            {
                //                selectedActor.HasMovedThisRound = true;
                //this doesnt work to disable movement
            }
            else if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesActivation)
            {
                if (selectedActor is Mech mech)
                {
                    mech.GenerateAndPublishHeatSequence(-1, true, false, selectedActor.GUID);
                    Mod.Log.Trace?.Write($"Generated and Published Heat Sequence for {mech.Description.UIName}.");
                }

                selectedActor.DoneWithActor();//need to to onactivationend too
                selectedActor.OnActivationEnd(selectedActor.GUID, __instance.GetInstanceID());
            }
        }
    }

    [HarmonyPatch(typeof(CombatHUDEquipmentSlot), "ActivateAbility", new Type[]
    {
        typeof(string),
        typeof(string)
    })]
    public static class CombatHUDEquipmentSlot_ActivateAbility_Confirmed
    {
        public static bool Prepare()
        {
            return Mod.Config.Fixes.AbilityResourceFix;
        }

        public static void Postfix(CombatHUDEquipmentSlot __instance, string creatorGUID, string targetGUID)
        {
            var HUD = Traverse.Create((object)__instance).Property("HUD", (object[])null).GetValue<CombatHUD>();
            var selectedActor = HUD.SelectedActor;
            if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesFiring)
            {
                //                selectedActor.HasFiredThisRound = true;
                // this doesnt work to disable firing.
            }
            else if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesMovement)
            {
                //                selectedActor.HasMovedThisRound = true;
                //this doesnt work to disable movement
            }
            else if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesActivation)
            {
                if (selectedActor is Mech mech)
                {
                    mech.GenerateAndPublishHeatSequence(-1, true, false, selectedActor.GUID);
                    Mod.Log.Info?.Write($"Generated and Published Heat Sequence for {mech.Description.UIName}.");
                }

                selectedActor.DoneWithActor();//need to to onactivationend too
                selectedActor.OnActivationEnd(selectedActor.GUID, __instance.GetInstanceID());
            }
        }
    }

    [HarmonyPatch(typeof(CombatHUDMechwarriorTray), "ResetAbilityButtons", new Type[] { typeof(AbstractActor) })]

    public static class CombatHUDMechwarriorTray_ResetAbilityButtons_Patch
    {
        public static bool Prepare()
        {
            return Mod.Config.Fixes.AbilityResourceFix;
        }
        public static void Postfix(CombatHUDMechwarriorTray __instance, AbstractActor actor)
        {
            var combat = UnityGameInstance.BattleTechGame.Combat;
            var forceInactive = actor.HasMovedThisRound || actor.HasFiredThisRound; // need to figure this part out; do other checks? this is still disabling the butons. integrat with CU?
            var abilityButtons = Traverse.Create(__instance).Property("AbilityButtons")
                .GetValue<CombatHUDActionButton[]>();
            foreach (var button in abilityButtons)
            {
                Mod.Log.Info?.Write($"Processing button for {button?.Ability?.Def?.Description?.Name}.");
                if (button?.Ability?.Def?.Resource == AbilityDef.ResourceConsumed.ConsumesActivation && forceInactive)
                {
                    Mod.Log.Info?.Write($"Disabling button for {button.Ability.Def.Description.Name}.");
                    button.DisableButton();
                }
            }
        }
    }
}
