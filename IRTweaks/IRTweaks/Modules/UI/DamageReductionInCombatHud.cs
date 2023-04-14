using CustAmmoCategories;
using System;
using UnityEngine;

namespace IRTweaks.Modules.UI
{
    static class DamageReductionInCombatHud
    {
        public static void RefreshPips(CombatHUDEvasiveBarPips pips)
        {
            AbstractActor actor = null;
            if (ModState.DamageReductionInCombatHud.ContainsKey(pips))
            {
                actor = ModState.DamageReductionInCombatHud[pips];
            }

            if (actor == null)
            {
                CombatHUDActorInfo actorInfo = pips.gameObject.transform.parent.gameObject.GetComponent<CombatHUDActorInfo>();
                Mod.Log.Debug?.Write($"DRInCH: Locating actor for {pips} from parent actorInfo: {actorInfo}");
                if (actorInfo != null)
                {
                    actor = actorInfo.DisplayedCombatant as AbstractActor;
                    if (actor != null)
                    {
                        ModState.DamageReductionInCombatHud[pips] = actor;
                    }
                }
            }

            if (actor != null)
            {
                RefreshText(pips, actor);
            }
            else
            {
                Mod.Log.Debug?.Write($"DRInCH: No actor for {pips}.");
            }
        }

        public static void RefreshActor(AbstractActor actor)
        {
            CombatHUDEvasiveBarPips pips = null;
            if (ModState.DamageReductionInCombatHudActors.ContainsKey(actor))
            {
                pips = ModState.DamageReductionInCombatHudActors[actor];
            }

            if (pips == null)
            {
                Mod.Log.Trace?.Write($"DRInCH: Looking for actor for {actor} based on unity tree.");
                foreach (CombatHUDActorInfo actorInfo in Resources.FindObjectsOfTypeAll(typeof(CombatHUDActorInfo)) as CombatHUDActorInfo[])
                {
                    AbstractActor thisActor = actorInfo.DisplayedCombatant as AbstractActor;
                    Mod.Log.Trace?.Write($"DRInCH: Checking thisActor {thisActor} == actor {actor}");
                    if (thisActor == actor)
                    {
                        pips = actorInfo.EvasiveDisplay;
                        ModState.DamageReductionInCombatHudActors[actor] = pips;
                        break;
                    }
                }
            }

            if (pips != null)
            {
                RefreshText(pips, actor);
            }
            else
            {
                Mod.Log.Trace?.Write($"DRInCH: No pips for actor {actor} {actor.UnitName}");
            }
        }

        public static void RefreshText(CombatHUDEvasiveBarPips pips, AbstractActor actor)
        {
            CombatHUDEvasivePipsText pipsText = pips.gameObject.GetComponent<CombatHUDEvasivePipsText>();
            if (pipsText == null)
            {
                Mod.Log.Debug?.Write($"DRInCH: No pipsText for pips {pips}, actor {actor} {actor.UnitName}");
                return;
            }

            float damageReduction = 1 - actor.StatCollection.GetStatistic("DamageReductionMultiplierAll").Value<float>();

            string text = "";
            if (damageReduction != 0)
            {
                text += $"{Math.Round(damageReduction * 100)}% DR\n";
            }
            if (pips.Current > 0)
            {
                text += Math.Round(pips.Current) + " EVA";
            }

            // We replace the vanilla pips with our own display text. No more >>>>.
            pips.ActivatePips(0);

            pipsText.text.SetText(text);
            pipsText.text.color = Color.white;
            pipsText.gameObject.SetActive(value: true);
            pipsText.text.gameObject.SetActive(value: true);
        }
    }

