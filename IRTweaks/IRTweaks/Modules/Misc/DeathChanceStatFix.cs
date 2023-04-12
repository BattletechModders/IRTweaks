using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(Contract), "FinalizeKilledMechWarriors")]
    static class Contract_FinalizeKilledMechWarriors
    {
        [HarmonyPrepare]
        public static bool Prepare() => Mod.Config.Fixes.DeathChanceStat;

        public static float CalcChance(float minchance, float basechance, float gutsReduction, float gutsAmount, SimGameState s, Pilot p)
        {
            float statmodifier = s.CompanyStats.GetValue<float>(p.LethalInjuries ? "LethalDeathChance" : "IncapacitatedDeathChance");
            return Math.Max(minchance, basechance - gutsReduction * gutsAmount + statmodifier);
        }

        /// <summary>
        /// changes
        /// <code>float num = pilot.LethalInjuries ? sim.Constants.Pilot.LethalDeathChance : sim.Constants.Pilot.IncapacitatedDeathChance;
        /// num = Mathf.Max(0f, num - sim.Constants.Pilot.GutsDeathReduction * (float)pilot.Guts);
        /// float num2 = sim.NetworkRandom.Float(0f, 1f);</code>
        /// to
        /// <code>float num = pilot.LethalInjuries ? sim.Constants.Pilot.LethalDeathChance : sim.Constants.Pilot.IncapacitatedDeathChance;
        /// num = CalcChance(0f, num, sim.Constants.Pilot.GutsDeathReduction, (float)pilot.Guts, sim, pilot);
        /// float num2 = sim.NetworkRandom.Float(0f, 1f);</code>
        /// </summary>
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code)
        {
            LinkedList<CodeInstruction> prev = new LinkedList<CodeInstruction>();
            CodeInstruction[] cmp = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Mul),
                new CodeInstruction(OpCodes.Sub),
                new CodeInstruction(OpCodes.Call, "Single Max(Single, Single)"),
            };

            foreach (CodeInstruction c in code)
            {
                prev.AddLast(c);

                if (prev.Count == cmp.Length
                    && (c.opcode == OpCodes.Call))
                {
                    bool match = prev.Zip(cmp, (p, cm) => (p.opcode == cm.opcode) && (cm.operand == null || p.operand.ToString().Equals(cm.operand))).Aggregate((a, b) => a && b);
                    if (match)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Ldloc_2);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(Contract_FinalizeKilledMechWarriors), "CalcChance"));
                        prev.Clear();
                        continue;
                    }
                }

                if (prev.Count == cmp.Length)
                {
                    yield return prev.First.Value;
                    prev.RemoveFirst();
                }
            }
            while (prev.Count > 0)
            {
                yield return prev.First.Value;
                prev.RemoveFirst();
            }
        }
    }
}
