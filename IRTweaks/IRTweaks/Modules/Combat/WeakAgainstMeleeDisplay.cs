using BattleTech.UI;
using HBS;
using Localize;
using System;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(CombatHUDStatusPanel), "ShowMeleeDamageMultipliers")]
    static class CombatHUDStatusPanel_ShowMeleeDamageMultipliers
    {
        static bool Prepare() => Mod.Config.Fixes.WeakAgainstMeleeFix;
        //private static MethodInfo _showDebuff = AccessTools.Method(typeof(CombatHUDStatusPanel), "ShowDebuff", new Type[]
        //{
        //    typeof(SVGAsset), typeof(Text), typeof(Text), typeof(Vector3), typeof(bool)
        //});

        static bool Prefix(CombatHUDStatusPanel __instance, AbstractActor actor)
        {
            if (actor.UnitType == UnitType.Vehicle)
            {
                var dmg = actor.Combat.Constants.ResolutionConstants.MeleeDamageMultiplierVehicle * 100;
                //_showDebuff.Invoke(__instance,
                //    new object[]
                //    {
                //        LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.StatusVehicleMeleeIcon,
                //        new Text(ModText.CG_Title_WeakMelee, Array.Empty<object>()),
                //        new Text(ModText.CG_Text_WeakMeleeVehicles, dmg),
                //        __instance.defaultIconScale, false
                //    });
                __instance.ShowDebuff(
                    LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.StatusVehicleMeleeIcon,
                    new Text(ModText.CG_Title_WeakMelee, Array.Empty<object>()),
                    new Text(ModText.CG_Text_WeakMeleeVehicles, dmg),
                    __instance.defaultIconScale, false);
                return false;
            }

            if (actor.UnitType == UnitType.Turret)
            {
                var dmg = actor.Combat.Constants.ResolutionConstants.MeleeDamageMultiplierTurret * 100;
                //_showDebuff.Invoke(__instance,
                //    new object[]
                //   {
                //        LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.StatusVehicleMeleeIcon,
                //        new Text(ModText.CG_Title_WeakMelee, Array.Empty<object>()),
                //        new Text(ModText.CG_Text_WeakMeleeTurrets, dmg),
                //        __instance.defaultIconScale, false
                //    });
                __instance.ShowDebuff(
                    LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.StatusVehicleMeleeIcon,
                    new Text(ModText.CG_Title_WeakMelee, Array.Empty<object>()),
                    new Text(ModText.CG_Text_WeakMeleeVehicles, dmg),
                    __instance.defaultIconScale, false);
                return false;
            }

            return false;
        }
    }
}