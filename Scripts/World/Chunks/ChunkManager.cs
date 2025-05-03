using UnityEngine;

namespace kfutils.rpg {

    /// <summary>
    /// A class for managing resources and other things related to game play within a world chunk.
    /// 
    /// This must *NOT* be a singleton, as there should be one for each loaded chunk, handling things 
    /// related specifically to that chunk.  For manangement of the world as a whole, including 
    /// coordination across / between chunks, see WorldManagement.
    /// </summary>
    public class ChunkManager : MonoBehaviour {

        [SerializeField] Transform looseItems;
        
        public Vector2Int location;


        public Transform LooseItems => looseItems;



    }

}