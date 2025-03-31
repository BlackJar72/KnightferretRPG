using UnityEngine;

namespace kfutils.rpg {

    [System.Serializable]
    public enum EItemType {
        
        General = 0, 
        Weapon = 1,  
        Shield = 2, 
        Armor = 3,
        Wearable = 4,   // Items that are worn but not armor (e.g., jewelry or clothing)
        Usable = 5,     // Like Consumable but not actully consumed (do these need to be separate?)
        Consumable = 6, 
        Wand = 7, 
        Special = 8 

    }


}
