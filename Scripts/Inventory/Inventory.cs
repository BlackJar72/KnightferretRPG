using System;
using System.Collections.Generic;
using UnityEngine;
using kfutils.rpg.ui;
using UnityEditor.Rendering;


namespace kfutils.rpg {
    
    public class Inventory : MonoBehaviour, IInventory {

        public List<ItemStack> inventory = new();

        private float weight;
        public float Weight { get => weight; }     


        public delegate void InventoryUpdate(Inventory inv);
        public event InventoryUpdate inventoryUpdated;

        public delegate void InventorySlotUpdate(Inventory inv, int slot);
        public event InventorySlotUpdate inventorySlotUpdated;


        // For Testing; TODO??: Get rid of this, but add some other way to add starting gear ... or, do I need to...?
        [SerializeField] ItemStack.ProtoStack[] startingItems;


        void Start()
        {
            foreach(ItemStack.ProtoStack stack in startingItems) AddToFirstEmptySlot(stack.MakeStack());
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
        public bool AddItemToSlot(int slot, ItemStack item)
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


        public bool CanAddAtSlot(ItemStack item, int slot) => true;


    }


}