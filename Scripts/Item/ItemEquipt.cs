using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class ItemEquipt : MonoBehaviour
    {

        [SerializeField] protected ItemPrototype prototype;

        [Tooltip("This holds a copy of the items local transform data, which will "
                    + "be copied on to the items real transform after it has been parented to the bones.")]
        [SerializeField] TransformData itemTransform;
        [SerializeField] protected ClipTransition equiptAnim;
        [SerializeField] protected GameObject renderedObject;

        public ClipTransition EquiptAnim => equiptAnim;
        public virtual bool IsReal => true;


        public void SetRenderLayer(Layers mask)
        {
            renderedObject.layer = (int)mask;
            Transform[] kids = renderedObject.GetComponentsInChildren<Transform>();
            foreach(Transform t in kids) t.gameObject.layer = (int)mask;
        }


        public TransformData ItemTransform => itemTransform;


        public void SetEquiptTransform()
        {
            transform.localPosition = itemTransform.position;
            transform.localRotation = itemTransform.rotation;
            transform.localScale = itemTransform.scale;
        }


        [ContextMenu("Set TransformData From Transform")]
        public void SetTransformDataFromTransform()
        {
            itemTransform = new TransformData(gameObject.transform);
        }


        public void BeDropped()
        {
            ChunkManager chunk = WorldManagement.WorldLogic.GetChunk(transform);
            ItemInWorld item = prototype.DropItemInFromHand(transform, 0.0f, Random.value * 0.25f);
        }


    
    }


}
