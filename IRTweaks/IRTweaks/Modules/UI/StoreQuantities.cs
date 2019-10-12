using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using HBS;
using IRTweaks.Modules.StoreUI;
using Localize;
using System;
using UnityEngine;

namespace IRTweaks.Modules.UI {
    public static class State {
        public static bool StoreIsBuying = false;
        public static bool StoreIsSelling = false;
        public static void Reset() {
            StoreIsBuying = false;
            StoreIsSelling = false;
        }
    }

    public static class StoreQuantities {
        public static void MultiPurchasePopup_Refresh_Postfix(SG_Stores_MultiPurchasePopup __instance, int ___costPerUnit, int ___quantityBeingSold,
            LocalizableText ___TitleText, LocalizableText ___DescriptionText, string ___itemName, HBSDOTweenButton ___ConfirmButton) {
            Mod.Log.Debug("SG_S_MPP:R entered.");
            int value = ___costPerUnit * ___quantityBeingSold;
            Mod.Log.Debug($"SG_S_MPP:R   value:{value} = costPerUnit:{___costPerUnit} x quantityBeingSold:{___quantityBeingSold}.");

            if (State.StoreIsBuying) {
                ___TitleText.SetText($"BUY: {___itemName}", new object[] { });
                ___DescriptionText.SetText($"BUY FOR <color=#F79B26>{SimGameState.GetCBillString(value)}</color>", new object[] { });
                ___ConfirmButton.SetText("BUY");
            } else if (State.StoreIsSelling) {
                ___TitleText.SetText("SELL: {___itemName}", new object[] { });
                ___DescriptionText.SetText($"SELL FOR <color=#F79B26>{SimGameState.GetCBillString(value)}</color>", new object[] { });
                ___ConfirmButton.SetText("SELL");
            }

        }

        public static bool MultiPurchasePopup_ReceiveButtonPress_Prefix(SG_Stores_MultiPurchasePopup __instance, string button, ref int ___quantityBeingSold, int ___maxYouCanSell) {
            Mod.Log.Debug("SG_S_MPP:RCP entered.");
            if (button == "Cancel" || button == "Confirm") { return true; }

            var shiftIsPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var ctrlIsPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            int quantity;
            if (shiftIsPressed) { quantity = Mod.Config.Store.QuantityOnShift; } else if (ctrlIsPressed) { quantity = Mod.Config.Store.QuantityOnControl; } else { quantity = 1; }


            if (button == "Up") {
                int newQuantity = ___quantityBeingSold + quantity;
                Mod.Log.Debug($"  UP raw newQuantity:{newQuantity} = quantityBeingSold:{___quantityBeingSold} + quantity:{quantity}");
                if (newQuantity <= ___maxYouCanSell) {
                    ___quantityBeingSold = newQuantity;
                } else {
                    ___quantityBeingSold = ___maxYouCanSell;
                    Mod.Log.Debug($"  UP normalized quantity to:{___quantityBeingSold}");
                }
            } else if (button == "Down") {
                int newQuantity = ___quantityBeingSold - quantity;
                Mod.Log.Debug($"  DOWN raw newQuantity:{newQuantity} = quantityBeingSold:{___quantityBeingSold} - quantity:{quantity}");
                if (newQuantity > 1) {
                    ___quantityBeingSold = newQuantity;
                } else {
                    ___quantityBeingSold = 1;
                }
            } else if (button == "Max") {
                Mod.Log.Debug($"  MAX newQuantity = maxYouCanSell:{___maxYouCanSell}");
                ___quantityBeingSold = ___maxYouCanSell;
            } else if (button == "Min") {
                Mod.Log.Debug($"  MIN newQuantity = 1");
                ___quantityBeingSold = 1;
            }


            __instance.Refresh();

            return false;
        }

