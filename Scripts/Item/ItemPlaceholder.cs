#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace kfutils.rpg {
    
    public class ItemPlaceholder : MonoBehaviour, IAutoAssignID 
    {

        [SerializeField] string id;
        [SerializeField] ItemPrototype prototype;
        [SerializeField] ItemMetadata metadata;
        [SerializeField] bool startWithPhysics;


        public string ID => id;
        public ItemPrototype Prototype => prototype;
        public ItemMetadata Metadata => metadata;
        public bool StartWithPhysics => startWithPhysics;


        public ItemData GetData() => new ItemData(this);
        

        public void BeAssignedID()
        {
            #if UNITY_EDITOR
            id = prototype.ID + "_" + System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            #endif
        }


    }   

}
