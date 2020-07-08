using BattleTech;
using Harmony;
using us.frostraptor.modUtils;

namespace IRTweaks.Modules.Combat {

    [HarmonyPatch(typeof(AbstractActor), "IsFriendly")]
    public static class AbstractActor_IsFriendly {
        public static bool Prefix(AbstractActor __instance, ICombatant target, ref bool __result) {
            if (__instance.team == null) {
                Mod.Log.Info($"WARNING - could not find team for actor: {CombatantUtils.Label(__instance)} to determine hostility vs: {CombatantUtils.Label(target)}. Returning not friendly!");
                __result = false;
                return false;
            } else {
                return true;
            }
        }
    }
}
