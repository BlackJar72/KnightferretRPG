using UnityEngine;


namespace kfutils.rpg {
    
    public class ItemEquipt : MonoBehaviour {

        [SerializeField] protected ItemPrototype prototype;

        [Tooltip("This holds a copy of the items local transform data, which will " 
                    + "be copied on to the items real transform after it has been parented to the bones.")]
        [SerializeField] TransformData itemTransform;


        public TransformData ItemTransform => itemTransform;


        public void SetEquiptTransform() {
            transform.localPosition = itemTransform.position;
            transform.localRotation = itemTransform.rotation;
            transform.localScale = itemTransform.scale;
        }


    
    }


}
