#if NO_CAC
#else

using BattleTech;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustAmmoCategories;
using Harmony;
using Localize;
using RootMotion.FinalIK;
using static CustAmmoCategories.CustomAmmoCategories;

using UnityEngine;

namespace IRTweaks.Modules.Combat 
{ 
    public class CAC_API_ShotMods
    {
        [HarmonyPatch(typeof(Mech), "TakeWeaponDamage")] 
        public static class Mech_TakeWeaponDamage_DamageModsBySkill
        {
            static bool Prepare() => Mod.Config.Combat.DamageModsBySkill.DisplayFloatiesOnTrigger && (Mod.Config.Combat.DamageModsBySkill?.HeatMods.Count > 0 ||
                                      Mod.Config.Combat.DamageModsBySkill?.StabilityMods.Count > 0 ||
                                      Mod.Config.Combat.DamageModsBySkill.APDmgMods.Count > 0);

            static void Postfix(Mech __instance, WeaponHitInfo hitInfo, int hitLocation, Weapon weapon, float damageAmount, float directStructureDamage, int hitIndex, DamageType damageType)
            {
                if (damageType == DamageType.DFASelf)

                    Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] Processing dmg floaties for {weapon.Name} uid {weapon.uid}");
                if (ModState.StabDmgMods.ContainsKey(hitInfo.attackerId))
                {
                    Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] containskey true. count: {ModState.StabDmgMods[hitInfo.attackerId].Count}, matching guid? {ModState.StabDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid)}.");
                }
                if (ModState.StabDmgMods.ContainsKey(hitInfo.attackerId) && ModState.StabDmgMods[hitInfo.attackerId].Count > 0 && ModState.StabDmgMods[hitInfo.attackerId].Any(x=>x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.StabDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");
                    
                    var idx = 0;
                    for (int i = 0; i < ModState.StabDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.StabDmgMods[hitInfo.attackerId][i]?.Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.Stability > 0 && ModState.StabDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.StabDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.StabDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] Removing idx {idx} from list: {ModState.StabDmgMods[hitInfo.attackerId][idx].Type} {ModState.StabDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.StabDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                if (ModState.HeatDmgMods.ContainsKey(hitInfo.attackerId) && ModState.HeatDmgMods[hitInfo.attackerId].Count > 0 && ModState.HeatDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.HeatDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.HeatDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.HeatDmgMods[hitInfo.attackerId][i]?.Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.Heat > 0 && ModState.HeatDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.HeatDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.HeatDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] Removing idx {idx} from list: {ModState.HeatDmgMods[hitInfo.attackerId][idx].Type} {ModState.HeatDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.HeatDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                if (ModState.APDmgMods.ContainsKey(hitInfo.attackerId) && ModState.APDmgMods[hitInfo.attackerId].Count > 0 && ModState.APDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.APDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.APDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.APDmgMods[hitInfo.attackerId][i]?.Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.cumulativeDamage > 0 && ModState.APDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.APDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.APDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] Removing idx {idx} from list: {ModState.APDmgMods[hitInfo.attackerId][idx].Type} {ModState.APDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.APDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                if (ModState.StdDmgMods.ContainsKey(hitInfo.attackerId) && ModState.StdDmgMods[hitInfo.attackerId].Count > 0 && ModState.StdDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.StdDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.StdDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.StdDmgMods[hitInfo.attackerId][i]?.Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.cumulativeDamage > 0 && ModState.StdDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.StdDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.StdDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Mech.TakeWeaponDamage] Removing idx {idx} from list: {ModState.StdDmgMods[hitInfo.attackerId][idx].Type} {ModState.StdDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.StdDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }
            }
        }

        [HarmonyPatch(typeof(Vehicle), "TakeWeaponDamage")]
        public static class Vehicle_TakeWeaponDamage_DamageModsBySkill
        {
            static bool Prepare() => Mod.Config.Combat.DamageModsBySkill.DisplayFloatiesOnTrigger && (Mod.Config.Combat.DamageModsBySkill?.HeatMods.Count > 0 ||
                Mod.Config.Combat.DamageModsBySkill?.StabilityMods.Count > 0 ||
                Mod.Config.Combat.DamageModsBySkill.APDmgMods.Count > 0);

            static void Postfix(Vehicle __instance, WeaponHitInfo hitInfo, int hitLocation, Weapon weapon, float damageAmount, float directStructureDamage, int hitIndex, DamageType damageType)
            {
                Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] Processing dmg floaties for {weapon.Name}, uid {weapon.uid}. AttackerID: {hitInfo.attackerId}. {string.Join("; ", ModState.StabDmgMods.Keys)}");

                if (ModState.StabDmgMods.ContainsKey(hitInfo.attackerId))
                {
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] containskey true. count: {ModState.StabDmgMods[hitInfo.attackerId].Count}, matching guid? {ModState.StabDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid)}.");
                }

                if (ModState.StabDmgMods.ContainsKey(hitInfo.attackerId) && ModState.StabDmgMods[hitInfo.attackerId].Count > 0 && ModState.StabDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.StabDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.StabDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.StabDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.Stability > 0 && ModState.StabDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.StabDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.StabDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] Removing idx {idx} from list: {ModState.StabDmgMods[hitInfo.attackerId][idx].Type} {ModState.StabDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.StabDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] Processing dmg floaties for {weapon.Name}, uid {weapon.uid}. AttackerID: {hitInfo.attackerId}. {string.Join("; ", ModState.HeatDmgMods.Keys)}");

                if (ModState.HeatDmgMods.ContainsKey(hitInfo.attackerId))
                {
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] containskey true. count: {ModState.HeatDmgMods[hitInfo.attackerId].Count}, matching guid? {ModState.HeatDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid)}.");
                }

