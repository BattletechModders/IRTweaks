using System.Collections.Generic;

namespace IRTweaks
{
    public class ModText
    {

        public const string DT_Title_ScrapAll = "SCRAP_ALL_TITLE";
        public const string DT_Title_ScrapAssaults = "SCRAP_ALL_ASSAULT";
        public const string DT_Title_ScrapHeavies = "SCRAP_ALL_HEAVY";
        public const string DT_Title_ScrapLights = "SCRAP_ALL_LIGHT";
        public const string DT_Title_ScrapMediums = "SCRAP_ALL_MEDIUM";

        public const string DT_Desc_ScrapAll = "SCRAP_ALL_DESC";
        public const string DT_Button_Cancel = "BUTTON_CANCEL";
        public const string DT_Button_Scrap = "BUTTON_SCRAP";

        public Dictionary<string, string> Dialog = new Dictionary<string, string>
        {
            { DT_Title_ScrapAll, "SCRAP ALL IN STORAGE" },
            { DT_Title_ScrapAssaults, "SCRAP ASSAULTS IN STORAGE" },
            { DT_Title_ScrapHeavies, "SCRAP HEAVIES IN STORAGE" },
            { DT_Title_ScrapLights, "SCRAP LIGHTS IN STORAGE" },
            { DT_Title_ScrapMediums, "SCRAP MEDIUMS IN STORAGE" },

            { DT_Desc_ScrapAll, "This will scrap units currently in storage, gaining {0} c-bills. This action cannot be reversed, you will need to load an earlier save. Are you sure?" },

            { DT_Button_Cancel, "CANCEL" },
            { DT_Button_Scrap, "SCRAP" }

        };

        public const string LT_MechBay_Confirm_Text = "MECHBAY_CONFIRM_TEXT";

        public Dictionary<string, string> Labels = new Dictionary<string, string>
        {
            { LT_MechBay_Confirm_Text, "Validate" }
        };

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
