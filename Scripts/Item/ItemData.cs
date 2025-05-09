using UnityEngine;


namespace kfutils.rpg {
    
    public class ItemData {

        [SerializeField] readonly string id;
        [SerializeField] readonly ItemPrototype prototype;
        [SerializeField] TransformData transformData;
        [SerializeField] ItemMetadata metadata;


        public string ID => id;
        public ItemPrototype Prototype => prototype;
        public TransformData TransformData => transformData;
        public ItemMetadata Metadata => metadata;


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

    }


}