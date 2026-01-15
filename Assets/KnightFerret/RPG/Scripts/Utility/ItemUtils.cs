using UnityEngine;


namespace kfutils.rpg
{

    public static class ItemUtils
    {


        public static int GetEquiptSlotForType(EEquiptSlot equiptType)
        {
            switch (equiptType)
            {
                case EEquiptSlot.NONE: return -1;
                case EEquiptSlot.HEAD: return 0;
                case EEquiptSlot.BODY: return 1;
                case EEquiptSlot.ARMS: return 2;
                case EEquiptSlot.LEGS: return 8;
                case EEquiptSlot.FEET: return 9;
                case EEquiptSlot.RHAND: return 4;
                case EEquiptSlot.LHAND: return 3;
                case EEquiptSlot.HANDS: return 4;
                case EEquiptSlot.RING: return 6;
                case EEquiptSlot.NECK: return 10;
                case EEquiptSlot.CLOTH: return 0;
                case EEquiptSlot.AMMO: return 7;
                case EEquiptSlot.BOW: return 3;
                default: return -1;
            }
        }



    }

}
