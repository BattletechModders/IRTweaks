using BattleTech;
using Harmony;
using IRBTModUtils.Extension;
using IRTweaks.Helper;
using System.Collections.Generic;
using System.Linq;
using us.frostraptor.modUtils;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(Team), "AddUnit")]
    static class SpawnProtection_Team_AddUnit
    {
        static bool Prepare() => Mod.Config.Fixes.SpawnProtection;

        static void Postfix(Team __instance, AbstractActor unit)
        {
            // The added unit is a reinforcement if round > 0
            if (__instance.Combat.TurnDirector.CurrentRound > 1 && Mod.Config.Combat.SpawnProtection.ApplyToReinforcements)
            {
                HostilityMatrix hm = __instance.Combat.HostilityMatrix;

                Mod.Log.Info?.Write($"Checking actor:{CombatantUtils.Label(unit)} that belongs to team:{unit?.team?.Name}");

                List<AbstractActor> actors = new List<AbstractActor>();
                if (hm.IsLocalPlayerEnemy(unit.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToEnemies)
                {
                    actors.Add(unit);
                }
                else if (hm.IsLocalPlayerNeutral(unit.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToNeutrals)
                {
                    actors.Add(unit);
                }
                else if (hm.IsLocalPlayerFriendly(unit.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToAllies)
                {
                    actors.Add(unit);
                }
                else if (unit.team.IsLocalPlayer)
                {
                    actors.Add(unit);
                }
                else
                {
                    Mod.Log.Info?.Write($" -- skipping unknown actor: {unit.DistinctId()}");
                }


                if (actors.Count == 1)
                {
                    Mod.Log.Info?.Write($"Applying protection to reinforcement:{CombatantUtils.Label(unit)}");
                    ProtectionHelper.ProtectActors(actors);
                }
            }
        }
    }

    [HarmonyPatch(typeof(TurnDirector), "BeginNewRound")]
    static class SpawnProtection_TurnDirector_BeginNewRound
    {
        static bool Prepare() => Mod.Config.Fixes.SpawnProtection;

        static void Postfix(TurnDirector __instance)
        {
            Mod.Log.Info?.Write($"Protecting lances on firstContact during first round:{__instance.CurrentRound}");
            if (__instance.CurrentRound == 1)
            {
                Mod.Log.Info?.Write($"Protecting units on first turn");
                CombatGameState combatState = UnityGameInstance.BattleTechGame.Combat;
                HostilityMatrix hm = combatState.HostilityMatrix;
                Team playerTeam = combatState.LocalPlayerTeam;

                // Includes player units
                List<AbstractActor> actors = new List<AbstractActor>();
                foreach (AbstractActor actor in combatState.AllActors)
                {
                    if (hm.IsLocalPlayerEnemy(actor.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToEnemies)
                    {
                        Mod.Log.Info?.Write($" -- adding enemy actor: {actor.DistinctId()}");
                        actors.Add(actor);
                    }
                    else if (hm.IsLocalPlayerNeutral(actor.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToNeutrals)
                    {
                        Mod.Log.Info?.Write($" -- adding neutral actor: {actor.DistinctId()}");
                        actors.Add(actor);
                    }
                    else if (hm.IsLocalPlayerFriendly(actor.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToAllies)
                    {
                        Mod.Log.Info?.Write($" -- adding allied actor: {actor.DistinctId()}");
                        actors.Add(actor);
                    }
                    else if (actor.team.IsLocalPlayer)
                    {
                        Mod.Log.Info?.Write($" -- adding player actor: {actor.DistinctId()}");
                        actors.Add(actor);
                    }
                    else
                    {
                        Mod.Log.Info?.Write($" -- skipping unknown actor: {actor.DistinctId()}");
                    }
                }

                List<AbstractActor> actorsToProtect = actors.Distinct().ToList();

                ProtectionHelper.ProtectActors(actorsToProtect);
            }
        }
    }
}
