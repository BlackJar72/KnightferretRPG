using System;
using UnityEngine;



namespace kfutils.rpg {

    /// <summary>
    /// Defines equipment slots items can be placed in, used especially by the inventory system, but include 
    /// with items as it is part of their data.
    /// 
    /// Items that cannot be equipt, but only stored in generic inventory slots, should simply have no flags 
    /// set.  Otherwise, all valid locations (e.g., RRING and LRING for rings) should be set for items.  
    /// Equipment slots should have one, and only one of these set.
    /// </summary>    
    [Flags]
    public enum EEquiptSlot {

        // Basic armor slots
        HEAD = 0x1 << 0,
        BODY = 0x1 << 1,
        ARMS = 0x1 << 2,
        LEGS = 0x1 << 3,
        FEET = 0x1 << 4,

        // Slots for weapons, shields, and items that are equipt to be used (whether optionally or required)
        RHAND = 0x1 << 5,
        LHAND = 0x1 << 6,
        HANDS = 0x1 << 7, // For two handed items; such items should flag this plus eahc hand

        // Special worn items, especially those which may be worn for magical or similar effects
        RRING = 0x1 << 8,
        LRING = 0x1 << 9,
        NECK  = 0x1 << 10, // For necklace / pendant / amulatet / talisman
        CLOTH = 0x1 << 11,  // Probably for a cloak, but perhaps for a "tabard" or general clothing?
        BELT  = 0x1 << 12,

        // Items that can be placed in the hot bar
        HOTBAR = 0x1 << 13
        
    }


}
