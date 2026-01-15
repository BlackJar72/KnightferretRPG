using System;
using System.Collections.Generic;
using UnityEngine;



namespace kfutils.rpg {
    
    public class WorldSpace : MonoBehaviour {

        // Parameters for loading
        [SerializeField] string chunkNameDelims = "_ -";   
        [SerializeField] int xNamePos;
        [SerializeField] int zNamePos;

        // Runtime parameters
        [SerializeField] int chunkSize = 128;
        [SerializeField] int renderDistance = 512;

        private int loadRange;
        private int xmax = 0, zmax = 0;     

        private GOChunk[,] chunks;
        private readonly List<GOChunk> loadedChunks = new();


        void Awake() {
            ChunkManagement.SetWorldSpace(this);
            loadRange = renderDistance / chunkSize;
            Terrain[] terrains = gameObject.GetComponentsInChildren<Terrain>();
            List<GOChunk> chunkar = new();
            GameObject terraObj;
            int minx, maxx, minz, maxz;
            maxx = maxz = int.MinValue;
            minx = minz = int.MaxValue;
            int x, z;
            for(int i = 0; i < terrains.Length; i++) {
                terraObj = terrains[i].gameObject;
                string tname = terraObj.name;
                string[] parts = tname.Split(chunkNameDelims.ToCharArray());
                try {
                    x = int.Parse(parts[xNamePos]);
                    z = int.Parse(parts[zNamePos]);
                    if(x > maxx) maxx = x;
                    if(x < minx) minx = x;
                    if(z > maxz) maxz = z;
                    if(z < minz) minz = z;
                    chunkar.Add(new GOChunk(terraObj, new Vector2Int(x,z)));
                } catch (Exception e) {
                    throw e;
                }
            }
            if(minx != 0) Debug.LogWarning("Lowest X chunk coord was not 0! Actual lowest was " + minx + ".");
            if(minz != 0) Debug.LogWarning("Lowest Z chunk coord was not 0! Actual lowest was " + minz + ".");
            chunks = new GOChunk[maxx - minx + 1, maxz - minz + 1];
            xmax = chunks.GetLength(0) - 1;
            zmax = chunks.GetLength(1) - 1;
            for(int i = 0; i < chunkar.Count; i++) {
                Vector2Int loc = chunkar[i].Location;
                loc.x -= minx;
                loc.y -= minz;
                if(chunks[loc.x, loc.y] == null) chunks[loc.x, loc.y] = new GOChunk(chunkar[i].TerrainObj, loc);
                else Debug.LogWarning("Chunk duplication: " + chunkar[i].TerrainObj.name + " is share coords with a previously initialize chunk");
            }
            for(int i = 0; i < chunks.GetLength(0); i++)
                for(int j = 0; j < chunks.GetLength(1); j++) {
                    if(chunks[i,j] == null) Debug.LogWarning("World space has missing chunk; hole at [" + i + ", " + j + "]. " );
                    else chunks[i, j].Unload();
                }
        }


        public void LoadInRange(int x, int z) {
            // First, getting bounding variables
            int minx = Mathf.Clamp(x - loadRange, 0, xmax);
            int maxx = Mathf.Clamp(x + loadRange, 0, xmax);
            int minz = Mathf.Clamp(z - loadRange, 0, zmax);
            int maxz = Mathf.Clamp(z + loadRange, 0, zmax);
            // Next load new
            for(int i = minx; i <= maxx; i++) 
                for(int j = minz; j <= maxz; j++) {
                    if(chunks[i,j] != null) {
                        chunks[i,j].Load();
                        loadedChunks.Add(chunks[i,j]);
                    }
                }
            // Then unload old
            for(int i = loadedChunks.Count - 1; i > -1; i--) {
                if(loadedChunks[i].OutOfRange(minx, maxx, minz, maxz)) {
                    loadedChunks[i].Unload();
                    loadedChunks.RemoveAt(i);
                }
            }
        }


        public void LoadAroundWorldPos(Transform pos) {
            int x = Mathf.FloorToInt(pos.position.x) / chunkSize;
            int z = Mathf.FloorToInt(pos.position.z) / chunkSize;
            //print(pos.position.x + ", " + pos.position.z + " -> " + x + ", " + z);
            LoadInRange(x, z);
        }


    }

}
