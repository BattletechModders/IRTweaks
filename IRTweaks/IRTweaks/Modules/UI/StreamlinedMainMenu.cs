using HBS;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IRTweaks.Modules.UI
{
    public static class StreamlinedMainMenu
    {

        [HarmonyPatch(typeof(SGContractsWidget), "Init")]
        [HarmonyPatch(new Type[] { typeof(SimGameState), typeof(Action<bool>) })]
        public static class SGContractsWidget_Init
        {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGContractsWidget __instance)
            {
                Mod.Log.Trace?.Write($"SGCW:I - entered.");

                RectTransform clRT = __instance.ContractList.GetComponent<RectTransform>();
                if (clRT != null)
                {
                    Vector3 ns = clRT.sizeDelta;
                    ns.y += 260;
                    clRT.sizeDelta = ns;
                }
                else
                {
                    Mod.Log.Info?.Write("ContractList rectTransform is null!");
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationButton), "ResetFlyoutsToPrefab")]
        public static class SGNavigationButton_ResetFlyoutsToPrefab
        {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationButton __instance)
            {
                Mod.Log.Trace?.Write($"SGNB:RFTP - entered button {__instance.text.GetParsedText()} with {__instance.flyoutButtonCount} flyout buttons for ID: {__instance.ID}");
                if (__instance.ID != DropshipLocation.CPT_QUARTER && !__instance.text.text.Contains("CMD Staff"))
                {
                    foreach (SGNavFlyoutButton flyoutButton in __instance.FlyoutButtonList)
                    {
                        flyoutButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationButton), "SetStateAccordingToSimDropship")]
        public static class SGNavigationButton_SetStateAccordingToSimDropship
        {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationButton __instance, DropshipType shipType)
            {
                Mod.Log.Trace?.Write($"SGNB:SSATSD - entered shipType:{shipType} for ID: {__instance.ID}");
                if (__instance.ID != DropshipLocation.CPT_QUARTER && !__instance.text.text.Contains("CMD Staff"))
                {
                    foreach (SGNavFlyoutButton flyoutButton in __instance.FlyoutButtonList)
                    {
                        flyoutButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationButton), "OnPointerEnter")]
        public static class SGNavigationButton_OnPointerEnter
        {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationButton __instance, PointerEventData eventData)
            {
                Mod.Log.Trace?.Write($"SGNB:OPE - entered.");
                if (__instance.ID != DropshipLocation.CPT_QUARTER && !__instance.text.text.Contains("CMD Staff"))
                {
                    foreach (SGNavFlyoutButton flyoutButton in __instance.FlyoutButtonList)
                    {
                        flyoutButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationButton), "OnClick")]
        public static class SGNavigationButton_OnClick
        {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationButton __instance)
            {
                Mod.Log.Debug?.Write($"SGNB:OC - button clicked for ID: {__instance.ID}");
                SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;

                switch (__instance.ID)
                {
                    case DropshipLocation.CMD_CENTER:
                        QueueOrForceActivation(DropshipMenuType.Contract, __instance.ID, __instance.buttonParent.navParent, simulation);
                        if (SGNavigationButton_FlyoutClicked.ClickedID != DropshipMenuType.INVALID_UNSET)
                        {
                            if (__instance.text.text.Contains("CMD Staff"))
                            {
                                switch (SGNavigationButton_FlyoutClicked.ClickedID)
                                {
                                    case DropshipMenuType.Darius:
                                    case DropshipMenuType.Alexander:
                                        QueueOrForceActivation(SGNavigationButton_FlyoutClicked.ClickedID, DropshipLocation.CMD_CENTER, __instance.buttonParent.navParent, simulation);
                                        break;
                                    case DropshipMenuType.Yang:
                                        QueueOrForceActivation(SGNavigationButton_FlyoutClicked.ClickedID, DropshipLocation.MECH_BAY, __instance.buttonParent.navParent, simulation);
                                        break;
                                    case DropshipMenuType.Sumire:
                                        QueueOrForceActivation(SGNavigationButton_FlyoutClicked.ClickedID, DropshipLocation.NAVIGATION, __instance.buttonParent.navParent, simulation);
                                        break;
                                    case DropshipMenuType.Farah:
                                        QueueOrForceActivation(SGNavigationButton_FlyoutClicked.ClickedID, DropshipLocation.ENGINEERING, __instance.buttonParent.navParent, simulation);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else if (__instance.text.text.Contains("Memorial"))
                            {
                                QueueOrForceActivation(SGNavigationButton_FlyoutClicked.ClickedID, DropshipLocation.BARRACKS, __instance.buttonParent.navParent, simulation);
                            }
                            SGNavigationButton_FlyoutClicked.ClickedID = DropshipMenuType.INVALID_UNSET;
                        }
                        break;
                    case DropshipLocation.BARRACKS:
                        QueueOrForceActivation(DropshipMenuType.Mechwarrior, __instance.ID, __instance.buttonParent.navParent, simulation);
                        break;
                    case DropshipLocation.ENGINEERING:
                        QueueOrForceActivation(DropshipMenuType.ShipUpgrade, __instance.ID, __instance.buttonParent.navParent, simulation);
                        break;
                    case DropshipLocation.MECH_BAY:
                        QueueOrForceActivation(DropshipMenuType.MechBay, __instance.ID, __instance.buttonParent.navParent, simulation);
                        break;
                    case DropshipLocation.NAVIGATION:
                        QueueOrForceActivation(DropshipMenuType.Navigation, __instance.ID, __instance.buttonParent.navParent, simulation);
                        break;
                    default:
                        break;
                }

                if (__instance.text.text.Contains("Store"))
                {
                    if (simulation.CurRoomState != DropshipLocation.SHOP)
                    {
                        __instance.buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.SHOP);
                    }
                    QueueOrForceActivation(DropshipMenuType.Shop, DropshipLocation.SHOP, __instance.buttonParent.navParent, simulation);
                }
                else if (__instance.text.text.Contains("Memorial"))
                {
                    if (simulation.CurRoomState != DropshipLocation.BARRACKS)
                    {
                        __instance.buttonParent.ArgoButtonFlyoutChangeRoom(DropshipLocation.BARRACKS);
                    }
                    QueueOrForceActivation(DropshipMenuType.MemorialWall, DropshipLocation.BARRACKS, __instance.buttonParent.navParent, simulation);
                }
            }

            private static void QueueOrForceActivation(DropshipMenuType menuType, DropshipLocation location, SGNavigationWidgetLeft sgnwl, SimGameState sgs)
            {
                if (sgs.CameraController.betweenRoomTransitionTime == 0f && sgs.CameraController.inRoomTransitionTime == 0f)
                {
                    // Check for a 0 animation time on SGRoomManager; if set, BTPerfFix is active and we need to force a transition
                    Mod.Log.Info?.Write($"DEBUG - calling SetSubroom for location:{location} and menuType:{menuType}!");
                    sgs.RoomManager.ChangeRoom(location);
                    sgs.RoomManager.SetSubRoom(location, menuType);
                }
                else
                {
                    // Let the animation happen via the queued activation
                    Mod.Log.Info?.Write("DEBUG - calling SetQueuedUIActivationID!");
                    sgnwl.SetQueuedUIActivationID(menuType, location);
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationWidgetLeft), "Init")]
        [HarmonyPatch(new Type[] { typeof(SimGameState), typeof(SGRoomManager) })]
        public static class SGNavigationWidgetLeft_Init
        {
            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationWidgetLeft __instance)
            {
                Mod.Log.Info?.Write($"SGNWL:I - entered with instanceType: {__instance.GetType()}.");

                __instance.shipMap.gameObject.SetActive(false);

                Vector3 startPos = __instance.locationList.transform.position;
                startPos.y += 200;
                __instance.locationList.transform.position = startPos;
            }
        }

        [HarmonyPatch(typeof(SGNavigationButton), "FlyoutClicked")]
        public static class SGNavigationButton_FlyoutClicked
        {
            public static DropshipMenuType ClickedID = DropshipMenuType.INVALID_UNSET;

            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Prefix(ref bool __runOriginal, SGNavigationButton __instance, DropshipMenuType buttonID)
            {
                if (!__runOriginal) return;

                Mod.Log.Debug?.Write($"SGNB:FC - button clicked for ID: {__instance.ID} for menuType:{buttonID} with transition:{SimGameCameraController.TransitionInProgress}");
                // Skip if there's already a transition in progress
                if (SimGameCameraController.TransitionInProgress) { return; }

                ClickedID = buttonID;
            }

        }

        [HarmonyPatch(typeof(SGNavigationList), "RefreshButtonStates")]
        public static class SGNavigationList_RefreshButtonStates
        {

            public static bool IsShopActive(SimGameState simState)
            {
                FactionValue owner = simState.CurSystem.OwnerValue;
                int reputation = (int)simState.GetReputation(owner);
                return reputation > -3;
            }

            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationList __instance, SimGameState simState)
            {

                if (SGNavigationList_Start.storeButton != null)
                {
                    if (!IsShopActive(simState))
                    {
                        Mod.Log.Info?.Write("Faction reputation too low, disabling store button.");
                        SGNavigationList_Start.storeButton.SetState(ButtonState.Disabled);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationList), "Start")]
        public static class SGNavigationList_Start
        {

            public static SGNavigationButton storeButton;
            public static SGNavigationButton staffButton;
            public static SGNavigationButton memorialButton;

            static bool Prepare() { return Mod.Config.Fixes.StreamlinedMainMenu; }

            static void Postfix(SGNavigationList __instance)
            {
                if (__instance.navParent != null)
                {
                    Mod.Log.Info?.Write($"SGNL:Start - adding new button.");
                    SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;

                    try
                    {
                        // Create the store button
                        Mod.Log.Info?.Write(" - Creating store button");
                        GameObject storeButtonGO = GameObject.Instantiate(__instance.argoButton.gameObject);
                        storeButtonGO.SetActive(true);
                        storeButtonGO.transform.position = __instance.argoButton.gameObject.transform.position;
                        storeButtonGO.transform.SetParent(__instance.argoButton.gameObject.transform.parent);
                        storeButtonGO.transform.localScale = Vector3.one;
                        storeButtonGO.transform.SetSiblingIndex(1);

                        storeButton = storeButtonGO.GetComponent<SGNavigationButton>();
                        storeButton.id = DropshipLocation.SHIP;
                        storeButton.SetupElement(__instance, __instance.radioSet, "Store",
                            LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.DropshipRoomCaptainsQuartersIcon, simulation);

                        // Create the staff button
                        Mod.Log.Info?.Write(" - Creating staff button");
                        GameObject staffButtonGO = GameObject.Instantiate(__instance.argoButton.gameObject);
                        staffButtonGO.SetActive(true);
                        staffButtonGO.transform.position = __instance.argoButton.gameObject.transform.position;
                        staffButtonGO.transform.SetParent(__instance.argoButton.gameObject.transform.parent);
                        staffButtonGO.transform.localScale = Vector3.one;
                        staffButtonGO.transform.SetSiblingIndex(7);

                        staffButton = staffButtonGO.GetComponent<SGNavigationButton>();
                        staffButton.id = DropshipLocation.CMD_CENTER;

                        staffButton.SetupElement(__instance, __instance.radioSet, "CMD Staff",
                            LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.DropshipRoomCommandCenterIcon, simulation);
                        staffButton.AddFlyoutButton("Darius", DropshipMenuType.Darius);
                        staffButton.AddFlyoutButton("Yang", DropshipMenuType.Yang);
                        staffButton.AddFlyoutButton("Sumire", DropshipMenuType.Sumire);
                        staffButton.AddFlyoutButton("Farah", DropshipMenuType.Farah);
                        if (simulation.GetCharacterStatus(SimGameState.SimGameCharacterType.ALEXANDER))
                        {
                            staffButton.AddFlyoutButton("Alexander", DropshipMenuType.Alexander);
                        }

                        // Create the memorial button
                        Mod.Log.Info?.Write(" - Creating memorial button");
                        GameObject memorialButtonGO = GameObject.Instantiate(__instance.argoButton.gameObject);
                        memorialButtonGO.SetActive(true);
                        memorialButtonGO.transform.position = __instance.argoButton.gameObject.transform.position;
                        memorialButtonGO.transform.SetParent(__instance.argoButton.gameObject.transform.parent);
                        memorialButtonGO.transform.localScale = Vector3.one;
                        memorialButtonGO.transform.SetSiblingIndex(9);

                        memorialButton = memorialButtonGO.GetComponent<SGNavigationButton>();
                        memorialButton.id = DropshipLocation.BARRACKS;

                        memorialButton.SetupElement(__instance, __instance.radioSet, "Memorial",
                            LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.DropshipRoomBarracksIcon, simulation);

                    }
                    catch (Exception e)
                    {
                        Mod.Log.Info?.Write("Error: " + e.Message);
                    }

                }
            }
        }

    }
}
