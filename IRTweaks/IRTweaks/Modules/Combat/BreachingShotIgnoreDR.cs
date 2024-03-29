﻿using CustAmmoCategories;
using Sheepy.BattleTechMod.AttackImprovementMod;
using System;
using UnityEngine;

namespace IRTweaks.Modules.Combat
{
#if NO_CAC
#else

    [HarmonyPatch(typeof(DamageModifiers), "DamageReductionMultiplierAll", new Type[] { typeof(Weapon), typeof(Vector3), typeof(ICombatant), typeof(bool), typeof(int), typeof(float), typeof(float), typeof(float), typeof(float) })]
    static class DamageModifiers_DamageReductionMultiplierAll
    {
        static bool Prepare() => Mod.Config.Fixes.BreachingShotIgnoresAllDR;

        static void Postfix(ref float __result, Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot, int location, float dmg, float ap, float heat, float stab)
        {
            if (IsBreachingShot || weapon.parent.Combat.HitLocation.GetAttackDirection(attackPosition, target) == AttackDirection.FromBack || weapon.uid == "CBTBE_FAKE_MELEE" || weapon.uid == "CBTBE_FAKE_DFA" || weapon.uid.EndsWith("_Melee") || weapon.uid.EndsWith("_DFA"))
            {
                if (__result <= 1f)
                    __result = 1f;
            }
        }
    }

    [HarmonyPatch(typeof(Criticals), "GetWeaponDamage", new Type[] {typeof(AbstractActor), typeof(WeaponHitInfo), typeof(Weapon)})]
    static class AIM_Criticals_GetWeaponDamage
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.BreachingShotIgnoresAllDR;

        [HarmonyPrefix]
        static void Prefix(ref bool __runOriginal, AbstractActor target, WeaponHitInfo hitInfo, Weapon weapon)
        {
            if (!__runOriginal) return;

            var combat = UnityGameInstance.BattleTechGame.Combat;
            var attackSequence = combat.AttackDirector.GetAttackSequence(hitInfo.attackSequenceId);
            if (attackSequence.IsBreachingShot || combat.HitLocation.GetAttackDirection(attackSequence.attackPosition, attackSequence.chosenTarget) == AttackDirection.FromBack || attackSequence.meleeAttackType != MeleeAttackType.NotSet)
            {
                ModState.ShouldGetReducedDamageIgnoreDR = true;
            }
        }

        [HarmonyPostfix]
        static void Postfix(AbstractActor target, WeaponHitInfo hitInfo, Weapon weapon)
        {
            ModState.ShouldGetReducedDamageIgnoreDR = false;
        }
    }

