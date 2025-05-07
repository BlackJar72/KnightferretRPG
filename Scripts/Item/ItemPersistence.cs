using UnityEngine;


namespace kfutils.rpg {
    
    public class ItemPersistence {

        private ItemPrototype itemPrototype;
        private TransformData transformData;


        public ItemPersistence(ItemInWorld worldItem) {
            itemPrototype = worldItem.Prototype;
            transformData = worldItem.transform.GetGlobalData();
        }


        public void SpawnItem(ChunkManager chunk) {
            ItemInWorld inWorld = Object.Instantiate(itemPrototype.InWorld, chunk.LooseItems);
            inWorld.transform.SetDataGlobal(transformData);
        }
        


    }


}
