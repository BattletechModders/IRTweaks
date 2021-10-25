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
using Harmony;
using Localize;
using static CustAmmoCategories.CustomAmmoCategories;

using UnityEngine;

namespace IRTweaks.Modules.Combat 
{ 
    public class IRT_CAC_DamageModsBySkill
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
                Mod.Config.Combat.DamageModsBySkill.APDmgMods.Count > 0);

            static void Prefix(AbstractActor __instance, string sourceID, int stackItemID)
            {
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

        [HarmonyPatch(typeof(AbstractActor), "InitEffectStats")]
        public static class AbstractActor_InitEffectStats_DamageModsBySkill
        {
            static bool Prepare() => Mod.Config.Combat.DamageModsBySkill?.HeatMods.Count > 0 || Mod.Config.Combat.DamageModsBySkill?.StabilityMods.Count > 0 ||Mod.Config.Combat.DamageModsBySkill.APDmgMods.Count > 0 || Mod.Config.Combat.DamageModsBySkill.StdDmgMods.Count > 0;

            static void Postfix(AbstractActor __instance)
            {
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

        public static float IRT_CAC_SkillStabDmgMod(Weapon weapon, Vector3 attackPosition, ICombatant target, bool IsBreachingShot,
            int location, float dmg, float ap, float heat, float stab)
        {
            var mult = 1f;

            foreach (var statmod in Mod.Config.Combat.DamageModsBySkill?.StabilityMods)
            {
                Mod.Log.Info?.Write($"Checking for stat {statmod.StatName}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.StatName)}.");
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
                Mod.Log.Info?.Write($"Checking for stat {statmod.StatName}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.StatName)} for weapon {weapon.Name}.");
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
                Mod.Log.Info?.Write($"Checking for stat {statmod.StatName}; result: {weapon.parent.StatCollection.ContainsStatistic(statmod.StatName)} for weapon {weapon.Name}.");
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
                    Mod.Log.Trace?.Write($"Found StdDmgMod stat, rolling vs {statmod.Probability}.");
                    if (Mod.Random.NextDouble() <= statmod.Probability)
                    {
                        Mod.Log.Trace?.Write($"Roll succeeded, multiplying {mult} by {statmod.Multiplier}.");
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