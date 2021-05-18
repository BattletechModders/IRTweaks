using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using IRTweaks.Helper;

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(Team), "AddUnit")]
    public static class Mech_AddToTeam
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection && false; //disabled for now

        public static void Postfix(Team __instance, AbstractActor unit)
        {
            if (unit is Mech mech)
            {
                var startingArmor = mech.SummaryArmorCurrent;
                ModState.UnitStartingArmor.Add(mech.GUID, startingArmor);
                ModState.UnitCurrentArmor.Add(mech.GUID, startingArmor);
                ModState.UnitCurrentHeat.Add(mech.GUID, mech.CurrentHeat);
            }
        }
    }

    [HarmonyPatch(typeof(Mech), "ApplyArmorStatDamage")]
    public static class Mech_ApplyArmorStatDamage
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection && false; //disabled for now
        public static void Postfix(Mech __instance)
        {
            
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (!ModState.UnitStartingArmor.ContainsKey(__instance.GUID))
            {
                ModState.UnitStartingArmor.Add(__instance.GUID, __instance.CurrentArmor);
                Mod.Log.Info?.Write($"CHEATDETECTION: Added key to UnitStartingArmor with CurrentArmor... this should have been done already though.");
            }

            if (!ModState.UnitCurrentArmor.ContainsKey(__instance.GUID))
            {
                ModState.UnitCurrentArmor.Add(__instance.GUID, __instance.CurrentArmor);
                Mod.Log.Info?.Write($"CHEATDETECTION: Added key to UnitStartingArmor with CurrentArmor... this should have been done already though.");
            }

            if (__instance.CurrentArmor > ModState.UnitCurrentArmor[__instance.GUID])
            {
                sim.CompanyStats.AddStatistic(Mod.Config.CheatDetection.CheatDetectionStat, true);
                Mod.Log.Info?.Write($"CHEATDETECTION: Caught you, you little shit. Cheated armor.");
            }

            if (__instance.CurrentArmor > ModState.UnitStartingArmor[__instance.GUID])
            {
                sim.CompanyStats.AddStatistic(Mod.Config.CheatDetection.CheatDetectionStat, true);
                Mod.Log.Info?.Write($"CHEATDETECTION: Caught you, you little shit. Cheated armor.");
            }

            ModState.UnitCurrentArmor[__instance.GUID] = __instance.CurrentArmor;
        }
    }

        [HarmonyPatch(typeof(Mech))]
    [HarmonyPatch("_heat", MethodType.Setter)]
    public static class Mech__heat_Setter
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection && false; //disabled for now

        public static void Postfix(Mech __instance, int value)
        {
            if (!ModState.UnitCurrentHeat.ContainsKey(__instance.GUID))
            {
                ModState.UnitCurrentHeat.Add(__instance.GUID, __instance.CurrentHeat);
                Mod.Log.Info?.Write($"CHEATDETECTION: Added key to UnitCurrentHeat with CurrentHeat value... this should have been done already though.");
            }
            if (ModState.UnitCurrentHeat.ContainsKey(__instance.GUID))
            {
                ModState.UnitCurrentHeat[__instance.GUID] = value;
            }
        }
    }


    [HarmonyPatch(typeof(Mech))]
    [HarmonyPatch("CurrentHeat", MethodType.Getter)]
    public static class Mech_CurrentHeat_Getter
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection && false; //disabled for now. this is the one that breaks spawning and CTDs with no errors for some reason.

        public static void Postfix(Mech __instance)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (!ModState.UnitCurrentHeat.ContainsKey(__instance.GUID))
            {
                ModState.UnitCurrentHeat.Add(__instance.GUID, __instance.CurrentHeat);
                Mod.Log.Info?.Write($"CHEATDETECTION: Added key to UnitCurrentHeat with CurrentHeat value... this should have been done already though.");
            }
            if (__instance.CurrentHeat != ModState.UnitCurrentHeat[__instance.GUID])
            {
                sim.CompanyStats.AddStatistic(Mod.Config.CheatDetection.CheatDetectionStat, true);
                Mod.Log.Info?.Write($"CHEATDETECTION: Caught you, you little shit. Cheated heat.");
            }
        }
    }

    [HarmonyPatch(typeof(CombatGameState))]
    [HarmonyPatch("OnCombatGameDestroyed")]
    public static class CombatGameState_OnCGSDestroyed
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection && false; //disabled for now
        public static void Prefix()
        {
            ModState.UnitCurrentArmor.Clear();
            ModState.UnitCurrentHeat.Clear();
            ModState.UnitStartingArmor.Clear();
        }
    }





    [HarmonyPatch(typeof(PilotDef))]
    [HarmonyPatch("ExperienceUnspent", MethodType.Setter)]
    public static class Pilot_XPUnspent_Setter
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection && false;

        public static void Postfix(PilotDef __instance, int value)
        {
            if (UnityGameInstance.BattleTechGame.Simulation == null) return;
            if (String.IsNullOrEmpty(__instance.Description.Id)) return;
            if (!ModState.PilotDefCurrentFreeXP.ContainsKey(__instance.Description.Id))
            {
                ModState.PilotDefCurrentFreeXP.Add(__instance.Description.Id, __instance.ExperienceUnspent);

            }
            ModState.PilotDefCurrentFreeXP[__instance.Description.Id] = value;
        }
    }


    [HarmonyPatch(typeof(PilotDef))]
    [HarmonyPatch("ExperienceUnspent", MethodType.Getter)]
    public static class Pilot_XPUnspent_Getter
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection && false;

        public static void Postfix(PilotDef __instance)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (String.IsNullOrEmpty(__instance.Description.Id)) return;
            if (!ModState.PilotDefCurrentFreeXP.ContainsKey(__instance.Description.Id))
            {
                ModState.PilotDefCurrentFreeXP.Add(__instance.Description.Id, __instance.ExperienceUnspent);
                Mod.Log.Info?.Write($"CHEATDETECTION: Added key to PilotCurrentXP with ExperienceUnspent value.");
            }
        }
    }


    [HarmonyPatch(typeof(Pilot))]
    [HarmonyPatch("FromPilotDef")]
    public static class Pilot_FromPilotDef
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection;

        public static void Postfix(Pilot __instance)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (String.IsNullOrEmpty(__instance.GUID)) return;
            if (!ModState.PilotCurrentFreeXP.ContainsKey(__instance.GUID))
            {
                ModState.PilotCurrentFreeXP.Add(__instance.GUID, __instance.UnspentXP);
                Mod.Log.Info?.Write($"CHEATDETECTION: {__instance.Description.Id}: Set ExperienceUnspent to UnspentXP value {__instance.UnspentXP} at FromPilotDef.");
            }

            ModState.PilotCurrentFreeXP[__instance.GUID]  = __instance.pilotDef.ExperienceUnspent;
            Mod.Log.Info?.Write($"CHEATDETECTION: {__instance.Description.Id}: PilotCurrentXP now {ModState.PilotCurrentFreeXP[__instance.GUID]} after setting to {__instance.pilotDef.ExperienceUnspent} at FromPilotDef, Post.");
        }
    }

    [HarmonyPatch(typeof(Pilot))]
    [HarmonyPatch("AddExperience")]
    public static class Pilot_AddXP
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection;

        public static void Postfix(Pilot __instance, int stackID, string sourceID, int value)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (String.IsNullOrEmpty(__instance.GUID)) return;
            if (!ModState.PilotCurrentFreeXP.ContainsKey(__instance.GUID))
            {
                ModState.PilotCurrentFreeXP.Add(__instance.GUID, __instance.UnspentXP);
                Mod.Log.Info?.Write($"CHEATDETECTION: {__instance.Description.Id}: Added key to PilotCurrentXP with {__instance.UnspentXP} UnspentXP value at AddExperience, Post.");
                return;
            }

            ModState.PilotCurrentFreeXP[__instance.GUID] += value;
            Mod.Log.Info?.Write($"CHEATDETECTION: {__instance.Description.Id}: PilotCurrentXP now {ModState.PilotCurrentFreeXP[__instance.GUID]} after adding {value} at AddExperience, Post.");
        }
    }



    [HarmonyPatch(typeof(Pilot))]
    [HarmonyPatch("SpendExperience")]
    public static class Pilot_SpendXP
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection;

        public static void Prefix(Pilot __instance)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (String.IsNullOrEmpty(__instance.GUID)) return;
            if (!ModState.PilotCurrentFreeXP.ContainsKey(__instance.GUID))
            {
                ModState.PilotCurrentFreeXP.Add(__instance.GUID, __instance.UnspentXP);
                Mod.Log.Info?.Write($"CHEATDETECTION: {__instance.Description.Id}: Added key to PilotCurrentXP with {__instance.UnspentXP} UnspentXP value at SpendExperience, Pre.");
            }
        }

        public static void Postfix(Pilot __instance, int stackID, string sourceID, uint value)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (String.IsNullOrEmpty(__instance.GUID)) return;

            ModState.PilotCurrentFreeXP[__instance.GUID] -= (int)value;
            Mod.Log.Info?.Write($"CHEATDETECTION: {__instance.Description.Id}: pilot UnspentXP was {__instance.UnspentXP} after subtracting {value} at SpendExperience, Post.");
            if (__instance.UnspentXP != ModState.PilotCurrentFreeXP[__instance.GUID])
            {
                Mod.Log.Info?.Write($"CHEATDETECTION: {__instance.Description.Id}: pilot UnspentXP was {__instance.UnspentXP} but state variable was {ModState.PilotCurrentFreeXP[__instance.GUID]}.");
                sim.CompanyStats.AddStatistic(Mod.Config.CheatDetection.CheatDetectionStat, true);
                Mod.Log.Info?.Write($"CHEATDETECTION: Caught you, you little shit. Cheated experience.");

                if (Mod.Config.CheatDetection.CheatDetectionNotify) GenericPopupBuilder.Create("CHEAT DETECTED!", "t-bone thinks you're cheating. if you aren't, you should let the RT crew know on Discord.").AddButton("Okay", null, true, null).CancelOnEscape().AddFader(new UIColorRef?(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill), 0f, true).Render();
            }
        }
    }

    [HarmonyPatch(typeof(SGBarracksMWDetailPanel), "OnPilotReset")]
    public static class SGBarracksMWDetailPanel_OnPilotReset
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection;
        public static void Postfix(SGBarracksMWDetailPanel __instance, Pilot ___tempPilot, Pilot ___curPilot)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (String.IsNullOrEmpty(___curPilot.GUID)) return;
            if (!ModState.PilotCurrentFreeXP.ContainsKey(___curPilot.GUID))
            {
                ModState.PilotCurrentFreeXP.Add(___curPilot.GUID, ___curPilot.UnspentXP);
                Mod.Log.Info?.Write($"CHEATDETECTION: {___curPilot.Description.Id}: Added key to PilotCurrentXP with UnspentXP {___curPilot.UnspentXP} but should have been done already. At OnPilotReset, Post.");
            }

            ModState.PilotCurrentFreeXP[___curPilot.GUID] = ___curPilot.UnspentXP;
            Mod.Log.Info?.Write(
                $"CHEATDETECTION: {___curPilot.Description.Id}: Free XP state was {ModState.PilotCurrentFreeXP[___curPilot.GUID]} after changing to basePilot {___curPilot.UnspentXP} at OnPilotReset, Post.");
        }
    }

    [HarmonyPatch(typeof(SGBarracksWidget), "ReceiveButtonPress")]
    public static class SGBarracksWidget_ReceiveButtonPress
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection;
        public static void Postfix(SGBarracksWidget __instance, string button, SGBarracksMWDetailPanel ___mechWarriorDetails)
        {
            if (button != "Close") return;
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;

            var curPilot = Traverse.Create(___mechWarriorDetails).Field("curPilot").GetValue<Pilot>();
            if (String.IsNullOrEmpty(curPilot.GUID)) return;
            if (!ModState.PilotCurrentFreeXP.ContainsKey(curPilot.GUID))
            {
                ModState.PilotCurrentFreeXP.Add(curPilot.GUID, curPilot.UnspentXP);
                Mod.Log.Info?.Write($"CHEATDETECTION: {curPilot.Description.Id}: Added key to PilotCurrentXP with UnspentXP {curPilot.UnspentXP} but should have been done already. At SGBarracksWidget.ReceiveButtonPress, Post.");
            }

            ModState.PilotCurrentFreeXP[curPilot.GUID] = curPilot.UnspentXP;
            Mod.Log.Info?.Write($"CHEATDETECTION: {curPilot.Description.Id}: Free XP state was {ModState.PilotCurrentFreeXP[curPilot.GUID]} after changing to basePilot {curPilot.UnspentXP} At SGBarracksWidget.ReceiveButtonPress, Post.");
        }
    }

    [HarmonyPatch(typeof(SGBarracksAdvancementPanel), "OnValueClick")]
    [HarmonyBefore(new string[]
    {
        "ca.gnivler.BattleTech.Abilifier"
    })]
    [HarmonyPriority(Priority.First)]
    public static class SGBarracksAdvancementPanel_OnValueClick_Patch
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection;
        public static void Prefix(SGBarracksAdvancementPanel __instance, Pilot ___curPilot, Pilot ___basePilot,
            List<SGBarracksSkillPip> ___gunPips, List<SGBarracksSkillPip> ___pilotPips,
            List<SGBarracksSkillPip> ___gutPips, List<SGBarracksSkillPip> ___tacPips, string type, int value)
        {
            if (___curPilot.StatCollection.GetValue<int>(type) > value)
            {
                var sim = UnityGameInstance.BattleTechGame.Simulation;
                if (sim == null) return;
                if (String.IsNullOrEmpty(___curPilot.GUID)) return;
                if (!ModState.PilotCurrentFreeXP.ContainsKey(___curPilot.GUID))
                {
                    ModState.PilotCurrentFreeXP.Add(___curPilot.GUID, ___curPilot.UnspentXP);
                    Mod.Log.Info?.Write($"CHEATDETECTION: {___curPilot.Description.Id}: Added key to PilotCurrentXP with UnspentXP {___curPilot.UnspentXP} but should have been done already. At SGBarracksAdvancementPanel.OnValueClick, Pre.");
                }

                ModState.PilotCurrentFreeXP[___curPilot.GUID] = ___basePilot.UnspentXP;
                Mod.Log.Info?.Write($"CHEATDETECTION: {___curPilot.Description.Id}: Free XP state was {ModState.PilotCurrentFreeXP[___curPilot.GUID]} after changing to basePilot {___basePilot.UnspentXP} At SGBarracksAdvancementPanel.OnValueClick, Pre.");
            }
        }
    }

    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("AddFunds")]
    public static class SimGameState_AddFunds
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection;

        public static void Prefix(SimGameState __instance, int val)
        {
            if (String.IsNullOrEmpty(__instance.InstanceGUID)) return;
            if (!ModState.SimGameFunds.ContainsKey(__instance.InstanceGUID))
            {
                ModState.SimGameFunds.Add(__instance.InstanceGUID, __instance.Funds);
                Mod.Log.Info?.Write($"CHEATDETECTION: Added key to SimGameFunds with Funds {__instance.Funds}; this should have been done already...WTF. At AddFunds, Pre.");
            }
            if (__instance.Funds != ModState.SimGameFunds[__instance.InstanceGUID])
            {
                __instance.CompanyStats.AddStatistic(Mod.Config.CheatDetection.CheatDetectionStat, true);
                Mod.Log.Info?.Write($"CHEATDETECTION: Caught you, you little shit. Cheated money. SGS Funds: {__instance.Funds} while tracker funds {ModState.SimGameFunds[__instance.InstanceGUID]} at AddFunds, Pre");
            }
        }

        public static void Postfix(SimGameState __instance, int val)
        {
            if (!ModState.SimGameFunds.ContainsKey(__instance.InstanceGUID))
            {
                ModState.SimGameFunds.Add(__instance.InstanceGUID, __instance.Funds);
                Mod.Log.Info?.Write($"CHEATDETECTION: Added key to SimGameFunds with Funds {__instance.Funds}; this should have been done already...WTF. At Addfunds, Post");
            }
            ModState.SimGameFunds[__instance.InstanceGUID] += val;
            if (__instance.Funds != ModState.SimGameFunds[__instance.InstanceGUID])
            {
                __instance.CompanyStats.AddStatistic(Mod.Config.CheatDetection.CheatDetectionStat, true);
                Mod.Log.Info?.Write($"CHEATDETECTION: Caught you, you little shit. Cheated money. SGS Funds: {__instance.Funds} while tracker funds {ModState.SimGameFunds[__instance.InstanceGUID]} after adding {val} at AddFunds, Post");

                if (Mod.Config.CheatDetection.CheatDetectionNotify) GenericPopupBuilder.Create("CHEAT DETECTED!", "t-bone thinks you're cheating. if you aren't, you should let the RT crew know on Discord.").AddButton("Okay", null, true, null).CancelOnEscape().AddFader(new UIColorRef?(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill), 0f, true).Render();
            }
        }

    }

    [HarmonyPatch(typeof(SimGameState), "SetSimGameStat",
        new Type[] {typeof(SimGameStat), typeof(StatCollection)})]

    public static class SGS_SetSimGameStat
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection;
        public static void Postfix(SimGameState __instance, SimGameStat stat, StatCollection statCol)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            var pilots = new List<Pilot>(sim.PilotRoster);
            pilots.Add(sim.Commander);

            foreach (var pilot in pilots)
            {
                if (pilot.StatCollection == statCol)
                {
                    if (stat.name == "ExperienceUnspent" &&
                        (stat.Type == typeof(int) || stat.typeString == "System.Int32"))
                    {
                        if (!ModState.PilotCurrentFreeXP.ContainsKey(pilot.GUID))
                        {
                            ModState.PilotCurrentFreeXP.Add(pilot.GUID, pilot.UnspentXP);
                            Mod.Log.Info?.Write(
                                $"CHEATDETECTION: Added key to ExperienceUnspent with ExperienceUnspent {pilot.UnspentXP}; this should have been done already...WTF. At SetSimGameStat, Post.");
                        }

                        var val = stat.ToInt();
                        if (stat.set)
                        {
                            ModState.PilotCurrentFreeXP[pilot.GUID] = val;
                        }
                        else
                        {
                            ModState.PilotCurrentFreeXP[pilot.GUID] += val;
                            if (pilot.UnspentXP != ModState.PilotCurrentFreeXP[pilot.GUID])
                            {
                                Mod.Log.Info?.Write($"CHEATDETECTION: {pilot.Description.Id}: pilot UnspentXP was {pilot.UnspentXP} but state variable was {ModState.PilotCurrentFreeXP[pilot.GUID]}. At SetSimGameStat, Post.");
                                sim.CompanyStats.AddStatistic(Mod.Config.CheatDetection.CheatDetectionStat, true);
                                Mod.Log.Info?.Write($"CHEATDETECTION: Caught you, you little shit. Cheated experience.");

                                if (Mod.Config.CheatDetection.CheatDetectionNotify) GenericPopupBuilder
                                    .Create("CHEAT DETECTED!",
                                        "t-bone thinks you're cheating. if you aren't, you should let the RT crew know on Discord.")
                                    .AddButton("Okay", null, true, null).CancelOnEscape()
                                    .AddFader(
                                        new UIColorRef?(LazySingletonBehavior<UIManager>.Instance
                                            .UILookAndColorConstants.PopupBackfill), 0f, true).Render();
                            }
                        }
                    }
                }
            }


            if (statCol == sim.CompanyStats)
            {
                if (stat.name == "Funds" && (stat.Type == typeof(int) || stat.typeString == "System.Int32"))
                {
                    if (!ModState.SimGameFunds.ContainsKey(sim.InstanceGUID))
                    {
                        ModState.SimGameFunds.Add(sim.InstanceGUID, sim.Funds);
                        Mod.Log.Info?.Write($"CHEATDETECTION: Added key to SimGameFunds with Funds {sim.Funds}; this should have been done already...WTF, At SetSimGameStat, Post.");
                    }

                    var val = stat.ToInt();
                    if (stat.set)
                    {
                        ModState.SimGameFunds[sim.InstanceGUID] = val;
                    }
                    else
                    {
                        ModState.SimGameFunds[sim.InstanceGUID] += val;
                        if (sim.Funds != ModState.SimGameFunds[sim.InstanceGUID])
                        {
                            sim.CompanyStats.AddStatistic(Mod.Config.CheatDetection.CheatDetectionStat, true);
                            Mod.Log.Info?.Write($"CHEATDETECTION: Caught you, you little shit. Cheated money. SGS Funds: {sim.Funds} while tracker funds {ModState.SimGameFunds[sim.InstanceGUID]} after adding {stat}. At SetSimGameStat, Post.");

                            if (Mod.Config.CheatDetection.CheatDetectionNotify) GenericPopupBuilder.Create("CHEAT DETECTED!", "t-bone thinks you're cheating. if you aren't, you should let the RT crew know on Discord.").AddButton("Okay", null, true, null).CancelOnEscape().AddFader(new UIColorRef?(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill), 0f, true).Render();
                        }
                    }
                }
            }
        }
    }



    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("Rehydrate")]
    public static class SimGameState_Rehydrate_CH
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection;

        public static void Postfix(SimGameState __instance)
        {
            if (!ModState.SimGameFunds.ContainsKey(__instance.InstanceGUID))
            {
                ModState.SimGameFunds.Add(__instance.InstanceGUID, __instance.Funds);
                Mod.Log.Info?.Write($"CHEATDETECTION: Added key to SimGameFunds with Funds {__instance.Funds} on rehydrate, post.");
            }
            else
            {
                ModState.SimGameFunds[__instance.InstanceGUID] = __instance.Funds;
                Mod.Log.Info?.Write($"CHEATDETECTION: Set ModState.SimGameFunds to {__instance.Funds} on rehydrate, post.");
            }

            var currentPilots = new List<Pilot>(__instance.PilotRoster);
            currentPilots.Add(__instance.Commander);
            foreach (var pilot in currentPilots)
            {
                if (String.IsNullOrEmpty(pilot.GUID)) return;
                if (!ModState.PilotCurrentFreeXP.ContainsKey(pilot.GUID))
                {
                    ModState.PilotCurrentFreeXP.Add(pilot.GUID, pilot.UnspentXP);
                    Mod.Log.Info?.Write($"CHEATDETECTION: {pilot.Description.Id}: Added key to PilotCurrentXP with ExperienceUnspent {pilot.UnspentXP} on rehydrate, post.");
                }
                else
                {
                    ModState.PilotCurrentFreeXP[pilot.GUID] = pilot.UnspentXP;
                    Mod.Log.Info?.Write($"CHEATDETECTION: {pilot.Description.Id}: Set tracker PilotCurrentXP with ExperienceUnspent {pilot.UnspentXP} on rehydrate, post.");
                }
            }
        }
    }


    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("Dehydrate")]
    public static class SimGameState_Dehydrate_CH
    {
        static bool Prepare() => Mod.Config.CheatDetection.CheatDetection;

        public static void Prefix(SimGameState __instance)
        {
            if (!ModState.SimGameFunds.ContainsKey(__instance.InstanceGUID))
            {
                ModState.SimGameFunds.Add(__instance.InstanceGUID, __instance.Funds);
                Mod.Log.Info?.Write($"CHEATDETECTION: Added key to SimGameFunds with Funds {__instance.Funds} on dehydrate, pre.");
            }

            if (__instance.Funds != ModState.SimGameFunds[__instance.InstanceGUID])
            {
                __instance.CompanyStats.AddStatistic(Mod.Config.CheatDetection.CheatDetectionStat, true);
                Mod.Log.Info?.Write($"CHEATDETECTION: Caught you, you little shit. Cheated money. SGS Funds: {__instance.Funds} while tracker funds {ModState.SimGameFunds[__instance.InstanceGUID]} on dehydrate, pre");

                if (Mod.Config.CheatDetection.CheatDetectionNotify) GenericPopupBuilder.Create("CHEAT DETECTED!", "t-bone thinks you're cheating. if you aren't, you should let the RT crew know on Discord.").AddButton("Okay", null, true, null).CancelOnEscape().AddFader(new UIColorRef?(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill), 0f, true).Render();
            }
            

            var currentPilots = new List<Pilot>(__instance.PilotRoster);
            currentPilots.Add(__instance.Commander);
            foreach (var pilot in currentPilots)
            {
                if (String.IsNullOrEmpty(pilot.GUID)) return;
                if (!ModState.PilotCurrentFreeXP.ContainsKey(pilot.GUID))
                {
                    ModState.PilotCurrentFreeXP.Add(pilot.GUID, pilot.UnspentXP);
                    Mod.Log.Info?.Write($"CHEATDETECTION: {pilot.Description.Id}: Added key to PilotCurrentXP with ExperienceUnspent {pilot.UnspentXP} on dehydrate, pre.");
                }

                if (pilot.UnspentXP != ModState.PilotCurrentFreeXP[pilot.GUID])
                {
                    Mod.Log.Info?.Write($"CHEATDETECTION: {pilot.Description.Id}: pilot UnspentXP was {pilot.UnspentXP} but state variable was {ModState.PilotCurrentFreeXP[pilot.GUID]} on dehydrate, pre.");
                    __instance.CompanyStats.AddStatistic(Mod.Config.CheatDetection.CheatDetectionStat, true);
                    Mod.Log.Info?.Write($"CHEATDETECTION: Caught you, you little shit. Cheated experience.");

                    if (Mod.Config.CheatDetection.CheatDetectionNotify) GenericPopupBuilder
                        .Create("CHEAT DETECTED!",
                            "t-bone thinks you're cheating. if you aren't, you should let the RT crew know on Discord.")
                        .AddButton("Okay", null, true, null).CancelOnEscape()
                        .AddFader(
                            new UIColorRef?(LazySingletonBehavior<UIManager>.Instance
                                .UILookAndColorConstants.PopupBackfill), 0f, true).Render();
                }
            }
        }
    }

}
