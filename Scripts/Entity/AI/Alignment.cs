using UnityEngine;


namespace kfutils.rpg
{

    /// <summary>
    /// Litterally the faction with whcih the entity is aligned.
    /// 
    /// For different setups edit to fit the game design.
    /// 
    /// This is intended primarily to determine how the creature should 
    /// react to the player. 
    /// </summary>
    [System.Serializable]
    public enum Alignment
    {
        player,   // The player and anything aligned specifically with the player per se 
        villages, // Generally, the "good guys"
        neutral,  // Includes animals and other non-aligned creatures, as well as unaligned humans
        avoidant, // Entities that avoid the player, such as wild prey animals that flee
        evil      // Alternately enemy, but changed for the possibility the PC could become evil

    }

}
