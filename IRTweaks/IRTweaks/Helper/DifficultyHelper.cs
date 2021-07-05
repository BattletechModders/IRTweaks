using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;

namespace IRTweaks.Helper
{
    class DifficultyHelper
    {
        public static void GetDifficultyModifierRange(SimGameDifficulty difficulty)
        {
            var diffSettings = new List<SimGameDifficulty.DifficultySetting>(difficulty.GetSettings());

            foreach (var setting in diffSettings)
            {
                var ordered = setting.Options.OrderBy(x=>x.CareerScoreModifier).ToList();
                //Mod.Log.Info?.Write($"Sorted options: {setting.Options.First().Name} is first with {setting.Options.First().CareerScoreModifier}, {setting.Options.Last().Name} is last with {setting.Options.Last().CareerScoreModifier}");
                ModState.MinDiffModifier += ordered.First().CareerScoreModifier;
                ModState.MaxDiffModifier += ordered.Last().CareerScoreModifier;
            }

            Mod.Log.Info?.Write($"Min Difficulty Modifier: {ModState.MinDiffModifier}. Final Max Difficulty Modifier: {ModState.MaxDiffModifier}");
        }
    }
}
