using UnityEngine;


namespace kfutils.rpg {

    public class ItemEquiptLocation : MonoBehaviour {
    
        [Tooltip("This is the bone in the chracter rig the item should be parented to; the script should probably also be on this bone. " 
                    + " This could also be an empty attached to the bone.")]
        [SerializeField] Transform bone; // The bone in the animation rig the item should be parented too
        [SerializeField] Layers layer = Layers.unityDefault;

        ItemEquipt  equiptItem;

        public ItemEquipt CurrentItem { get => equiptItem; }


        void Awake() {
            if(bone == null) bone = transform;
        }


        public ItemEquipt EquipItem(ItemEquipt prefab) {
            UnequiptCurrentItem();
            equiptItem = Instantiate(prefab, bone);
            equiptItem.SetEquiptTransform();
            equiptItem.SetRenderLayer(layer);
            return equiptItem;
        }


        public ItemEquipt EquipItem(ItemPrototype item) {
            return EquipItem(item.EquiptItem);
        }


        public ItemEquipt EquipItem(ItemStack item) {
            return EquipItem(item.item.EquiptItem);
        }


        public void UnequiptCurrentItem() {
            if (equiptItem is IUsable usable) usable.OnUnequipt();
            if (equiptItem != null) Destroy(equiptItem.gameObject);
        }


        public ItemEquipt EquiptArmor(ItemEquipt prefab) {
            // TODO: Equip as armor (i.e., attach to armature as skinned mesh)
            Debug.LogWarning("Using incomplete method; skinned mesh will be attached as non-skinned mesh.");
            return EquipItem(prefab);
        }


        public ItemEquipt EquiptArmor(ItemPrototype item) {
            // TODO: Equip as armor (i.e., attach to armature as skinned mesh)
            Debug.LogWarning("Using incomplete method; skinned mesh will be attached as non-skinned mesh.");
            return EquipItem(item.EquiptItem);
        }


        public ItemEquipt EquiptArmor(ItemStack item) {
            // TODO: Equip as armor (i.e., attach to armature as skinned mesh)
            Debug.LogWarning("Using incomplete method; skinned mesh will be attached as non-skinned mesh.");
            return EquipItem(item.item.EquiptItem);
        }

        

    }


}
