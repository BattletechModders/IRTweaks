
using BattleTech;
using BattleTech.UI;
using IRTweaks.Modules.StoreUI;
using System.Collections.Generic;

namespace IRTweaks {

    public static class State {

        public static Dictionary<string, int> PilotCalledShotModifiers = new Dictionary<string, int>();

        public static SelectionStateSensorLock SelectionStateSensorLock;



        public static void Reset() {
            // Reinitialize state
            PilotCalledShotModifiers.Clear();

            // Reset module state
            StoreUIModule.State.Reset();
        }
    }

}


