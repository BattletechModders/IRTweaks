using BattleTech;
using Harmony;

namespace IRTweaks.Patches
{
    // Cleanup known combat state
    [HarmonyPatch(typeof(CombatGameState), "OnCombatGameDestroyed")]
    static class CombatGameState_OnCombatGameDestroyed
    {
        static void Postfix(CombatGameState __instance)
        {
            ModState.Reset();
        }
    }
}
