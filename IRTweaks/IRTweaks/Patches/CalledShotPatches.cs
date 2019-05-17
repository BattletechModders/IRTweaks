using BattleTech;
using BattleTech.UI;
using Harmony;

namespace IRTweaks {

    [HarmonyPatch(typeof(HUDMechArmorReadout), "SetHoveredArmor")]
    public static class HUDMechArmorReadout_SetHoveredArmor {

        public static void Postfix(HUDMechArmorReadout __instance, ArmorLocation location, Mech ___displayedMech) {
            Mod.Log.Trace("HUDMAR:SHA entered");

            if (__instance.UseForCalledShots && location == ArmorLocation.Head) {
                Mod.Log.Debug("  Attempting to prevent headshot.");
                Statistic allowHeadshotStat = State.CurrentAttacker?.StatCollection.GetStatistic("IRTAllowHeadshot");
                bool allowHeadShot = allowHeadshotStat != null ? allowHeadshotStat.Value<bool>() : false;

                bool canBeTargeted = ___displayedMech.IsShutDown || ___displayedMech.IsProne || allowHeadShot;
                Mod.Log.Trace($"  canBeTargeted:{canBeTargeted} isShutdown:{___displayedMech.IsShutDown} isProne:{___displayedMech.IsProne} allowHeadshot:{allowHeadShot}");

                if (!canBeTargeted) {
                    Mod.Log.Trace("  preventing targeting of head.");
                    __instance.ClearHoveredArmor(ArmorLocation.Head);
                } else {
                    Mod.Log.Trace("  target head can be targeted.");
                }
                
            }
            
        }
    }

}
