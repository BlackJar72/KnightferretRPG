using UnityEngine;


namespace kfutils.rpg {

    public interface IInventory {


        public void RemoveItem(ItemStack item);

        public int GetLastSlot() ;
        public float CalculateWeight();
        public void SignalUpdate();
        public void SignalSlotUpdate(int slot);


        public int Count { get; }

        public ItemStack GetByBackingIndex(int index);

        public float Weight { get; } 
        

        /// <summary>
        /// Is there an instance of the item in the inventory?
        /// </summary>
        public bool HasItem(ItemStack item);


        /// <summary>
        /// Return the item type in the given slot.
        /// </summary>
        public ItemStack GetItemInSlot(int slot);


        /// <summary>
        /// Get the number of items in the given slot.
        /// </summary>
        public int GetNumberInSlot(int slot);


        /// <summary>
        /// Remove a number of items from the given slot. Will never remove more
        /// that there are.
        /// </summary>
        public void RemoveFromSlot(int slot, int number);
        
        public void RemoveAllFromSlot(int slot);


        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <param name="number">The number of items to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public bool AddItemToSlot(int slot, ItemStack item);


        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="number">The number to add.</param>
        /// <returns>Whether or not the item could be added.</returns>
        public bool AddToFirstEmptySlot(ItemStack item);



    }

}
