using UnityEngine;
using kfutils.rpg.ui;
using System.Collections.Generic;
using System.Buffers;



namespace kfutils.rpg {

    public static class InventoryManagement
    {

        public static Dictionary<string, InventoryData> inventoryData = new();
        public static InventoryData GetInventoryData(string id) => inventoryData.ContainsKey(id) ? inventoryData[id] : null;
        public static void StoreInventoryData(InventoryData data) => inventoryData.Add(data.ID, data);

        public static Dictionary<string, EquiptmentSlots> equiptData = new();
        public static EquiptmentSlots GetEquiptData(string id) => equiptData.ContainsKey(id) ? equiptData[id] : null;
        public static void StoreEquiptData(EquiptmentSlots data) => equiptData.Add(data.mainInventory.Owner.ID, data);
        public static void ReplaceEquiptData(EquiptmentSlots data)
        {
            equiptData.Remove(data.mainInventory.Owner.ID);
            equiptData.Add(data.mainInventory.Owner.ID, data);
        }

        public static Dictionary<string, Money> moneyData = new();
        public static Money GetMoneyData(string id) => moneyData.ContainsKey(id) ? moneyData[id] : -1;
        public static void StoreMoneyData(Money data, string id) => moneyData.Add(id, data);

        public static AInventory currentContainerInventory;

        public static HotBar hotBar;


        #region Redraw Control

        private static List<IRedrawing> waitingToRedraw = new();

        public delegate void InventoryUpdate(IInventory<ItemStack> inv);
        public static event InventoryUpdate inventoryUpdated;

        public delegate void InventoryUpdateAll();
        public static event InventoryUpdateAll inventoryUpdatedAll;


        public delegate void InventorySlotUpdate(IInventory<ItemStack> inv, int slot);
        public static event InventorySlotUpdate inventorySlotUpdated;

        public delegate void SpellbookUpdate(Spellbook inv);
        public static event SpellbookUpdate spellbookUpdated;

        public delegate void SlotsSwapped(SlotData slot1, SlotData slot2);
        public static event SlotsSwapped slotsSwappedEvent;

        public delegate void SlotEmptied(SlotData slot);
        public static event SlotEmptied slotEmptiedEvent;

        public delegate void HotbarActivated(SlotData slot);
        public static event HotbarActivated HotbarActivatedEvent;


        public static void Initialize()
        {
            waitingToRedraw = new List<IRedrawing>();
        }


        public static void SetInventoryData(Dictionary<string, InventoryData> loaded)
        {
            inventoryData = loaded;
        }


        public static void SetEquiptData(Dictionary<string, EquiptmentSlots> loaded)
        {
            equiptData = loaded;
            /*foreach (KeyValuePair<string, EquiptmentSlots> pair in equiptData)
            {
                Debug.Log(pair.Key);
                Debug.Log(pair.Value);
            }*/
        }


        public static void SetMoneyData(Dictionary<string, Money> loaded)
        {
            moneyData = loaded;
        }


        public static void AddRedraw(IRedrawing uiElement)
        {
            if (!waitingToRedraw.Contains(uiElement)) waitingToRedraw.Add(uiElement);
        }


        public static void DoRedraws()
        {
            for (int i = waitingToRedraw.Count - 1; i > -1; i--)
            {
                waitingToRedraw[i].DoRedraw();
                EquiptmentPanel equipt = waitingToRedraw[i] as EquiptmentPanel;
                if (equipt != null) equipt.ForwardUpdate();
                waitingToRedraw.RemoveAt(i);
            }
        }


        public static void SignalInventoryUpdate(IInventory<ItemStack> inv)
        {
            inventoryUpdated?.Invoke(inv);
        }


        public static void SignalSlotUpdate(IInventory<ItemStack> inv, int slot)
        {
            inventorySlotUpdated?.Invoke(inv, slot);
        }


        public static void SignalInventoryUpdateAll()
        {
            inventoryUpdatedAll?.Invoke();
        }


        public static void SignalSpellbookUpdate(Spellbook inv)
        {
            spellbookUpdated?.Invoke(inv);
        }


        public static void SignalSlotsSwapped(SlotData slot1, SlotData slot2)
        {
            slotsSwappedEvent?.Invoke(slot1, slot2);
        }


        public static void SignalSlotEmptied(SlotData slot)
        {
            slotEmptiedEvent?.Invoke(slot);
        }


        public static void SigalHotbarActivated(SlotData slot)
        {
            HotbarActivatedEvent?.Invoke(slot);
        }



        #endregion
        #region Misc Event Signalling

        public delegate void CloseAllInventories();
        public static event CloseAllInventories closeAllInvUI;

        public delegate void CloseStackManipulators();
        public static event CloseStackManipulators closeStackMainUI;

        public delegate void ToggleCharacterSheet();
        public static event ToggleCharacterSheet toggleCharacterSheet;

        public delegate void LoadNPCInventoryData();
        public static event LoadNPCInventoryData loadNPCInventoryData;


        public static void SignalCloseUIs()
        {
            closeAllInvUI?.Invoke();
        }


        public static void SignalStackManUIs()
        {
            closeStackMainUI?.Invoke();
        }


        public static void SignalToggleCharacterSheet()
        {
            toggleCharacterSheet?.Invoke();
        }


        public static void SignalLoadNPCInventoryData()
        {
            loadNPCInventoryData?.Invoke();
        }


#endregion






    }


}
