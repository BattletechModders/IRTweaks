using BattleTech;
using Harmony;
using System.Collections.Generic;
using System.Linq;
using us.frostraptor.modUtils;

namespace IRTweaks.Helper {
    public static class ProtectionHelper {
        public static void ProtectOnFirstRound() {
            Mod.Log.Info($"Protecting units on first turn'");
            CombatGameState combatState = UnityGameInstance.BattleTechGame.Combat;
            HostilityMatrix hm = combatState.HostilityMatrix;
            Team playerTeam = combatState.LocalPlayerTeam;

            // Includes player units
            List<AbstractActor> actors = new List<AbstractActor>();
            foreach (AbstractActor actor in combatState.AllActors) {
                if (hm.IsLocalPlayerEnemy(actor.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToEnemies) {
                    actors.Add(actor);
                } else if (hm.IsLocalPlayerNeutral(actor.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToNeutrals) {
                    actors.Add(actor);
                } else if (hm.IsLocalPlayerFriendly(actor.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToAllies) {
                    actors.Add(actor);
                }
            }
            actors.AddRange(playerTeam.units);
            List<AbstractActor> actorsToProtect = actors.Distinct().ToList();

            ProtectActors(actorsToProtect);
        }

        public static void ProtectActors(List<AbstractActor> actorsToProtect) {
            if (Mod.Config.Combat.SpawnProtection.ApplyGuard) {
                BraceAll(actorsToProtect);
            }

            if (Mod.Config.Combat.SpawnProtection.EvasionPips > 0) {
                AddEvasion(actorsToProtect);
            }
        }

        public static void BraceAll(List<AbstractActor> actors) {
            foreach (AbstractActor actor in actors) {
                // Turrets don't get protection
                if (actor.GetType() != typeof(Turret)) {
                    Mod.Log.Info($"Applying braced state to actor:{CombatantUtils.Label(actor)} of type:{actor.GetType()}");
                    actor.ApplyBraced();
                }

            }
        }

        public static void AddEvasion(List<AbstractActor> actors) {
            int evasionToAdd = Mod.Config.Combat.SpawnProtection.EvasionPips;
            CombatGameState combatState = UnityGameInstance.BattleTechGame.Combat;

            foreach (AbstractActor actor in actors) {
                // Turrets don't get protection
                if (actor.GetType() != typeof(Turret)) {
                    Mod.Log.Info($"Adding '{evasionToAdd}' evasion pips to actor:{CombatantUtils.Label(actor)} of type:{actor.GetType()}");

                    actor.EvasivePipsCurrent += evasionToAdd;
                    AccessTools.Property(typeof(AbstractActor), "EvasivePipsTotal").SetValue(actor, actor.EvasivePipsCurrent, null);
                    combatState.MessageCenter.PublishMessage(new EvasiveChangedMessage(actor.GUID, actor.EvasivePipsCurrent));

                }
            }
        }
    }
}
