using UnityEngine;

namespace kfutils.rpg {

    /// <summary>
    /// Data for specific items, that must be shared between version of the same item 
    /// (i.e., between ItemStack, ItemInWorld, and ItemEquipt), but must not be shared between 
    /// all items of a type.
    /// 
    /// This class should be serialized in saves, but should *NOT* be a SerializedField, and 
    /// must be a class (not a struct), as a single instance must be shared between (i.e. by 
    /// reference) between all class instance for an item.
    /// </summary>
    [System.Serializable]
    public class ItemMetadata {
    
        public int xp = 0;

        // Boolean flags        
        bool isQuestItem = false;
        bool isStollen = false;

    }


}
