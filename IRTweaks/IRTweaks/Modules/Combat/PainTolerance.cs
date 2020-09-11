using BattleTech;
using BattleTech.UI.TMProWrapper;
using Harmony;
using IRBTModUtils;
using Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace IRTweaks.Modules.Combat
{

    [HarmonyAfter(new string[] { "MechEngineer.Features.ComponentExplosions", "io.mission.modreputation" })]
    [HarmonyPatch(typeof(Mech), "DamageLocation")]
    static class Mech_DamageLocation
    {
        static bool Prepare() => Mod.Config.Fixes.PainTolerance;

        static void Prefix(Mech __instance, ArmorLocation aLoc, Weapon weapon, float totalArmorDamage, float directStructureDamage)
        {

            Mod.Log.Trace?.Write("M:DL - entered");

            if (aLoc == ArmorLocation.Head)
            {
                Mod.Log.Info?.Write($"Head hit from weapon:{weapon?.UIName} for {totalArmorDamage} armor damage and {directStructureDamage} structure damage.");

                float currHeadArmor = __instance.GetCurrentArmor(aLoc);
                int damageMod = (int)Math.Ceiling(totalArmorDamage);
                float damageThroughArmor = totalArmorDamage - currHeadArmor;
                Mod.Log.Debug?.Write($"TotalArmorDamage:{totalArmorDamage} - Head armor:{currHeadArmor} = throughArmor:{damageThroughArmor}");

                if (totalArmorDamage - currHeadArmor <= 0)
                {
                    damageMod = (int)Math.Floor(damageMod * Mod.Config.Combat.PainTolerance.HeadHitArmorOnlyResistPenaltyMulti);
                    Mod.Log.Info?.Write($"Head hit impacted armor only, reduced damage to:{damageMod}");
                }

                if (directStructureDamage != 0)
                {
                    Mod.Log.Debug?.Write($"Attack inflicted ${directStructureDamage}, adding to total resist damage.");
                    damageMod += (int)Math.Ceiling(directStructureDamage);
                }

                ModState.InjuryResistPenalty = damageMod * Mod.Config.Combat.PainTolerance.HeadDamageResistPenaltyPerArmorPoint;
                Mod.Log.Debug?.Write($"Headshot sets injury resist penalty to: {damageMod} x {Mod.Config.Combat.PainTolerance.HeadDamageResistPenaltyPerArmorPoint} = {ModState.InjuryResistPenalty}");
            }
        }
    }

    [HarmonyPatch(typeof(AmmunitionBox), "DamageComponent")]
    static class AmmunitionBox_DamageComponent
    {
        static bool Prepare() => Mod.Config.Fixes.PainTolerance;

        static void Prefix(AmmunitionBox __instance, ComponentDamageLevel damageLevel, bool applyEffects)
        {

            if (__instance == null || __instance.ammunitionBoxDef == null || SharedState.Combat?.Constants?.PilotingConstants == null) return; // Nothing to do

            if (__instance.ammunitionBoxDef.Capacity == 0)
            {
                Mod.Log.Warn?.Write($"Invalid ammoBox '{__instance.UIName}' detected with 0 capacity. Skipping.");
                return;
            }

            bool explosionsCauseInjuries = SharedState.Combat.Constants.PilotingConstants.InjuryFromAmmoExplosion;
            if (applyEffects && damageLevel == ComponentDamageLevel.Destroyed && explosionsCauseInjuries)
            {
                int value = __instance.StatCollection.GetValue<int>("CurrentAmmo");
                int capacity = __instance.ammunitionBoxDef.Capacity;
                float ratio = (float)value / (float)capacity;
                Mod.Log.Debug?.Write($"Ammo explosion ratio:{ratio} = current:{value} / capacity:{capacity}");
                int resistPenalty = (int)Math.Floor(ratio * Mod.Config.Combat.PainTolerance.AmmoExplosionResistPenaltyPerCapacityPercentile);
                Mod.Log.Debug?.Write($"Ammo explosion resist penalty: {resistPenalty} = " +
                    $"Floor( ratio: {ratio}% x penaltyPerAmmoExplosion: {Mod.Config.Combat.PainTolerance.AmmoExplosionResistPenaltyPerCapacityPercentile} )");

                // NOTE: Used to gate on ratio > 50%... why?
                ModState.InjuryResistPenalty = resistPenalty;
                Mod.Log.Debug?.Write($"Ammo explosion sets injury resist penalty to: {resistPenalty}");
            }
        }
    }

    [HarmonyAfter(new string[] { "co.uk.cwolf.MissionControl" })]
    [HarmonyBefore(new string[] { "us.frostraptor.SkillBasedInit", "dZ.Zappo.Pilot_Quirks" })]
    [HarmonyPatch(typeof(Pilot), "InjurePilot")]
    static class Pilot_InjurePilot
    {

        static bool Prepare() => Mod.Config.Fixes.PainTolerance;

        static bool Prefix(Pilot __instance, ref int dmg, DamageType damageType, ref bool ___needsInjury, ref InjuryReason ___injuryReason )
        {
            Mod.Log.Trace?.Write("P:SNI - entered");

            if (__instance.ParentActor == null) return true;

            // Compute the resist penalty for each damage type that we support
            if (damageType == DamageType.HeadShot)
            {
                Mod.Log.Info?.Write($"Actor suffered a headshot, injury resist was set to: {ModState.InjuryResistPenalty}");
            }
            else if (damageType == DamageType.AmmoExplosion)
            {
                Mod.Log.Info?.Write($"Actor suffered an ammo explosion, injury resist was set to: {ModState.InjuryResistPenalty}");
            }
            else if (damageType == DamageType.Knockdown)
            {
                ModState.InjuryResistPenalty = Mod.Config.Combat.PainTolerance.KnockdownResistPenalty;
                Mod.Log.Info?.Write($"Actor was knocked down, setting injury resist to: {ModState.InjuryResistPenalty}");
            }
            else if (damageType == DamageType.SideTorso)
            {
                ModState.InjuryResistPenalty = Mod.Config.Combat.PainTolerance.SideLocationDestroyedResistPenalty;
                Mod.Log.Info?.Write($"Actor torso/side destroyed, setting injury resist to: {ModState.InjuryResistPenalty}");
            }
            else if (damageType == DamageType.Overheat || damageType == DamageType.OverheatSelf || 
                "OVERHEATED".Equals(__instance.InjuryReasonDescription, StringComparison.InvariantCultureIgnoreCase))
            {
                // comparison string must match label in https://github.com/BattletechModders/MechEngineer/blob/master/source/Features/ShutdownInjuryProtection/Patches/Pilot_InjuryReasonDescription_Patch.cs
                Mod.Log.Debug?.Write($"Actor damage from overheating or ME heatDamage injury, computing overheat ratio.");
                float overheatRatio = PainHelper.CalculateOverheatRatio(__instance.ParentActor as Mech);
                int overheatPenalty = (int)Math.Floor(overheatRatio * Mod.Config.Combat.PainTolerance.OverheatResistPenaltyPerHeatPercentile);
                Mod.Log.Debug?.Write($"overheatPenalty:{overheatPenalty} = " +
                    $"Floor( overheatRatio:{overheatRatio} * penaltyPerOverheatDamage{Mod.Config.Combat.PainTolerance.OverheatResistPenaltyPerHeatPercentile} )");
                
                ModState.InjuryResistPenalty = overheatPenalty;
                Mod.Log.Info?.Write($"Actor overheated, setting injury resist to: {ModState.InjuryResistPenalty}");
            }

            // Need default resistance?

            if (ModState.InjuryResistPenalty != -1)
            {
                bool success = PainHelper.MakeResistCheck(__instance);
                if (success)
                {
                    Mod.Log.Info?.Write($"Ignoring {__instance.InjuryReason} injury on pilot: {__instance.Name}");
                    ___needsInjury = false;
                    ___injuryReason = InjuryReason.NotSet;

                    // Reset our mod state
                    ModState.InjuryResistPenalty = -1;

                    // Publish a floatie
                    string localText = new Text(Mod.LocalizedText.Floaties[ModText.FT_InjuryResist], new object[] {}).ToString();
                    IStackSequence stackSequence = new ShowActorInfoSequence(__instance.ParentActor, localText, FloatieMessage.MessageNature.PilotInjury, useCamera: false);
                    SharedState.Combat.MessageCenter.PublishMessage(new AddSequenceToStackMessage(stackSequence));

                    return false;
                }

            }

            return true;
        }
    }


    [HarmonyPatch(typeof(TurnDirector), "OnTurnActorActivateComplete")]
    static class TurnDirector_OnTurnActorActivateComplete
    {
        static bool Prepare() => Mod.Config.Fixes.PainTolerance;

        static void Postfix(TurnDirector __instance)
        {

            Mod.Log.Trace?.Write("TD:OTAAC - entered");

            // Reset the attack penalty in case we've flipped actors.
            ModState.InjuryResistPenalty = -1;
        }
    }

    // Prevent a duplicate INJURY IGNORED message
    [HarmonyPatch(typeof(AbstractActor), "CheckPilotStatusFromAttack")]
    static class AbstractActor_CheckPilotStatusFromAttack
    {
        static bool Prepare() => Mod.Config.Fixes.PainTolerance;

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            // Create a new label for our target point
            Label clearNeedsInjuryLabel = ilGenerator.DefineLabel();

            MethodInfo clearNeedsInjuryMI = AccessTools.DeclaredMethod(typeof(Pilot), "ClearNeedsInjury");

            int injuryStrIdx = 0, clearInjuryIdx = 0;
            for (int i = 0; i < codes.Count; i++) 
            {
                CodeInstruction instruction = codes[i];
                if (instruction.opcode == OpCodes.Ldstr && "{0}: INJURY IGNORED".Equals((string)instruction.operand, StringComparison.InvariantCultureIgnoreCase))
                {
                    injuryStrIdx = i;
                    Mod.Log.Debug?.Write($"AA:CPSFA Found INJURY IGNORED instruction at idx: {i}");
                }
                else if (instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == clearNeedsInjuryMI)
                {
                    clearInjuryIdx = i;
                    Mod.Log.Debug?.Write($"AA:CPSFA Found Pilot.ClearNeedsInjury instruction at idx: {i}");
                }
            }

            CodeInstruction cnjInstruction = codes[clearInjuryIdx - 1];
            cnjInstruction.labels.Add(clearNeedsInjuryLabel);

            codes.RemoveRange(injuryStrIdx - 1, 15);
            codes.Insert(injuryStrIdx - 1, new CodeInstruction(OpCodes.Br_S, clearNeedsInjuryLabel));

            return codes;
        }
    }

    // Prevent a duplicate INJURY IGNORED message
    [HarmonyPatch(typeof(Mech), "CompleteKnockdown")]
    static class Mech_CompleteKnockdown
    {
        static bool Prepare() => Mod.Config.Fixes.PainTolerance;

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            int targetIdx = 0;
            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction instruction = codes[i];
                if (instruction.opcode == OpCodes.Ldstr && "KNOCKDOWN: INJURY IGNORED".Equals((string)instruction.operand, StringComparison.InvariantCultureIgnoreCase))
                {
                    targetIdx = i;
                    Mod.Log.Debug?.Write($"M:CK Found INJURY IGNORED instruction at idx: {i}");
                }
            }

            codes.RemoveRange(targetIdx - 4, 12);

            return codes;
        }
    }
}
