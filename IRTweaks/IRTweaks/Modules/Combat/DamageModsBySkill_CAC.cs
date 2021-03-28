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
            static bool Prepare() => Mod.Config.Combat.DamageModsBySkill.HeatMod.Count > 0 || Mod.Config.Combat.DamageModsBySkill.StabilityMod.Count > 0 ||Mod.Config.Combat.DamageModsBySkill.APDmgMod.Count > 0;

            static void Postfix(AbstractActor __instance)
            {
                if (Mod.Config.Combat.DamageModsBySkill.HeatMod.Count > 0)
                {
                    foreach (var heatMod in Mod.Config.Combat.DamageModsBySkill.HeatMod)
                    {
                        __instance.StatCollection.AddStatistic<bool>(heatMod.Item1, false);
                        Mod.Log.Trace?.Write($"Added heatMod stat {heatMod.Item1}.");
                    }
                }

                if (Mod.Config.Combat.DamageModsBySkill.StabilityMod.Count > 0)
                {
                    foreach (var stabMod in Mod.Config.Combat.DamageModsBySkill.StabilityMod)
                    {
                        __instance.StatCollection.AddStatistic<bool>(stabMod.Item1, false);
                        Mod.Log.Trace?.Write($"Added stabMod stat {stabMod.Item1}.");
                    }
                }

                if (Mod.Config.Combat.DamageModsBySkill.APDmgMod.Count > 0)
                {
                    foreach (var apDmgMod in Mod.Config.Combat.DamageModsBySkill.APDmgMod)
                    {
                        __instance.StatCollection.AddStatistic<bool>(apDmgMod.Item1, false);
                        Mod.Log.Trace?.Write($"Added apDmgMod stat {apDmgMod.Item1}.");
                    }
                }

                if (Mod.Config.Combat.DamageModsBySkill.StdDmgMod.Count > 0)
                {
                    foreach (var stdDmgMod in Mod.Config.Combat.DamageModsBySkill.StdDmgMod)
                    {
                        __instance.StatCollection.AddStatistic<bool>(stdDmgMod.Item1, false);
                        Mod.Log.Trace?.Write($"Added stdDmgMod stat {stdDmgMod.Item1}.");
                    }
                }
            }
        }
        public static void FinishedLoading(List<string> loadOrder) 
        {
            if (Mod.Config.Combat.DamageModsBySkill.StabilityMod.Count > 0)
            {
                DamageModifiersCache.RegisterDamageModifier("IRTweaks_SkillDamage_StabMod", "IRT_CAC_SkillDamage_StabMod", false, false, false, false, true, IRT_CAC_SkillStabDmgMod, IRT_CAC_SkillStabDmgModName);
            }
            if (Mod.Config.Combat.DamageModsBySkill.HeatMod.Count > 0)
            {
                DamageModifiersCache.RegisterDamageModifier("IRTweaks_SkillDamage_HeatMod", "IRT_CAC_SkillDamage_HeatMod", false, false, false, true, false, IRT_CAC_SkillHeatDmgMod, IRT_CAC_SkillHeatDmgModName);
            }
            if (Mod.Config.Combat.DamageModsBySkill.APDmgMod.Count > 0)
            {
                DamageModifiersCache.RegisterDamageModifier("IRTweaks_SkillDamage_APDmgMod", "IRT_CAC_SkillDamage_APDmgMod", false, false, true, false, false, IRT_CAC_SkillAPDmgMod, IRT_CAC_SkillAPDmgModName);
            }
            if (Mod.Config.Combat.DamageModsBySkill.StdDmgMod.Count > 0)
            {
                DamageModifiersCache.RegisterDamageModifier("IRTweaks_SkillDamage_StdDmgMod", "IRT_CAC_SkillDamage_StdDmgMod", false, true, false, false, false, IRT_CAC_SkillStdDmgMod, IRT_CAC_SkillStdDmgModName);
            }
        }


        public static float IRT_CAC_SkillStabDmgMod(Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot,
            int location, float dmg, float ap, float heat, float stab)
        {
            var mult = 1f;
            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill.StabilityMod)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.Item1}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.Item1)}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.Item1))
                {
                    Mod.Log.Trace?.Write($"Found StabilityMod stat, rolling vs {statmod.Item2}.");
                    if (Mod.Random.NextDouble() <= statmod.Item2)
                    {
                        Mod.Log.Trace?.Write($"Roll succeeded, multiplying {mult} by {statmod.Item3}.");
                        mult *= statmod.Item3;
                    
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
            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill.HeatMod)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.Item1}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.Item1)}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.Item1))
                {
                    Mod.Log.Trace?.Write($"Found HeatMod stat, rolling vs {statmod.Item2}.");
                    if (Mod.Random.NextDouble() <= statmod.Item2)
                    {
                        Mod.Log.Trace?.Write($"Roll succeeded, multiplying {mult} by {statmod.Item3}.");
                        mult *= statmod.Item3;
                    
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
            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill.APDmgMod)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.Item1}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.Item1)}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.Item1))
                {
                    Mod.Log.Trace?.Write($"Found APDmgMod stat, rolling vs {statmod.Item2}.");
                    if (Mod.Random.NextDouble() <= statmod.Item2)
                    {
                        Mod.Log.Trace?.Write($"Roll succeeded, multiplying {mult} by {statmod.Item3}.");
                        mult *= statmod.Item3;
                    
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
            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill.StdDmgMod)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.Item1}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.Item1)}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.Item1))
                {
                    Mod.Log.Trace?.Write($"Found StdDmgMod stat, rolling vs {statmod.Item2}.");
                    if (Mod.Random.NextDouble() <= statmod.Item2)
                    {
                        Mod.Log.Trace?.Write($"Roll succeeded, multiplying {mult} by {statmod.Item3}.");
                        mult *= statmod.Item3;
                    
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