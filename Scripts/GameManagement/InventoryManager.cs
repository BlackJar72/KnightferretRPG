using UnityEngine;
using kfutils.rpg.ui;
using System.Collections.Generic;



namespace kfutils.rpg {

    public static class InventoryManager {

        public static AInventory currentContainerInventory;


#region Redraw Control

        private static List<IRedrawing> waitingToRedraw = new ();

        public delegate void InventoryUpdate(IInventory<ItemStack> inv);
        public static event InventoryUpdate inventoryUpdated;

        public delegate void InventorySlotUpdate(IInventory<ItemStack> inv, int slot);
        public static event InventorySlotUpdate inventorySlotUpdated;

        public delegate void SpellbookUpdate(Spellbook inv);
        public static event SpellbookUpdate spellbookUpdated;



        public static void Initialize() {
            waitingToRedraw = new List<IRedrawing>();
        }


        public static void AddRedraw(IRedrawing uiElement) {
            waitingToRedraw.Add(uiElement);
        }


        public static void DoRedraws() {
            for(int i = waitingToRedraw.Count - 1; i > -1; i--) {
                waitingToRedraw[i].DoRedraw();
                waitingToRedraw.RemoveAt(i);
            }
        }


        public static void SignalInventoryUpdate(IInventory<ItemStack> inv) {
            inventoryUpdated?.Invoke(inv);
        }


        public static void SignalSlotUpdate(IInventory<ItemStack> inv, int slot) {
            inventorySlotUpdated?.Invoke(inv, slot);
        }


        public static void SignalSpellbookUpdate(Spellbook inv) {
            spellbookUpdated?.Invoke(inv);
        }



#endregion
#region Misc Event Signalling

        public delegate void CloseAllInventories();
        public static event CloseAllInventories closeAllInvUI;

        public delegate void CloseStackManipulators();
        public static event CloseStackManipulators closeStackManUI;

        public delegate void ToggleCharacterSheet();
        public static event ToggleCharacterSheet toggleCharacterSheet;


        public static void SignalCloseUIs() {
            closeAllInvUI?.Invoke();
        }


        public static void SignalStackManUIs() {
            closeStackManUI?.Invoke();
        }


        public static void SignalToggleCharacterSheet() {
            toggleCharacterSheet?.Invoke();
        }


#endregion






    }


}
