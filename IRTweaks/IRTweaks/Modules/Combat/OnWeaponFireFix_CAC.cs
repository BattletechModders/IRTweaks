#if NO_CAC
#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
using CustAmmoCategories;
using CustomAmmoCategoriesPatches;
using CustomUnits;
using Harmony;

namespace IRTweaks.Modules.Combat
{
    [HarmonyPatch(typeof(CustomMech))]
    [HarmonyPatch("ApplyBraced", new Type[] { })]
    public static class CustomMech_ApplyBraced_OnFire
    {
        public static bool Prepare()
        {
            return Mod.Config.Fixes.OnWeaponFireFix;
        }
        public static void Postfix(CustomMech __instance)
        {
            Mod.Log.Debug?.Write($"[BracedLastRound] Processing BracedLastRound for {__instance.DisplayName} {__instance.GUID}.");
            if (!ModState.DidActorBraceLastRoundBeforeFiring.ContainsKey(__instance.GUID))
            {
                ModState.DidActorBraceLastRoundBeforeFiring.Add(__instance.GUID, true);
            }
            else ModState.DidActorBraceLastRoundBeforeFiring[__instance.GUID] = true;
        }
    }

    [HarmonyPatch(typeof(Mech))]
    [HarmonyPatch("ApplyBraced", new Type[] { })]
    public static class Mech_ApplyBraced_OnFire
    {
        public static bool Prepare()
        {
            return Mod.Config.Fixes.OnWeaponFireFix;
        }
        public static void Postfix(Mech __instance)
        {
            Mod.Log.Debug?.Write($"[BracedLastRound] Processing BracedLastRound for {__instance.DisplayName} {__instance.GUID}.");
            if (!ModState.DidActorBraceLastRoundBeforeFiring.ContainsKey(__instance.GUID))
            {
                ModState.DidActorBraceLastRoundBeforeFiring.Add(__instance.GUID, true);
            }
            else ModState.DidActorBraceLastRoundBeforeFiring[__instance.GUID] = true;
        }
    }

    [HarmonyPatch(typeof(Weapon), "ProcessOnFiredFloatieEffects", new Type[] { })]
    public static class Weapon_ProcessOnFiredFloatieEffects_Patch
    {
        public static bool Prepare()
        {
            return Mod.Config.Fixes.OnWeaponFireFix;
        }

        public static void Postfix(Weapon __instance, CombatGameState ___combat)
        {
            var effectsFromAmmoAndMode = new List<EffectData>();
            effectsFromAmmoAndMode.AddRange(__instance.ammo().statusEffects.Where(x=>x.effectType == EffectType.StatisticEffect && x.targetingData.effectTriggerType == EffectTriggerType.OnWeaponFire));
            effectsFromAmmoAndMode.AddRange(__instance.mode().statusEffects.Where(x => x.effectType == EffectType.StatisticEffect && x.targetingData.effectTriggerType == EffectTriggerType.OnWeaponFire));

            foreach (var effect in effectsFromAmmoAndMode)
            {
                if (effect.targetingData.effectTriggerType == EffectTriggerType.OnWeaponFire)
                {
                    string effectID = string.Format("{0}Effect_{1}_{2}", effect.targetingData.effectTriggerType.ToString(), __instance.parent.GUID, -1);
                    foreach (ICombatant combatant in ___combat.EffectManager.GetTargetCombatantForEffect(effect, __instance.parent, __instance.parent))
                    {
                        ___combat.EffectManager.CreateEffect(effect, effectID, -1, __instance.parent, combatant, default(WeaponHitInfo), 0, false);
                        if (!effect.targetingData.hideApplicationFloatie)
                        {
                            ___combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.parent.GUID, combatant.GUID, effect.Description.Name, FloatieMessage.MessageNature.Buff));
                        }
                    }
                }
            }

