using UnityEngine;


namespace kfutils.rpg {
    
    /// <summary>
    /// FIXME???  Not sure I should do it this way, it jsut put the relevant field data here -- that might actually be better.
    /// </summary>
    [CreateAssetMenu(menuName = "KF-RPG/Items/Item Prototype", fileName = "ItemPrototype", order = 10)]
    public class ItemPrototype : ScriptableObject {

        [SerializeField] string id;
        [SerializeField] Sprite icon;
        [SerializeField] bool isStackable;
        [SerializeField] EItemType itemType;
        [SerializeField] EEquiptSlot equiptType;

        [SerializeField] float weight;
        [Tooltip("The value of the item in copper.  (Multiply value in gold by 100, or in silver by 10.)")]
        [SerializeField] int value;
        
        // The instances stored here are not for specific items, but are to be used as prototypes similar to prefabs 
        // in the editor.
        [SerializeField] ItemInWorld worldItem;
        [SerializeField] ItemEquipt equiptItem;

        // TODO: Add tool tip data for use with the UI

        public string ID { get => id; }
        public float Weight { get => weight; }
        public int Value { get => value; }
        public ItemInWorld InWorld { get => worldItem; }
        public ItemEquipt EquiptItem {get => equiptItem; }
        public Sprite Icon {get => icon; }
        public bool IsStackable {get => isStackable; }
        public EEquiptSlot EquiptType { get => equiptType; }


        //-----------------------------------------------------------------------------------------------------------//
        //                                       INSTANCE FACTORIES                                                  //
        //-----------------------------------------------------------------------------------------------------------//


        public ItemStack ItemStackFactory(int number, int slot) {
            return new ItemStack(this, number, slot);
        }


        public ItemInWorld DropItemInWorld(Transform where, float distance, float force = 0.0f) {
            ItemInWorld dropped = Instantiate(worldItem);
            dropped.transform.position = where.position + (where.forward * distance);
            dropped.EnablePhysics();
            if(force == 0.0f) return dropped;
            dropped.ApplyImpulseForce(where.forward * force);
            return dropped;
        }
    
        
    }



}
