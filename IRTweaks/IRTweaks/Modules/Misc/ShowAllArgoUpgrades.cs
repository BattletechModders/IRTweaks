using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(SGEngineeringScreen), "PopulateUpgradeDictionary")]
    public static class SGEngineeringScreen_PopulateUpgradeDictionary_Patch
    {
        
        public static bool Prepare() => Mod.Config.Fixes.ShowAllArgoUpgrades;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = instructions.ToList();
            var targetMethod = AccessTools.Method(typeof(ShipModuleUpgrade), "get_RequiredModules");

            // want 2nd and final occurence of targetMethod
            var index = codes.FindIndex(c =>
                c == codes.Last(x => x.opcode == OpCodes.Callvirt && (MethodInfo) x.operand == targetMethod));

            // nop out the instructions for the 2nd conditional
            // && this.simState.HasShipUpgrade(shipModuleUpgrade2.RequiredModules, list)
            for (var i = -3; i < 4; i++) {
                codes[index + i].opcode = OpCodes.Nop;
            }

            return codes.AsEnumerable();
        }
    }
}
