using BattleTech;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRTweaks.Modules.Combat
{
    [HarmonyPatch(typeof(TurnDirector), "StartFirstRound")]
    static class TurnDirector_StartFirstRound
    {
        static bool Prepare() => Mod.Config.Fixes.TurnDirectorStartFirstRoundFix;

        // This fix should prevent the StartFirstRound from firing over and over again, which can happen
        //   from multiple-lances. It looks like TurnDirector.OnDropshipAnimationComplete invokes this,
        //   which can ultimately lead to weird state as the first round gets replayed over and over again.
        //   See https://github.com/BattletechModders/IRTweaks/issues/62 for more details.
        static bool Prefix(TurnDirector __instance)
        {
            if (__instance.GameHasBegun) return false;

            return true;
        }
    }
}
