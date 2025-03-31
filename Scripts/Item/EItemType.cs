using UnityEngine;

namespace kfutils.rpg {

    [System.Serializable]
    public enum EItemType {
        
        GENERAL = 0, 
        WEAPONS = 1,  
        SHIELD = 2, 
        ARMOR = 3,
        WEARABLE = 4,   // Items that are worn but not armor (e.g., jewelry or clothing)
        USABLE = 5,     // Like Consumable but not actully consumed (do these need to be separate?)
        CONSUMABLE = 6, 
        WAND = 7, 
        SPECIAL = 8 

    }


}
