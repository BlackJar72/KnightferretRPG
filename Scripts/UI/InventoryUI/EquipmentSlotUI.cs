using UnityEngine;
using UnityEngine.EventSystems;

namespace kfutils.rpg.ui {

    public class EquipmentSlotUI : InventorySlotUI {


        public override bool SwapWith(InventorySlotUI other)
        {
            if(base.SwapWith(other)) {
                if((inventory == other.inventory) && (item.slot != slotNumber)) {
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
                return true;
            }
            return false;
        }


        public override void OnPointerClick(PointerEventData eventData) {
            if(eventData.button == PointerEventData.InputButton.Left) {
                GameManager.Instance.UIManager.HideItemToolTip();
                GameManager.Instance.UIManager.HideItemStackManipulator();
                if(eventData.clickCount == 2) {
                    ItemStack toMove = item.Copy();
                    inventory.RemoveItem(item);
                    EntityManagement.playerCharacter.Inventory.AddToFirstEmptySlot(toMove);
                }
            } 
        }


    }

}
