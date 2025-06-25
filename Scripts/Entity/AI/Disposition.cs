using UnityEngine;


namespace kfutils.rpg
{

    /// <summary>
    /// The base disposition of the enemy to the player character, for 
    /// determining how some AI is run.
    /// 
    /// For different setups edit to fit the game design.
    /// 
    /// This is intended primarily to determine how the creature should 
    /// react to the player. 
    /// </summary>
    [System.Serializable]
    public enum Disposition
    {
        friendly = 0, // Entities friendly ot he player  
        neutral = 1,  // Includes animals and other non-aligned creatures, as well as unaligned humans
        avoidant = 2, // Entities that avoid the player, such as wild prey animals that flee
        hostile = 3   // Entities that should normally attack the player

    }

}