#endif

    [HarmonyPatch(typeof(AbstractActor), "GetReducedDamage", new Type[] {typeof(float), typeof(WeaponCategoryValue), typeof(LineOfFireLevel), typeof(bool)})]
    static class AbstractActor_GetReducedDamage
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.BreachingShotIgnoresAllDR;

        [HarmonyPrefix]
        static void Prefix(ref bool __runOriginal, AbstractActor __instance, float incomingDamage, WeaponCategoryValue weaponCategoryValue, LineOfFireLevel lofLevel, bool doLogging, ref float __result)
        {
            if (!__runOriginal) return;

            if (!ModState.ShouldGetReducedDamageIgnoreDR) return;

            var num = 1f;
            var dmgReductionCurrent = __instance.StatCollection.GetValue<float>("DamageReductionMultiplierAll");
            if (dmgReductionCurrent > 1f)
            {
                num = dmgReductionCurrent;
            }

            if (doLogging && AbstractActor.damageLogger.IsLogEnabled)
            {
                string message1 = __instance.BuildLogMessage(string.Format("!!! Found IgnoreDR flag from breaching shot! Ignoring value of DamageReductionMultiplierAll!"));
                AbstractActor.damageLogger.Log(message1, __instance.GameRep);
            }
            if (!string.IsNullOrEmpty(weaponCategoryValue.DamageReductionMultiplierStat))
            {
                num *= __instance.StatCollection.GetValue<float>(weaponCategoryValue.DamageReductionMultiplierStat);
            }
            if (doLogging && AbstractActor.damageLogger.IsLogEnabled)
            {
                string message2 = __instance.BuildLogMessage(string.Format("!!! Damage Multiplier now {0} from {1}", num, weaponCategoryValue.Name));
                AbstractActor.damageLogger.Log(message2, __instance.GameRep);
            }
            if (lofLevel != LineOfFireLevel.LOFBlocked)
            {
                if (lofLevel == LineOfFireLevel.LOFObstructed)
                {
                    num *= __instance.Combat.Constants.ToHit.DamageResistanceObstructed;
                }
            }
            else
            {
                num *= __instance.Combat.Constants.ToHit.DamageResistanceIndirectFire;
            }
            if (doLogging && AbstractActor.damageLogger.IsLogEnabled)
            {
                string message3 = __instance.BuildLogMessage(string.Format("!!! Damage Multiplier now {0} from {1}", num, lofLevel.ToString()));
                AbstractActor.damageLogger.Log(message3, __instance.GameRep);
            }
            if (num < 0f)
            {
                num = 0f;
                if (doLogging && AbstractActor.damageLogger.IsLogEnabled)
                {
                    string message4 = __instance.BuildLogMessage("!!! Damage Multiplier clamped to zero.");
                    AbstractActor.damageLogger.Log(message4, __instance.GameRep);
                }
            }
            __result = incomingDamage * num;
            
            __runOriginal = false;
        }
    }

    [HarmonyPatch(typeof(Mech), "ResolveWeaponDamage", new Type[] {typeof(WeaponHitInfo)})]
    static class Mech_ResolveWeaponDamage
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.BreachingShotIgnoresAllDR;

        [HarmonyPrefix]
        static void Prefix(ref bool __runOriginal, Mech __instance, WeaponHitInfo hitInfo)
        {
            if (!__runOriginal) return;

            var combat = UnityGameInstance.BattleTechGame.Combat;
            var attackSequence = combat.AttackDirector.GetAttackSequence(hitInfo.attackSequenceId);
            if (attackSequence.IsBreachingShot || combat.HitLocation.GetAttackDirection(attackSequence.attackPosition, attackSequence.chosenTarget) == AttackDirection.FromBack || attackSequence.meleeAttackType != MeleeAttackType.NotSet)
            {
                ModState.ShouldGetReducedDamageIgnoreDR = true;
            }
        }

        [HarmonyPostfix]
        static void Postfix(Mech __instance, WeaponHitInfo hitInfo)
        {
            ModState.ShouldGetReducedDamageIgnoreDR = false;
        }
    }

    [HarmonyPatch(typeof(AttackDirector.AttackSequence), "OnAttackSequenceImpact", new Type[]{typeof(MessageCenterMessage)})]
    static class AttackDirectorAttackSequence_OnAttackSequenceImpact
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.BreachingShotIgnoresAllDR;

        [HarmonyPrefix]
        static void Prefix(ref bool __runOriginal, AttackDirector.AttackSequence __instance, MessageCenterMessage message)
        {
            if (!__runOriginal) return;

            var combat = UnityGameInstance.BattleTechGame.Combat;
            if (__instance.IsBreachingShot || combat.HitLocation.GetAttackDirection(__instance.attackPosition, __instance.chosenTarget) == AttackDirection.FromBack || __instance.meleeAttackType != MeleeAttackType.NotSet)
            {
                ModState.ShouldGetReducedDamageIgnoreDR = true;
            }
        }

        [HarmonyPostfix]
        static void Postfix(AttackDirector.AttackSequence __instance, MessageCenterMessage message)
        {
            ModState.ShouldGetReducedDamageIgnoreDR = false;
        }
    }
}
