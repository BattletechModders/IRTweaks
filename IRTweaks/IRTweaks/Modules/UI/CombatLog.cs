using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using DG.Tweening;
using Harmony;
using HBS;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace IRTweaks.Modules.UI {
    public static class CombatLog {

        private static CombatHUDInfoSidePanel infoSidePanel;
        private static CombatChatModule combatChatModule;

        private static bool messagesSent = false;

        private static CombatGameState combat;
        private static MessageCenter messageCenter;

        public static void CombatHUD_Init_Postfix(CombatHUD __instance, CombatGameState Combat) {

            Mod.Log.Info("Initialization of CombatHUD");

            CombatLog.infoSidePanel = LazySingletonBehavior<UIManager>.Instance.GetOrCreateUIModule<CombatHUDInfoSidePanel>("", true);
            infoSidePanel.Init();
            infoSidePanel.Visible = false;

            // Combat Chat module
            CombatLog.combatChatModule = LazySingletonBehavior<UIManager>.Instance.GetOrCreateUIModule<CombatChatModule>("", true);
            if (CombatLog.combatChatModule == null) {
                Mod.Log.Error("Error creating combat chat module");
            } else {
                CombatLog.combatChatModule.CombatInit();
                CombatLog.infoSidePanel.BumpUp();
            }

            Mod.Log.Info($"CombatChatModule pos: {CombatLog.combatChatModule.gameObject.transform.position}");
            Mod.Log.Info($"RetreatEscMenu pos: {__instance.RetreatEscMenu.gameObject.transform.position}");

            Vector3 newPos = CombatLog.combatChatModule.gameObject.transform.position;
            newPos.x = __instance.RetreatEscMenu.gameObject.transform.position.x * 1.25f;
            newPos.y = __instance.RetreatEscMenu.gameObject.transform.position.y - 100f;
            //newPos.z = __instance.WeaponPanel.gameObject.transform.position.z;

            CombatLog.combatChatModule.gameObject.transform.position = newPos;
            Mod.Log.Info($"new CombatChatModule pos: {newPos}");

            combat = Combat;
        }

        public static void CombatHUD_OnCombatGameDestroyed_Postfix() {
            Mod.Log.Info("Combat game destroyed, cleaning up");
            combat = null;
        }

        public static void CombatHUD_Update_Postfix(CombatHUD __instance) {
            if (__instance.Combat.TurnDirector.GameHasBegun) {

                if (!CombatLog.messagesSent) {
                    Mod.Log.Info("On update - sending messages ");
                    __instance.Combat.MessageCenter.PublishMessage(new ChatMessage("AIM", "foo1", false));
                    __instance.Combat.MessageCenter.PublishMessage(new ChatMessage("AIM", "foo2", false));
                    __instance.Combat.MessageCenter.PublishMessage(new ChatMessage("AIM", "foo3", false));

                    __instance.Combat.MessageCenter.PublishMessage(new ChatMessage("CAC", "foo1", false));
                    __instance.Combat.MessageCenter.PublishMessage(new ChatMessage("CAC", "foo2", false));
                    __instance.Combat.MessageCenter.PublishMessage(new ChatMessage("CAC", "foo3", false));

                    CombatLog.messagesSent = true;
                }
            }
        }

        public static void CombatChatModule_Init_Postfix(CombatChatModule __instance, MessageCenter ____messageCenter,
            HBSDOTweenButton ____chatBtn, HBSDOTweenButton ____muteBtn, ActiveChatListView ____activeChatList) {
            ____chatBtn.enabled = false;
            ____chatBtn.gameObject.SetActive(false);
            ____muteBtn.enabled = false;
            ____muteBtn.gameObject.SetActive(false);

            Traverse chatListRectTransT = Traverse.Create(____activeChatList).Field("containerTransform");
            RectTransform chatListRectTrans = chatListRectTransT.GetValue<RectTransform>();
            Mod.Log.Info($"ChatList transform pos: {chatListRectTrans.position}");

            Rect origRect = chatListRectTrans.rect;
            Mod.Log.Info($"ChatList original dims: {origRect.height}h x {origRect.width}w");
            origRect.height += 50;
            Mod.Log.Info($"ChatList new      dims: {origRect.height}h x {origRect.width}w");

            ____messageCenter.AddSubscriber(MessageCenterMessageType.FloatieMessage, OnFloatie);
            messageCenter = ____messageCenter;
        }

        public static void CombatChatModule_OnPooled_Postfix(MessageCenter ____messageCenter) {
            ____messageCenter.RemoveSubscriber(MessageCenterMessageType.FloatieMessage, OnFloatie);
            messageCenter = null;
        }

        private static void OnFloatie(MessageCenterMessage message) {
            FloatieMessage floatieMessage = (FloatieMessage)message;

            AbstractActor target = combat.FindActorByGUID(floatieMessage.affectedObjectGuid);
            messageCenter.PublishMessage(new ChatMessage(target.DisplayName, floatieMessage.text.ToString(), false));
        }

        public static void CombatChatModule_CombatInit_Postfix(CombatChatModule __instance, MessageCenter ____messageCenter,
            HBSDOTweenButton ____chatBtn, HBSDOTweenButton ____muteBtn, HBS_InputField ____inputField, GameObject ____activeChatWindow) {
            ____chatBtn.enabled = true;
            ____chatBtn.gameObject.SetActive(true);
            ____muteBtn.enabled = false;
            ____muteBtn.gameObject.SetActive(false);
            ____inputField.enabled = false;
            ____inputField.gameObject.SetActive(false);

            // Hide the send button
            Transform sendButtonT = ____activeChatWindow.gameObject.transform.Find("uixPrf_genericButton");
            if (sendButtonT != null) {
                sendButtonT.gameObject.SetActive(false);
            } else {
                Mod.Log.Info("Could not find send button to disable!");
            }

            // Set the scroll spacing to 0
            Transform scrollListT = ____activeChatWindow.gameObject.transform.Find("panel_history/uixPrfPanl_listView/ScrollRect/Viewport/List");
            if (scrollListT != null) {
                VerticalLayoutGroup scrollListVLG = scrollListT.gameObject.GetComponent<VerticalLayoutGroup>();
                scrollListVLG.spacing = 0;
            } else {
                Mod.Log.Info("Could not find scrollList to change spacing!");
            }

            // Resize the image background
            Transform imageBackgroundT = ____activeChatWindow.gameObject.transform.Find("image_background");
            if (imageBackgroundT != null) {
                RectTransform imageBackgroundRT = imageBackgroundT.gameObject.GetComponent<RectTransform>();
                Rect ibRect = imageBackgroundRT.rect;
                Mod.Log.Info($"Background image size: {ibRect.height}h x {ibRect.width}");
                Vector3 newPos = imageBackgroundRT.position;
                newPos.y += 10;
                imageBackgroundRT.position = newPos;
                imageBackgroundRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ibRect.height - 40);
                imageBackgroundRT.ForceUpdateRectTransforms();
            } else {
                Mod.Log.Info("Could not find imageBackground to change size!");
            }


        }

        public static bool CombatChatModule_OnChatMessage_Prefix(CombatChatModule __instance, MessageCenterMessage message,
            ActiveChatListView ____activeChatList) {

            ChatMessage chatMessage = (ChatMessage)message;
            Mod.Log.Info($"Chat message is: {chatMessage}");
            try {
                ____activeChatList.Add(chatMessage);
                ____activeChatList.ScrollToBottom();
                ____activeChatList.Refresh();
            } catch (Exception e) {
                Mod.Log.Error($"Failed to send a message:{e.Message}");
                Mod.Log.Error($"{e.StackTrace}");
            }

            return false;
        }

        public static bool ChatListViewItem_SetData_Prefix(ChatListViewItem __instance, ChatMessage message,
            LocalizableText ____chatMessage) {

            string tagColor = "#" + ColorUtility.ToHtmlStringRGBA(LazySingletonBehavior<UIManager>.Instance.UIColorRefs.gold);
            string messageColor = "#" + ColorUtility.ToHtmlStringRGBA(LazySingletonBehavior<UIManager>.Instance.UIColorRefs.whiteHalf);
            ____chatMessage.text = "<line-height=50%>" +
                $"<size=-2><{tagColor}>{message.SenderName}</color></size>  " +
                $"<size=-3><{messageColor}>{message.Message}</color></size>" +
                "";
            
            DOTweenAnimation componentInChildren = ____chatMessage.GetComponentInChildren<DOTweenAnimation>();
            if (componentInChildren != null) {
                componentInChildren.delay = 50;
                componentInChildren.CreateTween();
                componentInChildren.DOPlay();
            }

            return false;
        }
    }
}