                if (ModState.HeatDmgMods.ContainsKey(hitInfo.attackerId) && ModState.HeatDmgMods[hitInfo.attackerId].Count > 0 && ModState.HeatDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.HeatDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.HeatDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.HeatDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.Heat > 0 && ModState.HeatDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.HeatDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.HeatDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] Removing idx {idx} from list: {ModState.HeatDmgMods[hitInfo.attackerId][idx].Type} {ModState.HeatDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.HeatDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] Processing dmg floaties for {weapon.Name}, uid {weapon.uid}. AttackerID: {hitInfo.attackerId}. {string.Join("; ", ModState.APDmgMods.Keys)}");

                if (ModState.APDmgMods.ContainsKey(hitInfo.attackerId))
                {
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] containskey true. count: {ModState.APDmgMods[hitInfo.attackerId].Count}, matching guid? {ModState.APDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid)}.");
                }

                if (ModState.APDmgMods.ContainsKey(hitInfo.attackerId) && ModState.APDmgMods[hitInfo.attackerId].Count > 0 && ModState.APDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.APDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.APDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.APDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.cumulativeDamage > 0 && ModState.APDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.APDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.APDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] Removing idx {idx} from list: {ModState.APDmgMods[hitInfo.attackerId][idx].Type} {ModState.APDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.APDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] Processing dmg floaties for {weapon.Name}, uid {weapon.uid}. AttackerID: {hitInfo.attackerId}. {string.Join("; ", ModState.StdDmgMods.Keys)}");

                if (ModState.StdDmgMods.ContainsKey(hitInfo.attackerId))
                {
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] containskey true. count: {ModState.StdDmgMods[hitInfo.attackerId].Count}, matching guid? {ModState.StdDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid)}.");
                }

                if (ModState.StdDmgMods.ContainsKey(hitInfo.attackerId) && ModState.StdDmgMods[hitInfo.attackerId].Count > 0 && ModState.StdDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.StdDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.StdDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.StdDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.cumulativeDamage > 0 && ModState.StdDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.StdDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.StdDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Vehicle.TakeWeaponDamage] Removing idx {idx} from list: {ModState.StdDmgMods[hitInfo.attackerId][idx].Type} {ModState.StdDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.StdDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }
            }
        }

        [HarmonyPatch(typeof(Turret), "TakeWeaponDamage")]
        public static class Turret_TakeWeaponDamage_DamageModsBySkill
        {
            static bool Prepare() => Mod.Config.Combat.DamageModsBySkill.DisplayFloatiesOnTrigger && (Mod.Config.Combat.DamageModsBySkill?.HeatMods.Count > 0 ||
                Mod.Config.Combat.DamageModsBySkill?.StabilityMods.Count > 0 ||
                Mod.Config.Combat.DamageModsBySkill.APDmgMods.Count > 0);

            static void Postfix(Turret __instance, WeaponHitInfo hitInfo, int hitLocation, Weapon weapon, float damageAmount, float directStructureDamage, int hitIndex, DamageType damageType)
            {
                Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] Processing dmg floaties for {weapon.Name} uid {weapon.uid}");
                if (ModState.StabDmgMods.ContainsKey(hitInfo.attackerId))
                {
                    Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] containskey true. count: {ModState.StabDmgMods[hitInfo.attackerId].Count}, matching guid? {ModState.StabDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid)}.");
                }
                if (ModState.StabDmgMods.ContainsKey(hitInfo.attackerId) && ModState.StabDmgMods[hitInfo.attackerId].Count > 0 && ModState.StabDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.StabDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.StabDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.StabDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.Stability > 0 && ModState.StabDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.StabDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.StabDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] Removing idx {idx} from list: {ModState.StabDmgMods[hitInfo.attackerId][idx].Type} {ModState.StabDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.StabDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                if (ModState.HeatDmgMods.ContainsKey(hitInfo.attackerId) && ModState.HeatDmgMods[hitInfo.attackerId].Count > 0 && ModState.HeatDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.HeatDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.HeatDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.HeatDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.Heat > 0 && ModState.HeatDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.HeatDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.HeatDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] Removing idx {idx} from list: {ModState.HeatDmgMods[hitInfo.attackerId][idx].Type} {ModState.HeatDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.HeatDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                if (ModState.APDmgMods.ContainsKey(hitInfo.attackerId) && ModState.APDmgMods[hitInfo.attackerId].Count > 0 && ModState.APDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.APDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.APDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.APDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.cumulativeDamage > 0 && ModState.APDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.APDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.APDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] Removing idx {idx} from list: {ModState.APDmgMods[hitInfo.attackerId][idx].Type} {ModState.APDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.APDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                if (ModState.StdDmgMods.ContainsKey(hitInfo.attackerId) && ModState.StdDmgMods[hitInfo.attackerId].Count > 0 && ModState.StdDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.StdDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.StdDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.StdDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.cumulativeDamage > 0 && ModState.StdDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.StdDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.StdDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Turret.TakeWeaponDamage] Removing idx {idx} from list: {ModState.StdDmgMods[hitInfo.attackerId][idx].Type} {ModState.StdDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.StdDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }
            }
        }


        [HarmonyPatch(typeof(BattleTech.Building), "TakeWeaponDamage")]
        public static class Building_TakeWeaponDamage_DamageModsBySkill
        {
            static bool Prepare() => Mod.Config.Combat.DamageModsBySkill.DisplayFloatiesOnTrigger && (Mod.Config.Combat.DamageModsBySkill?.HeatMods.Count > 0 ||
                Mod.Config.Combat.DamageModsBySkill?.StabilityMods.Count > 0 ||
                Mod.Config.Combat.DamageModsBySkill.APDmgMods.Count > 0);

            static void Postfix(BattleTech.Building __instance, WeaponHitInfo hitInfo, int hitLocation, Weapon weapon, float damageAmount, float directStructureDamage, int hitIndex, DamageType damageType)
            {
                Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] Processing dmg floaties for {weapon.Name} uid {weapon.uid}");
                if (ModState.StabDmgMods.ContainsKey(hitInfo.attackerId))
                {
                    Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] containskey true. count: {ModState.StabDmgMods[hitInfo.attackerId].Count}, matching guid? {ModState.StabDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid)}.");
                }
                if (ModState.StabDmgMods.ContainsKey(hitInfo.attackerId) && ModState.StabDmgMods[hitInfo.attackerId].Count > 0 && ModState.StabDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.StabDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.StabDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.StabDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.Stability > 0 && ModState.StabDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.StabDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.StabDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] Removing idx {idx} from list: {ModState.StabDmgMods[hitInfo.attackerId][idx].Type} {ModState.StabDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.StabDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                if (ModState.HeatDmgMods.ContainsKey(hitInfo.attackerId) && ModState.HeatDmgMods[hitInfo.attackerId].Count > 0 && ModState.HeatDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.HeatDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.HeatDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.HeatDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.Heat > 0 && ModState.HeatDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.HeatDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.HeatDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] Removing idx {idx} from list: {ModState.HeatDmgMods[hitInfo.attackerId][idx].Type} {ModState.HeatDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.HeatDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                if (ModState.APDmgMods.ContainsKey(hitInfo.attackerId) && ModState.APDmgMods[hitInfo.attackerId].Count > 0 && ModState.APDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.APDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.APDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.APDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.cumulativeDamage > 0 && ModState.APDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.APDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.APDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] Removing idx {idx} from list: {ModState.APDmgMods[hitInfo.attackerId][idx].Type} {ModState.APDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.APDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }

                if (ModState.StdDmgMods.ContainsKey(hitInfo.attackerId) && ModState.StdDmgMods[hitInfo.attackerId].Count > 0 && ModState.StdDmgMods[hitInfo.attackerId].Any(x => x?.WeaponGUID == weapon.uid))
                {
                    var list = ModState.StdDmgMods[hitInfo.attackerId];
                    Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] OUTPOT LIST #: {list.Count}");

                    var idx = 0;
                    for (int i = 0; i < ModState.StdDmgMods[hitInfo.attackerId].Count; i++)
                    {
                        if (ModState.StdDmgMods[hitInfo.attackerId][i].Mult > 1 && hitInfo.advInfo()?.resolve(__instance)?.cumulativeDamage > 0 && ModState.StdDmgMods[hitInfo.attackerId][i]?.WeaponGUID == weapon.uid)
                        {
                            var attacker = __instance.Combat.FindActorByGUID(hitInfo.attackerId);
                            var txt = ModState.StdDmgMods[hitInfo.attackerId][i].Txt;
                            __instance.Combat.MessageCenter.PublishMessage(
                                new AddSequenceToStackMessage(new ShowActorInfoSequence(attacker,
                                    txt,
                                    FloatieMessage.MessageNature.Buff, false)));
                            Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] HERE WE SHOULD SEE FLOATIE for {weapon.Name} with GUID {weapon.uid}, stored GUID was {ModState.StdDmgMods[hitInfo.attackerId][i]?.WeaponGUID}. AGI:{hitInfo.attackGroupIndex}, AWI:{hitInfo.attackWeaponIndex}");
                            idx = i;
                            break;
                        }
                    }
                    Mod.Log.Trace?.Write($"[Building.TakeWeaponDamage] Removing idx {idx} from list: {ModState.StdDmgMods[hitInfo.attackerId][idx].Type} {ModState.StdDmgMods[hitInfo.attackerId][idx].Txt}");
                    ModState.StdDmgMods[hitInfo.attackerId].RemoveAt(idx);
                }
            }
        }

        [HarmonyPatch(typeof(AbstractActor), "OnActivationEnd")]
        public static class AbstractActor_OnActivationEnd
        {
            static bool Prepare() => Mod.Config.Combat.DamageModsBySkill.DisplayFloatiesOnTrigger && (Mod.Config.Combat.DamageModsBySkill?.HeatMods.Count > 0 ||
                Mod.Config.Combat.DamageModsBySkill?.StabilityMods.Count > 0 ||
                Mod.Config.Combat.DamageModsBySkill.APDmgMods.Count > 0 || Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownBracedFactor > 0f);

            static void Prefix(AbstractActor __instance, string sourceID, int stackItemID)
            {
                if (Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownBracedFactor > 0f)
                {
                    if (!__instance.BracedLastRound)
                    {
                        if (ModState.DidActorBraceLastRoundBeforeFiring.ContainsKey(__instance.GUID))
                        {
                            ModState.DidActorBraceLastRoundBeforeFiring.Remove(__instance.GUID);
                            Mod.Log.Trace?.Write($"[AbstractActor.OnActivationEnd] {__instance.DisplayName} is not BracedLastRound, removing DidActorBraceLastRoundBeforeFiring state");
                        }
                    }
                }

                if (ModState.StabDmgMods.ContainsKey(__instance.GUID))
                {
                    ModState.StabDmgMods.Remove(__instance.GUID);
                    Mod.Log.Trace?.Write($"[AbstractActor.OnActivationEnd] End of turn StabDmgMods cleanup for {__instance.DisplayName} - {__instance.GUID}");
                }
                if (ModState.HeatDmgMods.ContainsKey(__instance.GUID))
                {
                    ModState.HeatDmgMods.Remove(__instance.GUID);
                    Mod.Log.Trace?.Write($"[AbstractActor.OnActivationEnd] End of turn HeatDmgMods cleanup for {__instance.DisplayName} - {__instance.GUID}");
                }
                if (ModState.APDmgMods.ContainsKey(__instance.GUID))
                {
                    ModState.APDmgMods.Remove(__instance.GUID);
                    Mod.Log.Trace?.Write($"[AbstractActor.OnActivationEnd] End of turn APDmgMods cleanup for {__instance.DisplayName} - {__instance.GUID}");
                }
                if (ModState.StdDmgMods.ContainsKey(__instance.GUID))
                {
                    ModState.StdDmgMods.Remove(__instance.GUID);
                    Mod.Log.Trace?.Write($"[AbstractActor.OnActivationEnd] End of turn StdDmgMods cleanup for {__instance.DisplayName} - {__instance.GUID}");
                }
            }
        }

        [HarmonyPatch(typeof(Weapon), "InitStats")]
        public static class Weapon_InitStats_Patch
        {
            static bool Prepare() => Mod.Config.Combat.ToHitStatMods.WeaponToHitMods.Count > 0;

            static void Postfix(Weapon __instance)
            {
                foreach (var weaponToHitMod in Mod.Config.Combat.ToHitStatMods.WeaponToHitMods)
                {
                    __instance.StatCollection.AddStatistic<float>(weaponToHitMod.SourceStatName, 0);
                    Mod.Log.Trace?.Write($"Added weaponToHitMod stat {weaponToHitMod.SourceStatName} at value 0.");
                    if (__instance.componentDef.statusEffects.FirstOrDefault(x =>
                            x.effectType == EffectType.StatisticEffect &&
                            x.statisticData.statName == weaponToHitMod.SourceStatName) is EffectData effect)
                    {
                        var parsed = float.Parse(effect.statisticData.modValue, CultureInfo.InvariantCulture);
                        __instance.StatCollection.ModifyStat("IRT_THM", -1, weaponToHitMod.SourceStatName,StatCollection.StatOperation.Set, parsed);
                        Mod.Log.Trace?.Write($"weaponToHitMod stat {weaponToHitMod.SourceStatName} parsed and changed to {parsed}.");
                    }
                }
            }
        }

        [HarmonyPatch(typeof(AbstractActor), "InitEffectStats")]
        public static class AbstractActor_InitEffectStats_DamageModsBySkill
        {
            static bool Prepare() => Mod.Config.Combat.ToHitStatMods.ActorToHitMods.Count > 0 || Mod.Config.Combat.DamageModsBySkill.HeatMods.Count > 0 || Mod.Config.Combat.DamageModsBySkill.StabilityMods.Count > 0 ||Mod.Config.Combat.DamageModsBySkill.APDmgMods.Count > 0 || Mod.Config.Combat.DamageModsBySkill.StdDmgMods.Count > 0 || !string.IsNullOrEmpty(Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownCheckStatName) || !string.IsNullOrEmpty(Mod.Config.Combat.OnWeaponHitOpts.ForceShutdownOnHitStat);

            static void Postfix(AbstractActor __instance)
            {
                if (!string.IsNullOrEmpty(Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownCheckStatName))
                {
                    __instance.StatCollection.AddStatistic<float>(
                        Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownCheckStatName, 0f);
                    Mod.Log.Trace?.Write($"Added SelfKnockdownCheckEffectID source stat {Mod.Config.Combat.OnWeaponFireOpts.SelfKnockdownCheckStatName} at value 0f.");
                }

                if (!string.IsNullOrEmpty(Mod.Config.Combat.OnWeaponHitOpts.ForceShutdownOnHitStat))
                {
                    __instance.StatCollection.AddStatistic<float>(
                        Mod.Config.Combat.OnWeaponHitOpts.ForceShutdownOnHitStat, 0f);
                    Mod.Log.Trace?.Write($"Added ForceShutdownOnHitStat source stat {Mod.Config.Combat.OnWeaponHitOpts.ForceShutdownOnHitStat} at value 0f.");
                }

                if (Mod.Config.Combat.ToHitStatMods.ActorToHitMods.Count > 0)
                {
                    foreach (var toHitMod in Mod.Config.Combat.ToHitStatMods.ActorToHitMods)
                    {
                        __instance.StatCollection.AddStatistic<float>(toHitMod.SourceStatName, 0);
                        Mod.Log.Trace?.Write($"Added toHitMod source stat {toHitMod.SourceStatName} at value 0.");
                        __instance.StatCollection.AddStatistic<bool>(toHitMod.TargetStatName, false);
                        Mod.Log.Trace?.Write($"Added toHitMod target stat {toHitMod.TargetStatName} at value false.");
                    }
                }

                if (Mod.Config.Combat.DamageModsBySkill?.HeatMods.Count > 0)
                {
                    foreach (var heatMod in Mod.Config.Combat.DamageModsBySkill?.HeatMods)
                    {
                        __instance.StatCollection.AddStatistic<bool>(heatMod.StatName, false);
                        Mod.Log.Trace?.Write($"Added heatMod stat {heatMod.StatName}.");
                    }
                }

                if (Mod.Config.Combat.DamageModsBySkill?.StabilityMods.Count > 0)
                {
                    foreach (var stabMod in Mod.Config.Combat.DamageModsBySkill?.StabilityMods)
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
            if (Mod.Config.Combat.ToHitStatMods.ActorToHitMods.Count > 0)
            {
                ToHitModifiersHelper.registerModifier("IRTweaks_ToHitMod", "CAC Accuracy Modifier", true, false, IRT_CAC_ToHitMod, IRT_CAC_ToHitModName);
            }

            if (Mod.Config.Combat.DamageModsBySkill?.StabilityMods.Count > 0)
            {
                DamageModifiersCache.RegisterDamageModifier("IRTweaks_SkillDamage_StabMod", "IRT_CAC_SkillDamage_StabMod", false, false, false, false, true, IRT_CAC_SkillStabDmgMod, IRT_CAC_SkillStabDmgModName);
            }
            if (Mod.Config.Combat.DamageModsBySkill?.HeatMods.Count > 0)
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

        public static float IRT_CAC_ToHitMod(ToHit toHit, AbstractActor attacker, Weapon weapon, ICombatant target, Vector3 attackPos, Vector3 targetPos, LineOfFireLevel lof, MeleeAttackType meleType, bool isCalled)
        {
            var mod = 0f;

            var defenseModTotal = 0f;
            var evasiveModTotal = 0f;
            var currentDefense = toHit.GetEnemyEffectModifier(target, weapon);
            var currentEvasive = toHit.GetTargetSpeedModifier(target, weapon);
            foreach (var weaponToHitMod in Mod.Config.Combat.ToHitStatMods.WeaponToHitMods)
            {
                Mod.Log.Trace?.Write($"Checking for source stat {weaponToHitMod.SourceStatName}; result: {weapon.StatCollection.ContainsStatistic(weaponToHitMod.SourceStatName)}.");
                var weaponStatVal = weapon.StatCollection.GetValue<float>(weaponToHitMod.SourceStatName);
                if (weaponStatVal != 0)
                {
                    Mod.Log.Trace?.Write($"Checking for target stat {weaponToHitMod.TargetStatName}; result: {weapon.StatCollection.ContainsStatistic(weaponToHitMod.TargetStatName)}.");
                    if (target.StatCollection.GetValue<bool>(weaponToHitMod.TargetStatName)) // like IsAerialUnit or whatever
                    {
                        Mod.Log.Info?.Write($"found stat {weaponToHitMod.TargetStatName} on target, processing weapon statvalue {weaponStatVal}");
                        if (weaponToHitMod.Multi)
                        {
                            if (weaponToHitMod.Type == "ABSOLUTE")
                            {
                                Mod.Log.Error?.Write($"ERROR: toHitMod.Multi on stat {weaponToHitMod.SourceStatName} not allowed with toHitMod.Type 'ABSOLUTE'. Doing nothing!");
                            }
                            else if (weaponToHitMod.Type == "DEFENSE")
                            {
                                var defenseMod = currentDefense * weaponStatVal;
                                defenseModTotal += defenseMod;
                                mod += defenseMod;
                                Mod.Log.Trace?.Write($"found stat {weaponToHitMod.TargetStatName} on target {target?.DisplayName}, processing source weaponstat {attacker?.DisplayName} {weapon?.UIName} statvalue {weaponStatVal}. defenseMod {defenseMod}, total mod {mod}");
                            }
                            else if (weaponToHitMod.Type == "EVASIVE")
                            {
                                
                                var evasiveMod = currentEvasive * weaponStatVal;
                                evasiveModTotal += evasiveMod;
                                mod += evasiveMod;
                                Mod.Log.Trace?.Write($"found stat {weaponToHitMod.TargetStatName} on target {target?.DisplayName}, processing source weaponstat {attacker?.DisplayName} {weapon?.UIName} statvalue {weaponStatVal}. evasiveMod {evasiveMod}, total mod {mod}");
                            }
                        }
                        else if (!weaponToHitMod.Multi)
                        {
                            if (weaponToHitMod.Type == "ABSOLUTE")
                            {
                                mod += weaponStatVal;
                                Mod.Log.Trace?.Write($"found stat {weaponToHitMod.TargetStatName} on target {target?.DisplayName}, processing source weaponstat {attacker?.DisplayName} {weapon?.UIName} statvalue {weaponStatVal}. absolute mod {weaponStatVal}, total mod {mod}");
                            }
                            else if (weaponToHitMod.Type == "DEFENSE")
                            {
                                //var defenseNet = currentDefense + weaponStatVal;
                                //var finalVal = weaponStatVal;
                                //if (defenseNet <= 0) finalVal = 0;
                                defenseModTotal += weaponStatVal;
                                mod += weaponStatVal;
                                Mod.Log.Trace?.Write($"found stat {weaponToHitMod.TargetStatName} on target {target?.DisplayName}, processing source weaponstat {attacker?.DisplayName} {weapon?.UIName} statvalue {weaponStatVal}. defenseMod {weaponStatVal}, total mod {mod}");
                            }
                            else if (weaponToHitMod.Type == "EVASIVE")
                            {
                                //var evasiveNet = currentEvasive + weaponStatVal;
                                //var finalVal = weaponStatVal;
                                //if (evasiveNet <= 0) finalVal = 0;
                                evasiveModTotal += weaponStatVal;
                                mod += weaponStatVal;
                                Mod.Log.Trace?.Write($"found stat {weaponToHitMod.TargetStatName} on target {target?.DisplayName}, processing source weaponstat {attacker?.DisplayName} {weapon?.UIName} statvalue {weaponStatVal}. evasiveMod {weaponStatVal}, total mod {mod}");
                            }
                        }
                    }
                }
            }

            foreach (var toHitMod in Mod.Config.Combat.ToHitStatMods.ActorToHitMods)
            {
                Mod.Log.Trace?.Write($"Checking for source stat {toHitMod.SourceStatName}; result: {attacker.StatCollection.ContainsStatistic(toHitMod.SourceStatName)}.");
                var statVal = attacker.StatCollection.GetValue<float>(toHitMod.SourceStatName);
                if (statVal != 0)
                {
                    Mod.Log.Trace?.Write($"Checking for target stat {toHitMod.TargetStatName}; result: {attacker.StatCollection.ContainsStatistic(toHitMod.TargetStatName)}.");
                    if (target.StatCollection.GetValue<bool>(toHitMod.TargetStatName)) // like IsAerialUnit or whatever
                    {
                        if (toHitMod.Multi)
                        {
                            if (toHitMod.Type == "ABSOLUTE")
                            {
                                Mod.Log.Error?.Write($"ERROR: toHitMod.Multi on stat {toHitMod.SourceStatName} not allowed with toHitMod.Type 'ABSOLUTE'. Doing nothing!");
                            }
                            else if (toHitMod.Type == "DEFENSE")
                            {
                                var defenseMod = currentDefense * statVal;
                                defenseModTotal += defenseMod;
                                mod += defenseMod;
                                Mod.Log.Trace?.Write($"found stat {toHitMod.TargetStatName} on target {target?.DisplayName}, processing source {attacker?.DisplayName} statvalue {statVal}. defenseMod {defenseMod}, total mod {mod}");
                            }
                            else if (toHitMod.Type == "EVASIVE")
                            {
                                var evasiveMod = currentEvasive * statVal;
                                evasiveModTotal += evasiveMod;
                                mod += evasiveMod;
                                Mod.Log.Trace?.Write($"found stat {toHitMod.TargetStatName} on target {target?.DisplayName}, processing source {attacker?.DisplayName} statvalue {statVal}. evasiveMod {evasiveMod}, total mod {mod}");
                            }
                        }
                        else if (!toHitMod.Multi)
                        {
                            if (toHitMod.Type == "ABSOLUTE")
                            {
                                mod += statVal;
                                Mod.Log.Trace?.Write($"found stat {toHitMod.TargetStatName} on target {target?.DisplayName}, processing source {attacker?.DisplayName} statvalue {statVal}. absolute mod {statVal}, total mod {mod}");
                            }
                            else if (toHitMod.Type == "DEFENSE")
                            {
                                //var defenseNet = currentDefense + statVal;
                                //var finalVal = statVal;
                                //if (defenseNet <= 0) finalVal = 0;
                                defenseModTotal += statVal;
                                mod += statVal;
                                Mod.Log.Trace?.Write($"found stat {toHitMod.TargetStatName} on target {target?.DisplayName}, processing source {attacker?.DisplayName} statvalue {statVal}. defenseMod {statVal}, total mod {mod}");
                            }
                            else if (toHitMod.Type == "EVASIVE")
                            {
                                //var evasiveNet = currentEvasive + statVal;
                                //var finalVal = statVal;
                                //if (evasiveNet <= 0) finalVal = 0;
                                evasiveModTotal += statVal;
                                mod += statVal;
                                Mod.Log.Trace?.Write($"found stat {toHitMod.TargetStatName} on target {target?.DisplayName}, processing source {attacker?.DisplayName} statvalue {statVal}. evasiveMod {statVal}, total mod {mod}");
                            }
                        }
                    }
                }
            }

            var defenseNetFinal = currentDefense + defenseModTotal;
            Mod.Log.Trace?.Write($"Final mod summary: defenseNetFinal {defenseNetFinal}, mod {mod}");
            if (defenseNetFinal < 0) mod -= defenseNetFinal;
            var evasiveNetFinal = currentEvasive + evasiveModTotal;
            Mod.Log.Trace?.Write($"Final mod summary: evasiveNetFinal {evasiveNetFinal}, mod {mod}");
            if (evasiveNetFinal < 0) mod -= evasiveNetFinal;
            Mod.Log.Trace?.Write($"Final mod: {mod}");
            return mod;
        }

        public static string IRT_CAC_ToHitModName(ToHit toHit, AbstractActor attacker, Weapon weapon, ICombatant target, Vector3 attackPos, Vector3 targetPos, LineOfFireLevel lof, MeleeAttackType meleType, bool isCalled)
        {
            var name = "";
            var defenseModTotal = 0f;
            var evasiveModTotal = 0f;
            var currentDefense = toHit.GetEnemyEffectModifier(target, weapon);
            var currentEvasive = toHit.GetTargetSpeedModifier(target, weapon);
            foreach (var weaponToHitMod in Mod.Config.Combat.ToHitStatMods.WeaponToHitMods)
            {
                var weaponStatVal = weapon.StatCollection.GetValue<float>(weaponToHitMod.SourceStatName);
                if (weaponStatVal != 0)
                {
                    if (target.StatCollection.GetValue<bool>(weaponToHitMod.TargetStatName)) // like IsAerialUnit or whatever
                    {
                        if (weaponToHitMod.Multi)
                        {
                            if (weaponToHitMod.Type == "ABSOLUTE")
                            {
                                Mod.Log.Error?.Write($"ERROR: toHitMod.Multi on stat {weaponToHitMod.SourceStatName} not allowed with toHitMod.Type 'ABSOLUTE'. Doing nothing!");
                            }
                            else if (weaponToHitMod.Type == "DEFENSE")
                            {
                                var defenseMod = currentDefense * weaponStatVal;
                                defenseModTotal += defenseMod;
                                name += $"+ WEAPON [TGT DEF x{weaponStatVal}] ";
                            }
                            else if (weaponToHitMod.Type == "EVASIVE")
                            {
                                var evasiveMod = currentEvasive * weaponStatVal;
                                evasiveModTotal += evasiveMod;
                                name += $"+ WEAPON [TGT EV x{weaponStatVal}] ";
                            }
                        }
                        else if (!weaponToHitMod.Multi)
                        {
                            if (weaponToHitMod.Type == "ABSOLUTE")
                            {
                                name += $"+ WEAPON [{weaponStatVal}] ";
                            }
                            else if (weaponToHitMod.Type == "DEFENSE")
                            {
                                defenseModTotal += weaponStatVal;
                                name += $"+ WEAPON [{weaponStatVal}] ";
                            }
                            else if (weaponToHitMod.Type == "EVASIVE")
                            {
                                evasiveModTotal += weaponStatVal;
                                name += $"+ WEAPON [{weaponStatVal}] ";
                            }
                        }
                    }
                }
            }

            foreach (var toHitMod in Mod.Config.Combat.ToHitStatMods.ActorToHitMods)
            {
                var statVal = attacker.StatCollection.GetValue<float>(toHitMod.SourceStatName);
                if (statVal != 0)
                {
                    if (target.StatCollection.GetValue<bool>(toHitMod.TargetStatName)) // like IsAerialUnit or whatever
                    {
                        if (toHitMod.Multi)
                        {
                            if (toHitMod.Type == "ABSOLUTE")
                            {
                                Mod.Log.Error?.Write($"ERROR: toHitMod.Multi on stat {toHitMod.SourceStatName} not allowed with toHitMod.Type 'ABSOLUTE'. Doing nothing!");
                            }
                            else if (toHitMod.Type == "DEFENSE")
                            {
                                var defenseMod = currentDefense * statVal;
                                defenseModTotal += defenseMod;
                                name += $"+ UNIT [TGT DEF x{statVal}] ";
                            }
                            else if (toHitMod.Type == "EVASIVE")
                            {
                                var evasiveMod = currentEvasive * statVal;
                                evasiveModTotal += evasiveMod;
                                name += $"+ UNIT [TGT EV x{statVal}] ";
                            }
                        }
                        else if (!toHitMod.Multi)
                        {
                            if (toHitMod.Type == "ABSOLUTE")
                            {
                                name += $"+ UNIT [{statVal}] ";
                            }
                            else if (toHitMod.Type == "DEFENSE")
                            {
                                defenseModTotal += statVal;
                                name += $"+ UNIT [{statVal}] ";
                            }
                            else if (toHitMod.Type == "EVASIVE")
                            {
                                evasiveModTotal += statVal;
                                name += $"+ UNIT [{statVal}] ";
                            }
                        }
                    }
                }
            }

            return name;
        }


        public static float IRT_CAC_SkillStabDmgMod(Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot,
            int location, float dmg, float ap, float heat, float stab)
        {
            var mult = 1f;

            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill?.StabilityMods)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.StatName}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.StatName)}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.StatName))
                {
                    Mod.Log.Info?.Write($"Found StabilityMod stat, rolling vs {statmod.Probability}.");
                    if (Mod.Random.NextDouble() <= statmod.Probability)
                    {
                        Mod.Log.Info?.Write($"Roll succeeded, multiplying {mult} by {statmod.Multiplier}.");
                        mult *= statmod.Multiplier;
                    }
                }
            }
            if (Mod.Config.Combat.DamageModsBySkill.DisplayFloatiesOnTrigger && stab > 0 && mult > 1)
            {
                var displaynumber = Mathf.RoundToInt((mult - 1) * 100);
                var txt = new Text("{0}: {1}% Bonus Stability Damage!",
                    new object[]
                    {
                        weapon.UIName,
                        displaynumber
                    });

                //take below and propagate down to rest of dmgMods
                //var msg = new ShowActorInfoSequence(weapon.parent, txt, FloatieMessage.MessageNature.Buff, false);
                
                var activeDmgMod = new ActiveDmgMod(weapon.uid, DmgModType.Stability, mult, txt);
                Mod.Log.Trace?.Write($"Created msg for floatie, but hsouldnt be firing it yet. actorKey: {weapon.parent.GUID} Type Weapon {weapon.Name} GUID:{activeDmgMod?.WeaponGUID}, type: {activeDmgMod.Type}");
                if (!ModState.StabDmgMods.ContainsKey(weapon.parent.GUID))
                {
                    ModState.StabDmgMods.Add(weapon.parent.GUID, new List<ActiveDmgMod> { activeDmgMod });
                }
                else
                {
                    ModState.StabDmgMods[weapon.parent.GUID].Add(activeDmgMod);
                }
            }
            return mult;
        }

        public static string IRT_CAC_SkillStabDmgModName(Weapon weapon, Vector3 attackPosition, ICombatant target,
            bool IsBreachingShot, int location, float dmg, float ap, float heat, float stab)
        {
            return "IRTweaks_SkillDamage_StabDmgMod_" + target.DisplayName;
        }

        public static float IRT_CAC_SkillHeatDmgMod(Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot,
            int location, float dmg, float ap, float heat, float stab)
        {
            var mult = 1f;

            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill?.HeatMods)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.StatName}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.StatName)} for weapon {weapon.Name}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.StatName))
                {
                    Mod.Log.Info?.Write($"Found HeatMod stat, rolling vs {statmod.Probability}.");
                    if (Mod.Random.NextDouble() <= statmod.Probability)
                    {
                        Mod.Log.Info?.Write($"Roll succeeded, multiplying {mult} by {statmod.Multiplier}.");
                        mult *= statmod.Multiplier;
                    }
                }
            }
            if (Mod.Config.Combat.DamageModsBySkill.DisplayFloatiesOnTrigger && heat > 0 && mult > 1)
            {
                var displaynumber = Mathf.RoundToInt((mult - 1) * 100);
                var txt = new Text("{0}: {1}% Bonus Heat Damage!",
                    new object[]
                    {
                        weapon.UIName,
                        displaynumber
                    });

                //take below and propagate down to rest of dmgMods
                //var msg = new ShowActorInfoSequence(weapon.parent, txt, FloatieMessage.MessageNature.Buff, false);
               
                var activeDmgMod = new ActiveDmgMod(weapon.uid, DmgModType.Heat, mult, txt);
                Mod.Log.Trace?.Write($"Created msg for floatie, but hsouldnt be firing it yet. actorKey: {weapon.parent.GUID} Type Weapon {weapon.Name} GUID:{activeDmgMod?.WeaponGUID}, type: {activeDmgMod.Type}");
                if (!ModState.HeatDmgMods.ContainsKey(weapon.parent.GUID))
                {
                    ModState.HeatDmgMods.Add(weapon.parent.GUID, new List<ActiveDmgMod> { activeDmgMod });
                }
                else
                {
                    ModState.HeatDmgMods[weapon.parent.GUID].Add(activeDmgMod);
                }
            }
            return mult;
        }

        public static string IRT_CAC_SkillHeatDmgModName(Weapon weapon, Vector3 attackPosition, ICombatant target,
            bool IsBreachingShot, int location, float dmg, float ap, float heat, float stab)
        {
            return "IRTweaks_SkillDamage_HeatDmgMod_" + target.DisplayName;
        }

        public static float IRT_CAC_SkillAPDmgMod(Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot,
            int location, float dmg, float ap, float heat, float stab)
        {
            var mult = 1f;

            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill.APDmgMods)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.StatName}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.StatName)} for weapon {weapon.Name}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.StatName))
                {
                    Mod.Log.Info?.Write($"Found APDmgMod stat, rolling vs {statmod.Probability}.");
                    if (Mod.Random.NextDouble() <= statmod.Probability)
                    {
                        Mod.Log.Info?.Write($"Roll succeeded, multiplying {mult} by {statmod.Multiplier}.");
                        mult *= statmod.Multiplier;
                    }
                }
            }
            if (Mod.Config.Combat.DamageModsBySkill.DisplayFloatiesOnTrigger && ap > 0 || mult > 1)
            {
                var displaynumber = Mathf.RoundToInt((mult - 1) * 100);
                var txt = new Text("{0}: {1}% Bonus AP Damage!",
                    new object[]
                    {
                        weapon.UIName,
                        displaynumber
                    });

                //take below and propagate down to rest of dmgMods
                //var msg = new ShowActorInfoSequence(weapon.parent, txt, FloatieMessage.MessageNature.Buff, false);
                
                var activeDmgMod = new ActiveDmgMod(weapon.uid, DmgModType.AP, mult, txt);
                Mod.Log.Trace?.Write($"Created msg for floatie, but hsouldnt be firing it yet. actorKey: {weapon.parent.GUID} Type Weapon {weapon.Name} GUID:{activeDmgMod?.WeaponGUID}, type: {activeDmgMod.Type}");
                if (!ModState.APDmgMods.ContainsKey(weapon.parent.GUID))
                {
                    ModState.APDmgMods.Add(weapon.parent.GUID, new List<ActiveDmgMod> { activeDmgMod });
                }
                else
                {
                    ModState.APDmgMods[weapon.parent.GUID].Add(activeDmgMod);
                }
            }
            return mult;
        }

        public static string IRT_CAC_SkillAPDmgModName(Weapon weapon, Vector3 attackPosition, ICombatant target,
            bool IsBreachingShot, int location, float dmg, float ap, float heat, float stab)
        {
            return "IRTweaks_SkillDamage_APDmgMod_" + target.DisplayName;
        }

        public static float IRT_CAC_SkillStdDmgMod(Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot,
            int location, float dmg, float ap, float heat, float stab)
        {
            var mult = 1f;

            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill.StdDmgMods)
            {
                Mod.Log.Trace?.Write($"Checking for stat {statmod.StatName}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.StatName)} for weapon {weapon.Name}.");
                if (weapon.parent.StatCollection.GetValue<bool>(statmod.StatName))
                {
                    Mod.Log.Info?.Write($"Found StdDmgMod stat, rolling vs {statmod.Probability}.");
                    if (Mod.Random.NextDouble() <= statmod.Probability)
                    {
                        Mod.Log.Info?.Write($"Roll succeeded, multiplying {mult} by {statmod.Multiplier}.");
                        mult *= statmod.Multiplier;
                    }
                }
            }
            if (Mod.Config.Combat.DamageModsBySkill.DisplayFloatiesOnTrigger && dmg > 0 && mult > 1)
            {
                var displaynumber = Mathf.RoundToInt((mult - 1) * 100);
                var txt = new Text("{0}: {1}% Bonus Damage!",
                    new object[]
                    {
                        weapon.UIName,
                        displaynumber
                    });

                //take below and propagate down to rest of dmgMods
                //var msg = new ShowActorInfoSequence(weapon.parent, txt, FloatieMessage.MessageNature.Buff, false);
                
                var activeDmgMod = new ActiveDmgMod(weapon.uid, DmgModType.Standard, mult, txt);
                Mod.Log.Trace?.Write($"Created msg for floatie, but hsouldnt be firing it yet. actorKey: {weapon.parent.GUID} Type Weapon {weapon.Name} GUID:{activeDmgMod?.WeaponGUID}, type: {activeDmgMod.Type}");
                if (!ModState.StdDmgMods.ContainsKey(weapon.parent.GUID))
                {
                    ModState.StdDmgMods.Add(weapon.parent.GUID, new List<ActiveDmgMod> { activeDmgMod });
                }
                else
                {
                    ModState.StdDmgMods[weapon.parent.GUID].Add(activeDmgMod);
                }
            }
            return mult;
        }

        public static string IRT_CAC_SkillStdDmgModName(Weapon weapon, Vector3 attackPosition, ICombatant target,
            bool IsBreachingShot, int location, float dmg, float ap, float heat, float stab)
        {
            return "IRTweaks_SkillDamage_StdDmgMod_" + target.DisplayName;
        }
    }
}

#endif