            List<Effect> allEffectsTargeting = ___combat.EffectManager.GetAllEffectsTargeting(__instance.parent);
            for (int i = 0; i < allEffectsTargeting.Count; i++)
            {
                if (allEffectsTargeting[i].EffectData.effectType == EffectType.FloatieEffect && allEffectsTargeting[i].EffectData.targetingData.effectTriggerType == EffectTriggerType.OnWeaponFire)
                {
                    allEffectsTargeting[i].Trigger(); // this won't work, no trigger for stat effect. may need to build one?
                }
                else if (allEffectsTargeting[i].EffectData.effectType == EffectType.StatisticEffect &&
                         allEffectsTargeting[i].EffectData.targetingData.effectTriggerType ==
                         EffectTriggerType.OnWeaponFire)
                {
                    if (allEffectsTargeting[i] is StatisticEffect effect)
                    {
                        var effectData = Traverse.Create(allEffectsTargeting[i]).Field("effectData")
                            .GetValue<EffectData>();

                        if (effectData.targetingData.triggerLimit <= 0 || effect.triggerCount <
                            effectData.targetingData.triggerLimit)
                        {
                            int triggerCount = effect.triggerCount + 1;
                            Traverse.Create(effect).Property("triggerCount").SetValue(triggerCount);

                            var timer = Traverse.Create(effect).Field("eTimer").GetValue<ETimer>();
                            timer.IncrementActivations(effectData.targetingData.extendDurationOnTrigger);
                            timer.IncrementActivations(effectData.targetingData.extendDurationOnTrigger);
                        }
                    }
                }
            }
            Mod.Log.Trace?.Write($"[ProcessOnFiredFloatieEffects] Dumping tags: {__instance.parent.GetTags().ToJSON()}.");
            if (__instance.parent is Mech mech && mech.isHasStability() &&
                !mech.GetTags().Contains(Mod.Config.Combat.OnWeaponFireOpts.IgnoreSelfKnockdownTag))
            {
                if (__instance.StatusEffects().Any(x =>
                        x?.statisticData?.statName == Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownCheckStatName))
                {
                    ModState.AttackShouldCheckForKnockDown = true;
                }
            }
        }
    }

   [HarmonyPatch(typeof(AttackDirector.AttackSequence), "OnAttackSequenceResolveDamage", new Type[] { typeof(MessageCenterMessage) })]
    public static class AttackDirectorAttackSequence_OnAttackSequenceResolveDamage
    {
        public static bool Prepare()
        {
            return !string.IsNullOrEmpty(Mod.Config.Combat.OnWeaponHitOpts.ForceShutdownOnHitStat) && Mod.Config.Fixes.OnWeaponFireFix;
        }

        public static void Postfix(AttackDirector.AttackSequence __instance, MessageCenterMessage message)
        {
            AttackSequenceResolveDamageMessage attackSequenceResolveDamageMessage = (AttackSequenceResolveDamageMessage)message;
            WeaponHitInfo hitInfo = attackSequenceResolveDamageMessage.hitInfo;
            AttackDirector.AttackSequence attackSequence = __instance.Director.GetAttackSequence(hitInfo.attackSequenceId);
            Weapon weapon = __instance.GetWeapon(attackSequenceResolveDamageMessage.hitInfo.attackGroupIndex, attackSequenceResolveDamageMessage.hitInfo.attackWeaponIndex);
            foreach (EffectData effectData in weapon.StatusEffects())
            {
                if (effectData.targetingData.effectTriggerType == EffectTriggerType.OnHit &&
                    effectData?.statisticData?.statName ==
                    Mod.Config.Combat.OnWeaponHitOpts.ForceShutdownOnHitStat)
                {
                    for (int j = 0; j < attackSequence?.allAffectedTargetIds.Count; j++)
                    {
                        AbstractActor abstractActor =
                            __instance.Director.Combat.FindActorByGUID(attackSequence.allAffectedTargetIds[j]);
                        if (abstractActor is Mech mech && !mech.IsShutDown)
                        {
                            if (mech.GetTags().Contains(Mod.Config.Combat.OnWeaponHitOpts.IgnoreShutdownTag)) continue;
                            int firstHitLocationForTarget = hitInfo.GetFirstHitLocationForTarget(abstractActor.GUID);
                            if (firstHitLocationForTarget >= 0 && !abstractActor.IsDead)
                            {
                                ModState.AttackShouldCheckActorsForShutdown.Add(mech.GUID);
                            }
                        }
                    }

                    var advInfo = hitInfo.advInfo();
                    if (advInfo == null) continue;
                    foreach (var aoeRecord in advInfo.hits)
                    {
                        if (aoeRecord.isHit && aoeRecord.isAOE && aoeRecord.target is Mech aoeMech && !aoeMech.IsShutDown)
                        {
                            if (aoeMech.GetTags().Contains(Mod.Config.Combat.OnWeaponHitOpts.IgnoreShutdownTag)) continue;
                            //int firstHitLocationForTarget = hitInfo.GetFirstHitLocationForTarget(aoeMech.GUID);
                            if (!aoeMech.IsDead && !ModState.AttackShouldCheckActorsForShutdown.Contains(aoeMech.GUID))
                            {
                                ModState.AttackShouldCheckActorsForShutdown.Add(aoeMech.GUID);
                                Mod.Log.Info?.Write($"[OnAttackSequenceResolveDamage] Added {aoeMech.DisplayName} to state for AOE shutdown check.");
                            }
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(AttackDirector), "OnAttackComplete", new Type[] { typeof(MessageCenterMessage) })]
    public static class AttackDirector_OnAttackComplete
    {
        public static bool Prepare()
        {
            return Mod.Config.Fixes.OnWeaponFireFix;
        }

        public static void Prefix(AttackDirector __instance, MessageCenterMessage message)
        {
            AttackCompleteMessage attackCompleteMessage = (AttackCompleteMessage)message;
            int sequenceId = attackCompleteMessage.sequenceId;
            AttackDirector.AttackSequence attackSequence = __instance.GetAttackSequence(sequenceId);
            if (attackSequence != null)
            {
                if (ModState.AttackShouldCheckForKnockDown)
                {
                    ModState.AttackShouldCheckForKnockDown = false;

                    var attacker = attackSequence.attacker;
                    if (attacker.isHasStability())
                    {
                        Mod.Log.Info?.Write(
                            $"[OnAttackComplete] Processing OnWeaponFire self-knockdown check for {attacker.DisplayName}.");

                        var knockdownChance =
                            attacker.StatCollection.GetValue<float>(
                                Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownCheckStatName);
                        var fromBraced = 0f;
                        Mod.Log.Trace?.Write($"[OnAttackComplete] TRACE {attacker.DisplayName} {attacker.GUID} Actor braced last round? {ModState.DidActorBraceLastRoundBeforeFiring.ContainsKey(attacker.GUID)}. DistMovedThisRound: {attacker.DistMovedThisRound}");
                        if (ModState.DidActorBraceLastRoundBeforeFiring.ContainsKey(attacker.GUID) &&
                            attacker.DistMovedThisRound <= 20f)
                        {
                            fromBraced = Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownBracedFactor;
                        }

                        var fromPiloting = attacker.GetPilot().Piloting *
                                           Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownPilotingFactor;
                        var fromTonnage = 0f;
                        if (attacker is Mech mechTonnage)
                        {
                            if (mechTonnage.tonnage < Mod.Config.Combat.OnWeaponFireOpts
                                    .SelfKnockdownTonnageBonusThreshold)
                            {
                                fromTonnage = mechTonnage.tonnage * Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownTonnageFactor;
                            }
                            else
                            {
                                var baseTonnageFactor = mechTonnage.tonnage * Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownTonnageFactor;
                                var bonusTonnage = mechTonnage.tonnage - Mod.Config.Combat.OnWeaponFireOpts
                                    .SelfKnockdownTonnageBonusThreshold;
                                fromTonnage = baseTonnageFactor + (bonusTonnage * Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownTonnageBonusFactor);
                            }
                        }
                        var finalChance = knockdownChance - (fromBraced + fromPiloting + fromTonnage);
                        var roll = Mod.Random.NextDouble();

                        Mod.Log.Info?.Write(
                            $"[OnAttackComplete] Final self-knockdown chance: {finalChance} from weapon effects {knockdownChance} - (braced state: {fromBraced} + piloting factor {fromPiloting} + tonnage factor {fromTonnage}) VS roll {roll}.");
                        if (roll <= finalChance)
                        {
                            if (!attacker.IsFlaggedForKnockdown)
                            {
                                attacker.FlagForKnockdown();
                                attacker.HandleKnockdown(-1,
                                    $"{attacker.DisplayName}_{Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownCheckStatName}",
                                    attacker.CurrentPosition, null);
                                Mod.Log.Info?.Write(
                                    $"[AttackDirector.OnAttackComplete] Found knockdown flag at OnAttackComplete, knocking down {attacker.DisplayName}.");

                                if (attacker is Mech mech)
                                {
                                    mech.GenerateAndPublishHeatSequence(-1, true, false, mech.GUID);
                                    Mod.Log.Debug?.Write(
                                        $"Generated and Published Heat Sequence for {mech.Description.UIName}.");
                                }

                                attacker.DoneWithActor(); //need to to onactivationend too
                                attacker.OnActivationEnd(attacker.GUID, -1);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Mod.Config.Combat.OnWeaponHitOpts.ForceShutdownOnHitStat))
                {
                    for (var index = ModState.AttackShouldCheckActorsForShutdown.Count - 1; index >= 0; index--)
                    {
                        var targetActorID = ModState.AttackShouldCheckActorsForShutdown[index];
                        //if (!ModState.AttackShouldCheckActorsForShutdown.Contains(targetActorID)) continue;
                        Mod.Log.Info?.Write(
                            $"[OnAttackComplete] Processing OnHit forced shutdown check for {targetActorID}.");
                        ModState.AttackShouldCheckActorsForShutdown.Remove(targetActorID);
                        var targetActor = __instance.Combat.FindActorByGUID(targetActorID);
                        if (targetActor is Mech mech)
                        {
                            Mod.Log.Info?.Write(
                                $"[OnAttackComplete] Processing OnHit forced shutdown check for {mech.DisplayName}.");
                            var shutdownChance =
                                mech.StatCollection.GetValue<float>(
                                    Mod.Config.Combat.OnWeaponHitOpts.ForceShutdownOnHitStat);

                            var fromGuts = mech.GetPilot().Guts *
                                           Mod.Config.Combat.OnWeaponHitOpts.ResistShutdownGutsFactor;
                            var finalChance = shutdownChance - fromGuts;
                            var roll = Mod.Random.NextDouble();

                            Mod.Log.Info?.Write(
                                $"[OnAttackComplete] Final shutdown on hit chance: {finalChance} from weapon effects {shutdownChance} - guts factor {fromGuts}) VS roll {roll}.");
                            if (roll <= finalChance)
                            {
                                MechShutdownSequence mechShutdownSequence =
                                    new MechShutdownSequence(mech, attackSequence.attacker.GUID);
                                mechShutdownSequence.RootSequenceGUID = attackSequence.stackItemUID;
                                mech.Combat.MessageCenter.PublishMessage(
                                    new AddSequenceToStackMessage(mechShutdownSequence));
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif