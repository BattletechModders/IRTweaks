using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
using CustomUnits;
using Harmony;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(Mech), "ApplyBraced", new Type[] {})]
    public static class Mech_ApplyBraced
    {
        public static bool Prepare()
        {
            return Mod.Config.Abilities.BraceEffectConfig.effectDataJO.Count > 0;
        }

        public static void Postfix(Mech __instance)
        {
            foreach (var effectData in ModState.BraceEffectsInit.effects)
            {
                __instance.Combat.EffectManager.CreateEffect(effectData, effectData.Description.Id, -1,__instance, __instance, default(WeaponHitInfo),1);
                Mod.Log.Info?.Write($"[Mech_ApplyBraced] Applying effect with ID {effectData.Description.Id}.");
            }
        }
    }
#if NO_CAC
#else

    [HarmonyPatch(typeof(CustomMech), "ApplyBraced", new Type[] { })]
    public static class CustomMech_ApplyBraced
    {
        public static bool Prepare()
        {
            return Mod.Config.Abilities.BraceEffectConfig.effectDataJO.Count > 0;
        }

        public static void Postfix(CustomMech __instance)
        {
            foreach (var effectData in ModState.BraceEffectsInit.effects)
            {
                __instance.Combat.EffectManager.CreateEffect(effectData, effectData.Description.Id, -1, __instance, __instance, default(WeaponHitInfo), 1);
                Mod.Log.Info?.Write($"[CustomMech_ApplyBraced] Applying effect with ID {effectData.Description.Id}.");
            }
        }
    }
#endif
}