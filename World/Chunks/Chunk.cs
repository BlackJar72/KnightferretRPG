using System.Collections.Generic;
using UnityEngine;



namespace kfutils.rpg {

    public class Chunk : MonoBehaviour {

        [SerializeReference] Terrain terrain;
        [SerializeField] List<ItemInWorld> items;
        [SerializeField] Transform itemHolder; // A place in the hierarchy where loose items will go


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

}
