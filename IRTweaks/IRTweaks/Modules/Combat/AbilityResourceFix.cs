using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using SVGImporter;

namespace IRTweaks.Modules.Combat
{
    public static class AbilityResourceEffects
    {
        public static bool GetAbilityUsedFiring(this AbstractActor actor)
        {
            return actor.StatCollection.GetValue<bool>("ActorConsumedFiring");
        }

        public static void DisableAbilitiesUsingResource(this CombatSelectionHandler handler, AbilityDef.ResourceConsumed resource)
        {
            for (int i = handler.ActivatedAbilityButtons.Count - 1; i >= 0; i--)
            {
                if (resource == AbilityDef.ResourceConsumed.ConsumesFiring)
                {
                    if (handler.ActivatedAbilityButtons[i].Ability.Def.ActivationTime == AbilityDef.ActivationTiming.ConsumedByFiring)
                    {
                        handler.ActivatedAbilityButtons[i].DisableButton();
                    }
                }
                else if (resource == AbilityDef.ResourceConsumed.ConsumesMovement)
                {
                    if (handler.ActivatedAbilityButtons[i].Ability.Def.ActivationTime == AbilityDef.ActivationTiming.ConsumedByMovement)
                    {
                        handler.ActivatedAbilityButtons[i].DisableButton();
                    }
                }
            }
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

    [HarmonyPatch(typeof(AbstractActor), "OnNewRound")]
    [HarmonyPatch(new Type[]
    {
        typeof(int)
    })]
    public static class AbstractActor_OnNewRound
    {
        static bool Prepare() => Mod.Config.Fixes.AbilityResourceFix;

        public static void Postfix(AbstractActor __instance)
        {
            __instance.StatCollection.Set<bool>("ActorConsumedFiring", false);
        }
    }

    [HarmonyPatch(typeof(SelectionStateMove), "GetAllMeleeTargets", new Type[]{})]
    public static class SelectionStateMove_GetAllMeleeTargets
    {
        static bool Prepare() => Mod.Config.Fixes.AbilityResourceFix;
        
        public static void Postfix(SelectionStateMove __instance, List<ICombatant> __result)
        {
            if (__instance.SelectedActor.GetAbilityUsedFiring())
            {
                __result = new List<ICombatant>();
            }
        }
    }

    [HarmonyPatch(typeof(CombatHUDMechwarriorTray), "ResetAbilityButton", new Type[] {typeof(AbstractActor), typeof(CombatHUDActionButton), typeof(Ability), typeof(bool) })]
    public static class CombatHUDMechwarriorTray_ResetAbilityButton
    {
        static bool Prepare() => Mod.Config.Fixes.AbilityResourceFix;

        public static void Postfix(CombatHUDMechwarriorTray __instance, AbstractActor actor, CombatHUDActionButton button, Ability ability, bool forceInactive)
        {
            if (actor != null && ability != null && actor.GetAbilityUsedFiring())
            {
                if (ability.Def.ActivationTime == AbilityDef.ActivationTiming.ConsumedByFiring) button.DisableButton();
            }
        }
    }

    [HarmonyPatch(typeof(CombatHUDWeaponPanel), "ResetAbilityButton", new Type[] { typeof(AbstractActor), typeof(CombatHUDActionButton), typeof(Ability), typeof(bool) })]
    public static class CombatHUDWeaponPanel_ResetAbilityButton
    {
        static bool Prepare() => Mod.Config.Fixes.AbilityResourceFix;

        public static void Postfix(CombatHUDMechwarriorTray __instance, AbstractActor actor, CombatHUDActionButton button, Ability ability, bool forceInactive)
        {
            if (actor != null && ability != null && actor.GetAbilityUsedFiring())
            {
                if (ability.Def.ActivationTime == AbilityDef.ActivationTiming.ConsumedByFiring) button.DisableButton();
            }
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
                    Mod.Log.Info?.Write($"[CombatSelectionHandler_AddFireState] Adding SelectionStateDoneWithMech.");
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
                if (selectedActor.HasBegunActivation || selectedActor.HasMovedThisRound && !selectedActor.HasActivatedThisRound)
                {
                    if (selectedActor is Mech mech)
                    {
                        mech.GenerateAndPublishHeatSequence(-1, true, false, selectedActor.GUID);
                        Mod.Log.Info?.Write($"Generated and Published Heat Sequence for {mech.Description.UIName}.");
                    }

                    selectedActor.DoneWithActor(); //need to to onactivationend too
                    selectedActor.OnActivationEnd(selectedActor.GUID, __instance.GetInstanceID());
                    return;
                }

                selectedActor.CreateEffect(AbilityResourceEffects.AbilityUsedFiringData, null, AbilityResourceEffects.AbilityUsedFiringData.Description.Id, -1, selectedActor);
                for (int i = selectionStack.Count - 1; i >= 0; i--)
                {
                    if (selectionStack[i] is SelectionStateFire selectionStateFire)
                    {
                        selectionStateFire.OnInactivate();
                        selectionStateFire.OnRemoveFromStack();
                        selectionStack.Remove(selectionStateFire);
                    }
                    else switch (selectionStack[i])
                    {
                        case SelectionStateFireMulti selectionStateFireMulti:
                            selectionStateFireMulti.OnInactivate();
                            selectionStateFireMulti.OnRemoveFromStack();
                            selectionStack.Remove(selectionStateFireMulti);
                            break;
                        case SelectionStateMoraleAttack selectionStateMoraleAttack:
                            selectionStateMoraleAttack.OnInactivate();
                            selectionStateMoraleAttack.OnRemoveFromStack();
                            selectionStack.Remove(selectionStateMoraleAttack);
                            break;
                        case SelectionStateMove selectionStateMove:
                            selectionStateMove.RefreshPossibleTargets();
                            break;
                        case SelectionStateJump selectionStateJump:
                            selectionStateJump.RefreshPossibleTargets();
                            break;
                    }
                }
                HUD.MechWarriorTray.FireButton.DisableButton();
                if (!selectionStack.Any(x => x is SelectionStateDoneWithMech) && selectedActor.HasMovedThisRound)
                {
                    Mod.Log.Info?.Write($"[CombatHUDActionButton_ActivateAbility_Confirmed] Adding SelectionStateDoneWithMech.");
                    var doneState = new SelectionStateDoneWithMech(selectedActor.Combat, HUD,
                        HUD.MechWarriorTray.DoneWithMechButton, selectedActor);
                    var addState = Traverse.Create(HUD.SelectionHandler)
                        .Method("addNewState", new Type[] { typeof(SelectionState) });
                    addState.GetValue(doneState);
                }
                HUD.SelectionHandler.DisableAbilitiesUsingResource(AbilityDef.ResourceConsumed.ConsumesFiring);
                // this DOES work to disable firing.
            }
            else if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesMovement)
            {

                //selectedActor.HasMovedThisRound = true;
                selectedActor.CreateEffect(AbilityResourceEffects.ImmobileEffectData, null, AbilityResourceEffects.ImmobileEffectData.Description.Id, -1, selectedActor);
                HUD.SelectionHandler.DisableAbilitiesUsingResource(AbilityDef.ResourceConsumed.ConsumesMovement);
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
                if (selectedActor.HasBegunActivation || selectedActor.HasMovedThisRound && !selectedActor.HasActivatedThisRound)
                {
                    if (selectedActor is Mech mech)
                    {
                        mech.GenerateAndPublishHeatSequence(-1, true, false, selectedActor.GUID);
                        Mod.Log.Info?.Write($"Generated and Published Heat Sequence for {mech.Description.UIName}.");
                    }

                    selectedActor.DoneWithActor(); //need to to onactivationend too
                    selectedActor.OnActivationEnd(selectedActor.GUID, __instance.GetInstanceID());
                    return;
                }

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
                    Mod.Log.Info?.Write($"[CombatHUDEquipmentSlot_ActivateAbility_Confirmed] Adding SelectionStateDoneWithMech.");
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
