using BattleTech;
using Harmony;
using IRBTModUtils;
using IRBTModUtils.Extension;
using System.Collections.Generic;
using System.Linq;
using us.frostraptor.modUtils;

namespace IRTweaks.Helper
{
    public static class ProtectionHelper
    {
        public static void ProtectOnFirstRound()
        {
            Mod.Log.Info?.Write($"Protecting units on first turn'");
            CombatGameState combatState = UnityGameInstance.BattleTechGame.Combat;
            HostilityMatrix hm = combatState.HostilityMatrix;
            Team playerTeam = combatState.LocalPlayerTeam;

            // Includes player units
            List<AbstractActor> actors = new List<AbstractActor>();
            foreach (AbstractActor actor in combatState.AllActors)
            {
                if (hm.IsLocalPlayerEnemy(actor.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToEnemies)
                {
                    actors.Add(actor);
                }
                else if (hm.IsLocalPlayerNeutral(actor.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToNeutrals)
                {
                    actors.Add(actor);
                }
                else if (hm.IsLocalPlayerFriendly(actor.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToAllies)
                {
                    actors.Add(actor);
                }
            }
            actors.AddRange(playerTeam.units);
            List<AbstractActor> actorsToProtect = actors.Distinct().ToList();

            ProtectActors(actorsToProtect);
        }

        public static void ProtectActors(List<AbstractActor> actorsToProtect)
        {

            foreach (AbstractActor actor in actorsToProtect)
            {
                if (actor.GetType() != typeof(Turret))
                {
                    Mod.Log.Debug?.Write($" Spawn protection skipping turret:{CombatantUtils.Label(actor)}");
                    continue;
                }

                if (!ModState.SpawnProtectedUnits.Contains(actor.GUID))
                {
                    Mod.Log.Info?.Write($" Spawn protecting actor: {actor.DistinctId()}");
                    if (Mod.Config.Combat.SpawnProtection.ApplyGuard)
                    {
                        Mod.Log.Info?.Write($" -- applying Braced state");
                        actor.ApplyBraced();
                    }

                    if (Mod.Config.Combat.SpawnProtection.EvasionPips > 0)
                    {
                        Mod.Log.Info?.Write($" -- setting evasion pips to: {Mod.Config.Combat.SpawnProtection.EvasionPips}");
                        actor.EvasivePipsCurrent = Mod.Config.Combat.SpawnProtection.EvasionPips;
                        AccessTools.Property(typeof(AbstractActor), "EvasivePipsTotal").SetValue(actor, actor.EvasivePipsCurrent, null);
                        SharedState.Combat.MessageCenter.PublishMessage(new EvasiveChangedMessage(actor.GUID, actor.EvasivePipsCurrent));
                    }
                }
                else
                {
                    Mod.Log.Info?.Write($"Actor: {actor.DistinctId()} already protected, skipping.");
                }
            }
        }
    }
}
