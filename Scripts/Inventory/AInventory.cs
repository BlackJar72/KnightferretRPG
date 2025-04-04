using Unity.VisualScripting;
using UnityEngine;


namespace kfutils.rpg {

    public abstract class AInventory : MonoBehaviour, IInventory<ItemStack>
    {
        public abstract int Count { get; }
        public abstract float Weight { get; }
        public abstract bool AddItemToSlot(int slot, ItemStack item);
        public abstract bool AddToFirstEmptySlot(ItemStack item);
        public abstract bool AddToFirstReallyEmptySlot(ItemStack item);
        public abstract float CalculateWeight();
        public abstract ItemStack GetByBackingIndex(int index);
        public abstract ItemStack GetItemInSlot(int slot);
        public abstract int GetLastSlot();
        public abstract int GetNumberInSlot(int slot);
        public abstract bool HasItem(ItemStack item);
        public abstract void RemoveFromSlot(int slot, int number);
        public abstract void RemoveAllFromSlot(int slot);
        public abstract void RemoveItem(ItemStack item);
        public abstract int FindFirstEmptySlot();


        public virtual void SignalUpdate() {
            InventoryManager.SignalInventoryUpdate(this);
            CalculateWeight();
        }


        public virtual void SignalSlotUpdate(int slot) {
            InventoryManager.SignalSlotUpdate(this, slot);
            CalculateWeight();
        }


    }

}
