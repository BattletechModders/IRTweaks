#if NO_CAC
#else

using BattleTech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustAmmoCategories;
using CustomUnits;
using static CustAmmoCategories.CustomAmmoCategories;
using UnityEngine;

namespace IRTweaks.Modules.Combat 
{ 
    public class IRT_CAC_Obstruct
    { 
        

        public static void FinishedLoading(List<string> loadOrder) 
        {
            if (Mod.Config.Combat.ObstructionTweaks.ObstructionDRByTags.Count > 0)
            {
                DamageModifiersCache.RegisterDamageModifier("IRTweaks_Obstruct_DmgMod", "IRT_CAC_Obstruct_DmgMod", false, true, true, true, true, IRT_CAC_ObstructDmgMod, IRT_CAC_ObstructDmgModName);
            }
        }

        

        public static float IRT_CAC_ObstructDmgMod(Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot,
            int location, float dmg, float ap, float heat, float stab)
        {
            var actor = weapon.parent;
            var lineOfFireLevel = target.Combat.FindActorByGUID(actor.GUID).VisibilityCache
                .VisibilityToTarget(target).LineOfFireLevel;
            if (lineOfFireLevel != LineOfFireLevel.LOFObstructed) return 1f;
            if (target is Mech mech)
            {
                if (Mod.Config.Combat.ObstructionTweaks.ObstructionDRByTags.Keys.Any(x =>
                    mech.MechDef.MechTags.Any(y => y == x)))
                {
                    var validLocs = Mod.Config.Combat.ObstructionTweaks.DRMechLocs;


                    if (Mod.Config.Combat.ObstructionTweaks.QuadTags.Any(x =>
                        mech.MechDef.MechTags.Any(y => y == x)))
                    {
                        Mod.Log.Debug?.Write($"Found mech Quad tag. Adding Arms to valid Locs.");
                        validLocs.Add(ArmorLocation.LeftArm);
                        validLocs.Add(ArmorLocation.RightArm);
                        Mod.Log.Debug?.Write($"Valid hit locations for damage reduction: {validLocs.ToList()}.");
                    }
                    
                    if (validLocs.Contains((ArmorLocation)location))
                    {
                        Mod.Log.Debug?.Write($"{(ArmorLocation)location} is valid hit location for damage reduction.");
                        var damageReductions = Mod.Config.Combat.ObstructionTweaks.ObstructionDRByTags
                            .Where(x => mech.MechDef.MechTags.Contains(x.Key)).Select(y => y.Value).ToList();
                        if (damageReductions.Count > 0)
                        {
                            var damageReduction = damageReductions.Min();
                            Mod.Log.Info?.Write(
                                $"Applied {damageReduction} multiplier for armor damage reduction to {(ArmorLocation)location}.");
                            return damageReduction;
                        }
                    }
                }
            }

            if (target is Vehicle vehicle)
            {
                if (Mod.Config.Combat.ObstructionTweaks.ObstructionDRByTags.Keys.Any(x =>
                    vehicle.VehicleDef.VehicleTags.Any(y => y == x)))
                {

                    if (Mod.Config.Combat.ObstructionTweaks.DRVehicleLocs.Contains((VehicleChassisLocations)location))
                    {
                        Mod.Log.Debug?.Write($"{(VehicleChassisLocations)location} is valid hit location for damage reduction.");
                        var damageReductions = Mod.Config.Combat.ObstructionTweaks.ObstructionDRByTags
                            .Where(x => vehicle.VehicleDef.VehicleTags.Contains(x.Key)).Select(y => y.Value).ToList();
                        if (damageReductions.Count > 0)
                        {
                            var damageReduction = damageReductions.Min();
                            Mod.Log.Info?.Write(
                                $"Applied {damageReduction} multiplier for armor damage reduction to {(VehicleChassisLocations)location}.");
                            return damageReduction;
                        }
                    }
                }
            }
            else if (target is Mech vmech && vmech.GetCustomInfo().FakeVehicle)
            {
                var loc = (ChassisLocations) location;
                if (Mod.Config.Combat.ObstructionTweaks.DRVehicleLocs.Contains(loc.toFakeVehicleChassis()))
                {
                    Mod.Log.Debug?.Write($"Location is int value {location}: {(VehicleChassisLocations)location} as VehicleChassisLocation; {loc.toFakeVehicleChassis()} after processing; is valid hit location for damage reduction.");
                    var damageReductions = Mod.Config.Combat.ObstructionTweaks.ObstructionDRByTags
                        .Where(x => vmech.MechDef.MechTags.Contains(x.Key)).Select(y => y.Value).ToList();

                    if (damageReductions.Count > 0)
                    {
                        var damageReduction = damageReductions.Min();
                        Mod.Log.Info?.Write(
                            $"Applied {damageReduction} multiplier for armor damage reduction to {(VehicleChassisLocations)location}.");
                        return damageReduction;
                    }
                }
            }

            return 1f;
        }

        public static string IRT_CAC_ObstructDmgModName(Weapon weapon, Vector3 attackPosition, ICombatant target,
            bool IsBreachingShot, int location, float dmg, float ap, float heat, float stab)
        {
            return "IRT_CAC_Obstruct_DmgMod_" + target.DisplayName;
        }

    }
}

#endif