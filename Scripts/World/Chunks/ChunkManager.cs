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

        string id = null;

        [SerializeField] Transform looseItems;
        
        public Vector2Int location;

        private ChunkData data;


        public Transform LooseItems => looseItems;


        public void SetID(string id) {
            this.id = id;
        }


        public void Init() {
            data = WorldManagement.GetChunkData(id);
            if(data == null) {
                data = new ChunkData(id);
                // TODO?
                WorldManagement.StoreChunkData(data);
            }
        }





    }

}