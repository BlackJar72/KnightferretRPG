using UnityEngine;


namespace kfutils.rpg
{

    [System.Serializable]
    public enum QuestLinkage
    {
        
        /// <summary>
        /// Only on way forward, a stage or step rather than a branch, only the firs option is vallid (should only be one)
        /// </summary>
        UNARY = 0,
        /// <summary>
        /// One objective (node) must be complete, all others are optional; think choose you own ending/adventure books 
        /// (though the player may chose other nodes as well).
        /// </summary>
        OR = 1,
        /// <summary>
        /// All the objectives (nodes) moust be completed, but not in any particular order; think of late game Morrowind 
        /// or most of Daggerfall's main quest. 
        /// </summary>
        AND = 2,
        /// <summary>
        /// One and only one objective (node) must be completed, once one is compeleted others become invalide; forexample, 
        /// choosing between two opposing factions in a conflict
        /// </summary>
        XOR = 3,
        /// <summary>
        /// Failure; doing this will fail the quest. 
        /// (This might need to be handled differently, as fail conditions probably belong to more than one node or branch) 
        /// </summary>
        NOT = 4
    }


}
