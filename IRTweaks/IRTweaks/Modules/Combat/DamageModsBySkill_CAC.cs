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
using Harmony;
using static CustAmmoCategories.CustomAmmoCategories;
using UnityEngine;

namespace IRTweaks.Modules.Combat 
{ 
    public class IRT_CAC_DamageModsBySkill
    {
        [HarmonyPatch(typeof(AbstractActor), "InitEffectStats")]
        public static class AbstractActor_InitEffectStats_DamageModsBySkill
        {
            static bool Prepare() => Mod.Config.Combat.DamageModsBySkill.HeatMods.Count > 0 || Mod.Config.Combat.DamageModsBySkill.StabilityMods.Count > 0 ||Mod.Config.Combat.DamageModsBySkill.APDmgMods.Count > 0;

            static void Postfix(AbstractActor __instance)
            {
                if (Mod.Config.Combat.DamageModsBySkill.HeatMods.Count > 0)
                {
                    foreach (var heatMod in Mod.Config.Combat.DamageModsBySkill.HeatMods)
                    {
                        __instance.StatCollection.AddStatistic<bool>(heatMod.StatName, false);
                        Mod.Log.Trace?.Write($"Added heatMod stat {heatMod.StatName}.");
                    }
                }

                if (Mod.Config.Combat.DamageModsBySkill.StabilityMods.Count > 0)
                {
                    foreach (var stabMod in Mod.Config.Combat.DamageModsBySkill.StabilityMods)
                    {
                        __instance.StatCollection.AddStatistic<bool>(stabMod.StatName, false);
                        Mod.Log.Trace?.Write($"Added stabMod stat {stabMod.StatName}.");
                    }
                }

                if (Mod.Config.Combat.DamageModsBySkill.APDmgMods.Count > 0)
                {
                    foreach (var apDmgMod in Mod.Config.Combat.DamageModsBySkill.APDmgMods)
                    {
                        __instance.StatCollection.AddStatistic<bool>(apDmgMod.StatName, false);
                        Mod.Log.Trace?.Write($"Added apDmgMod stat {apDmgMod.StatName}.");
                    }
                }

                if (Mod.Config.Combat.DamageModsBySkill.StdDmgMods.Count > 0)
                {
                    foreach (var stdDmgMod in Mod.Config.Combat.DamageModsBySkill.StdDmgMods)
                    {
                        __instance.StatCollection.AddStatistic<bool>(stdDmgMod.StatName, false);
                        Mod.Log.Trace?.Write($"Added stdDmgMod stat {stdDmgMod.StatName}.");
                    }
                }
            }
        }
        public static void FinishedLoading(List<string> loadOrder) 
        {
            if (Mod.Config.Combat.DamageModsBySkill.StabilityMods.Count > 0)
            {
                DamageModifiersCache.RegisterDamageModifier("IRTweaks_SkillDamage_StabMod", "IRT_CAC_SkillDamage_StabMod", false, false, false, false, true, IRT_CAC_SkillStabDmgMod, IRT_CAC_SkillStabDmgModName);
            }
            if (Mod.Config.Combat.DamageModsBySkill.HeatMods.Count > 0)
            {
                DamageModifiersCache.RegisterDamageModifier("IRTweaks_SkillDamage_HeatMod", "IRT_CAC_SkillDamage_HeatMod", false, false, false, true, false, IRT_CAC_SkillHeatDmgMod, IRT_CAC_SkillHeatDmgModName);
            }
            if (Mod.Config.Combat.DamageModsBySkill.APDmgMods.Count > 0)
            {
                DamageModifiersCache.RegisterDamageModifier("IRTweaks_SkillDamage_APDmgMod", "IRT_CAC_SkillDamage_APDmgMod", false, false, true, false, false, IRT_CAC_SkillAPDmgMod, IRT_CAC_SkillAPDmgModName);
            }
            if (Mod.Config.Combat.DamageModsBySkill.StdDmgMods.Count > 0)
            {
                DamageModifiersCache.RegisterDamageModifier("IRTweaks_SkillDamage_StdDmgMod", "IRT_CAC_SkillDamage_StdDmgMod", false, true, false, false, false, IRT_CAC_SkillStdDmgMod, IRT_CAC_SkillStdDmgModName);
            }
        }


