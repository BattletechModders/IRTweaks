using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BattleTech.Save;
using BattleTech.Save.Core;
using Harmony;

// ReSharper disable InconsistentNaming

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(SaveBlock<GameInstanceSave>), "CompressBytes")]
    public static class SaveBlockGameInstanceSave_CompressBytes_Patch
    {
        public static bool Prepare() =>  Mod.Config.Fixes.ReduceSaveCompression;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_S &&
                    codes[i].operand is sbyte intOperand &&
                    intOperand == 9)
                {
                    codes[i].operand = 6;
                }
            }

            return codes.AsEnumerable();
        }
    }
}
