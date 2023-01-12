using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BattleTech.UI.TMProWrapper;
using HBS.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace IRTweaks.Modules.Misc
{

    [HarmonyPatch(typeof(LineOfSight), "FindSecondaryImpactTarget")]
    static class LineOfSight_FindSecondaryImpactTarget
    {
        static void Postfix(LineOfSight __instance, RaycastHit[] rayInfos, AbstractActor attacker,
            ICombatant initialTarget, Vector3 attackPosition, ref Vector3 impactPoint, ref bool __result)
        {
            var combat = UnityGameInstance.BattleTechGame.Combat;
            if (!combat.Constants.ToHit.StrayShotsEnabled)
            {
                var num = float.MaxValue;
                for (int i = 0; i < rayInfos.Length; i++)
                {
                    if (rayInfos[i].distance < num)
                    {
                        impactPoint = rayInfos[i].point;
                        num = rayInfos[i].distance;
                    }
                }
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(SimGameDifficultySettingsModule), "UpdateDifficultyScoreBar")]
    static class SimGameDifficultySettingsModule_UpdateDifficultyScoreBar_Patch
    {

        static bool Prefix(SimGameDifficultySettingsModule __instance, SimGameDifficulty ___cachedDiff,
            PreGameCareerModeSettingsTotalScoreDescAndBar ___difficultyBarAndMod)
        {
            float num = __instance.CalculateRawScoreMod();
            bool active = __instance.ShouldShowDifficultyData();
            ___difficultyBarAndMod.gameObject.SetActive(active);
            ___difficultyBarAndMod.RefreshInfo(num);

            if (__instance.CanModifyStartSettings)
            {
                var atlasSkull = GameObject.Find("atlasSkull-image");
                var atlasImage = atlasSkull.gameObject.GetComponent<Image>();
                var currentModifier = Mathf.Abs(Mathf.Max(0,num) - ModState.MaxDiffModifier);
                var scaledModifier = Mathf.FloorToInt(currentModifier * 255 / ModState.MaxDiffModifier);
                Mod.Log.Info?.Write($"COLOR SCORE MOD THING: real score {num} vs current {currentModifier} vs scaled {scaledModifier}");
                Mod.Log.Info?.Write($"From UPDATEDIFFICULTYSCOREBAR: Atlas values: {atlasImage.color.r}, {atlasImage.color.g}, {atlasImage.color.b}, {atlasImage.color.a}");
                atlasImage.color = new Color32(255, (byte)scaledModifier, (byte)scaledModifier, 255);;
            }
            
            return false;
        }
    }

    [HarmonyPatch(typeof(PreGameCareerModeSettingsTotalScoreDescAndBar), "RefreshInfo", new Type[] {typeof(float)})]
    static class PreGameCareerModeSettingsTotalScoreDescAndBar_RefreshInfo_Patch
    {
        static bool Prepare() => Mod.Config.Fixes.RandomStartByDifficulty;
        static bool Prefix(PreGameCareerModeSettingsTotalScoreDescAndBar __instance, float newMod)
        {
            bool flag = false;
            SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
            if (simulation != null && simulation.DifficultySettings.GetRawCareerModifier() < newMod)
            {
                flag = true;
            }

            var absDiffRange = Mathf.Abs(ModState.MinDiffModifier) + ModState.MaxDiffModifier;

            var newModShiftedAbs = newMod + Mathf.Abs(ModState.MinDiffModifier);

            __instance.ShowAttemptedToRaiseDifficultyWarningIcon(flag);

            float fillAmount = newModShiftedAbs / absDiffRange;
            __instance.DifficultyFillBar.fillAmount = fillAmount;
            __instance.TotalScoreModifierValue.SetText("{0:n2}", new object[]
            {
                newMod
            });
            return false;
        }
    }

    [HarmonyPatch(typeof(SimGameState), "_OnInit")]
    static class SimGameState__OnInit_Patch
    {
        static void Postfix(SimGameState __instance, GameInstance game, SimGameDifficulty difficulty)
        {
            ModState.OnSimInit();
        }
    }

    [HarmonyPatch(typeof(SimGameState), "Destroy")]
    static class SimGameState__Destroy_Patch
    {
        static void Postfix(SimGameState __instance)
        {
            ModState.OnSimInit();
        }
    }

    [HarmonyPatch(typeof(SimGameDifficultySettingsModule), "InitSettings")]
    static class SimGameDifficultySettingsModule_InitSettings_Patch
    {
        static void Prefix(SimGameDifficultySettingsModule __instance, SimGameDifficulty ___cachedDiff, PreGameCareerModeSettingsTotalScoreDescAndBar ___difficultyBarAndMod)
        {

            if (ModState.HaveDiffSettingsInitiated) return;

            ___cachedDiff = UnityGameInstance.BattleTechGame.DifficultySettings;

            if (ModState.MaxDiffModifier == 0f && ModState.MinDiffModifier == 0f)
            {
                Helper.DifficultyHelper.GetDifficultyModifierRange(___cachedDiff);
            }

            if (__instance.CanModifyStartSettings)
            {
                var atlasSkull = GameObject.Find("atlasSkull-image");
                var atlasImage = atlasSkull.gameObject.GetComponent<Image>();
                var currentModifier = Mathf.Abs(Mathf.Max(0,__instance.CalculateRawScoreMod()) - ModState.MaxDiffModifier);
                var scaledModifier = Mathf.FloorToInt(currentModifier * 255 / ModState.MaxDiffModifier);
                atlasImage.color = new Color32(255, (byte)scaledModifier, (byte)scaledModifier, 255);
                Mod.Log.Info?.Write($"From INITSETTINGS: Atlas values: {atlasImage.color.r}, {atlasImage.color.g}, {atlasImage.color.b}, {atlasImage.color.a}");
            }
            
            var settings = ___cachedDiff.GetSettings();
            settings.Sort(delegate(SimGameDifficulty.DifficultySetting a, SimGameDifficulty.DifficultySetting b)
            {
                if (a.UIOrder != b.UIOrder)
                {
                    return a.UIOrder.CompareTo(b.UIOrder);
                }
                return a.Name.CompareTo(b.Name);
            });
            var startCount = -5;
            foreach (var setting in settings)
            {
                if (setting.Visible && setting.StartOnly && setting.Enabled)
                {
                    startCount += 1;
                }
            }

            var startYAdjust = startCount / 2;

            var regularDiffs = GameObject.Find("difficulty_scroll");
            var startOnly = GameObject.Find("OBJ_startOnly_settings");
            var startRect = startOnly.GetComponent<RectTransform>();

            //try to move bottons down?

            if (startYAdjust > 0 )
            {
                if (__instance.CanModifyStartSettings)
                {
                    var barRect = ___difficultyBarAndMod.gameObject.GetComponent<RectTransform>();
                    var currentbarPos = barRect.position;
                    currentbarPos.x = 700f;
                    currentbarPos.y = 400f;
                    barRect.position = currentbarPos;
                }
                else
                {
                    var barRect = ___difficultyBarAndMod.gameObject.GetComponent<RectTransform>();
                    var currentbarPos = barRect.position;
                    currentbarPos.y = 820f;
                    barRect.position = currentbarPos;
                }


                var currentStartPosition = startRect.position;
                currentStartPosition.y += Mod.Config.Misc.DifficultyUIScaling.StartOnlyPositionY;
                startRect.position = currentStartPosition;

                var currentStartSizeDelta = startRect.sizeDelta;
            
                currentStartSizeDelta.y += startYAdjust * Mod.Config.Misc.DifficultyUIScaling.StartOnlyScalar;
                startRect.sizeDelta = currentStartSizeDelta;

                var regularRect = regularDiffs.GetComponent<RectTransform>();
                var currentRegSizeDelta = regularRect.sizeDelta;
                currentRegSizeDelta.y -= startYAdjust - 1 * Mod.Config.Misc.DifficultyUIScaling.StartOnlyScalar;
                regularRect.sizeDelta = currentRegSizeDelta;

                var currentRegPosition = regularRect.position;
                currentRegPosition.y += Mod.Config.Misc.DifficultyUIScaling.RegularPositionY;
                currentRegPosition.y -= (startYAdjust * Mod.Config.Misc.DifficultyUIScaling.StartOnlyPositionY);
                regularRect.position = currentRegPosition;

                var bottomButtons = GameObject.Find("bottomBarButtons");
                var bottomRect = bottomButtons.GetComponent<RectTransform>();
                var currentBottomPosition = bottomRect.position;
                currentBottomPosition.y = 0;
                bottomRect.position = currentBottomPosition;
            }

            var startTransformLayoutGroup = startOnly.GetComponent<RectTransform>().GetComponent<GridLayoutGroup>();
            startTransformLayoutGroup.childAlignment = TextAnchor.UpperCenter;
            startTransformLayoutGroup.cellSize = new Vector2(375, 40);
            startTransformLayoutGroup.spacing = new Vector2(25, 10);

        }

        static void Postfix(SimGameDifficultySettingsModule __instance, SimGameDifficulty ___cachedDiff, string ___ironManModeId, string ___autoEquipMechsId, string ___mechPartsReqId, string ___skipPrologueId, string ___randomMechId, string ___argoUpgradeCostId, SGDSToggle ___ironManModeToggle, SGDSDropdown ___mechPartsReqDropdown, GameObject ___disabledOverlay, List<SGDSDropdown> ___activeDropdowns, List<SGDSToggle> ___activeToggles, List<SGDSDropdown> ___cachedDropdowns, List<SGDSToggle> ___cachedToggles, SGDSToggle ___togglePrefab, SGDSDropdown ___dropdownPrefab)
        {

            var existingStartOnlyVars = new List<string>()
            {
                ___ironManModeId,
                ___autoEquipMechsId,
                ___mechPartsReqId,
                ___skipPrologueId,
                ___randomMechId,
                ___argoUpgradeCostId
            };

            ___cachedDiff = UnityGameInstance.BattleTechGame.DifficultySettings;
            var settings = ___cachedDiff.GetSettings();
            settings.Sort(delegate(SimGameDifficulty.DifficultySetting a, SimGameDifficulty.DifficultySetting b)
            {
                if (a.UIOrder != b.UIOrder)
                {
                    return a.UIOrder.CompareTo(b.UIOrder);
                }
                return a.Name.CompareTo(b.Name);
            });

            foreach (var setting in settings)
            {
                if (setting.Visible)
                {
                    int curSettingIndex = ___cachedDiff.GetCurSettingIndex(setting.ID);

                    if (setting.StartOnly && existingStartOnlyVars.All(x => x != setting.ID))
                    {
                        if (!setting.Toggle)
                        {
                            var sourceSettingDropDownGO = ___mechPartsReqDropdown.gameObject;

                            GameObject newDropDownObject = UnityEngine.Object.Instantiate<GameObject>(sourceSettingDropDownGO, sourceSettingDropDownGO.transform.parent);

                            SGDSDropdown newDropDown = newDropDownObject.GetOrAddComponent<SGDSDropdown>();

                            var dropdown = newDropDown.dropdown;//Traverse.Create(newDropDown).Field("dropdown").GetValue<HBS_Dropdown>();
                            var dropdownrect = dropdown.gameObject.GetComponent<RectTransform>();
                            dropdownrect.sizeDelta = new Vector2(170, 40);

                            var dropdownLabel = dropdown.m_CaptionText;//Traverse.Create(dropdown).Field("m_CaptionText").GetValue<LocalizableText>();
                            dropdownLabel.enableWordWrapping = false;

                            if (!ModState.InstantiatedDropdowns.Contains(newDropDown))
                            {
                                ___activeDropdowns.Add(newDropDown);
                                newDropDown.Initialize(__instance, setting, curSettingIndex);
                                newDropDown.gameObject.SetActive(true);
                                ModState.InstantiatedDropdowns.Add(newDropDown);
                            }
                        }
                        else if (setting.Toggle)
                        {
                            var sourceDiffToggleGO = ___ironManModeToggle.gameObject;
                            GameObject sourceDiffToggle = UnityEngine.Object.Instantiate<GameObject>(sourceDiffToggleGO, sourceDiffToggleGO.transform.parent);
                            SGDSToggle newToggle = sourceDiffToggle.GetOrAddComponent<SGDSToggle>();

                            if (!ModState.InstantiatedToggles.Contains(newToggle))
                            {
                                ___activeToggles.Add(newToggle);
                                newToggle.Initialize(__instance, setting, curSettingIndex);
                                newToggle.gameObject.SetActive(true);
                                ModState.InstantiatedToggles.Add(newToggle);
                            }
                        }
                    }
                }
            }

            var newDisabledOverlay =
                UnityEngine.Object.Instantiate<GameObject>(___disabledOverlay, ___disabledOverlay.transform.parent);
           ___disabledOverlay.SetActive(false);
           newDisabledOverlay.SetActive(!__instance.CanModifyStartSettings);
           __instance.UpdateDifficultyScoreBar();
           ModState.HaveDiffSettingsInitiated = true;
        }
    }

    [HarmonyPatch(typeof(SimGameState), "AddRandomStartingMechs")]
    static class RandomStartByDifficulty_SimGameState_AddRandomStartingMechs
    {
        static bool Prepare() => Mod.Config.Fixes.RandomStartByDifficulty;

        static void Prefix(SimGameState __instance)
        {
            Mod.Log.Trace?.Write("SGS:ARSM entered.");
            SimGameConstantOverride sgco = __instance.ConstantOverrides;

            if (sgco.ConstantOverrides.ContainsKey("CareerMode"))
            {
                // Patch starting mechs
                if (sgco.ConstantOverrides["CareerMode"].ContainsKey(ModStats.HBS_RandomMechs))
                {
                    string startingMechsS = sgco.ConstantOverrides["CareerMode"][ModStats.HBS_RandomMechs];
                    Mod.Log.Info?.Write($"Replacing starting random mechs with:{startingMechsS}");
                    string[] startingMechs = startingMechsS.Split(',');
                    __instance.Constants.CareerMode.StartingRandomMechLists = startingMechs;
                }
                else
                {
                    Mod.Log.Debug?.Write($"key: {ModStats.HBS_RandomMechs} not found");
                }

                // Patch faction reputation
                if (sgco.ConstantOverrides["CareerMode"].ContainsKey(ModStats.HBS_FactionRep))
                {
                    string factionRepS = sgco.ConstantOverrides["CareerMode"][ModStats.HBS_FactionRep];
                    string[] factions = factionRepS.Split(',');
                    foreach (string factionToken in factions)
                    {
                        string[] factionSplit = factionToken.Split(':');
                        string factionId = factionSplit[0];
                        int factionRep = int.Parse(factionSplit[1]);
                        Mod.Log.Info?.Write($"Applying rep: {factionRep} to faction: ({factionId})");
                        FactionDef factionDef = FactionDef.GetFactionDefByEnum(__instance.DataManager, factionId);
                        __instance.AddReputation(factionDef.FactionValue, factionRep, false);
                    }
                }
                else
                {
                    Mod.Log.Debug?.Write($"key: {ModStats.HBS_RandomMechs} not found");
                }

            }
            else if (!sgco.ConstantOverrides.ContainsKey("CareerMode"))
            {
                Mod.Log.Debug?.Write("key:CareerMode not found");
            }
        }
    }

    [HarmonyPatch(typeof(SimGameConstantOverride), "ApplyOverride")]
    static class RandomStartByDifficulty_SimGameConstantOverride_ApplyOverride
    {
        static bool Prepare() => Mod.Config.Fixes.RandomStartByDifficulty;

        static void Postfix(SimGameConstantOverride __instance, string constantType, string constantName)
        {
            Mod.Log.Trace?.Write("SGCO:AO entered.");

            if (constantName != null && constantName.ToLower().Equals(ModStats.HBS_StrayShotEnabler.ToLower()))
            {
                bool value = Convert.ToBoolean(__instance.ConstantOverrides[constantType][constantName]);
                Mod.Log.Debug?.Write($" Setting StrayShotsEnabled to {value} ");
                ToHitConstantsDef thcd = __instance.simState.CombatConstants.ToHit;
                thcd.StrayShotsEnabled = value;

                //Traverse traverse = Traverse.Create(___simState.CombatConstants).Property("ToHit");
                //traverse.SetValue(thcd);
                __instance.simState.CombatConstants.ToHit = thcd;
                Mod.Log.Debug?.Write($" Replaced ToHit");
            }

            if (constantName != null && constantName.ToLower().Equals(ModStats.HBS_StrayShotHitsUnits.ToLower()))
            {
                bool value = Convert.ToBoolean(__instance.ConstantOverrides[constantType][constantName]);
                Mod.Log.Debug?.Write($" Setting StrayShotsHitUnits to {value} ");
                ToHitConstantsDef thcd = __instance.simState.CombatConstants.ToHit;
                thcd.StrayShotsHitUnits = value;

                //Traverse traverse = Traverse.Create(___simState.CombatConstants).Property("ToHit");
                //traverse.SetValue(thcd);
                __instance.simState.CombatConstants.ToHit = thcd;
                Mod.Log.Debug?.Write($" Replaced ToHit");
            }

            if (constantName != null && constantName.ToLower().Equals(ModStats.HBS_StrayShotValidTargets.ToLower()))
            {
                string value = __instance.ConstantOverrides[constantType][constantName];
                Mod.Log.Debug?.Write($" Setting StrayShotValidTargets to {value} ");
                ToHitConstantsDef thcd = __instance.simState.CombatConstants.ToHit;
                thcd.StrayShotValidTargets = (StrayShotValidTargets)Enum.Parse(typeof(StrayShotValidTargets), value);

                //Traverse traverse = Traverse.Create(___simState.CombatConstants).Property("ToHit");
                //traverse.SetValue(thcd);
                __instance.simState.CombatConstants.ToHit = thcd;
                Mod.Log.Debug?.Write($" Replaced ToHit");
            }
        }
    }

    [HarmonyPatch(typeof(SGDifficultySettingObject), "CurCareerModifier")]
    static class RandomStartByDifficulty_SGDifficultySettingObject_CurCareerModifier
    {
        static bool Prepare() => Mod.Config.Fixes.RandomStartByDifficulty;

        static bool Prefix(SGDifficultySettingObject __instance, ref float __result, int ___curIdx)
        {
            Mod.Log.Trace?.Write("SGDSO:CCM entered.");

            float careerScoreModifier = 0f;
            if (__instance != null && __instance.Setting != null && __instance.Setting.Options != null && __instance.Setting.Options.Count > ___curIdx)
            {
                careerScoreModifier = __instance.Setting.Options[___curIdx].CareerScoreModifier;
            }

            __result = careerScoreModifier;
            //__result = (careerScoreModifier <= -1f) ? 0f : careerScoreModifier;

            return false;
        }

    }

}
