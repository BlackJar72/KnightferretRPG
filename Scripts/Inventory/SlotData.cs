using UnityEngine;



namespace kfutils.rpg {
        
    [System.Serializable]
    public enum InvType {
        MAIN,
        EQUIPT,
        SPELLS
    }

    
    [System.Serializable]
    public class SlotData {

        public InvType inventory;
        public int invSlot;
        public bool filled;

        public static bool operator ==(SlotData slot1, SlotData slot2) 
                    => (slot1.inventory == slot2.inventory) && (slot1.invSlot == slot2.invSlot); 

        public static bool operator !=(SlotData slot1, SlotData slot2) 
                    => (slot1.inventory != slot2.inventory) || (slot1.invSlot != slot2.invSlot);
        // Allow access to object identity as well (I hope)
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();




    }


}
