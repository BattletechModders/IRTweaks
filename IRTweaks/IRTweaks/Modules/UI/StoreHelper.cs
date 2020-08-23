using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace IRTweaks.Modules.StoreUI {
    public class BuyHelper {

        readonly SG_Shop_Screen shopScreen;
        readonly SimGameState simGameState;
        InventoryDataObject_SHOP shopIDO;
        readonly MechLabInventoryWidget_ListView inventoryWidget;

        public BuyHelper(SG_Shop_Screen shopScreen, InventoryDataObject_SHOP selectedController, SimGameState simGameState) {
            this.shopScreen = shopScreen;
            this.simGameState = simGameState;
            shopIDO = selectedController;

            Traverse traverse = Traverse.Create(shopScreen).Field("inventoryWidget");
            inventoryWidget = (MechLabInventoryWidget_ListView)traverse.GetValue();
        }

        // Clone of SoldMultipleItems
        public void BuyMultipleItems(int quantityBought) {
            Mod.Log.Debug?.Write("BH:BMI entered.");

            //bool isNotStoreOrSalvage = shopIDO.GetItemType() != MechLabDraggableItemType.StorePart && 
            //    shopIDO.GetItemType() != MechLabDraggableItemType.SalvagePart;
            //if (simGameState.InMechLabStore() || !isNotStoreOrSalvage) {
            //    Mod.Logger.Info($"BH:BMI - in the mech lab store, or the part isn't a store part or salvage (is {shopIDO.GetItemType()}). Aborting!");
            //    return;
            //}

            ShopDefItem toBuySDI = shopIDO.shopDefItem;

            // Look for the item in the store inventory
            Shop itemShop = shopIDO.GetShop();
            List<ShopDefItem> activeInventory = itemShop.ActiveInventory;
            Shop.PurchaseType storePT = toBuySDI.IsInfinite ? Shop.PurchaseType.Normal : Shop.PurchaseType.Special;
            ShopDefItem storeSDI = activeInventory.Find((ShopDefItem cachedItem) => cachedItem.ID == toBuySDI.ID && cachedItem.Type == shopIDO.shopDefItem.Type);
            if (storeSDI == null) {
                Mod.Log.Info?.Write("BH:BMI - item not found in store inventory. Aborting!");
                return;
            }

            // Check the price
            int itemPrice = itemShop.GetPrice(storeSDI, storePT, itemShop.ThisShopType);
            Mod.Log.Info?.Write($"BH:BMI - itemPrice:{itemPrice} for purchaseType:{storePT}");
            int purchasePrice = itemPrice * quantityBought;
            if (purchasePrice > simGameState.Funds) {
                Mod.Log.Info?.Write($"BH:BMI - purchasePrice:{purchasePrice} (itemPrice:{itemPrice} x quantity:{quantityBought}) > funds: {simGameState.Funds}. Aborting!");
                return;
            }

            // Check the quantity available
            if (storePT != Shop.PurchaseType.Normal) {
                if (storeSDI.Count < quantityBought) {
                    Mod.Log.Info?.Write($"BH:BMI - store has quantity {storeSDI.Count} in inventory, but {quantityBought} was requested. Aborting!");
                    return;
                }

                storeSDI.Count -= quantityBought;
                if (storeSDI.Count < 1) {
                    Mod.Log.Debug?.Write($"BH:BMI - Store count below 1, removing!");
                    activeInventory.Remove(storeSDI);
                }
                Mod.Log.Debug?.Write($"BH:BMI - Reduced store count by {quantityBought} to {storeSDI.Count}!");
            }

            for (int i = 0; i < quantityBought; i++) {
                // Decrement the price
                simGameState.AddFunds(-itemPrice, null, true, true);
                simGameState.AddFromShopDefItem(storeSDI, false, itemPrice, SimGamePurchaseMessage.TransactionType.Purchase);
                Mod.Log.Debug?.Write($"BH:BMI - Reducing funds by -{itemPrice} and adding 1 of {storeSDI.ID}");

                if (!toBuySDI.IsInfinite) {
                    if (shopIDO.quantity > 1) {
                        shopIDO.ModifyQuantity(-1);
                        Mod.Log.Debug?.Write($"BH:BMI - reducing shop inventory by 1 to {shopIDO.quantity}");
                    } else {
                        Mod.Log.Debug?.Write($"BH:BMI - shop inventory below 1, removing!");
                        inventoryWidget.RemoveDataItem(shopIDO);
                        if (shopIDO != null) {
                            shopIDO.Pool();
                        }
                        shopIDO = null;
                    }

                }
            }

            inventoryWidget.RefreshInventoryList();

            Mod.Log.Debug?.Write("BH:BMI - Updating all money spots");
            shopScreen.UpdateMoneySpot();
            shopScreen.RefreshAllMoneyListings();

            if (toBuySDI.Type == ShopItemType.MechPart) {
                shopScreen.OnItemSelected(inventoryWidget.GetSelectedViewItem());
            }

            Mod.Log.Debug?.Write("BH:BMI - triggering iron man save");
            Traverse ironManT = Traverse.Create(shopScreen).Field("triggerIronManAutoSave");
            ironManT.SetValue(true);
        }
    }
}


