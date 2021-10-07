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
using Harmony;

namespace IRTweaks.Modules.Combat
{

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

                        }
                    }
                }
            }
        }
    }
}
#endif