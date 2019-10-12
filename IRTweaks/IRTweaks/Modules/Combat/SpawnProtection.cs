using BattleTech;
using IRTweaks.Helper;
using System.Collections.Generic;
using us.frostraptor.modUtils;

namespace IRTweaks.Modules.Combat {
    public static class SpawnProtection {
        public static object CombatantHelper { get; private set; }

        public static void Team_AddUnit_Postfix(Team __instance, AbstractActor unit) {
            // The added unit is a reinforcement if round > 0
            if (__instance.Combat.TurnDirector.CurrentRound > 1 && Mod.Config.Combat.SpawnProtection.ApplyToReinforcements) {
                HostilityMatrix hm = __instance.Combat.HostilityMatrix;

                Mod.Log.Info($"Checking actor:{CombatantUtils.Label(unit)} that belongs to team:{unit?.team?.Name}");

                List<AbstractActor> actor = new List<AbstractActor>();
                if (hm.IsLocalPlayerEnemy(unit.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToEnemies) {
                    actor.Add(unit);
                } else if (hm.IsLocalPlayerNeutral(unit.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToNeutrals) {
                    actor.Add(unit);
                } else if (hm.IsLocalPlayerFriendly(unit.TeamId) && Mod.Config.Combat.SpawnProtection.ApplyToAllies) {
                    actor.Add(unit);
                }

                if (actor.Count == 1) {
                    Mod.Log.Info($"Applying protection to reinforcement:{CombatantUtils.Label(unit)}");
                    ProtectionHelper.ProtectActors(actor);
                }
            }
        }

        public static void TurnDirector_BeginNewRound_Postfix(TurnDirector __instance) {
            Mod.Log.Info($"Protecting lances on firstContact during first round:{__instance.CurrentRound}");
            if (__instance.CurrentRound == 1) {
                ProtectionHelper.ProtectOnFirstRound();
            }
        }
    }
}
