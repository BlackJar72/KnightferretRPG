using UnityEngine;

namespace kfutils.rpg.ui {

    public class EquipmentSlotUI : InventorySlotUI {


        public override void SwapWith(InventorySlotUI other)
        {
            base.SwapWith(other);
            if((inventory == other.inventory) && (item.slot != slotNumber)) {
            if(!CanSwapSlotTypes(other)) return;
            if((other.item.item == item.item) && item.item.IsStackable) {
                item.stackSize += other.item.stackSize;
                other.inventory.RemoveItem(other.item);
            } else {
                item.slot = other.slotNumber;
                other.item.slot = slotNumber;
                if(other.inventory != inventory) {
                    other.inventory.AddItemToSlot(item.slot, item);
                    inventory.AddItemToSlot(other.item.slot, other.item);
                    inventory.RemoveItem(item);
                    other.inventory.RemoveItem(other.item);
                } else {
                    inventory.AddItemToSlot(slotNumber, other.item.Copy());
                    inventory.RemoveAllFromSlot(other.slotNumber);
                }
            }
            inventory.SignalUpdate();           
        }
        }


    }

}
