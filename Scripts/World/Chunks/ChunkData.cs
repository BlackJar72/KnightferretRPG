using UnityEngine;

namespace kfutils.rpg {

    /// <summary>
    /// A class storing data about chunks (mostly about transient and mutable contents of chunks).
    /// 
    /// This class needs to be stored in a static dictionary, and linked to a chunk at run time, so 
    /// it can survive the loading and unloading of the chunk and its scene.
    /// </summary>
    public class ChunkData {

        private string id;


        public string ID => id;


        public ChunkData(string id) {
            this.id = id;
        }

        

    }


}