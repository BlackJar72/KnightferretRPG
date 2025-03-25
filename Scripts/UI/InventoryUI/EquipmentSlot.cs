using UnityEngine;

namespace kfutils.rpg.ui {

    public class EquipmentSlot : InventorySlot {
        
        [Tooltip("Which slot type this is; this should only be set to one value, though items can have more than one.")]
        [SerializeField] EEquiptSlot slotType;

        

    }

}
