using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using HBS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IRTweaks.Modules.UI {
    public static class StreamlinedMainMenu {


        [HarmonyPatch(typeof(SGContractsWidget), "Init")]
        [HarmonyPatch(new Type[] { typeof(SimGameState), typeof(Action<bool>) })]
        public static class SGContractsWidget_Init {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGContractsWidget __instance, GameObject ___ContractList) {
                Mod.Log.Trace($"SGCW:I - entered.");

                RectTransform clRT = ___ContractList.GetComponent<RectTransform>();
                if (clRT != null) {
                    Vector3 ns = clRT.sizeDelta;
                    ns.y += 260;
                    clRT.sizeDelta = ns;
                } else {
                    Mod.Log.Info("ContractList rectTransform is null!");
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationButton), "ResetFlyoutsToPrefab")]
        public static class SGNavigationButton_ResetFlyoutsToPrefab {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationButton __instance, LocalizableText ___text, List<SGNavFlyoutButton> ___FlyoutButtonList, int ___flyoutButtonCount) {
                Mod.Log.Trace($"SGNB:RFTP - entered button {___text.GetParsedText()} with {___flyoutButtonCount} flyout buttons for ID: {__instance.ID}");
                if (__instance.ID != DropshipLocation.CPT_QUARTER && !___text.text.Contains("CMD Staff")) {
                    foreach (SGNavFlyoutButton flyoutButton in ___FlyoutButtonList) {
                        flyoutButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationButton), "SetStateAccordingToSimDropship")]
        public static class SGNavigationButton_SetStateAccordingToSimDropship {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationButton __instance, DropshipType shipType, List<SGNavFlyoutButton> ___FlyoutButtonList, LocalizableText ___text) {
                Mod.Log.Trace($"SGNB:SSATSD - entered shipType:{shipType} for ID: {__instance.ID}");
                if (__instance.ID != DropshipLocation.CPT_QUARTER && !___text.text.Contains("CMD Staff")) {
                    foreach (SGNavFlyoutButton flyoutButton in ___FlyoutButtonList) {
                        flyoutButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationButton), "OnPointerEnter")]
        public static class SGNavigationButton_OnPointerEnter {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationButton __instance, PointerEventData eventData, List<SGNavFlyoutButton> ___FlyoutButtonList, LocalizableText ___text) {
                Mod.Log.Trace($"SGNB:OPE - entered.");
                if (__instance.ID != DropshipLocation.CPT_QUARTER && !___text.text.Contains("CMD Staff")) {
                    foreach (SGNavFlyoutButton flyoutButton in ___FlyoutButtonList) {
                        flyoutButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationButton), "OnClick")]
        public static class SGNavigationButton_OnClick {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationButton __instance, SGNavigationList ___buttonParent, LocalizableText ___text) {
                Mod.Log.Debug($"SGNB:OC - button clicked for ID: {__instance.ID}");

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

                SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
                if (___text.text.Contains("Store")) {
                    if (simulation.CurRoomState != DropshipLocation.SHOP) {
                        ___buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.SHOP);
                    }
                    ___buttonParent.navParent.SetQueuedUIActivationID(DropshipMenuType.Shop, DropshipLocation.SHOP);
                } else if (___text.text.Contains("Memorial")) {
                    if (simulation.CurRoomState != DropshipLocation.BARRACKS) {
                        ___buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.BARRACKS);
                    }
                    ___buttonParent.navParent.SetQueuedUIActivationID(DropshipMenuType.MemorialWall, DropshipLocation.BARRACKS);
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationWidgetLeft), "Init")]
        [HarmonyPatch(new Type[] { typeof(SimGameState), typeof(SGRoomManager) })]
        public static class SGNavigationWidgetLeft_Init {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationWidgetLeft __instance, SGShipMap ___shipMap, SGNavigationList ___locationList) {
                Mod.Log.Info($"SGNWL:I - entered with instanceType: {__instance.GetType()}.");

                ___shipMap.gameObject.SetActive(false);

                Vector3 startPos = ___locationList.transform.position;
                startPos.y += 200;
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
                    switch (buttonID) {
                        case DropshipMenuType.Darius:
                        case DropshipMenuType.Alexander:
                            if (simulation.CurRoomState != DropshipLocation.CMD_CENTER) {
                                ___buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.CMD_CENTER);
                            }
                            ___buttonParent.navParent.SetQueuedUIActivationID(buttonID, DropshipLocation.CMD_CENTER);
                            break;
                        case DropshipMenuType.Yang:
                            if (simulation.CurRoomState != DropshipLocation.MECH_BAY) {
                                ___buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.MECH_BAY);
                            }
                            ___buttonParent.navParent.SetQueuedUIActivationID(buttonID, DropshipLocation.MECH_BAY);
                            break;
                        case DropshipMenuType.Sumire:
                            if (simulation.CurRoomState != DropshipLocation.NAVIGATION) {
                                ___buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.NAVIGATION);
                            }
                            ___buttonParent.navParent.SetQueuedUIActivationID(buttonID, DropshipLocation.NAVIGATION);
                            break;
                        case DropshipMenuType.Farah:
                            if (simulation.CurRoomState != DropshipLocation.ENGINEERING) {
                                ___buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.ENGINEERING);
                            }
                            ___buttonParent.navParent.SetQueuedUIActivationID(buttonID, DropshipLocation.ENGINEERING);
                            break;
                        default:
                            break;
                    }
                } else if (___text.text.Contains("Memorial")) {
                    if (simulation.CurRoomState != DropshipLocation.BARRACKS) {
                        ___buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.BARRACKS);
                    }
                    ___buttonParent.navParent.SetQueuedUIActivationID(buttonID, DropshipLocation.BARRACKS);
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationList), "RefreshButtonStates")]
        public static class SGNavigationList_RefreshButtonStates {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationList __instance, SimGameState simState) {

                if (SGNavigationList_Start.storeButton != null) {
                    Faction owner = simState.CurSystem.Owner;
                    int reputation = (int)simState.GetReputation(owner);
                    if (reputation <= -3) {
                        Mod.Log.Info("Faction reputation too low, disabling store button.");
                        SGNavigationList_Start.storeButton.SetState(ButtonState.Disabled);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationList), "Start")]
        public static class SGNavigationList_Start {

            public static SGNavigationButton storeButton;
            public static SGNavigationButton staffButton;
            public static SGNavigationButton memorialButton;

            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationList __instance, HBSRadioSet ___radioSet, SGNavigationButton ___argoButton) {
                if (__instance.navParent != null) {
                    Mod.Log.Info($"SGNL:Start - adding new button.");
                    SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;

                    try {
                        // Create the store button
                        Mod.Log.Info(" - Creating store button");
                        GameObject storeButtonGO = GameObject.Instantiate(___argoButton.gameObject);
                        storeButtonGO.SetActive(true);
                        storeButtonGO.transform.position = ___argoButton.gameObject.transform.position;
                        storeButtonGO.transform.SetParent(___argoButton.gameObject.transform.parent);
                        storeButtonGO.transform.localScale = Vector3.one;
                        storeButtonGO.transform.SetSiblingIndex(1);

                        storeButton = storeButtonGO.GetComponent<SGNavigationButton>();
                        Traverse storeButtonT = Traverse.Create(storeButton).Field("id");
                        storeButtonT.SetValue(DropshipLocation.SHIP);
                        storeButton.SetupElement(__instance, ___radioSet, "Store",
                            LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.DropshipRoomCaptainsQuartersIcon, simulation);

                        // Create the staff button
                        Mod.Log.Info(" - Creating staff button");
                        GameObject staffButtonGO = GameObject.Instantiate(___argoButton.gameObject);
                        staffButtonGO.SetActive(true);
                        staffButtonGO.transform.position = ___argoButton.gameObject.transform.position;
                        staffButtonGO.transform.SetParent(___argoButton.gameObject.transform.parent);
                        staffButtonGO.transform.localScale = Vector3.one;
                        staffButtonGO.transform.SetSiblingIndex(7);

                        staffButton = staffButtonGO.GetComponent<SGNavigationButton>();
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

                        // Create the memorial button
                        Mod.Log.Info(" - Creating memorial button");
                        GameObject memorialButtonGO = GameObject.Instantiate(___argoButton.gameObject);
                        memorialButtonGO.SetActive(true);
                        memorialButtonGO.transform.position = ___argoButton.gameObject.transform.position;
                        memorialButtonGO.transform.SetParent(___argoButton.gameObject.transform.parent);
                        memorialButtonGO.transform.localScale = Vector3.one;
                        memorialButtonGO.transform.SetSiblingIndex(9);

                        memorialButton = memorialButtonGO.GetComponent<SGNavigationButton>();
                        Traverse memorialButtonT = Traverse.Create(memorialButton).Field("id");
                        memorialButtonT.SetValue(DropshipLocation.BARRACKS);

                        memorialButton.SetupElement(__instance, ___radioSet, "Memorial",
                            LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.DropshipRoomBarracksIcon, simulation);
                        //memorialButton.AddFlyoutButton("Memorial Wall", DropshipMenuType.MemorialWall);

                    } catch (Exception e) {
                        Mod.Log.Info("Error: " + e.Message);
                    }

                }
            }
        }

    }
}
