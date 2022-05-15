#if NO_CAC
#else

using System;
using BattleTech;
using BattleTech.UI;
using CustAmmoCategories;
using Harmony;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IRTweaks.Modules.UI
{
    // Add damage reduction to the combat hud, next to evasion pips.
    [HarmonyPatch(typeof(CombatHUDEvasiveBarPips), "ShowCurrent")]
    static class CombatHUDEvasiveBarPips_ShowCurrent
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        static void Postfix(CombatHUDEvasiveBarPips __instance)
        {
            if (ModState.DamageReductionInCombatHud.ContainsKey(__instance)) {
                RefreshText(__instance, ModState.DamageReductionInCombatHud[__instance]);
            }
        }

        public static void RefreshText(CombatHUDEvasiveBarPips pips, AbstractActor actor) {
            CombatHUDEvasivePipsText pipsText = pips.gameObject.GetComponent<CombatHUDEvasivePipsText>();
            if (pipsText == null) {
                return;
            }

            float damageReduction = 1 - actor.StatCollection.GetStatistic("DamageReductionMultiplierAll").Value<float>();

            string text = "";
            if (damageReduction > 0) {
                text += $"{Math.Round(damageReduction * 100)}% DR\n";
            }
            if (pips.Current > 0) {
                text += Math.Round(pips.Current) + " pips";
            }

            if (string.IsNullOrEmpty(text)) {
                pipsText.text.gameObject.SetActive(value: false);
                pipsText.text.enableWordWrapping = false;
            }
            else
            {
                pipsText.text.SetText(text);
                pipsText.text.gameObject.SetActive(value: true);

                // We replace the vanilla pips with our own display text. No more >>>>.
                pips.ActivatePips(0);
            }
        }
    }

    [HarmonyPatch(typeof(CombatHUDEvasiveBarPips), "CacheActorData")]
    static class CombatHUDEvasiveBarPips_CacheActorData
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        static void Postfix(CombatHUDEvasiveBarPips __instance, AbstractActor actor)
        {
            ModState.DamageReductionInCombatHud[__instance] = actor;
            ModState.DamageReductionInCombatHudActors[actor] = __instance;
        }
    }

    [HarmonyPatch(typeof(CombatHUDStatusPanel), "ShowMoveIndicators", new Type[] { typeof(AbstractActor), typeof(float) })]
    static class CombatHUDStatusPanel_ShowMoveIndicators
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        // We want to display damage reduction even if there are 0 evasion pips.
        static void Postfix(CombatHUDStatusPanel __instance, AbstractActor target, float currentEvasive)
        {
            float damageReduction = 1 - target.StatCollection.GetStatistic("DamageReductionMultiplierAll").Value<float>();
            if (damageReduction > 0)
            {
                __instance.evasiveDisplay.gameObject.SetActive(value: true);
            }
        }
    }

    [HarmonyPatch(typeof(EffectManager), "CreateEffect", new Type[] { typeof(EffectData), typeof(string), typeof(int), typeof(ICombatant), typeof(ICombatant), typeof(WeaponHitInfo), typeof(int), typeof(bool) })]
    static class EffectManager_CreateEffect
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        static void Postfix(ICombatant target) {
            AbstractActor actor = target as AbstractActor;

            // This might be a building, in which case casting as an AbstractActor will return null.
            if (actor != null && ModState.DamageReductionInCombatHudActors.ContainsKey(actor)) {
                CombatHUDEvasiveBarPips_ShowCurrent.RefreshText(ModState.DamageReductionInCombatHudActors[actor], actor);
            }
        }
    }

    [HarmonyPatch(typeof(EffectManager), "EffectComplete")]
    static class EffectManager_EffectComplete
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        static void Postfix(Effect e) {
            AbstractActor actor = e.Target as AbstractActor;

            // This might be a building, in which case casting as an AbstractActor will return null.
            if (actor != null && ModState.DamageReductionInCombatHudActors.ContainsKey(actor)) {
                CombatHUDEvasiveBarPips_ShowCurrent.RefreshText(ModState.DamageReductionInCombatHudActors[actor], actor);
            }
        }
    }

    // We replace the vanilla FireSequence method because it fires a number of unnecessary / incorrect floaties.
    // The game logic is identical, but this version emits different floaties.
    [HarmonyPatch(typeof(AttackStackSequence), "FireSequence")]
    static class AttackStackSequence_FireSequence
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        static bool Prefix(AttackStackSequence __instance, AttackDirector.AttackSequence sequence) {
            CombatGameState combat = Traverse.Create(__instance).Property("Combat").GetValue<CombatGameState>();
            EffectData effect = combat.Constants.Visibility.FiredWeaponsEffect;

            combat.EffectManager.CreateEffect(effect, sequence.id.ToString(), sequence.stackItemUID, __instance.owningActor, __instance.owningActor, default(WeaponHitInfo), -1);
            __instance.owningActor.UpdateVisibilityCache(combat.GetAllCombatants());
            combat.AttackDirector.PerformAttack(sequence);

            AbstractActor abstractActor = sequence.chosenTarget as AbstractActor;

            if (abstractActor != null) {
                string floatie = "";
                if (sequence.meleeAttackType == MeleeAttackType.DFA) {
                    floatie = "DFA - Ignores DR";
                } else if (sequence.isMelee) {
                    floatie = "Melee - Ignores DR";
                } else if (sequence.IsBreachingShot) {
                    floatie = "Breaching Shot - Ignores DR";
                } else if (combat.HitLocation.GetAttackDirection(__instance.owningActor, abstractActor) == AttackDirection.FromBack) {
                    floatie = "Rear Attack - Ignores DR";
                }

                if (floatie != "") {
                    combat.MessageCenter.PublishMessage(new AddSequenceToStackMessage(new ShowActorInfoSequence(abstractActor, floatie, FloatieMessage.MessageNature.Debuff, useCamera: false)));
                }
            }

            return false;
        }
    }
}

#endif
