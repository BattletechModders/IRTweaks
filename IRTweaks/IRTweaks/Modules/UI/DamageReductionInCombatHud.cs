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
            CombatHUDEvasivePipsText pipsText = __instance.gameObject.GetComponent<CombatHUDEvasivePipsText>();

            if (pipsText != null && ModState.DamageReductionInCombatHud.ContainsKey(__instance)) {
                StatCollection stats = ModState.DamageReductionInCombatHud[__instance].StatCollection;
                float damageReduction = 1 - stats.GetStatistic("DamageReductionMultiplierAll").Value<float>();

                string text = "";
                if (damageReduction > 0) {
                    text += $"{Math.Round(damageReduction * 100)}% DR ";
                }
                if (__instance.Current > 0) {
                    text += __instance.Current + " pips";
                }

                if (string.IsNullOrEmpty(text)) {
                    pipsText.text.gameObject.SetActive(value: false);
                }
                else
                {
                    pipsText.text.SetText(text);
                    pipsText.text.gameObject.SetActive(value: true);
                }
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
}

#endif
