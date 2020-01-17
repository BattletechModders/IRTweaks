
using BattleTech.UI;
using System.Collections.Generic;

namespace IRTweaks {

    public static class ModState {

        public static Dictionary<string, int> PilotCalledShotModifiers = new Dictionary<string, int>();
        public static SelectionStateSensorLock SelectionStateSensorLock;

        public static void Reset() {
            // Reinitialize state
            PilotCalledShotModifiers.Clear();

        }
    }

}


