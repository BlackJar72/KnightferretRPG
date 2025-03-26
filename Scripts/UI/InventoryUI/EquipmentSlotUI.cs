using UnityEngine;

namespace kfutils.rpg.ui {

    public class EquipmentSlotUI : InventorySlotUI {
        
        [Tooltip("Which slot type this is; this should only be set to one value, though items can have more than one.")]
        [SerializeField] EEquiptSlot slotType;
        public EEquiptSlot SlotType { get => slotType; }

        

    }

}
