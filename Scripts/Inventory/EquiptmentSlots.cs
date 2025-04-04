using UnityEngine;
using kfutils.rpg.ui;
using System;


namespace kfutils.rpg {

    public class EquiptmentSlots : AInventory {

        public ItemStack[] slots = new ItemStack[12];

        private float weight;
        public override float Weight { get => weight; }

        public override int Count => slots.Length;

        public ItemStack GetItem(int index) => slots[index];


        private void Awake() {
            for(int i = 0; i < slots.Length; i++) {
                slots[i] = new ItemStack(null, 0, i);
            }
        }


        public override bool AddItemToSlot(int slot, ItemStack item) {
            slots[slot] = item;
            SignalUpdate();
            // TODO: Add equipting of item
            return true;
        }


        public override bool AddToFirstEmptySlot(ItemStack item) {
            return AddItemToSlot(item.slot, item);
        }


        public override bool AddToFirstReallyEmptySlot(ItemStack item) {
            return AddItemToSlot(item.slot, item);
        }


        public override float CalculateWeight() {
            weight = 0f;
            for(int i = 0; i < slots.Length; i++) {
                if(slots[i].item != null) {
                    weight += slots[i].stackSize * slots[i].item.Weight;
                }
            }
            return weight;
        }


        public override ItemStack GetByBackingIndex(int index) {
            return slots[index];
        }


        public override ItemStack GetItemInSlot(int slot) {
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
        [Obsolete] override public int GetLastSlot() {
            return slots.Length;
        }


        public override int GetNumberInSlot(int slot) {
            return slots[slot].stackSize;
        }


        public override bool HasItem(ItemStack item) {
            bool output = false;
            for(int i = 0; (i < slots.Length) && !output; i++) {
                output = output || (slots[i].item = item.item);
            }
            return output;
        }


        public override void RemoveFromSlot(int slot, int number) {
            number = Mathf.Min(number, slots[slot].stackSize);
            slots[slot].stackSize -= number;
            if(slots[slot].stackSize < 1) {
                // TODO: Add unequipting of item
                ClearItem(slots[slot]);
                SignalUpdate();
            } else {
                SignalSlotUpdate(slot);
            }
        }


        public override void RemoveAllFromSlot(int slot) {
            ClearItem(slots[slot]);
            SignalUpdate();
        }


        public override void RemoveItem(ItemStack item) {
            for(int i = 0; i < slots.Length; i++) {
                if(slots[i] == item) {
                    ClearItem(slots[i]);
                    SignalUpdate();
                    return;
                }
            }            



        }


    /// <summary>
    ///  This is not meaningful in this context; it will give an invalid result by design.
    ///  DO NOT USE THIS!
    /// </summary>
    /// <returns></returns>
    [Obsolete] public override int FindFirstEmptySlot() {
            return -1; // Invalid by design -- this should never be used.
        }
    }


}