using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using Harmony;
using IRBTModUtils;
using Localize;

namespace IRTweaks.Modules.UI
{

    [HarmonyPatch(typeof(SimGameOptionsMenu), "Update")]
    static class SimGameOptionsMenu_Update
    {
        static bool Prepare() => Mod.Config.Fixes.DisableCombatSaves || Mod.Config.Fixes.DisableCombatRestarts;

        static void Postfix(SimGameOptionsMenu __instance, 
            HBSDOTweenButton ___saveGame, HBSTooltipHBSButton ___saveTooltip, 
            HBSDOTweenButton ___restartMission, HBSTooltipHBSButton ___restartTooltip)
        {
            CombatGameState combatGameState = SharedState.Combat;
            if (combatGameState != null && !combatGameState.TurnDirector.IsMissionOver && combatGameState.TurnDirector.GameHasBegun)
            {
                Mod.Log.Trace?.Write("SGOM:CS - in combat.");

                if (Mod.Config.Fixes.DisableCombatRestarts)
                {
                    Mod.Log.Debug?.Write("Disabling combat restarts.");
                    ___restartMission.SetState(ButtonState.Disabled);

                    string title = new Text(Mod.LocalizedText.Tooltips[ModText.TT_CombatRestartMission_Title]).ToString();
                    string details = new Text(Mod.LocalizedText.Tooltips[ModText.TT_CombatRestartMission_Details]).ToString();
                    BaseDescriptionDef def = new BaseDescriptionDef("SGMTipData", title, details, null);
                    ___restartTooltip.SetStateDataForButtonState(ButtonState.Disabled, TooltipUtilities.GetStateDataFromObject(def));
                }

                if (Mod.Config.Fixes.DisableCombatSaves && ___saveGame.State != ButtonState.Disabled)
                {
                    Mod.Log.Debug?.Write("Disabling combat saves.");
                    ___saveGame.SetState(ButtonState.Disabled);

                    string title = new Text(Mod.LocalizedText.Tooltips[ModText.TT_CombatSave_Title]).ToString();
                    string details = new Text(Mod.LocalizedText.Tooltips[ModText.TT_CombatSave_Details]).ToString();
                    BaseDescriptionDef def = new BaseDescriptionDef("SGMTipData", title, details, null);
                    ___saveTooltip.SetStateDataForButtonState(ButtonState.Disabled, TooltipUtilities.GetStateDataFromObject(def));
                }

            }
        }
    }


}