        public static float IRT_CAC_SkillStabDmgMod(Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot,
            int location, float dmg, float ap, float heat, float stab)
        {
            var mult = 1f;
            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill.StabilityMods)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.StatName}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.StatName)}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.StatName))
                {
                    Mod.Log.Trace?.Write($"Found StabilityMod stat, rolling vs {statmod.Probability}.");
                    if (Mod.Random.NextDouble() <= statmod.Probability)
                    {
                        Mod.Log.Trace?.Write($"Roll succeeded, multiplying {mult} by {statmod.Multiplier}.");
                        mult *= statmod.Multiplier;
                    
                    }
                }
            }
            return mult;
        }

        public static string IRT_CAC_SkillStabDmgModName(Weapon weapon, Vector3 attackPosition, ICombatant target,
            bool IsBreachingShot, int location, float dmg, float ap, float heat, float stab)
        {
            return "IRT_CAC_Obstruct_StabDmgMod_" + target.DisplayName;
        }

        public static float IRT_CAC_SkillHeatDmgMod(Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot,
            int location, float dmg, float ap, float heat, float stab)
        {
            var mult = 1f;
            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill.HeatMods)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.StatName}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.StatName)}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.StatName))
                {
                    Mod.Log.Trace?.Write($"Found HeatMod stat, rolling vs {statmod.Probability}.");
                    if (Mod.Random.NextDouble() <= statmod.Probability)
                    {
                        Mod.Log.Trace?.Write($"Roll succeeded, multiplying {mult} by {statmod.Multiplier}.");
                        mult *= statmod.Multiplier;
                    
                    }
                }
            }
            return mult;
        }

        public static string IRT_CAC_SkillHeatDmgModName(Weapon weapon, Vector3 attackPosition, ICombatant target,
            bool IsBreachingShot, int location, float dmg, float ap, float heat, float stab)
        {
            return "IRT_CAC_Obstruct_HeatDmgMod_" + target.DisplayName;
        }

        public static float IRT_CAC_SkillAPDmgMod(Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot,
            int location, float dmg, float ap, float heat, float stab)
        {
            var mult = 1f;
            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill.APDmgMods)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.StatName}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.StatName)}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.StatName))
                {
                    Mod.Log.Trace?.Write($"Found APDmgMod stat, rolling vs {statmod.Probability}.");
                    if (Mod.Random.NextDouble() <= statmod.Probability)
                    {
                        Mod.Log.Trace?.Write($"Roll succeeded, multiplying {mult} by {statmod.Multiplier}.");
                        mult *= statmod.Multiplier;
                    
                    }
                }
            }
            return mult;
        }

        public static string IRT_CAC_SkillStdDmgModName(Weapon weapon, Vector3 attackPosition, ICombatant target,
            bool IsBreachingShot, int location, float dmg, float ap, float heat, float stab)
        {
            return "IRT_CAC_Obstruct_StdDmgMod_" + target.DisplayName;
        }


        public static float IRT_CAC_SkillStdDmgMod(Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot,
            int location, float dmg, float ap, float heat, float stab)
        {
            var mult = 1f;
            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill.StdDmgMods)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.StatName}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.StatName)}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.StatName))
                {
                    Mod.Log.Trace?.Write($"Found StdDmgMod stat, rolling vs {statmod.Probability}.");
                    if (Mod.Random.NextDouble() <= statmod.Probability)
                    {
                        Mod.Log.Trace?.Write($"Roll succeeded, multiplying {mult} by {statmod.Multiplier}.");
                        mult *= statmod.Multiplier;
                    
                    }
                }
            }
            return mult;
        }

        public static string IRT_CAC_SkillAPDmgModName(Weapon weapon, Vector3 attackPosition, ICombatant target,
            bool IsBreachingShot, int location, float dmg, float ap, float heat, float stab)
        {
            return "IRT_CAC_Obstruct_APDmgMod_" + target.DisplayName;
        }
    }
}