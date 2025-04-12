using UnityEngine;


namespace kfutils.rpg {

    public class ItemEquiptLocation : MonoBehaviour {
    
        [Tooltip("This is the bone in the chracter rig the item should be parented to; the script should probably also be on this bone. " 
                    + " This could also be an empty attached to the bone.")]
        [SerializeField] Transform bone; // The bone in the animation rig the item should be parented too

        ItemEquipt  equiptItem;

        public ItemEquipt CurrectItem { get => equiptItem; }


        void Awake() {
            if(bone == null) bone = transform;
        }


        public ItemEquipt EquipItem(ItemEquipt prefab) {
            UnequiptCurrentItem();
            equiptItem = Instantiate(prefab, bone);
            equiptItem.SetEquiptTransform();
            return equiptItem;
        }


        public ItemEquipt EquipItem(ItemPrototype item) {
            return EquipItem(item.EquiptItem);
        }


        public ItemEquipt EquipItem(ItemStack item) {
            return EquipItem(item.item.EquiptItem);
        }


        public void UnequiptCurrentItem() {
            if(equiptItem != null) Destroy(equiptItem.gameObject);
        }

    }


}
