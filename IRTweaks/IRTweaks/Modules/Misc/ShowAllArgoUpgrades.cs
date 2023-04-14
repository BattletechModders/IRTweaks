using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace IRTweaks.Modules.Misc
{
    // Shows all possible Argo updates, but greys them out
    [HarmonyPatch(typeof(SGEngineeringScreen), "PopulateUpgradeDictionary")]
    public static class SGEngineeringScreen_PopulateUpgradeDictionary_Patch
    {

        public static bool Prepare() => Mod.Config.Fixes.ShowAllArgoUpgrades;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            MethodInfo targetMethod = AccessTools.Method(typeof(ShipModuleUpgrade), "get_RequiredModules");

            // want 2nd and final occurence of targetMethod
            int index = codes.FindIndex(c =>
                c == codes.Last(x => x.opcode == OpCodes.Callvirt && (MethodInfo)x.operand == targetMethod));

            // nop out the instructions for the 2nd conditional
            // && this.simState.HasShipUpgrade(shipModuleUpgrade2.RequiredModules, list)
            for (int i = -3; i < 4; i++)
            {
                codes[index + i].opcode = OpCodes.Nop;
            }

            return codes.AsEnumerable();
        }
    }
}
