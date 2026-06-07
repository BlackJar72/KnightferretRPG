using UnityEngine;
using kfutils.rpg.ui;
using System;
using System.Text;
using System.Collections;


namespace kfutils.rpg {


    [Serializable]
    public class EquiptmentSlots : IInventory<StackRef> {

        public bool belongsToPC = false;

        public StackRef[] slots = new StackRef[12];

        private float weight;
        public float Weight { get => weight; }

        public int Count => slots.Length;

        public StackRef GetItem(int index) => slots[index];

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


        public bool AddItemNoSlot(StackRef item)
        {
            int slot = ItemUtils.GetEquiptSlotForType(item.Item.EquiptType);
            if ((slot == 6) && slots[6].StackSize > 0) slot = 5;
            if ((slot > -1) && (slot < 12)) return AddItemToSlot(slot, item);
            return false;
        }


        public bool AddItemToSlot(int slot, StackRef item)
        {
            if ((item != null) && (item.Item != null) && (item.Item.EquiptType == EEquiptSlot.HANDS)) return AddTwoHandedItem(item);
            slots[slot] = item;
            item.Slot = slot;
            if (mainInventory.Owner == null) EquipItemDelayed(item);
            else mainInventory.Owner.EquiptItemToBody(item.Item as ItemStack); // FIXME!
            SignalUpdate();
            return true;
        }


        IEnumerator EquipItemDelayed(StackRef item)
        {
            yield return new WaitForSeconds(0.05f);
            yield return new WaitForEndOfFrame(); 
            mainInventory.Owner.EquiptItemToBody(item.Item as ItemStack);
        }


        public StackRef GetLHandItem() => slots[lhand];
        public StackRef GetRHandItem() => slots[rhand];


        public StackRef GetOtherHandItem(int slot) {
            if(slot == rhand) return slots[lhand];
            else return slots[rhand];
        }


        public bool AddTwoHandedItem(StackRef item) {
            if(item.Slot == lhand) item.Slot = rhand;
            if(item.Slot == rhand) {
                item.Slot = rhand;
                slots[rhand] = item;
                slots[lhand].CoAssign(item);
                slots[lhand].Slot = lhand;
                mainInventory.Owner.EquiptItemToBody(item.Item as ItemStack); // FIXME
                mainInventory.Owner.UnequiptItemFromBody(EEquiptSlot.LHAND);
                SignalUpdate();
                return true;
            }
            return false;
        }


        public int AddToFirstEmptySlot(StackRef item) {
            AddItemToSlot(item.Slot, item);
            return item.Slot;
        }


        public int AddToFirstReallyEmptySlot(StackRef item) {
            AddItemToSlot(item.Slot, item);
            return item.Slot;
        }


        public float CalculateWeight() {
            weight = 0f;
            for(int i = 0; i < slots.Length; i++) {
                if((slots[i].Item != null) && !((i == lhand) && (slots[i].Item.EquiptType == EEquiptSlot.HANDS))) {
                    weight += slots[i].StackSize * slots[i].Item.Weight; 
                }
            }
            return weight;
        }


        public StackRef GetByBackingIndex(int index) {
            return slots[index];
        }


        public StackRef GetItemInSlot(int slot) {
            return slots[slot];
        }


        private void ClearTwoHandedItem(StackRef stack) {
            slots[rhand].ClearRef();
            slots[lhand].ClearRef(); 
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
            return slots[slot].StackSize;
        }


        public bool HasItem(StackRef item) {
            bool output = false;
            for(int i = 0; (i < slots.Length) && !output; i++) {
                output = output || (slots[i].Item == item.Item);
            }
            return output;
        }


        public void RemoveFromSlot(int slot, int number) {
            number = Mathf.Min(number, slots[slot].StackSize);
            slots[slot].StackSize -= number;
            if(slots[slot].StackSize < 1) {
                if(slots[slot].Item.IsStackable) {
                    ItemManagement.itemRegistry.Remove(slots[slot].Item.ID);
                }
                mainInventory.Owner.UnequiptItemFromBody(slots[slot].Item as ItemStack); //FIXME
                RemoveAllFromSlot(slot);
            } else {
                SignalSlotUpdate(slot);
            }
        }


        public void RemoveAllFromSlot(int slot) {
                if(((slot == rhand) || (slot == lhand)) && (slots[slot] != null) && (slots[slot].Item != null) 
                            && (slots[slot].Item.EquiptType == EEquiptSlot.HANDS)) {
                    ClearTwoHandedItem(slots[slot]);
                } else {
                    mainInventory.Owner.UnequiptItemFromBody(slots[slot].Item as ItemStack); // FIXME
                    slots[slot].ClearRef();
                    SignalUpdate();
                }
            SignalUpdate();
        }


        public void Clear()
        {
            for(int i = 0; i < slots.Length; i++) {
                RemoveAllFromSlot(i);
            }
        }


        // Mostly the same as remove item, but slightly changed to update hotbar correctly.
        // Other ways, that avoid code duplication, would complicate things greatly in other ways.
        public void ConsumeItem(int slot, int number)
        {
            number = Mathf.Min(number, slots[slot].StackSize);
            slots[slot].StackSize -= number;
            if (slots[slot].StackSize < 1)
            {
                if (slots[slot].Item.IsStackable)
                {
                    ItemManagement.itemRegistry.Remove(slots[slot].Item.ID);
                }
                mainInventory.Owner.UnequiptItemFromBody(slots[slot].Item as ItemStack); // FIXME
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


        public void RemoveItem(StackRef item)
        {
            if ((item != null) && (item.Item != null) && (item.Item.EquiptType == EEquiptSlot.HANDS)) ClearTwoHandedItem(item);
            else for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i] == item)
                    {
                        mainInventory.Owner.UnequiptItemFromBody(item.Item as ItemStack); // FIXME
                        slots[i].ClearRef();
                        SignalUpdate();
                        return;
                    }
                }
        }


        public virtual void SignalUpdate() {
            // TODO (FIXME?): Is this needed?  How should this functionality now be handled if so?
            if (belongsToPC)
            {
                //InventoryManagement.SignalInventoryUpdate(this);
            }
            CalculateWeight();
        }


        public virtual void SignalSlotUpdate(int slot) {
            // TODO (FIXME?): Is this needed?  How should this functionality now be handled if so?
            //if (belongsToPC) InventoryManagement.SignalSlotUpdate(this, slot);
            //CalculateWeight();
        }


        /// <summary>
        ///  This is not meaningful in this context; it will give an invalid result by design.
        ///  DO NOT USE THIS!
        /// </summary>
        /// <returns></returns>
        [Obsolete] public int FindFirstEmptySlot() {
                return -1; // Invalid by design -- this should never be used.
        }


        public StackRef GetItemForSlotType(EEquiptSlot slotID)
        {
            if (slotID == EEquiptSlot.HANDS)
            {
                return slots[rhand];
            }
            for (int i = 0; i < slots.Length; i++)
            {
                if ((slots[i].Item != null) && slots[i].Item.EquiptType == slotID) return slots[i];
            }
            return null;
        }
        

        public bool OwnedByPC => belongsToPC;
        

        public static bool BelongsToPC(IInventory<StackRef> inv) => inv.OwnedByPC;
    }


}
