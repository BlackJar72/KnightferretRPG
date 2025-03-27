using UnityEngine;
using kfutils.rpg.ui;
using System.Collections.Generic;
using Unity.VisualScripting;



namespace kfutils.rpg {

    public static class InventoryManager {


#region Redraw Control

        private static List<IRedrawing> waitingToRedraw = new ();

        public delegate void InventoryUpdate(IInventory inv);
        public static event InventoryUpdate inventoryUpdated;

        public delegate void InventorySlotUpdate(IInventory inv, int slot);
        public static event InventorySlotUpdate inventorySlotUpdated;


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


        public static void SignalUpdate(IInventory inv) {
            inventoryUpdated?.Invoke(inv);
        }


        public static void SignalSlotUpdate(IInventory inv, int slot) {
            inventorySlotUpdated?.Invoke(inv, slot);
        }

#endregion


    }


}
