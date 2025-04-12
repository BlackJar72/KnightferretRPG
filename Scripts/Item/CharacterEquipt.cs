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
        //[SerializeField] ItemEquiptLocation CLOTH;
        [SerializeField] ItemEquiptLocation BELT;


        public ItemEquipt EquipItem(ItemStack item) {
            if(item.CanEquipt()) {
                switch(item.item.EquiptType) {
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
                    case EEquiptSlot.RHAND:
                        return RHAND.EquipItem(item);
                    case EEquiptSlot.HANDS:
                        return RHAND.EquipItem(item);
                    case EEquiptSlot.LHAND:
                        return LHAND.EquipItem(item);
                    case EEquiptSlot.RING:
                        if(item.slot == EquiptmentSlots.rring) {
                            return RRING.EquipItem(item);
                        } else {
                            return LRING.EquipItem(item);
                        }
                    case EEquiptSlot.NECK:
                        break;
                    case EEquiptSlot.BELT:
                        break;
                    default: return null;
                }
            }
            return null;
        }


        public ItemEquipt GetRHandItem() {
            return RHAND.CurrectItem;
        }


        public ItemEquipt GetLHandItem() {
            return LHAND.CurrectItem;
        }


        public void UnequipItem(ItemStack item) {
            if(item.CanEquipt()) {
                switch(item.item.EquiptType) {
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
                    case EEquiptSlot.RHAND:
                        RHAND.UnequiptCurrentItem();
                        break;
                    case EEquiptSlot.HANDS:
                        RHAND.UnequiptCurrentItem();
                        break;
                    case EEquiptSlot.LHAND:
                        LHAND.UnequiptCurrentItem();
                        break;
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
                case EEquiptSlot.RHAND:
                    RHAND.UnequiptCurrentItem();
                    break;
                case EEquiptSlot.HANDS:
                    RHAND.UnequiptCurrentItem();
                    break;
                case EEquiptSlot.LHAND:
                    LHAND.UnequiptCurrentItem();
                    break;
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
