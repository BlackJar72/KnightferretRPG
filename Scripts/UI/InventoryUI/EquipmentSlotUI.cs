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
                        ItemStack localStack = item.Copy();
                        ItemStack otherStack = other.item.Copy();
                        inventory.RemoveAllFromSlot(slotNumber);
                        inventory.RemoveAllFromSlot(other.slotNumber);
                        inventory.AddItemToSlot(other.slotNumber, localStack);
                        inventory.AddItemToSlot(slotNumber, otherStack);
                    }
                    inventory.SignalUpdate();           
            }
        }


    }

}
