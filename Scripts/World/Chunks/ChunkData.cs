using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEditor;
using UnityEngine;

namespace kfutils.rpg {

    /// <summary>
    /// A class storing data about chunks (mostly about transient and mutable contents of chunks).
    /// 
    /// This class needs to be stored in a static dictionary, and linked to a chunk at run time, so 
    /// it can survive the loading and unloading of the chunk and its scene.
    /// </summary>
    public class ChunkData {

        [SerializeField] string id;
        [SerializeField] bool clean = true; // First time loaded, and should be treated as such


        public string ID => id;
        public bool Clean => clean;

        [SerializeField] List<string> itemsInChunk = new();
        public List<string> ItemsInChunkList => itemsInChunk;

        [SerializeField] List<ActivityProp> activityProps;
        public List<ActivityProp> ActivityProps => activityProps;

        [SerializeField] List<string> effectsInChunk = new();
        public List<string> EffectsInChunkList => effectsInChunk;


        public void AddItem(string id)
        {
            if (!itemsInChunk.Contains(id)) itemsInChunk.Add(id);
        }


        public void AddEffect(string id)
        {
            if (!itemsInChunk.Contains(id)) itemsInChunk.Add(id);
        }


        /*
        No matter how I try to fanagle it, no form of real item persistence is going to 
        work without items having a way to maintain there own identity, through something 
        like and id -- though I'm not sure how to work that when items can be added, removed, 
        and stacked, and items stacks can be split and combined -- not mention that massive 
        refactoring of my inventory system to to accomodate items with unique identies (instaed 
        of simply being examples of prototype and nothing more).
        */


        public void AddActivityProp(ActivityProp prop)
        {
            
        }


        public ChunkData(string id)
        {
            this.id = id;
        }


        public string ListItemsInChunk() {
            System.Text.StringBuilder sb = new(" [");
            for(int i = 0; i < itemsInChunk.Count; i++) {
                sb.Append(itemsInChunk[i]).Append(", ");
            }
            sb.Append("] ");
            return sb.ToString();
        }


        /// <summary>
        /// Unlike the typical use of a dirty flag, here we are more concerned about the reverse, 
        /// doing things specific for first load.  Therefore, making this "dirty" is premenant;
        /// </summary>
        public void MakeDirty() {
            clean = false;
        }
        

    }


}