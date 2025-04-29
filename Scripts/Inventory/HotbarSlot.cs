using UnityEngine;



namespace kfutils.rpg {
    
    public class HotbarSlot {
        
        public enum RefType {
            MAIN,
            EQUIPT,
            SPELLS
        }


        [SerializeField] RefType inventory;
        [SerializeField] int slot;



    }


}
