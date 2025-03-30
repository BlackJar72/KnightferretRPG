using UnityEngine;

namespace kfutils.rpg {

    [System.Serializable]
    public enum EItemType {
        
        GENERAL, 
        WEAPONS, 
        ARMOR, 
        WEARABLE,   // Items that are worn but not armor (e.g., jewelry or clothing)
        USABLE,     // Like Consumable but not actully consumed (do these need to be separate?)
        CONSUMABLE, 
        INGEDIENT, 
        SPECIAL 

    }


}
