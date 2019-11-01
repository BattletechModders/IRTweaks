using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using HBS;
using SVGImporter;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IRTweaks.Modules.UI {
    public static class StreamlinedMainMenu {

        public static void SGNavigationButton_ResetFlyoutsToPrefab(SGNavigationButton __instance, LocalizableText ___text, List<SGNavFlyoutButton> ___FlyoutButtonList,
            int ___flyoutButtonCount) {
            Mod.Log.Info($"SGNB:RFTP - entered button {___text.GetParsedText()} with {___flyoutButtonCount} flyout buttons for ID: {__instance.ID}");
            if (__instance.ID != DropshipLocation.CPT_QUARTER && !___text.text.Contains("CMD Staff") && !___text.text.Contains("Memorial")) {
                Mod.Log.Info($"Clearing flyouts from ID: {__instance.ID}");
                foreach (SGNavFlyoutButton flyoutButton in ___FlyoutButtonList) {
                    // Mod.Log.Info($" -- Disabling flyoutButton: {flyoutButton.name}");
                    flyoutButton.gameObject.SetActive(false);
                }
            }
        }

        public static void SGNavigationButton_SetStateAccordingToSimDropship(SGNavigationButton __instance, DropshipType shipType, List<SGNavFlyoutButton> ___FlyoutButtonList, LocalizableText ___text) {
            Mod.Log.Info($"SGNB:SSATSD - entered shipType:{shipType} for ID: {__instance.ID}");
            if (__instance.ID != DropshipLocation.CPT_QUARTER && !___text.text.Contains("CMD Staff") && !___text.text.Contains("Memorial")) {
                Mod.Log.Info($"Clearing flyouts from ID: {__instance.ID}");
                foreach (SGNavFlyoutButton flyoutButton in ___FlyoutButtonList) {
                    // Mod.Log.Info($" --- Disabling flyoutButton: {flyoutButton.name}");
                    flyoutButton.gameObject.SetActive(false);
                }
            }
        }

        //public static void SGNavigationButton_AddFlyoutButton(SGNavigationButton __instance, string FlyoutLabel, DropshipMenuType FlyoutType) {
        //    Mod.Log.Info($"SGNB:AFB - adding button with label:{FlyoutLabel} and type:{FlyoutType} for ID: {__instance.ID}");
        //}

        //public static void SGNavigationButton_FlyoutClicked(SGNavigationButton __instance, DropshipMenuType buttonID) {
        //    Mod.Log.Info($"SGNB:FC - flyoutClicked for buttonID:{buttonID} for ID: {__instance.ID}");
        //}

        public static void SGNavigationButton_OnClick(SGNavigationButton __instance, SGNavigationList ___buttonParent) {
            Mod.Log.Info($"SGNB:OC - button clicked for ID: {__instance.ID}");

            switch (__instance.ID) {
                case DropshipLocation.CMD_CENTER:
                    ___buttonParent.navParent.SetQueuedUIActivationID(DropshipMenuType.Contract, __instance.ID);
                    break;
                case DropshipLocation.BARRACKS:
                    ___buttonParent.navParent.SetQueuedUIActivationID(DropshipMenuType.Mechwarrior, __instance.ID);
                    break;
                case DropshipLocation.ENGINEERING:
                    ___buttonParent.navParent.SetQueuedUIActivationID(DropshipMenuType.ShipUpgrade, __instance.ID);
                    break;
                case DropshipLocation.MECH_BAY:
                    ___buttonParent.navParent.SetQueuedUIActivationID(DropshipMenuType.MechBay, __instance.ID);
                    break;
                case DropshipLocation.NAVIGATION:
                    ___buttonParent.navParent.SetQueuedUIActivationID(DropshipMenuType.Navigation, __instance.ID);
                    break;
                default:
                    break;
            }
        }

        public static void SGNavigationButton_OnPointerEnter(SGNavigationButton __instance, PointerEventData eventData, List<SGNavFlyoutButton> ___FlyoutButtonList, LocalizableText ___text) {
            Mod.Log.Info($"SGNB:OPE - entered.");
            if (__instance.ID != DropshipLocation.CPT_QUARTER && !___text.text.Contains("CMD Staff") && !___text.text.Contains("Memorial") ) {
                Mod.Log.Info($"Clearing flyouts from ID: {__instance.ID} with text:{___text.text}");
                foreach (SGNavFlyoutButton flyoutButton in ___FlyoutButtonList) {
                    // Mod.Log.Info($" ---- Disabling flyoutButton: {flyoutButton.name}");
                    flyoutButton.gameObject.SetActive(false);
                }
            }
        }

        public static void SGNavigationButton_SetupElement(SGNavigationButton __instance, SGNavigationList listWidget, 
            HBSRadioSet radioSet, string labelText, SVGAsset Icon, SimGameState simGameState, SVGImage ___flyoutContainer) {
            Mod.Log.Info($"SGNB:SE - setup element invoked for instance: {__instance.ID}");
        }

        [HarmonyPatch(typeof(SGNavigationWidgetLeft), "Init")]
        [HarmonyPatch(new Type[] { typeof(SimGameState), typeof(SGRoomManager) })]
        public static class SGNavigationWidgetLeft_Init {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationWidgetLeft __instance, SGShipMap ___shipMap, SGNavigationList ___locationList) {
                Mod.Log.Info($"SGNWL:I - entered with instanceType: {__instance.GetType()}.");

                ___shipMap.gameObject.SetActive(false);

                Vector3 startPos = ___locationList.transform.position;
                startPos.y += 100;
                ___locationList.transform.position = startPos;
            }
        }

        [HarmonyPatch(typeof(SGNavigationButton), "FlyoutClicked")]
        public static class SGNavigationButton_FlyoutClicked {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Prefix(SGNavigationButton __instance, DropshipMenuType buttonID, LocalizableText ___text, SGNavigationList ___buttonParent) {
                // Skip if there's already a transition in progress
                if (SimGameCameraController.TransitionInProgress) { return; }

                SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
                if (___text.text.Contains("CMD Staff")) {
                    if (buttonID == DropshipMenuType.Darius && simulation.CurRoomState != DropshipLocation.CMD_CENTER) {
                        ___buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.CMD_CENTER);
                    } else if (buttonID == DropshipMenuType.Yang && simulation.CurRoomState != DropshipLocation.MECH_BAY) {
                        ___buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.MECH_BAY);
                    } else if (buttonID == DropshipMenuType.Sumire && simulation.CurRoomState != DropshipLocation.NAVIGATION) {
                        ___buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.NAVIGATION);
                    } else if (buttonID == DropshipMenuType.Farah && simulation.CurRoomState != DropshipLocation.ENGINEERING) {
                        ___buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.ENGINEERING);
                    }
                } 

                //if (__instance.ID == DropshipLocation.SHIP) {
                //    if (buttonID == DropshipMenuType.HiringHall) {
                //        if (this.simState.CurRoomState != DropshipLocation.HIRING) {
                //            this.buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.HIRING);
                //            return;
                //        }
                //    } else if (buttonID == DropshipMenuType.Shop && this.simState.CurRoomState != DropshipLocation.SHOP) {
                //        this.buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.SHOP);
                //        return;
                //    }
                //} else if (this.buttonParent.navParent.SetQueuedUIActivationID(buttonID, this.ID)) {
                //    this.OnClick();
                //}
            }
        }

        [HarmonyPatch(typeof(SGNavigationList), "Start")]
        public static class SGNavigationList_Start {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationList __instance, HBSRadioSet ___radioSet, SGNavigationButton ___argoButton) {
                if (__instance.navParent != null) {
                    Mod.Log.Info($"SGNL:Start - adding new button.");
                    SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;

                    try {
                        // Create the staff button
                        Mod.Log.Info(" - Creating staff button");
                        var staffButtonGO = GameObject.Instantiate(___argoButton.gameObject);
                        staffButtonGO.SetActive(true);
                        staffButtonGO.transform.position = ___argoButton.gameObject.transform.position;
                        staffButtonGO.transform.SetParent(___argoButton.gameObject.transform.parent);
                        staffButtonGO.transform.localScale = Vector3.one;

                        var staffButton = staffButtonGO.GetComponent<SGNavigationButton>();
                        Mod.Log.Info($"StaffRoom id is: {staffButton.ID}");
                        Traverse staffButtonT = Traverse.Create(staffButton).Field("id");
                        staffButtonT.SetValue(DropshipLocation.CMD_CENTER);

                        staffButton.SetupElement(__instance, ___radioSet, "CMD Staff",
                            LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.DropshipRoomCommandCenterIcon, simulation);
                        staffButton.AddFlyoutButton("Darius", DropshipMenuType.Darius);
                        staffButton.AddFlyoutButton("Yang", DropshipMenuType.Yang);
                        staffButton.AddFlyoutButton("Sumire", DropshipMenuType.Sumire);
                        staffButton.AddFlyoutButton("Farah", DropshipMenuType.Farah);
                        if (simulation.GetCharacterStatus(SimGameState.SimGameCharacterType.ALEXANDER)) {
                            staffButton.AddFlyoutButton("Alexander", DropshipMenuType.Alexander);
                        }

                        ___radioSet.AddButtonToRadioSet(staffButton);

                        // Create the memorial button
                        Mod.Log.Info(" - Creating memorial button");
                        var memorialButtonGO = GameObject.Instantiate(___argoButton.gameObject);
                        memorialButtonGO.SetActive(true);
                        memorialButtonGO.transform.position = ___argoButton.gameObject.transform.position;
                        memorialButtonGO.transform.SetParent(___argoButton.gameObject.transform.parent);
                        memorialButtonGO.transform.localScale = Vector3.one;

                        var memorialButton = memorialButtonGO.GetComponent<SGNavigationButton>();
                        Mod.Log.Info($"MemorialButton id is: {memorialButton.ID}");
                        Traverse memorialButtonT = Traverse.Create(memorialButton).Field("id");
                        memorialButtonT.SetValue(DropshipLocation.BARRACKS);

                        memorialButton.SetupElement(__instance, ___radioSet, "Memorial",
                            LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.DropshipRoomBarracksIcon, simulation);
                        memorialButton.AddFlyoutButton("Memorial Wall", DropshipMenuType.MemorialWall);

                        ___radioSet.AddButtonToRadioSet(memorialButton);

                    } catch (Exception e) {
                        Mod.Log.Info("Error: " + e.Message);
                    }

                }
            }
        }

    }
}
