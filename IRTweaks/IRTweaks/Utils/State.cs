
using BattleTech;
using BattleTech.UI;
using IRTweaks.Modules.Combat;
using IRTweaks.Modules.StoreUI;
using IRTweaks.Modules.Tooltip;
using System.Collections.Generic;

namespace IRTweaks {

    public static class State {

        public static Dictionary<string, int> PilotCalledShotModifiers = new Dictionary<string, int>();
        public static SelectionStateSensorLock SelectionStateSensorLock;

        public static void Reset() {
            // Reinitialize state
            PilotCalledShotModifiers.Clear();

        }
    }

}


