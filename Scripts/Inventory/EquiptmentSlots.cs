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


        const int rhand = 4;
        const int lhand = 3;
        
        public const int rring = 6;
        public const int lring = 5;


        private void Awake() {
            for(int i = 0; i < slots.Length; i++) {
                slots[i] = new ItemStack(null, 0, i);
            }
        }


        public bool AddItemToSlot(int slot, ItemStack item) {
            if((item != null) && (item.item != null) && (item.item.EquiptType == EEquiptSlot.HANDS)) return AddTwoHandedItem(item);
            slots[slot] = item;
            mainInventory.Owner.EquiptItem(item);
            SignalUpdate();
            return true;
        }


        public ItemStack GetLHandItem() => slots[lhand];


        public ItemStack GetOtherHandItem(int slot) {
            if(slot == rhand) return slots[lhand];
            else return slots[rhand];
        }


        public bool AddTwoHandedItem(ItemStack item) {
            if(item.slot == lhand) item.slot = rhand;
            if(item.slot == rhand) {
                slots[rhand] = item;
                slots[lhand] = item.Copy();
                slots[lhand].slot = lhand;
                mainInventory.Owner.EquiptItem(item);
                mainInventory.Owner.UnequiptItem(EEquiptSlot.LHAND);
                SignalUpdate();
                return true;
            }
            return false;
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
                if((slots[i].item != null) && !((i == lhand) && (slots[i].item.EquiptType == EEquiptSlot.HANDS))) {
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


        private void ClearTwoHandedItem(ItemStack stack) {
            slots[rhand] = new ItemStack(null, 0, rhand);
            slots[lhand] = new ItemStack(null, 0, lhand); 
            mainInventory.Owner.UnequiptItem(EEquiptSlot.RHAND);
            mainInventory.Owner.UnequiptItem(EEquiptSlot.LHAND);
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
                mainInventory.Owner.UnequiptItem(slots[slot]);
                RemoveAllFromSlot(slot);
            } else {
                SignalSlotUpdate(slot);
            }
        }


        public void RemoveAllFromSlot(int slot) {
                if(((slot == rhand) || (slot == lhand)) && (slots[slot] != null) && (slots[slot].item != null) 
                            && (slots[slot].item.EquiptType == EEquiptSlot.HANDS)) {
                    ClearTwoHandedItem(slots[slot]);
                } else {
                    mainInventory.Owner.UnequiptItem(slots[slot]);
                    slots[slot] = new ItemStack(null, 0, slot);
                    SignalUpdate();
                }
            SignalUpdate();
        }


        public void RemoveItem(ItemStack item) {
            if((item != null) && (item.item != null) && (item.item.EquiptType == EEquiptSlot.HANDS)) ClearTwoHandedItem(item);
            else for(int i = 0; i < slots.Length; i++) {
                if(slots[i] == item) {
                    mainInventory.Owner.UnequiptItem(item);
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
