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


        public override string ToString() {
            return " [" + inventory.ToString() + ", Slot " + invSlot + "; Filled? " + filled + "] "; 
        }




    }


}
