using UnityEngine;


namespace kfutils.rpg {
    
    public class ItemPlaceholder : MonoBehaviour, ISerializationCallbackReceiver {

        [SerializeField] string id;
        [SerializeField] ItemPrototype prototype;
        [SerializeField] ItemMetadata metadata;
        [SerializeField] bool startWithPhysics;


        public string ID => id;
        public ItemPrototype Prototype => prototype;
        public ItemMetadata Metadata => metadata;
        public bool StartWithPhysics => startWithPhysics;


        public ItemData GetData() => new ItemData(this);


        public void OnAfterDeserialize() {
            if((prototype != null) && string.IsNullOrEmpty(id)) id = prototype.ID + System.Guid.NewGuid();
        }
        public void OnBeforeSerialize()  {/*Do Nothing*/}


    }   

}
