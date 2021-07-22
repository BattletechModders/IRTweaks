using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
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
