using System;
using System.Collections.Generic;
using UnityEngine;
using kfutils.rpg.ui;


namespace kfutils.rpg {
    
    public class Inventory : AInventory {

        public List<ItemStack> inventory = new();

        private float weight;
        public override float Weight { get => weight; }

        public override int Count => inventory.Count;


        public delegate void InventoryUpdate(IInventory<ItemStack> inv);
        public event InventoryUpdate inventoryUpdated;

        public delegate void InventorySlotUpdate(IInventory<ItemStack> inv, int slot);
        public event InventorySlotUpdate inventorySlotUpdated;



        // For Testing; TODO??: Get rid of this, but add some other way to add starting gear ... or, do I need to...?
        [SerializeField] ItemStack.ProtoStack[] startingItems;


        void Start() // FIXME: This will need to be moved to an method run only at the start of a new game!!
        {
            foreach(ItemStack.ProtoStack stack in startingItems) AddToFirstEmptySlot(stack.MakeStack());
        }


        public override void RemoveItem(ItemStack item) {
            inventory.Remove(item);
            SignalUpdate();
        }

        public override ItemStack GetByBackingIndex(int index) {
            return inventory[index];
        }



        public override int GetLastSlot() {
            int output = 0;
            foreach(ItemStack stack in inventory) {
                output = Mathf.Max(output, stack.slot);
            }
            return output;
        }


        public override float CalculateWeight() {
            weight = 0;
            foreach(ItemStack stack in inventory) {
                if((stack != null) && (stack.item != null)) {
                    weight += stack.item.Weight * stack.stackSize;
                }
            }
            return weight;
        }


        /// <summary>
        /// Is there an instance of the item in the inventory?
        /// </summary>
        public override bool HasItem(ItemStack item) {
            return inventory.Contains(item);
        }


        /// <summary>
        /// Return the item type in the given slot.
        /// </summary>
        public override ItemStack GetItemInSlot(int slot) {
            for(int i = 0; i < inventory.Count; i++ ) {
                if(inventory[i].slot == slot) return inventory[i];
            }
            return null;
        }


        /// <summary>
        /// Get the number of items in the given slot.
        /// </summary>
        public override int GetNumberInSlot(int slot) {
            for(int i = 0; i < inventory.Count; i++ ) {
                if(inventory[i].slot == slot) return inventory[i].stackSize;
            }
            return 0;
        }


        /// <summary>
        /// Remove a number of items from the given slot. Will never remove more
        /// that there are.
        /// </summary>
        public override void RemoveFromSlot(int slot, int number) {
            for(int i = inventory.Count - 1; i > -1; i-- ) {
                if(inventory[i].slot == slot) {
                    number = Mathf.Min(number, inventory[i].stackSize);
                    inventory[i].stackSize -= number;
                    if(inventory[i].stackSize < 1) {
                        inventory.RemoveAt(i);
                        SignalUpdate();
                    } else {
                        SignalSlotUpdate(i);
                    }
                    return;                    
                }
            }
        }


        public override void RemoveAllFromSlot(int slot) {
            for(int i = inventory.Count - 1; i > -1; i--) {
                if(inventory[i].slot == slot) {
                    inventory.RemoveAt(i);
                    SignalUpdate();
                    return;
                }
            }
        }


        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <param name="number">The number of items to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public override bool AddItemToSlot(int slot, ItemStack item)
        {
            inventory.Add(item);
            SignalUpdate();
            return true;
        }


        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="number">The number to add.</param>
        /// <returns>Whether or not the item could be added.</returns>
        public override bool AddToFirstEmptySlot(ItemStack item) {
            if((item == null) || (item.item == false)) return true;
            if(item.item.IsStackable) {
                for(int i = 0; i < inventory.Count; i++) {
                    if(inventory[i].item == item.item) {
                        inventory[i].stackSize += item.stackSize;
                        SignalSlotUpdate(inventory[i].slot);
                        return true;
                    }
                }
            }
            item.slot = FindFirstEmptySlot();
            inventory.Add(item);
            SignalUpdate();
            return true;
        }


        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="number">The number to add.</param>
        /// <returns>Whether or not the item could be added.</returns>
        public override bool AddToFirstReallyEmptySlot(ItemStack item) {
            if((item == null) || (item.item == false)) return true;
            if((item.slot < 0) || (GetItemInSlot(item.slot) != null)) {
                item.slot = FindFirstEmptySlot();
            }
            inventory.Add(item);
            SignalUpdate();
            return true;
        }


        public override int FindFirstEmptySlot() {
            // FIXME: Find a way with better time complexity than O=n^2
            int i = 0;
            int last = GetLastSlot() + 1;
            bool found = false;
            for(i = 0; i < last; i++) {
                found = true;
                for(int j = 0; (j < inventory.Count) && found; j++) {
                    found = found && (inventory[j].slot != i);
                }
                if(found) break;
            }                
            return i;
        }


        public bool CanAddAtSlot(ItemStack item, int slot) => true;
        
        
    }


}