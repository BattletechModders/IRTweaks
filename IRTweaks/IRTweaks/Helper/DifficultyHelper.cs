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
            var diffSettings = difficulty.GetSettings();

            foreach (var setting in diffSettings)
            {
                setting.Options.Sort(delegate(SimGameDifficulty.DifficultyOption a, SimGameDifficulty.DifficultyOption b)
                {
                    if (a.CareerScoreModifier != b.CareerScoreModifier)
                    {
                        return a.CareerScoreModifier.CompareTo(b.CareerScoreModifier);
                    }
                    return a.Name.CompareTo(b.Name);
                });
                //Mod.Log.Info?.Write($"Sorted options: {setting.Options.First().Name} is first with {setting.Options.First().CareerScoreModifier}, {setting.Options.Last().Name} is last with {setting.Options.Last().CareerScoreModifier}");
                ModState.MinDiffModifier += setting.Options.First().CareerScoreModifier;
                ModState.MaxDiffModifier += setting.Options.Last().CareerScoreModifier;
            }

            Mod.Log.Info?.Write($"Min Difficulty Modifier: {ModState.MinDiffModifier}. Final Max Difficulty Modifier: {ModState.MaxDiffModifier}");
        }
    }
}
