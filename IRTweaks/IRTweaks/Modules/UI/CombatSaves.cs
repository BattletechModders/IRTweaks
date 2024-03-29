﻿using BattleTech.UI.Tooltips;
using IRBTModUtils;
using Localize;
using System;

namespace IRTweaks.Modules.UI
{

    [HarmonyPatch(typeof(SimGameOptionsMenu), "Update")]
    static class SimGameOptionsMenu_Update
    {
        static bool Prepare() => Mod.Config.Fixes.DisableCombatSaves || Mod.Config.Fixes.DisableCombatRestarts;

        static void Postfix(SimGameOptionsMenu __instance)
        {
            CombatGameState combatGameState = SharedState.Combat;
            if (combatGameState != null && !combatGameState.TurnDirector.IsMissionOver && combatGameState.TurnDirector.GameHasBegun)
            {
                Mod.Log.Trace?.Write("SGOM:CS - in combat.");

                if (Mod.Config.Fixes.DisableCombatRestarts)
                {
                    Mod.Log.Trace?.Write("Disabling combat restarts.");
                    __instance.restartMission.SetState(ButtonState.Disabled);

                    string title = new Text(Mod.LocalizedText.Tooltips[ModText.TT_CombatRestartMission_Title]).ToString();
                    string details = new Text(Mod.LocalizedText.Tooltips[ModText.TT_CombatRestartMission_Details]).ToString();
                    BaseDescriptionDef def = new BaseDescriptionDef("SGMTipData", title, details, null);
                    __instance.restartTooltip.SetStateDataForButtonState(ButtonState.Disabled, TooltipUtilities.GetStateDataFromObject(def));
                }

                if (Mod.Config.Fixes.DisableCombatSaves && __instance.saveGame.State != ButtonState.Disabled)
                {
                    Mod.Log.Trace?.Write("Disabling combat saves.");
                    __instance.saveGame.SetState(ButtonState.Disabled);

                    string title = new Text(Mod.LocalizedText.Tooltips[ModText.TT_CombatSave_Title]).ToString();
                    string details = new Text(Mod.LocalizedText.Tooltips[ModText.TT_CombatSave_Details]).ToString();
                    BaseDescriptionDef def = new BaseDescriptionDef("SGMTipData", title, details, null);
                    __instance.saveTooltip.SetStateDataForButtonState(ButtonState.Disabled, TooltipUtilities.GetStateDataFromObject(def));
                }
            }
        }
    }

    [HarmonyPatch(typeof(SimGameOptionsMenu), "QuitPopup")]
    static class SimGameOptionsMenu_QuitPopup
    {
        static bool Prepare() => Mod.Config.Fixes.DisableCombatSaves;

        static void Prefix(ref bool __runOriginal, SimGameOptionsMenu __instance, Action quitAction)
        {
            if (!__runOriginal) return;

            CombatGameState combatGameState = SharedState.Combat;
            if (combatGameState != null && !combatGameState.TurnDirector.IsMissionOver && combatGameState.TurnDirector.GameHasBegun)
            {
                Mod.Log.Trace?.Write("SGOM:CS - in combat.");

                string text = Strings.T("Are you sure you want to quit?");
                string body = Strings.T("You will lose all unsaved progress since your last manual save or autosave");
                string title = Strings.T("Are you sure you want to quit?");
                string title2 = text;
                string body2 = Strings.T("You will forfeit the match if you quit");

                GameInstance battleTechGame = UnityGameInstance.BattleTechGame;
                GenericPopupBuilder genericPopupBuilder = GenericPopupBuilder.Create(text, body);
                genericPopupBuilder.AddButton("Cancel", null, true, null)
                    .AddButton("Quit", quitAction, true, null)
                    .CancelOnEscape();
                genericPopupBuilder.IsNestedPopupWithBuiltInFader().Render();

                __runOriginal = false;
                return;
            }

        }
    }

    [HarmonyPatch(typeof(CombatGameState), "TriggerAutoSaving")]
    static class CombatGameState_TriggerAutoSaving
    {
        static bool Prepare() => Mod.Config.Fixes.DisableCombatSaves;

        static void Prefix(ref bool __runOriginal, CombatGameState __instance)
        {
            if (!__runOriginal) return;

            if (__instance.NeedsStoryMissionStartSave)
            {
                __instance.NeedsStoryMissionStartSave = false;
                Mod.Log.Info?.Write("[CombatGameState_TriggerAutoSaving] Skipping combat autosave.");
            }
        }
    }
}
