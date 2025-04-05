using UnityEngine;
using kfutils.rpg.ui;
using System;


namespace kfutils.rpg {

    [Serializable]
    public class EquiptmentSlots : IInventory<ItemStack> {

        public ItemStack[] slots = new ItemStack[12];

        private float weight;
        public float Weight { get => weight; }

        public int Count => slots.Length;

        public ItemStack GetItem(int index) => slots[index];

        public CharacterInventory mainInventory;


        private void Awake() {
            for(int i = 0; i < slots.Length; i++) {
                slots[i] = new ItemStack(null, 0, i);
            }
        }


        public bool AddItemToSlot(int slot, ItemStack item) {
            slots[slot] = item;
            item.CopyInto(slots[slot]);
            SignalUpdate();
            // TODO: Add equipting of item
            return true;
        }


        public bool AddToFirstEmptySlot(ItemStack item) {
            return AddItemToSlot(item.slot, item);
        }


        public bool AddToFirstReallyEmptySlot(ItemStack item) {
            return AddItemToSlot(item.slot, item);
        }


        public float CalculateWeight() {
            weight = 0f;
            for(int i = 0; i < slots.Length; i++) {
                if(slots[i].item != null) {
                    weight += slots[i].stackSize * slots[i].item.Weight;
                }
            }
            return weight;
        }


        public ItemStack GetByBackingIndex(int index) {
            return slots[index];
        }


        public ItemStack GetItemInSlot(int slot) {
            return slots[slot];
        }


        private void ClearItem(ItemStack stack) {
            stack.item = null;
            stack.stackSize = 0;
            SignalUpdate();
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
                slots[slot] = new ItemStack(null, 0, slot);
                SignalUpdate();
            } else {
                SignalSlotUpdate(slot);
            }
        }


        public void RemoveAllFromSlot(int slot) {
            slots[slot] = new ItemStack(null, 0, slot);
            SignalUpdate();
        }


        public void RemoveItem(ItemStack item) {
            for(int i = 0; i < slots.Length; i++) {
                if(slots[i] == item) {
                    slots[i] = new ItemStack(null, 0, i);
                    SignalUpdate();
                    return;
                }
            }
        }


        public virtual void SignalUpdate() {
            InventoryManager.SignalInventoryUpdate(this);
            CalculateWeight();
        }


        public virtual void SignalSlotUpdate(int slot) {
            InventoryManager.SignalSlotUpdate(this, slot);
            CalculateWeight();
        }


        /// <summary>
        ///  This is not meaningful in this context; it will give an invalid result by design.
        ///  DO NOT USE THIS!
        /// </summary>
        /// <returns></returns>
        [Obsolete] public int FindFirstEmptySlot() {
                return -1; // Invalid by design -- this should never be used.
        }


    }


}
