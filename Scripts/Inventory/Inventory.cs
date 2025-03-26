using System;
using System.Collections.Generic;
using UnityEngine;
using kfutils.rpg.ui;
using UnityEditor.Rendering;


namespace kfutils.rpg {
    
    public class Inventory : MonoBehaviour, IInventory {

        public List<ItemStack> inventory = new();
        public float weight;     


        public delegate void InventoryUpdate(Inventory inv);
        public event InventoryUpdate inventoryUpdated;

        public delegate void InventorySlotUpdate(Inventory inv, int slot);
        public event InventorySlotUpdate inventorySlotUpdated;



        public void AddItem(ItemStack item) {
            inventory.Add(item);
            SignalUpdate();
        }


        public void RemoveItem(ItemStack item) {
            inventory.Remove(item);
            SignalUpdate();
        }


        public int GetLastSlot() {
            int output = 0;
            foreach(ItemStack stack in inventory) {
                output = Mathf.Max(output, stack.slot);
            }
            return output;
        }


        public float CalculateWeight() {
            weight = 0;
            foreach(ItemStack stack in inventory) {
                weight += (stack.item.Weight * stack.stackSize);
            }
            return weight;
        }


        public void SignalUpdate() {
            inventoryUpdated?.Invoke(this);
            CalculateWeight();
        }


        public void SignalSlotUpdate(int slot) {
            inventorySlotUpdated?.Invoke(this, slot);
            CalculateWeight();
        }





        /// <summary>
        /// Find an existing stack of this item type.
        /// </summary>
        /// <returns>-1 if no stack exists or if the item is not stackable.</returns>
        private int FindStack(ItemStack stack) {
            if (!stack.item.IsStackable) {
                return -1;
            }
            for(int i = 0; i < inventory.Count; i++) {
                if (inventory[i].item == stack.item) {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// Find an existing stack of this item type.
        /// </summary>
        /// <returns>-1 if no stack exists.</returns>
        private int FindIndex(ItemStack stack) {
            for(int i = 0; i < inventory.Count; i++) {
                if (inventory[i].item == stack.item) {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// Is there an instance of the item in the inventory?
        /// </summary>
        public bool HasItem(ItemStack item) {
            return inventory.Contains(item);
        }


        /// <summary>
        /// Return the item type in the given slot.
        /// </summary>
        public ItemStack GetItemInSlot(int slot) {
            for(int i = 0; i < inventory.Count; i++ ) {
                if(inventory[i].slot == slot) return inventory[i];
            }
            return null;
        }


        /// <summary>
        /// Get the number of items in the given slot.
        /// </summary>
        public int GetNumberInSlot(int slot) {
            for(int i = 0; i < inventory.Count; i++ ) {
                if(inventory[i].slot == slot) return inventory[i].stackSize;
            }
            return 0;
        }


        /// <summary>
        /// Remove a number of items from the given slot. Will never remove more
        /// that there are.
        /// </summary>
        public void RemoveFromSlot(int slot, int number)
        {
            for(int i = inventory.Count; i > -1; i-- ) {
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


        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <param name="number">The number of items to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public bool AddItemToSlot(int slot, ItemStack item, int number)
        {
            int i;
            for(i = 0; (i < inventory.Count) && (inventory[i].slot != slot); i++);
            if(i >= inventory.Count) AddToFirstEmptySlot(item);
            else if((inventory[i].item = item.item) && item.item.IsStackable) inventory[i].stackSize += item.stackSize; 
            else AddToFirstEmptySlot(item);
            SignalUpdate();
            return true;
        }
        

        /// <summary>
        /// Find an empty slot.
        /// </summary>
        /// <returns>-1 if all slots are full.</returns>
        /// FIXME: For large inventories this could become a problem unless there is an emply slot near the top, as O = n^2.
        ///        (Also, I'm not sure this is even needed, as slots are created on the fly.)
        private int FindEmptySlot()
        {
            int i = 0;
            bool found = false;
            do {
                foreach(ItemStack stack in inventory) {
                    found = found || (stack.slot == i);
                }
                if(found) return i;
                i++;
            } while(!found);
            return -1;
        }


        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="number">The number to add.</param>
        /// <returns>Whether or not the item could be added.</returns>
        public bool AddToFirstEmptySlot(ItemStack item) {
            inventory.Add(item);
            item.slot = inventory.IndexOf(item);
            return true;
        }


        /// <summary>
        /// Could this item fit anywhere in the inventory?
        /// </summary>
        public bool HasSpaceFor(ItemStack item) => true;
        

        
    }


}