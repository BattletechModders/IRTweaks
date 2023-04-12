using BattleTech.Save;
using System.Collections.Generic;

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(SimGameState), "Rehydrate")]
    public static class SimGameState_Rehydrate
    {
        static bool Prepare() => Mod.Config.Fixes.FactionValueFix;

        public static void Postfix(SimGameState __instance, GameInstanceSave gameInstanceSave)
        {
            var save = gameInstanceSave.SimGameSave;
            if (save.IgnoredContractTargets == null) return;
            __instance.ignoredContractTargets = new List<string>();
            foreach (var factionID2 in save.IgnoredContractTargets)
            {
                var factionValue = FactionEnumeration.GetFactionByID(factionID2);
                if (!factionValue.IsCareerIgnoredContractTarget)
                {
                    Mod.Log.Info?.Write($"FactionValueFix: {factionValue.Name} is no longer IsCareerIgnoredContractTarget = true, removing from ignoredContractTargets.");
                    __instance.ignoredContractTargets.RemoveAll(x => x == factionValue.Name);
                }
            }
        }
    }
}
