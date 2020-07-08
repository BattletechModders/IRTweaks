using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using us.frostraptor.modUtils;

namespace IRTweaks {

    public static class PainHelper {

        public static float CalculateOverheatRatio(Mech mech) {
            int overheatLevel = mech.OverheatLevel;
            int maxHeat = mech.MaxHeat;
            int overheatRange = maxHeat - overheatLevel;

            int currentHeat = mech.CurrentHeat;
            int currentOverheat = currentHeat - overheatLevel;
            Mod.Log.Debug($"Actor:{mech.DisplayName}_{mech?.pilot?.Name} has maxHeat:{maxHeat}, overheat:{overheatLevel}, currentHeat:{currentHeat}");

            float overheatRatio = (float)currentOverheat / (float)overheatRange;
            Mod.Log.Debug($"overheatRatio:{overheatRatio}% = currentOverheat:{currentOverheat} / overheatRange:{overheatRange}");
            return overheatRatio * 100f;
        }

        public static bool MakeResistCheck(Pilot pilot) {
            int normalizedGunnery = SkillUtils.GetGunneryModifier(pilot);
            float baseResist = normalizedGunnery * Mod.Config.Combat.PainTolerance.ResistPerGuts;
            float resistPenalty = ModState.InjuryResistPenalty;
            float resistChance = Math.Max(0, baseResist - resistPenalty);
            Mod.Log.Debug($"baseResist:{baseResist} - resistPenalty:{resistPenalty} = resistChance:{resistChance}");

            int check = Mod.Random.Next(0, 100);
            bool success = resistChance >= check;
            if (success) {
                Mod.Log.Info($"Pilot:{pilot?.Name} resisted injury with check:{check} <= resistChance:{resistChance}");
            } else {
                Mod.Log.Debug($"Pilot failed to resist injury with check:{check} > resistChance:{resistChance}");
            }

            return success;
        }
    }

}
