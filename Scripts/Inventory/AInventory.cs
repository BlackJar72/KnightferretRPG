using Unity.VisualScripting;
using UnityEngine;


namespace kfutils.rpg {

    public abstract class AInventory : MonoBehaviour, IInventory
    {
        public abstract int Count { get; }
        public abstract float Weight { get; }
        public abstract bool AddItemToSlot(int slot, ItemStack item);
        public abstract bool AddToFirstEmptySlot(ItemStack item);
        public abstract float CalculateWeight();
        public abstract ItemStack GetByBackingIndex(int index);
        public abstract ItemStack GetItemInSlot(int slot);
        public abstract int GetLastSlot();
        public abstract int GetNumberInSlot(int slot);
        public abstract bool HasItem(ItemStack item);
        public abstract void RemoveFromSlot(int slot, int number);
        public abstract void RemoveItem(ItemStack item);


        public void SignalUpdate() {
            InventoryManager.SignalUpdate(this);
            CalculateWeight();
        }


        public void SignalSlotUpdate(int slot) {
            InventoryManager.SignalSlotUpdate(this, slot);
            CalculateWeight();
        }
        
    }

}
