using BattleTech;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRTweaks.Modules.Combat
{
    public static class StatUtil
    {
        public static bool GetIsFiringDisabled(this Weapon weapon)
        {
            return weapon.StatCollection.GetValue<bool>("IsFiringDisabled");
        }
    }

    [HarmonyPatch(typeof(Weapon), "InitStats",
    new Type[] { })]
    public static class Weapon_InitStats
    {
        static bool Prepare() => Mod.Config.Fixes.DisableFiringStat;
        public static void Postfix(Weapon __instance)
        {
            __instance.StatCollection.AddStatistic<bool>("IsFiringDisabled", false);
        }
    }

    [HarmonyPatch(typeof(Weapon), "IsDisabled", MethodType.Getter)]
    public static class Weapon_IsDisabled
    {
        static bool Prepare() => Mod.Config.Fixes.DisableFiringStat;
        public static void Postfix(Weapon __instance, ref bool __result)
        {
            if (!__result)
            {
                if (__instance.GetIsFiringDisabled())
                {
                    __result = true;
                }
            }
        }
    }
}
