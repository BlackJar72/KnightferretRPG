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


        public void EquipItem(ItemStack item) {
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
                        RHAND.EquipItem(item);
                        break;
                    case EEquiptSlot.HANDS:
                        RHAND.EquipItem(item);
                        break;
                    case EEquiptSlot.LHAND:
                        LHAND.EquipItem(item);
                        break;
                    case EEquiptSlot.RRING:
                        RRING.EquipItem(item);
                        break;
                    case EEquiptSlot.LRING:
                        LRING.EquipItem(item);
                        break;
                    case EEquiptSlot.NECK:
                        break;
                    case EEquiptSlot.BELT:
                        break;
                    default: break;
                }
            }
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
                    case EEquiptSlot.RRING:
                        RRING.UnequiptCurrentItem();
                        break;
                    case EEquiptSlot.LRING:
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
                case EEquiptSlot.RRING:
                    RRING.UnequiptCurrentItem();
                    break;
                case EEquiptSlot.LRING:
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
