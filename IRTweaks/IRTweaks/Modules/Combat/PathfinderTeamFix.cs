using us.frostraptor.modUtils;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(AbstractActor), "IsFriendly")]
    static class AbstractActor_IsFriendly
    {
        static void Prefix(ref bool __runOriginal, AbstractActor __instance, ICombatant target, ref bool __result)
        {
            if (!__runOriginal) return;

            if (__instance.team == null)
            {
                Mod.Log.Info?.Write($"WARNING - could not find team for actor: {CombatantUtils.Label(__instance)} to determine hostility vs: {CombatantUtils.Label(target)}. Returning not friendly!");
                __result = false;
                __runOriginal = false;
            }
        }
    }
}
