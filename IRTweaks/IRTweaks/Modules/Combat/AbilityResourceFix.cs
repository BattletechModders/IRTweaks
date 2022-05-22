using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace IRTweaks.Modules.Combat
{
    public static class AbilityResourceEffects
    {
        public static bool GetAbilityUsedFiring(this AbstractActor actor)
        {
            return actor.StatCollection.GetValue<bool>("ActorConsumedFiring");
        }
        public static EffectDurationData Duration => new EffectDurationData {duration = 1, stackLimit = 1};

        public static EffectTargetingData TargetingData =>
            new EffectTargetingData
            {
                effectTriggerType = EffectTriggerType.Passive,
                triggerLimit = 0,
                extendDurationOnTrigger = 0,
                specialRules = AbilityDef.SpecialRules.NotSet,
                auraEffectType = AuraEffectType.NotSet,
                effectTargetType = EffectTargetType.Creator,
                alsoAffectCreator = false,
                range = 0f,
                forcePathRebuild = true,
                forceVisRebuild = false,
                showInTargetPreview = false,
                showInStatusPanel = false
            };

        public static EffectData ImmobileEffectData =>
            new EffectData
            {
                effectType = EffectType.StatisticEffect,
                targetingData = AbilityResourceEffects.TargetingData,
                Description = new DescriptionDef("ConsumesMovementFix", "Ability Used Movement", "Ability Used Movement", "uixSvgIcon_action_multitarget",  0, 0f, false, null, null, null),
                durationData = AbilityResourceEffects.Duration,
                statisticData = new StatisticEffectData
                {
                    statName = "irbtmu_immobile_unit",
                    operation = StatCollection.StatOperation.Set,
                    modValue = "true",
                    modType = "System.Boolean"
                }
            };
        public static EffectData AbilityUsedFiringData=>
            new EffectData
            {
                effectType = EffectType.StatisticEffect,
                targetingData = AbilityResourceEffects.TargetingData,
                Description = new DescriptionDef("ConsumesFiringFix", "Ability Used Firing", "Ability Used Firing", "uixSvgIcon_action_multitarget", 0, 0f, false, null, null, null),
                durationData = AbilityResourceEffects.Duration,
                statisticData = new StatisticEffectData
                {
                    statName = "ActorConsumedFiring",
                    operation = StatCollection.StatOperation.Set,
                    modValue = "true",
                    modType = "System.Boolean"
                }
            };
    }

    [HarmonyPatch(typeof(AbstractActor), "InitEffectStats")]
    public static class AbstractActor_InitEffectStats_AbilityResourceFix
    {
        static bool Prepare() => Mod.Config.Fixes.AbilityResourceFix;

        static void Postfix(AbstractActor __instance)
        {
            __instance.StatCollection.AddStatistic<bool>("ActorConsumedFiring", false);
        }
    }

    [HarmonyPatch(typeof(CombatSelectionHandler), "AddFireState", new Type[] { typeof(AbstractActor) })]
    public static class CombatSelectionHandler_AddFireState
    {
        public static bool Prepare()
        {
            return Mod.Config.Fixes.AbilityResourceFix;
        }

        public static bool Prefix(CombatSelectionHandler __instance, AbstractActor actor)
        {
            if (actor != null && actor.GetAbilityUsedFiring())
            {
                var HUD = Traverse.Create(__instance).Property("HUD").GetValue<CombatHUD>();
                HUD.MechWarriorTray.FireButton.DisableButton();
                var selectionStack = Traverse.Create(HUD.SelectionHandler).Property("SelectionStack")
                    .GetValue<List<SelectionState>>();
                if (!selectionStack.Any(x => x is SelectionStateDoneWithMech) && actor.HasMovedThisRound)
                {
                    var doneState = new SelectionStateDoneWithMech(actor.Combat, HUD,
                        HUD.MechWarriorTray.DoneWithMechButton, actor);
                    var addState = Traverse.Create(HUD.SelectionHandler)
                        .Method("addNewState", new Type[] { typeof(SelectionState) });
                    addState.GetValue(doneState);
                }
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CombatSelectionHandler), "AddFireState", new Type[] {typeof(AbstractActor), typeof(ICombatant), typeof(CombatHUDAttackModeSelector.SelectedButton)})]
    public static class CombatSelectionHandler_AddFireState2
    {
        public static bool Prepare()
        {
            return Mod.Config.Fixes.AbilityResourceFix;
        }

        public static bool Prefix(CombatSelectionHandler __instance, AbstractActor actor, ICombatant target, CombatHUDAttackModeSelector.SelectedButton preferredButton)
        {
            if (actor != null && actor.GetAbilityUsedFiring())
            {
                var HUD = Traverse.Create(__instance).Property("HUD").GetValue<CombatHUD>();
                HUD.MechWarriorTray.FireButton.DisableButton(); //donewithmech button getting disabled after movement
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CombatHUDMechwarriorTray), "ResetMechwarriorButtons", new Type[] {typeof(AbstractActor)})]
    public static class CombatHUDMechwarriorTray_ResetMechwarriorButtons
    {
        public static bool Prepare()
        {
            return Mod.Config.Fixes.AbilityResourceFix;
        }

        public static void Postfix(CombatHUDMechwarriorTray __instance, AbstractActor actor)
        {
            if (actor != null && actor.GetAbilityUsedFiring())
            {
                __instance.FireButton.DisableButton();
            }
        }
    }



    [HarmonyPatch(typeof(CombatHUDActionButton), "ActivateAbility", new Type[] {typeof(string), typeof(string)})]
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
            var selectionStack = Traverse.Create(HUD.SelectionHandler).Property("SelectionStack")
                .GetValue<List<SelectionState>>();
            if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesFiring)
            {
                selectedActor.CreateEffect(AbilityResourceEffects.AbilityUsedFiringData, null, AbilityResourceEffects.AbilityUsedFiringData.Description.Id, -1, selectedActor);
                for (int i = selectionStack.Count - 1; i >= 0; i--)
                {
                    if (selectionStack[i] is SelectionStateFire selectionStateFire)
                    {
                        selectionStateFire.OnInactivate();
                        selectionStateFire.OnRemoveFromStack();
                        selectionStack.Remove(selectionStateFire);
                    }
                    else if (selectionStack[i] is SelectionStateFireMulti selectionStateFireMulti)
                    {
                        selectionStateFireMulti.OnInactivate();
                        selectionStateFireMulti.OnRemoveFromStack();
                        selectionStack.Remove(selectionStateFireMulti);
                        
                    }
                    else if (selectionStack[i] is SelectionStateMoraleAttack selectionStateMoraleAttack)
                    {
                        selectionStateMoraleAttack.OnInactivate();
                        selectionStateMoraleAttack.OnRemoveFromStack();
                        selectionStack.Remove(selectionStateMoraleAttack);
                    }
                }
                HUD.MechWarriorTray.FireButton.DisableButton();
                if (!selectionStack.Any(x => x is SelectionStateDoneWithMech) && selectedActor.HasMovedThisRound)
                {
                    var doneState = new SelectionStateDoneWithMech(selectedActor.Combat, HUD,
                        HUD.MechWarriorTray.DoneWithMechButton, selectedActor);
                    var addState = Traverse.Create(HUD.SelectionHandler)
                        .Method("addNewState", new Type[] { typeof(SelectionState) });
                    addState.GetValue(doneState);
                }
                // this DOES work to disable firing.
            }
            else if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesMovement)
            {

                //selectedActor.HasMovedThisRound = true;
                selectedActor.CreateEffect(AbilityResourceEffects.ImmobileEffectData, null, AbilityResourceEffects.ImmobileEffectData.Description.Id, -1, selectedActor);
                //this DOES work to disable movement
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

    [HarmonyPatch(typeof(CombatHUDEquipmentSlot), "ActivateAbility", new Type[] {typeof(string), typeof(string)})]
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
            var selectionStack = Traverse.Create(HUD.SelectionHandler).Property("SelectionStack")
                .GetValue<List<SelectionState>>();
            if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesFiring)
            {
                selectedActor.CreateEffect(AbilityResourceEffects.AbilityUsedFiringData, null, AbilityResourceEffects.AbilityUsedFiringData.Description.Id, -1, selectedActor);
                for (int i = selectionStack.Count - 1; i >= 0; i--)
                {
                    if (selectionStack[i] is SelectionStateFire selectionStateFire)
                    {
                        selectionStateFire.OnInactivate();
                        selectionStateFire.OnRemoveFromStack();
                        selectionStack.Remove(selectionStateFire);
                    }
                    else if (selectionStack[i] is SelectionStateFireMulti selectionStateFireMulti)
                    {
                        selectionStateFireMulti.OnInactivate();
                        selectionStateFireMulti.OnRemoveFromStack();
                        selectionStack.Remove(selectionStateFireMulti);
                    }
                    else if (selectionStack[i] is SelectionStateMoraleAttack selectionStateMoraleAttack)
                    {
                        selectionStateMoraleAttack.OnInactivate();
                        selectionStateMoraleAttack.OnRemoveFromStack();
                        selectionStack.Remove(selectionStateMoraleAttack);
                    }
                }
                HUD.MechWarriorTray.FireButton.DisableButton();
                if (!selectionStack.Any(x => x is SelectionStateDoneWithMech) && selectedActor.HasMovedThisRound)
                {
                    var doneState = new SelectionStateDoneWithMech(selectedActor.Combat, HUD,
                        HUD.MechWarriorTray.DoneWithMechButton, selectedActor);
                    var addState = Traverse.Create(HUD.SelectionHandler)
                        .Method("addNewState", new Type[] { typeof(SelectionState) });
                    addState.GetValue(doneState);
                }
                // this DOES work to disable firing.
            }
            else if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesMovement)
            {
                //selectedActor.HasMovedThisRound = true;
                selectedActor.CreateEffect(AbilityResourceEffects.ImmobileEffectData, null, AbilityResourceEffects.ImmobileEffectData.Description.Id, -1, selectedActor);
                //selectedActor.Combat.EffectManager.CreateEffect(ImmobileEffect.ImmobileEffectData,"IRTweaks_Immobilized", -1, selectedActor, selectedActor, default(WeaponHitInfo), 1, false);
                //this DOES work to disable movement
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
                Mod.Log.Trace?.Write($"Processing button for {button?.Ability?.Def?.Description?.Name}.");
                if (button?.Ability?.Def?.Resource == AbilityDef.ResourceConsumed.ConsumesActivation && forceInactive)
                {
                    Mod.Log.Trace?.Write($"Disabling button for {button.Ability.Def.Description?.Name}.");
                    button.DisableButton();
                }
            }
        }
    }
}
