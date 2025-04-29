using UnityEngine;



namespace kfutils.rpg {
        

    public enum InvType {
        MAIN,
        EQUIPT,
        SPELLS
    }

    
    public class HotbarSlot {

        [SerializeField] InvType inventory;
        [SerializeField] int invSlot;

    }


}
