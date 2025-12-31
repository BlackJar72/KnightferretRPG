using System.Collections;
using UnityEngine;



namespace kfutils.rpg {

    [System.Serializable]
    public enum InvType
    {
        NONE = -1,
        MAIN = 0,
        EQUIPT = 1,
        SPELLS = 2
    }

    
    [System.Serializable]
    public class SlotData {

        public InvType inventory = InvType.NONE;
        public int invSlot = -1;
        public bool filled = false;

        public static bool operator ==(SlotData slot1, SlotData slot2) 
                    => (slot1.inventory == slot2.inventory) && (slot1.invSlot == slot2.invSlot); 

        public static bool operator !=(SlotData slot1, SlotData slot2) 
                    => (slot1.inventory != slot2.inventory) || (slot1.invSlot != slot2.invSlot);
                    
        // Allow access to object identity as well (I hope)
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();


        public SlotData Copy()
        {
            SlotData result = new();
            result.inventory = inventory;
            result.invSlot = invSlot;
            result.filled = filled;
            return result;
        }


        public SlotData CopyInto(SlotData other)
        {
            other.inventory = inventory;
            other.invSlot = invSlot;
            other.filled = filled;
            return other;
        }


        public static SlotData SlotDataFromStack(ItemStack item, int slot, InvType type) {
            SlotData result = new SlotData();
            result.inventory = type;
            result.invSlot = slot;
            result.filled = (item != null) && (item.item != null);
            return result;
        }


        public override string ToString() {
            return " [" + inventory.ToString() + ", Slot " + invSlot + "; Filled? " + filled + "] "; 
        }




    }


}
