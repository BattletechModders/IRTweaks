
using BattleTech;
using BattleTech.UI;
using System.Collections.Generic;

namespace IRTweaks {

    public static class State {

        public static Dictionary<string, int> CurrentAttackerCalledShotMod = new Dictionary<string, int>();

        public static AbstractActor CurrentAttacker;
        public static Weapon CurrentWeapon;
        public static ICombatant CurrentTarget;

        public static SelectionStateSensorLock SelectionStateSensorLock;

        public static void Reset() {
            // Reinitialize state
            CurrentAttackerCalledShotMod.Clear();

            CurrentAttacker = null;
            CurrentWeapon = null;
            CurrentTarget = null;
        }
    }

}


