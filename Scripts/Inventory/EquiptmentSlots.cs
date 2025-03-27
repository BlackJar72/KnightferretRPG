using UnityEngine;
using kfutils.rpg.ui;
using System;
using UnityEditor.Build.Pipeline;
using Unity.Cinemachine;


namespace kfutils.rpg {

    public class EquiptmentSlots : MonoBehaviour, IInventory {

        private ItemStack[] slots = new ItemStack[12];

        private float weight;
        public float Weight { get => weight; } 


        public delegate void InventoryUpdate(Inventory inv);
        public event InventoryUpdate inventoryUpdated;

        public delegate void InventorySlotUpdate(Inventory inv, int slot);
        public event InventorySlotUpdate inventorySlotUpdated;


        public bool AddItemToSlot(int slot, ItemStack item) {
            slots[slot] = item;
            // TODO: Add equipting of item
            return false;
        }


        public bool AddToFirstEmptySlot(ItemStack item) {
            return false;
        }


        public float CalculateWeight() {
            weight = 0f;
            for(int i = 0; i < slots.Length; i++) {
                weight += slots[i].stackSize * slots[i].item.Weight;
            }
            return weight;
        }


        public ItemStack GetItemInSlot(int slot) {
            return slots[slot];
        }


        private void ClearItem(ItemStack stack) {
            stack.item = null;
            stack.stackSize = 0;
        }


        /// <summary>
        /// This will just return the index of the last equiptment slot, probably not 
        /// usedl in many (any?) circumstances, and I have no idea what another, more 
        /// useful or meaningful interpretation of this.
        /// </summary>
        /// <returns></returns>
        [Obsolete] public int GetLastSlot() {
            return slots.Length;
        }


        public int GetNumberInSlot(int slot) {
            return slots[slot].stackSize;
        }


        public bool HasItem(ItemStack item) {
            bool output = false;
            for(int i = 0; (i < slots.Length) && !output; i++) {
                output = output || (slots[i].item = item.item);
            }
            return output;
        }


        public void RemoveFromSlot(int slot, int number) {
            number = Mathf.Min(number, slots[slot].stackSize);
            slots[slot].stackSize -= number;
            if(slots[slot].stackSize < 1) {
                // TODO: Add unequipting of item
                ClearItem(slots[slot]);
            }
        }


        public void RemoveItem(ItemStack item) {
            for(int i = 0; i < slots.Length; i++) {
                if(slots[i] == item) {
                    ClearItem(slots[i]);
                    return;
                }
            }            
        }


        public void SignalSlotUpdate(int slot) {
            throw new System.NotImplementedException();
        }


        public void SignalUpdate() {
            throw new System.NotImplementedException();
        }


    }


}