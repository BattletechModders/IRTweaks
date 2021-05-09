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
        static bool Prepare() => Mod.Config.Fixes.CheatDetection && false; //disabled for now

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
        static bool Prepare() => Mod.Config.Fixes.CheatDetection && false; //disabled for now
        public static void Postfix(Mech __instance)
        {
            
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (!ModState.UnitStartingArmor.ContainsKey(__instance.GUID))
            {
                ModState.UnitStartingArmor.Add(__instance.GUID, __instance.CurrentArmor);
                Mod.Log.Info?.Write($"Added key to UnitStartingArmor with CurrentArmor... this should have been done already though.");
            }

            if (!ModState.UnitCurrentArmor.ContainsKey(__instance.GUID))
            {
                ModState.UnitCurrentArmor.Add(__instance.GUID, __instance.CurrentArmor);
                Mod.Log.Info?.Write($"Added key to UnitStartingArmor with CurrentArmor... this should have been done already though.");
            }

            if (__instance.CurrentArmor > ModState.UnitCurrentArmor[__instance.GUID])
            {
                sim.CompanyStats.AddStatistic("CheaterCheaterPumpkinEater", true);
                Mod.Log.Info?.Write($"Caught you, you little shit. Cheated armor.");
            }

            if (__instance.CurrentArmor > ModState.UnitStartingArmor[__instance.GUID])
            {
                sim.CompanyStats.AddStatistic("CheaterCheaterPumpkinEater", true);
                Mod.Log.Info?.Write($"Caught you, you little shit. Cheated armor.");
            }

            ModState.UnitCurrentArmor[__instance.GUID] = __instance.CurrentArmor;
        }
    }

        [HarmonyPatch(typeof(Mech))]
    [HarmonyPatch("_heat", MethodType.Setter)]
    public static class Mech__heat_Setter
    {
        static bool Prepare() => Mod.Config.Fixes.CheatDetection && false; //disabled for now

        public static void Postfix(Mech __instance, int value)
        {
            if (!ModState.UnitCurrentHeat.ContainsKey(__instance.GUID))
            {
                ModState.UnitCurrentHeat.Add(__instance.GUID, __instance.CurrentHeat);
                Mod.Log.Info?.Write($"Added key to UnitCurrentHeat with CurrentHeat value... this should have been done already though.");
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
        static bool Prepare() => Mod.Config.Fixes.CheatDetection && false; //disabled for now. this is the one that breaks spawning and CTDs with no errors for some reason.

        public static void Postfix(Mech __instance)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (!ModState.UnitCurrentHeat.ContainsKey(__instance.GUID))
            {
                ModState.UnitCurrentHeat.Add(__instance.GUID, __instance.CurrentHeat);
                Mod.Log.Info?.Write($"Added key to UnitCurrentHeat with CurrentHeat value... this should have been done already though.");
            }
            if (__instance.CurrentHeat != ModState.UnitCurrentHeat[__instance.GUID])
            {
                sim.CompanyStats.AddStatistic("CheaterCheaterPumpkinEater", true);
                Mod.Log.Info?.Write($"Caught you, you little shit. Cheated heat.");
            }
        }
    }

    [HarmonyPatch(typeof(CombatGameState))]
    [HarmonyPatch("OnCombatGameDestroyed")]
    public static class CombatGameState_OnCGSDestroyed
    {
        static bool Prepare() => Mod.Config.Fixes.CheatDetection && false; //disabled for now
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
        static bool Prepare() => Mod.Config.Fixes.CheatDetection && false;

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
        static bool Prepare() => Mod.Config.Fixes.CheatDetection && false;

        public static void Postfix(PilotDef __instance)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (String.IsNullOrEmpty(__instance.Description.Id)) return;
            if (!ModState.PilotDefCurrentFreeXP.ContainsKey(__instance.Description.Id))
            {
                ModState.PilotDefCurrentFreeXP.Add(__instance.Description.Id, __instance.ExperienceUnspent);
                Mod.Log.Info?.Write($"Added key to PilotCurrentXP with ExperienceUnspent value.");
            }
        }
    }


    [HarmonyPatch(typeof(Pilot))]
    [HarmonyPatch("FromPilotDef")]
    public static class Pilot_FromPilotDef
    {
        static bool Prepare() => Mod.Config.Fixes.CheatDetection;

        public static void Postfix(Pilot __instance)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (String.IsNullOrEmpty(__instance.GUID)) return;
            if (!ModState.PilotCurrentFreeXP.ContainsKey(__instance.GUID))
            {
                ModState.PilotCurrentFreeXP.Add(__instance.GUID, __instance.UnspentXP);
                Mod.Log.Info?.Write($"{__instance.Description.Id}: Set ExperienceUnspent to UnspentXP value.");
            }

            ModState.PilotCurrentFreeXP[__instance.GUID]  = __instance.pilotDef.ExperienceUnspent;
        }
    }

    [HarmonyPatch(typeof(Pilot))]
    [HarmonyPatch("AddExperience")]
    public static class Pilot_AddXP
    {
        static bool Prepare() => Mod.Config.Fixes.CheatDetection;

        public static void Postfix(Pilot __instance, int stackID, string sourceID, int value)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (String.IsNullOrEmpty(__instance.GUID)) return;
            if (!ModState.PilotCurrentFreeXP.ContainsKey(__instance.GUID))
            {
                ModState.PilotCurrentFreeXP.Add(__instance.GUID, __instance.UnspentXP);
                Mod.Log.Info?.Write($"{__instance.Description.Id}: Added key to PilotCurrentXP with UnspentXP value.");
            }

            ModState.PilotCurrentFreeXP[__instance.GUID] += value;
        }
    }

    [HarmonyPatch(typeof(Pilot))]
    [HarmonyPatch("SpendExperience")]
    public static class Pilot_SpendXP
    {
        static bool Prepare() => Mod.Config.Fixes.CheatDetection;

        public static void Prefix(Pilot __instance)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (String.IsNullOrEmpty(__instance.GUID)) return;
            if (!ModState.PilotCurrentFreeXP.ContainsKey(__instance.GUID))
            {
                ModState.PilotCurrentFreeXP.Add(__instance.GUID, __instance.UnspentXP);
                Mod.Log.Info?.Write($"{__instance.Description.Id}: Added key to PilotCurrentXP with UnspentXP value.");
            }
        }

        public static void Postfix(Pilot __instance, int stackID, string sourceID, uint value)
        {
            var sim = UnityGameInstance.BattleTechGame.Simulation;
            if (sim == null) return;
            if (String.IsNullOrEmpty(__instance.GUID)) return;
            if (!ModState.PilotCurrentFreeXP.ContainsKey(__instance.GUID))
            {
                ModState.PilotCurrentFreeXP.Add(__instance.GUID, __instance.UnspentXP);
                Mod.Log.Info?.Write($"{__instance.Description.Id}: Added key to PilotCurrentXP with UnspentXP value.");
            }

            ModState.PilotCurrentFreeXP[__instance.GUID] -= (int)value;
            if (__instance.UnspentXP != ModState.PilotCurrentFreeXP[__instance.GUID])
            {
                sim.CompanyStats.AddStatistic("CheaterCheaterPumpkinEater", true);
                Mod.Log.Info?.Write($"Caught you, you little shit. Cheated experience.");
                GenericPopupBuilder.Create("CHEAT DETECTED!", "t-bone thinks you're cheating. if you aren't, you should let the RT crew know on Discord.").AddButton("Okay", null, true, null).CancelOnEscape().AddFader(new UIColorRef?(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill), 0f, true).Render();
            }
        }
    }



    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("AddFunds")]
    public static class SimGameState_AddFunds
    {
        static bool Prepare() => Mod.Config.Fixes.CheatDetection;

        public static void Prefix(SimGameState __instance, int val)
        {
            if (String.IsNullOrEmpty(__instance.InstanceGUID)) return;
            if (!ModState.SimGameFunds.ContainsKey(__instance.InstanceGUID))
            {
                ModState.SimGameFunds.Add(__instance.InstanceGUID, __instance.Funds);
                Mod.Log.Info?.Write($"Added key to SimGameFunds with Funds value; this should have been done already...WTF");
            }
            if (__instance.Funds != ModState.SimGameFunds[__instance.InstanceGUID])
            {
                __instance.CompanyStats.AddStatistic("CheaterCheaterPumpkinEater", true);
                Mod.Log.Info?.Write($"Caught you, you little shit. Cheated money.");
            }
        }

        public static void Postfix(SimGameState __instance, int val)
        {
            if (!ModState.SimGameFunds.ContainsKey(__instance.InstanceGUID))
            {
                ModState.SimGameFunds.Add(__instance.InstanceGUID, __instance.Funds);
                Mod.Log.Info?.Write($"Added key to SimGameFunds with Funds value; this should have been done already...WTF");
            }
            ModState.SimGameFunds[__instance.InstanceGUID] += val;
            if (__instance.Funds != ModState.SimGameFunds[__instance.InstanceGUID])
            {
                __instance.CompanyStats.AddStatistic("CheaterCheaterPumpkinEater", true);
                Mod.Log.Info?.Write($"Caught you, you little shit. Cheated money.");
                GenericPopupBuilder.Create("CHEAT DETECTED!", "t-bone thinks you're cheating. if you aren't, you should let the RT crew know on Discord.").AddButton("Okay", null, true, null).CancelOnEscape().AddFader(new UIColorRef?(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill), 0f, true).Render();
            }
        }

    }


    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("Rehydrate")]
    public static class SimGameState_Rehydrate_CH
    {
        static bool Prepare() => Mod.Config.Fixes.CheatDetection;

        public static void Postfix(SimGameState __instance)
        {
            if (!ModState.SimGameFunds.ContainsKey(__instance.InstanceGUID))
            {
                ModState.SimGameFunds.Add(__instance.InstanceGUID, __instance.Funds);
                Mod.Log.Info?.Write($"Added key to SimGameFunds with Funds value.");
            }

            var currentPilots = new List<Pilot>(__instance.PilotRoster);
            currentPilots.Add(__instance.Commander);
            foreach (var pilot in currentPilots)
            {
                if (String.IsNullOrEmpty(pilot.GUID)) return;
                if (!ModState.PilotCurrentFreeXP.ContainsKey(pilot.GUID))
                {
                    ModState.PilotCurrentFreeXP.Add(pilot.GUID, pilot.UnspentXP);
                    Mod.Log.Info?.Write($"{pilot.Description.Id}: Added key to PilotCurrentXP with ExperienceUnspent value.");
                }
            }
        }
    }

}
