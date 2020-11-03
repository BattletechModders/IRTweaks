using System.Collections.Generic;

namespace IRTweaks
{
    public class ModText
    {
        public const string FT_InjuryResist = "INJURY_RESIST";

        public Dictionary<string, string> Floaties = new Dictionary<string, string>
        {
            { FT_InjuryResist, "INJURY RESISTED!" },
        };

        public const string Mod_CalledShot = "CALLED_SHOT";

        public Dictionary<string, string> Modifiers = new Dictionary<string, string>
        {
            { Mod_CalledShot, "Called Shot" }
        };

        public const string TT_CombatSave_Title = "COMBAT_SAVE_TITLE";
        public const string TT_CombatSave_Details = "COMBAT_SAVE_DETAILS";

        public const string TT_CombatRestartMission_Title = "COMBAT_RESTART_TITLE";
        public const string TT_CombatRestartMission_Details = "COMBAT_RESTART_DETAILS";

        public Dictionary<string, string> Tooltips = new Dictionary<string, string>
        {
            { TT_CombatSave_Title, "Combat Saves Disabled" },
            { TT_CombatSave_Details, "Saving during combat is disabled to prevent errors in modded games." },

            { TT_CombatRestartMission_Title, "Mission Restarts Disabled" },
            { TT_CombatRestartMission_Details, "Restarting a missing during combat is disabled to prevent errors in modded games. Restarting often leads to corruption at the salvage screen." }
        };

        public const string SGDS_Desc = "DESCRIPTION";
        public const string SGDS_Label = "LABEL";
        public Dictionary<string, string> SimGameDifficultyStrings = new Dictionary<string, string>()
        {
            { SGDS_Desc, "Overall Difficulty" },
            { SGDS_Label, "Difficulty" },
        };
    }
}
