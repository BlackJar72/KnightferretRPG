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
    [Flags] // Should this be flags? Or simplified to one-slot-per-item?
    public enum EEquiptSlotFlags {

        // Basic armor slots
        HEAD = 0x1 << EEquiptSlot.HEAD,
        BODY = 0x1 << EEquiptSlot.BODY,
        ARMS = 0x1 << EEquiptSlot.ARMS,
        LEGS = 0x1 << EEquiptSlot.LEGS,
        FEET = 0x1 << EEquiptSlot.FEET,

        // Slots for weapons, shields, and items that are equipt to be used (whether optionally or required)
        RHAND = 0x1 << EEquiptSlot.RHAND,
        LHAND = 0x1 << EEquiptSlot.LHAND,
        HANDS = 0x1 << EEquiptSlot.HANDS, // For two handed items; such items should flag this plus each hand

        // Special worn items, especially those which may be worn for magical or similar effects
        RING = 0x1 << EEquiptSlot.RING,
        NECK  = 0x1 << EEquiptSlot.NECK, // For necklace / pendant / amulatet / talisman
        CLOTH  = 0x1 << EEquiptSlot.CLOTH,
        AMMO  = 0x1 << EEquiptSlot.AMMO,

        // Special slots for non-items (e.g., spells, abilities, etc.)
        SPELL = 0x1 << EEquiptSlot.SPELL, // For currently selected (equipt) spell

        // Items (etc.) that can be placed in the hot bar
        BOW = 0x1 << EEquiptSlot.BOW
        
    }


    /// <summary>
    /// And euum to respresent speccific slots; the flags are keyed to them.
    /// </summary>
    public enum EEquiptSlot {

        NONE = -1,
        
        // Basic armor slots
        HEAD = 0,
        BODY = 1,
        ARMS = 2,
        LEGS = 3,
        FEET = 4,

        // Slots for weapons, shields, and items that are equipt to be used (whether optionally or required)
        RHAND = 5,
        LHAND = 6,
        HANDS = 7, // For two handed items; such items should flag this plus eahc hand

        // Special worn items, especially those which may be worn for magical or similar effects
        RING = 8,
        NECK  = 9, // For necklace / pendant / amulatet / talisman
        CLOTH  = 10, // Fot non-armor items such as cloaks, capes, etc. (Not currently used!)
        AMMO  = 11,

        // Special slots for non-items (e.g., spells, abilities, etc.)
        SPELL = 12, // For currently selected (equipt) spell

        // Items that can be placed in the hot bar
        BOW = 13
        
    }


}
