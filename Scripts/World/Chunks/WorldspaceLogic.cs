using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg {

    public class WorldspaceLogic : MonoBehaviour {
        
        [SerializeField] Worldspace worldspace;

        // Runtime parameters
        [SerializeField] int chunkSize = 128;
        [SerializeField] int renderDistance = 512;

        // Chunks
        [SerializeField] GameObject chunkHolder;
        [SerializeField] string chunkNameDelims = "_ -";   
        [SerializeField] int xNamePos = 1;
        [SerializeField] int zNamePos = 2;

        [SerializeField] GameObject chunkManagerPrefab;     

        private int loadRange;
        private int xmax = 0, zmax = 0;     

        private ChunkManager[,] chunks;
        private readonly List<ChunkManager> loadedChunks = new();


        public void Init() {
            chunkSize = worldspace.ChunkSize;
            loadRange = (renderDistance / chunkSize) + 1;
            Terrain[] terrains = chunkHolder.GetComponentsInChildren<Terrain>();
            int minx, maxx, minz, maxz;
            maxx = maxz = int.MinValue;
            minx = minz = int.MaxValue;
            int x, z;
            if(!worldspace.MultiChunk) {
                chunks = new ChunkManager[1,1];
                chunks[0,0] = chunkHolder.GetComponentInChildren<ChunkManager>();
                return;
            }
            ChunkManager[] chunkar = new ChunkManager[terrains.Length];
            for(int i = 0; i < terrains.Length; i++) {
                chunkar[i] = terrains[i].GetComponentInChildren<ChunkManager>();
                string tname = terrains[i].gameObject.name;
                string[] parts = tname.Split(chunkNameDelims.ToCharArray());
                try {
                    x = int.Parse(parts[xNamePos]);
                    z = int.Parse(parts[zNamePos]);
                    if(x > maxx) maxx = x;
                    if(x < minx) minx = x;
                    if(z > maxz) maxz = z;
                    if(z < minz) minz = z;
                    chunkar[i].location.Set(x, z);
                } catch (System.Exception e) {
                    throw e;
                }
            }
            if(minx != 0) Debug.LogWarning("Lowest X chunk coord was not 0! Actual lowest was " + minx + ".");
            if(minz != 0) Debug.LogWarning("Lowest Z chunk coord was not 0! Actual lowest was " + minz + ".");
            chunks = new ChunkManager[maxx - minx + 1, maxz - minz + 1];
            xmax = chunks.GetLength(0) - 1;
            zmax = chunks.GetLength(1) - 1;
            for(int i = 0; i < terrains.Length; i++) {
                Vector2Int loc = chunkar[i].location;
                loc.x -= minx;
                loc.y -= minz;
                if(chunks[loc.x, loc.y] == null) chunks[loc.x, loc.y] = chunkar[i];
                else Debug.LogWarning("Chunk duplication: " + terrains[i].gameObject.name + " is shares coords with a previously initialize chunk");
            }
            for(int i = 0; i < chunks.GetLength(0); i++)
                for(int j = 0; j < chunks.GetLength(1); j++) {
                    if(chunks[i,j] == null) Debug.LogWarning("World space has missing chunk; hole at [" + i + ", " + j + "]. " );
                }
        }


        [ContextMenu("Add Chunk Managers")]
        public void AddChunkManagers() {
            Transform[] chunktrs = chunkHolder.GetComponentsInChildren<Transform>();
            for(int i = 0; i < chunktrs.Length; i++) {
                if(chunktrs[i].GetComponentInChildren<ChunkManager>() == null) {
                    Instantiate(chunkManagerPrefab, chunktrs[i]).transform.localPosition = Vector3.zero;
                }
            }
        }


        public ChunkManager GetChunk(int x, int z) {
            if((x > -1) && (x < chunks.GetLength(0)) && (z > -1) && (z < chunks.GetLength(1))) {
                return chunks[x,z];
            }
            return null;
        }


    }

}
