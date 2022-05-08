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
}

#endif
