using UnityEngine;
using kfutils.rpg.ui;
using System;
using System.Text;
using System.Buffers;


namespace kfutils.rpg {


    [Serializable]
    public class EquiptmentSlots : IInventory<ItemStack> {

        public bool belongsToPC = false;

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


        public void CopyInto(EquiptmentSlots other)
        {
            belongsToPC = other.belongsToPC;
            weight = other.weight;
            for (int i = 0; i < slots.Length; i++)
            {
                RemoveAllFromSlot(i);
                AddItemToSlot(i, other.slots[i]);
            }
        }


        public override string ToString()
        {
            StringBuilder sb = new(Environment.NewLine);
            sb.Append("Equitped items, numbering " + slots.Length + Environment.NewLine);
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                {
                    sb.Append(i + "   " + slots[i].ToString() + Environment.NewLine);
                }
                else
                {
                    sb.Append(i + "   NULL" + Environment.NewLine);
                }
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }


        public void PreSave()
        {
            InventoryManagement.ReplaceEquiptData(this);
        }


        public bool AddItemNoSlot(ItemStack item)
        {
            int slot = ItemUtils.GetEquiptSlotForType(item.item.EquiptType);
            if ((slot == 6) && slots[6].stackSize > 0) slot = 5;
            if ((slot > -1) && (slot < 12)) return AddItemToSlot(slot, item);
            return false;
        }


        public bool AddItemToSlot(int slot, ItemStack item)
        {
            if ((item != null) && (item.item != null) && (item.item.EquiptType == EEquiptSlot.HANDS)) return AddTwoHandedItem(item);
            slots[slot] = item;
            mainInventory.Owner.EquiptItemToBody(item);
            SignalUpdate();
            return true;
        }


        public ItemStack GetLHandItem() => slots[lhand];
        public ItemStack GetRHandItem() => slots[rhand];


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
                mainInventory.Owner.EquiptItemToBody(item);
                mainInventory.Owner.UnequiptItemFromBody(EEquiptSlot.LHAND);
                SignalUpdate();
                return true;
            }
            return false;
        }


        public int AddToFirstEmptySlot(ItemStack item) {
            AddItemToSlot(item.slot, item);
            return item.slot;
        }


        public int AddToFirstReallyEmptySlot(ItemStack item) {
            AddItemToSlot(item.slot, item);
            return item.slot;
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


        private void ClearTwoHandedItem(ItemStack stack) {
            slots[rhand] = new ItemStack(null, 0, rhand);
            slots[lhand] = new ItemStack(null, 0, lhand); 
            mainInventory.Owner.UnequiptItemFromBody(EEquiptSlot.RHAND);
            mainInventory.Owner.UnequiptItemFromBody(EEquiptSlot.LHAND);
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
                if(slots[slot].item.IsStackable) {
                    ItemManagement.itemRegistry.Remove(slots[slot].ID);
                }
                mainInventory.Owner.UnequiptItemFromBody(slots[slot]);
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
                    mainInventory.Owner.UnequiptItemFromBody(slots[slot]);
                    slots[slot] = new ItemStack(null, 0, slot);
                    SignalUpdate();
                }
            SignalUpdate();
        }


        // Mostly the same as remove item, but slightly changed to update hotbar correctly.
        // Other ways, that avoid code duplication, would complicate things greatly in other ways.
        public void ConsumeItem(int slot, int number)
        {
            number = Mathf.Min(number, slots[slot].stackSize);
            slots[slot].stackSize -= number;
            if (slots[slot].stackSize < 1)
            {
                if (slots[slot].item.IsStackable)
                {
                    ItemManagement.itemRegistry.Remove(slots[slot].ID);
                }
                mainInventory.Owner.UnequiptItemFromBody(slots[slot]);
                RemoveAllFromSlot(slot);
                ClearHotbarSlot(slot);
            }
            else
            {
                SignalSlotUpdate(slot);
            }
        }


        private void ClearHotbarSlot(int slot)
        {
            PlayerInventory playerInventory = mainInventory as PlayerInventory;
            if (playerInventory != null)
            {
                playerInventory.Hotbar.RemoveEquiptSlot(slot);
            }
        }


        public void RemoveItem(ItemStack item)
        {
            if ((item != null) && (item.item != null) && (item.item.EquiptType == EEquiptSlot.HANDS)) ClearTwoHandedItem(item);
            else for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i] == item)
                    {
                        mainInventory.Owner.UnequiptItemFromBody(item);
                        slots[i] = new ItemStack(null, 0, i);
                        SignalUpdate();
                        return;
                    }
                }
        }


        public virtual void SignalUpdate() {
            InventoryManagement.SignalInventoryUpdate(this);
            CalculateWeight();
        }


        public virtual void SignalSlotUpdate(int slot) {
            InventoryManagement.SignalSlotUpdate(this, slot);
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


        public ItemStack GetItemForSlotType(EEquiptSlot slot) { 
            if(slot == EEquiptSlot.HANDS) {
                return slots[rhand];
            }
            for(int i = 0; i < slots.Length; i++) {
                if((slots[i].item != null) && slots[i].item.EquiptType == slot) return slots[i];
            }
            return null;
        }

        public bool BelongsToPC(IInventory<ItemStack> inv) => belongsToPC;
    }


}
