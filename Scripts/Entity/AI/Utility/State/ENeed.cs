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
        POTTY = 2, // Do I really need this in a RPG? Probably not.
        SOCIAL = 3,
        ENJOYMENT = 4
    }



}