    // Add damage reduction to the combat hud, next to evasion pips.
    [HarmonyPatch(typeof(CombatHUDEvasiveBarPips), "ShowCurrent")]
    public static class CombatHUDEvasiveBarPips_ShowCurrent
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        static void Postfix(CombatHUDEvasiveBarPips __instance)
        {
            // This method is also called for CombatHUDStabilityBarPips, CombatHUDLifeBarPips, etc. We only care about actual EvasiveBarPips instances
            if (__instance.GetType() == typeof(CombatHUDEvasiveBarPips))
            {
                DamageReductionInCombatHud.RefreshPips(__instance);
            }
        }

    }

    [HarmonyPatch(typeof(CombatHUDEvasiveBarPips), "CalcPipsAndActivate")]
    static class CombatHUDEvasiveBarPips_CalcPipsAndActivate
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        static void Postfix(CombatHUDEvasiveBarPips __instance)
        {
            // This method is also called for CombatHUDStabilityBarPips, CombatHUDLifeBarPips, etc. We only care about actual EvasiveBarPips instances
            if (__instance.GetType() == typeof(CombatHUDEvasiveBarPips))
            {
                DamageReductionInCombatHud.RefreshPips(__instance);
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
            __instance.evasiveDisplay.gameObject.SetActive(value: true);
        }
    }

    [HarmonyPatch(typeof(EffectManager), "CreateEffect", new Type[] { typeof(EffectData), typeof(string), typeof(int), typeof(ICombatant), typeof(ICombatant), typeof(WeaponHitInfo), typeof(int), typeof(bool) })]
    static class EffectManager_CreateEffect
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        static void Postfix(ICombatant target)
        {
            AbstractActor actor = target as AbstractActor;

            // This might be a building, in which case casting as an AbstractActor will return null.
            if (actor != null)
            {
                DamageReductionInCombatHud.RefreshActor(actor);
            }
        }
    }

    [HarmonyPatch(typeof(EffectManager), "EffectComplete")]
    static class EffectManager_EffectComplete
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        static void Postfix(Effect e)
        {
            AbstractActor actor = e.Target as AbstractActor;

            // This might be a building, in which case casting as an AbstractActor will return null.
            if (actor != null)
            {
                DamageReductionInCombatHud.RefreshActor(actor);
            }
        }
    }

    // Override CAC's logic with our own. We always activate the pips text, and put different text in it.
    [HarmonyPatch(typeof(CustAmmoCategories.CombatHUDEvasiveBarPips_ShowCurrent), "Postfix")]
    static class CombatHUDEvasiveBarPips_ShowCurrent_Postfix_Patch
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        static void Prefix(ref bool __runOriginal)
        {
            if (!__runOriginal) return;
            __runOriginal = false;
        }
    }

    // We replace the vanilla FireSequence method because it fires a number of unnecessary / incorrect floaties.
    // The game logic is identical, but this version emits different floaties.
    [HarmonyPatch(typeof(AttackStackSequence), "FireSequence")]
    static class AttackStackSequence_FireSequence
    {
        static bool Prepare() => Mod.Config.Fixes.DamageReductionInCombatHud;

        static void Prefix(ref bool __runOriginal, AttackStackSequence __instance, AttackDirector.AttackSequence sequence)
        {
            if (!__runOriginal) return;

            EffectData effect = __instance.Combat.Constants.Visibility.FiredWeaponsEffect;
            __instance.owningActor.CreateEffect(effect, null, sequence.id.ToString(), sequence.stackItemUID, __instance.owningActor);
            __instance.owningActor.UpdateVisibilityCache(__instance.Combat.GetAllCombatants());
            __instance.Combat.AttackDirector.PerformAttack(sequence);

            AbstractActor abstractActor = sequence.chosenTarget as AbstractActor;

            if (abstractActor != null)
            {
                string floatie = "";
                if (sequence.meleeAttackType == MeleeAttackType.DFA)
                {
                    floatie = "DFA - Ignores DR";
                }
                else if (sequence.isMelee)
                {
                    floatie = "Melee - Ignores DR";
                }
                else if (sequence.IsBreachingShot)
                {
                    floatie = "Breaching Shot - Ignores DR";
                }
                else if (__instance.Combat.HitLocation.GetAttackDirection(__instance.owningActor, abstractActor) == AttackDirection.FromBack)
                {
                    floatie = "Rear Attack - Ignores DR";
                }

                if (floatie != "")
                {
                    __instance.Combat.MessageCenter.PublishMessage(new AddSequenceToStackMessage(new ShowActorInfoSequence(abstractActor, floatie, FloatieMessage.MessageNature.Debuff, useCamera: false)));
                }
            }

            __runOriginal = false;
        }
    }
}
