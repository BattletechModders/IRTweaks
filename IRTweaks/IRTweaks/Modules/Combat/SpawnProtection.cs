using BattleTech;
using Harmony;
using IRTweaks.Helper;
using System.Collections.Generic;
using us.frostraptor.modUtils;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(Team), "AddUnit")]
    static class SpawnProtection_Team_AddUnit
    {
        static bool Prepare => Mod.Config.Fixes.SpawnProtection;

        static void Postfix(Team __instance, AbstractActor unit)
        {
            // The added unit is a reinforcement if round > 0
            if (__instance.Combat.TurnDirector.CurrentRound > 1 && Mod.Config.Combat.SpawnProtection.ApplyToReinforcements)
            {
                HostilityMatrix hm = __instance.Combat.HostilityMatrix;

                Mod.Log.Info?.Write($"Checking actor:{CombatantUtils.Label(unit)} that belongs to team:{unit?.team?.Name}");

                List<AbstractActor> actor = new List<AbstractActor>();
                if (hm.IsLocalPlayerEnemy(unit.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToEnemies)
                {
                    actor.Add(unit);
                }
                else if (hm.IsLocalPlayerNeutral(unit.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToNeutrals)
                {
                    actor.Add(unit);
                }
                else if (hm.IsLocalPlayerFriendly(unit.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToAllies)
                {
                    actor.Add(unit);
                }

                if (actor.Count == 1)
                {
                    Mod.Log.Info?.Write($"Applying protection to reinforcement:{CombatantUtils.Label(unit)}");
                    ProtectionHelper.ProtectActors(actor);
                }
            }
        }
    }

    [HarmonyPatch(typeof(TurnDirector), "BeginNewRound")]
    static class SpawnProtection_TurnDirector_BeginNewRound
    {
        static bool Prepare => Mod.Config.Fixes.SpawnProtection;

        static void Postfix(TurnDirector __instance)
        {
            Mod.Log.Info?.Write($"Protecting lances on firstContact during first round:{__instance.CurrentRound}");
            if (__instance.CurrentRound == 1)
            {
                ProtectionHelper.ProtectOnFirstRound();
            }
        }
    }
}