        public static bool Shop_Screen_ReceiveButtonPress_Prefix(SG_Shop_Screen __instance, string button,
            InventoryDataObject_SHOP ___selectedController, bool ___isInBuyingState, SimGameState ___simState) {

            Mod.Log.Debug($"SG_S_S:RBP entered with button:({button})");

            State.Reset();
            if (button != "Capitalism" || ___selectedController == null) {
                return true;
            } else {
                int cBillValue = ___selectedController.GetCBillValue();
                if (___isInBuyingState) {
                    Mod.Log.Debug($"SG_S_S:RBP - processing a purchase.");

                    if (___simState.InMechLabStore() &&
                        (___selectedController.GetItemType() == MechLabDraggableItemType.StorePart ||
                            ___selectedController.GetItemType() == MechLabDraggableItemType.SalvagePart)) {
                        // TODO: Can we handle this better than HBS does?
                        return false;
                    }
                    Shop shop = ___selectedController.GetShop();
                    int price = shop.GetPrice(___selectedController.shopDefItem, Shop.PurchaseType.Normal, shop.ThisShopType);
                    if (___selectedController.quantity > 1 || ___selectedController.shopDefItem.IsInfinite) {
                        State.StoreIsBuying = true;

                        if (___selectedController.shopDefItem.IsInfinite) { ___selectedController.quantity = 99; }
                        BuyHelper buyHelper = new BuyHelper(__instance, ___selectedController, ___simState);

                        int maxCanPurchase = (int)Math.Floor(___simState.Funds / (double)price);
                        Mod.Log.Debug($"SG_S_S:RBP - maxCanPurchase:{maxCanPurchase} = funds:{___simState.Funds} / price:{price}.");
                        int popupQuantity = maxCanPurchase < ___selectedController.quantity ? maxCanPurchase : ___selectedController.quantity;
                        Mod.Log.Debug($"SG_S_S:RBP - maxCanPurchase:{maxCanPurchase} controllerQuantity:{___selectedController.quantity} -> popupQuantity:{popupQuantity}.");

                        SG_Stores_MultiPurchasePopup orCreatePopupModule =
                            LazySingletonBehavior<UIManager>.Instance.GetOrCreatePopupModule<SG_Stores_MultiPurchasePopup>(string.Empty);
                        orCreatePopupModule.SetData(___simState, ___selectedController.shopDefItem,
                            ___selectedController.GetName(), popupQuantity, price, buyHelper.BuyMultipleItems);
                    } else {
                        GenericPopupBuilder.Create("Confirm?", Strings.T("Purchase for {0}?", SimGameState.GetCBillString(price)))
                            .AddButton("Cancel")
                            .AddButton("Accept", __instance.BuyCurrentSelection)
                            .CancelOnEscape()
                            .AddFader(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill)
                            .Render();
                    }

                } else {
                    Mod.Log.Debug($"SG_S_S:RBP - processing a sale.");
                    State.StoreIsSelling = true;
                    int num = cBillValue;
                    if (___selectedController.quantity > 1) {
                        SG_Stores_MultiPurchasePopup orCreatePopupModule =
                            LazySingletonBehavior<UIManager>.Instance.GetOrCreatePopupModule<SG_Stores_MultiPurchasePopup>(string.Empty);
                        orCreatePopupModule.SetData(___simState, ___selectedController.shopDefItem,
                            ___selectedController.GetName(), ___selectedController.quantity, num, __instance.SoldMultipleItems);
                    } else if (num >= ___simState.Constants.Finances.ShopWarnBeforeSellingPriceMinimum) {
                        GenericPopupBuilder.Create("Confirm?", Strings.T("Sell for {0}?", SimGameState.GetCBillString(num)))
                                .AddButton("Cancel")
                                .AddButton("Accept", __instance.SellCurrentSelection)
                                .CancelOnEscape()
                                .AddFader(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill)
                                .Render();
                    } else {
                        // Sell a single instance
                        __instance.SellCurrentSelection();
                    }
                }
                return false;
            }
        }
    }
}
