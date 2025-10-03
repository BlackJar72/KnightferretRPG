using System;
using UnityEngine;


namespace kfutils.rpg
{

    /// <summary>
    /// Derived from the old character engine, but simpler, and intended purely for use 
    /// in NPC AI.
    /// </summary>
    [System.Serializable][Flags]
    public enum ENeedID
    {
        ENERGY = 0,
        FOOD = 1,
        SOCIAL = 2,
        ENJOYMENT = 3
    } 
    
    
    /// <summary>
    /// Derived from the old character engine, but simpler, and intended purely for use 
    /// in NPC AI.
    /// </summary>
    [System.Serializable]
    [Flags]
    public enum ENeeds
    {
        NONE = 0, 
        ENERGY = 0x1 << ENeedID.ENERGY,
        FOOD = 0x1 << ENeedID.FOOD,
        SOCIAL = 0x1 << ENeedID.SOCIAL,
        ENJOYMENT = 0x1 << ENeedID.ENJOYMENT
    }


    /// <summary>
    /// Used to tag how an IActivityObject should be handled.
    /// </summary>
    [System.Serializable]
    public enum EObjectActivity
    {
        NONE = 0, 
        NEED_DISCRETE = 1,
        NEED_CONTINUOUS = 2,
        COMBAT_OPTION = 3,
        WEAPON = 4,
        SPELL = 5,
        HEALTH_ITEM = 6,
        SELF = 7

    }



}
