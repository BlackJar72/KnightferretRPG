using UnityEngine;


namespace kfutils.rpg {
    
    [System.Serializable]
    public class ItemData {

        [SerializeField] ItemPrototype prototype;
        [SerializeField] readonly string id;
        [SerializeField] public TransformData transformData;
        [SerializeField] ItemMetadata metadata;


        public string ID => id;
        public ItemPrototype Prototype => prototype;
        public TransformData TransformData => transformData;
        public ItemMetadata Metadata => metadata;
        public bool physics = false;


        public ItemData() {
            id = System.Guid.NewGuid().ToString();
        }


        public ItemData(string id, ItemPrototype prototype, TransformData transformData, ItemMetadata metadata) {
            this.id = id;
            this.prototype = prototype;
            this.transformData = transformData;
            this.metadata = metadata;
        }


        public ItemData(string id, ItemPrototype prototype, Transform transform, ItemMetadata metadata) {
            this.id = id;
            this.prototype = prototype;
            this.transformData = transform.GetGlobalData();
            this.metadata = metadata;
        }


        public ItemData(ItemPlaceholder placeholder) {
            id = placeholder.ID;
            prototype = placeholder.Prototype;
            transformData = placeholder.transform.GetGlobalData();
            metadata = placeholder.Metadata;
        }


        public ItemData(ItemInWorld item) {
            id = item.ID;
            prototype = item.Prototype;
            transformData = item.transform.GetGlobalData();
            metadata = new();
        }


        public ItemData(ItemStack item) {
            id = item.ID;
            prototype = item.item;
            transformData = new();
            metadata = new();
        }



    }


}