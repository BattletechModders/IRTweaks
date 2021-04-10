using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using Localize;
using SVGImporter;
using UnityEngine;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(CombatHUDStatusPanel), "ShowMeleeDamageMultipliers")]
    static class CombatHUDStatusPanel_ShowMeleeDamageMultipliers
    {
        static bool Prepare() => Mod.Config.Fixes.WeakAgainstMeleeFix;
        private static MethodInfo _showDebuff = AccessTools.Method(typeof(CombatHUDStatusPanel), "ShowDebuff", new Type[]
        {
            typeof(SVGAsset), typeof(Text), typeof(Text), typeof(Vector3), typeof(bool)
        });

        static bool Prefix(CombatHUDStatusPanel __instance, AbstractActor actor)
        {
            if (actor.UnitType == UnitType.Vehicle)
            {
                var dmg = actor.Combat.Constants.ResolutionConstants.MeleeDamageMultiplierVehicle*100;
                _showDebuff.Invoke(__instance,
                    new object[]
                    {
                        LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.StatusVehicleMeleeIcon,
                        new Text("WEAK AGAINST MELEE", Array.Empty<object>()),
                        new Text("Melee attacks do {0}% damage to vehicles.", dmg),
                        __instance.defaultIconScale, false
                    });
                return false;
            }

            if (actor.UnitType == UnitType.Turret)
            {
                var dmg = actor.Combat.Constants.ResolutionConstants.MeleeDamageMultiplierTurret*100;
                _showDebuff.Invoke(__instance,
                    new object[]
                    {
                        LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.StatusVehicleMeleeIcon,
                        new Text("WEAK AGAINST MELEE", Array.Empty<object>()),
                        new Text("Melee attacks do {0}% damage to turrets.", dmg),
                        __instance.defaultIconScale, false
                    });
                return false;
            }

            return false;
        }
    }
}