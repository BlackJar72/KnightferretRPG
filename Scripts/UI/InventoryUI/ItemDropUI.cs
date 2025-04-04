using UnityEngine;

namespace kfutils.rpg.ui {

    public class ItemDropUI : InventorySlotUI {

        [SerializeField] bool shouldThrow;


        public override bool SwapWith(InventorySlotUI other) {
            if((other.item != null) && (other.item.item != null) && (other.item.item.InWorld != null)) {
                PCActing pc = EntityManagement.playerCharacter;
                if(shouldThrow) {
                    other.item.item.DropItemInWorld(pc.playerCam.transform, 0.5f, 5.0f * (1  + pc.attributes.jumpForce) * Mathf.Sqrt(other.item.item.Weight));
                } else {
                    other.item.item.DropItemInWorld(pc.playerCam.transform, 0.5f);
                }
                other.inventory.RemoveFromSlot(other.item.slot, 1);
            }
            return true;
        }



    }

}
