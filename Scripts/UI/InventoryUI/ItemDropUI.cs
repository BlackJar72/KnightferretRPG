using UnityEngine;

namespace kfutils.rpg.ui {

    public class ItemDropUI : InventorySlotUI {

        [SerializeField] bool shouldThrow;


        public override void SwapWith(InventorySlotUI other) {
            if((other.item != null) && (other.item.item != null) && (other.item.item.InWorld != null)) {
                PCActing pc = EntityManagement.playerCharacter;
                if(shouldThrow) {
                    other.item.item.DropItemInWorld(pc.playerCam.transform, 0.5f, 20f + pc.attributes.jumpForce * 10);
                } else {
                    other.item.item.DropItemInWorld(pc.playerCam.transform, 0.5f);
                }
                other.inventory.RemoveFromSlot(other.item.slot, 1);
            }
        }



    }

}
