using UnityEngine;


namespace kfutils.rpg {
    
    [System.Serializable]
    public class CharacterEquipt {
        
        [SerializeField] ItemEquiptLocation HEAD;
        [SerializeField] ItemEquiptLocation BODY;
        [SerializeField] ItemEquiptLocation ARMS;
        [SerializeField] ItemEquiptLocation LEGS;
        [SerializeField] ItemEquiptLocation FEET;

        // Slots for weapons, shields, and items that are equipt to be used (whether optionally or required)
        [SerializeField] ItemEquiptLocation RHAND;
        [SerializeField] ItemEquiptLocation LHAND;
        [SerializeField] ItemEquiptLocation HANDS;

        // Special worn items, especially those which may be worn for magical or similar effects
        [SerializeField] ItemEquiptLocation RRING;
        [SerializeField] ItemEquiptLocation LRING;
        [SerializeField] ItemEquiptLocation NECK;
        [SerializeField] ItemEquiptLocation BELT;


        public ItemEquipt EquipItem(ItemStack item) {
            if(item.CanEquipt()) {
                switch(item.item.EquiptType) {
                    // Armor
                    case EEquiptSlot.HEAD:
                        return HEAD.EquiptArmor(item);
                    case EEquiptSlot.BODY:
                        return BODY.EquiptArmor(item);
                    case EEquiptSlot.ARMS:
                        return ARMS.EquiptArmor(item);
                    case EEquiptSlot.LEGS:
                        return LEGS.EquiptArmor(item);
                    case EEquiptSlot.FEET:
                        return FEET.EquiptArmor(item);
                    // Equipment
                    case EEquiptSlot.RHAND:
                        return RHAND.EquipItem(item);
                    case EEquiptSlot.HANDS:
                        return RHAND.EquipItem(item);
                    case EEquiptSlot.LHAND:
                        return LHAND.EquipItem(item);
                    // Accessories
                    case EEquiptSlot.RING:
                        if(item.slot == EquiptmentSlots.rring) {
                            return RRING.EquipItem(item);
                        } else {
                            return LRING.EquipItem(item);
                        }
                    case EEquiptSlot.NECK:
                        return NECK.EquiptArmor(item);
                    case EEquiptSlot.BELT:
                        return BELT.EquiptArmor(item);
                    default: return null;
                }
            }
            return null;
        }


        public ItemEquipt GetItem(EEquiptSlot slot, int slotNumber = 0) {
            switch(slot) {
                // Armor
                case EEquiptSlot.HEAD:
                    return HEAD.CurrentItem;
                case EEquiptSlot.BODY:
                    return BODY.CurrentItem;
                case EEquiptSlot.ARMS:
                    return ARMS.CurrentItem;
                case EEquiptSlot.LEGS:
                    return LEGS.CurrentItem;
                case EEquiptSlot.FEET:
                    return FEET.CurrentItem;
                // Equipment
                case EEquiptSlot.RHAND:
                    return RHAND.CurrentItem;
                case EEquiptSlot.HANDS:
                    return RHAND.CurrentItem;
                case EEquiptSlot.LHAND:
                    return LHAND.CurrentItem;
                // Accessories
                case EEquiptSlot.RING:
                    if((slotNumber % 2) == 0) {
                        return RRING.CurrentItem;
                    } else {
                        return LRING.CurrentItem;
                    }
                case EEquiptSlot.NECK:
                    return NECK.CurrentItem;
                case EEquiptSlot.BELT:
                    return BELT.CurrentItem;
                default: return null;
            }
        }


        public ItemEquipt GetRHandItem() {
            return RHAND.CurrentItem;
        }


        public ItemEquipt GetLHandItem() {
            return LHAND.CurrentItem;
        }


        public void UnequipItem(ItemStack item) {
            if(item.CanEquipt()) {
                switch(item.item.EquiptType) {
                    // Armor
                    case EEquiptSlot.HEAD:
                        break;
                    case EEquiptSlot.BODY:
                        break;
                    case EEquiptSlot.ARMS:
                        break;
                    case EEquiptSlot.LEGS:
                        break;
                    case EEquiptSlot.FEET:
                        break;
                    // Equipment
                    case EEquiptSlot.RHAND:
                        RHAND.UnequiptCurrentItem();
                        break;
                    case EEquiptSlot.HANDS:
                        RHAND.UnequiptCurrentItem();
                        break;
                    case EEquiptSlot.LHAND:
                        LHAND.UnequiptCurrentItem();
                        break;
                    // Accessories
                    case EEquiptSlot.RING:
                        if(item.slot == EquiptmentSlots.rring) {
                            RRING.UnequiptCurrentItem();
                        } else {
                            LRING.UnequiptCurrentItem();
                        }
                        break;
                    case EEquiptSlot.NECK:
                        break;
                    case EEquiptSlot.BELT:
                        break;
                    default: break;
                }
            }
        }


        public void UnequipItem(EEquiptSlot slot) {
            switch(slot) {
                // Armor
                case EEquiptSlot.HEAD:
                    break;
                case EEquiptSlot.BODY:
                    break;
                case EEquiptSlot.ARMS:
                    break;
                case EEquiptSlot.LEGS:
                    break;
                case EEquiptSlot.FEET:
                    break;
                // Equipment
                case EEquiptSlot.RHAND:
                    RHAND.UnequiptCurrentItem();
                    break;
                case EEquiptSlot.HANDS:
                    RHAND.UnequiptCurrentItem();
                    break;
                case EEquiptSlot.LHAND:
                    LHAND.UnequiptCurrentItem();
                    break;
                // Accessories
                case EEquiptSlot.RING:
                    Debug.LogWarning("UnequipItem(EEquiptSlot slot) being used from RING (all rings removed); this is not intended for rings.");
                    RRING.UnequiptCurrentItem();
                    LRING.UnequiptCurrentItem();
                    break;
                case EEquiptSlot.NECK:
                    break;
                case EEquiptSlot.BELT:
                    break;
                default: break;
            }
        }

        

    }

}
