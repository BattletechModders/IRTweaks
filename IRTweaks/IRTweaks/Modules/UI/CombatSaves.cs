using BattleTech;
using BattleTech.Save.SaveGameStructure;
using BattleTech.UI;
using BattleTech.UI.Tooltips;

namespace IRTweaks.Modules.UI {
    public static class CombatSaves {

        public static void SimGameOptionsMenu_CanSave_Postfix(GameInstance gameInstance, SaveReason saveReason, bool ironmanPreventsSave, bool logDetails, ref bool __result) {
            CombatGameState combatGameState = UnityGameInstance.BattleTechGame.Combat;
            if (combatGameState != null && !combatGameState.TurnDirector.IsMissionOver && combatGameState.TurnDirector.GameHasBegun) {
                Mod.Log.Trace("SGOM:CS - in combat.");
                __result = false;
            } else {
                Mod.Log.Trace("SGOM:CS - outside of combat.");
            }
        }

        public static void SimGameOptionsMenu_SetSaveTooltip_Postfix(SimGameOptionsMenu __instance, HBSTooltipHBSButton ___saveTooltip, SaveReason ___reason, HBSDOTweenButton ___saveGame) {
            if (___saveTooltip == null) {
                return;
            }

            GameInstance battleTechGame = UnityGameInstance.BattleTechGame;
            CombatGameState combat = battleTechGame.Combat;
            if (combat != null && !combat.TurnDirector.IsMissionOver && combat.TurnDirector.GameHasBegun) {
                string details = "Saving during combat missions disabled to prevent errors and bugs.";
                BaseDescriptionDef def = new BaseDescriptionDef("SGMTipData", "Unable to Save", details, null);
                ___saveTooltip.SetStateDataForButtonState(ButtonState.Disabled, TooltipUtilities.GetStateDataFromObject(def));
            }
        }

    }
}
