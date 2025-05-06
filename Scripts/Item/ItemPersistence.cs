using UnityEngine;


namespace kfutils.rpg {
    
    public class ItemPersistence {

        private ItemPrototype itemPrototype;
        private TransformData transformData;



        public void SpawnItem(ChunkManager chunk) {
            ItemInWorld inWorld = Object.Instantiate(itemPrototype.InWorld, chunk.LooseItems);
            inWorld.transform.SetData(transformData);
        }


    }


}
