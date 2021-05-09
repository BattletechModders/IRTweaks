using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(SimGameState), "AddPilotToRoster")]
    static class SimGameState_AddPilotToRoster
    {
        static bool Prefix(SimGameState __instance, PilotDef pilotDef)
        {
            if (__instance.PilotRoster.Any(x=>x.pilotDef.Description.Id == pilotDef.Description.Id))
            {
                Mod.Log.Info?.Write($"Pilot {pilotDef.Description.Id} already in Roster! Not adding to Roster!");
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(SimGameState), "AddPilotToHiringHall")]
    static class SimGameState_AddPilotToHiringHall
    {
        static bool Prefix(SimGameState __instance, PilotDef pilotDef)
        {
            if (__instance.PilotRoster.Any(x=>x.pilotDef.Description.Id == pilotDef.Description.Id))
            {
                Mod.Log.Info?.Write($"Pilot {pilotDef.Description.Id} already in Roster! Not adding to Hiring Hall!");
                return false;
            }
            return true;
        }
    }
}
