using BattleTech;
using System;
using us.frostraptor.modUtils;

namespace IRTweaks {

    public static class PainHelper {

        public static float CalculateOverheatRatio(Mech mech) {
            int overheatLevel = mech.OverheatLevel;
            int maxHeat = mech.MaxHeat;
            int overheatRange = maxHeat - overheatLevel;

            int currentHeat = mech.CurrentHeat;
            int currentOverheat = currentHeat - overheatLevel;
            Mod.Log.Debug?.Write($"Actor:{mech.DisplayName}_{mech?.pilot?.Name} has maxHeat:{maxHeat}, overheat:{overheatLevel}, currentHeat:{currentHeat}");

            float overheatRatio = (float)currentOverheat / (float)overheatRange;
            Mod.Log.Debug?.Write($"overheatRatio:{overheatRatio}% = currentOverheat:{currentOverheat} / overheatRange:{overheatRange}");
            return overheatRatio * 100f;
        }

        public static bool MakeResistCheck(Pilot pilot) {
            int normalizedGunnery = SkillUtils.GetGunneryModifier(pilot);
            float baseResist = normalizedGunnery * Mod.Config.Combat.PainTolerance.ResistPerGuts;
            float resistPenalty = ModState.InjuryResistPenalty;
            float resistChance = Math.Max(0, baseResist - resistPenalty);
            Mod.Log.Debug?.Write($"baseResist:{baseResist} - resistPenalty:{resistPenalty} = resistChance:{resistChance}");

            int check = Mod.Random.Next(0, 100);
            bool success = resistChance >= check;
            if (success) {
                Mod.Log.Info?.Write($"Pilot:{pilot?.Name} resisted injury with check:{check} <= resistChance:{resistChance}");
            } else {
                Mod.Log.Info?.Write($"Pilot failed to resist injury with check:{check} > resistChance:{resistChance}");
            }

            return success;
        }
    }

}
