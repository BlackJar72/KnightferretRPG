using UnityEngine;


namespace kfutils.rpg
{

    /// <summary>
    /// Derived from the old character engine, but simpler, and intended purely for use 
    /// in NPC AI.
    /// </summary>
    [System.Serializable]
    public enum ENeed
    {
        ENERGY = 0,
        FOOD = 1,
        SOCIAL = 2,
        ENJOYMENT = 3
    }


    /// <summary>
    /// Used to tag how an IActivityObject should be handled.
    /// </summary>
    public enum EObjectActivity
    {
        NEED_DISCRETE = 0,
        NEED_CONTINUOUS = 1,
        COMBAT_OPTION = 2,
        WEAPON = 3,
        SPELL = 4,
        HEALTH_ITEM = 5

    }



}
