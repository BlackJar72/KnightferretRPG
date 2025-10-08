using kfutils.rpg.ui;
using UnityEngine;


namespace kfutils.rpg {

    /// <summary>
    /// FIXME???  Not sure I should do it this way, it jsut put the relevant field data here -- that might actually be better.
    /// </summary>
    [CreateAssetMenu(menuName = "KF-RPG/Items/Item Prototype", fileName = "ItemPrototype", order = 10)]
    public class ItemPrototype : ScriptableObject
    {

        [SerializeField] string id;
        [SerializeField] string itemName;
        [SerializeField] Sprite icon;
        [SerializeField] bool isStackable;
        [SerializeField] EItemType itemType;
        [SerializeField] EEquiptSlot equiptType;
        [SerializeField] string description;

        [SerializeField] float weight;
        [Tooltip("The value of the item in copper.  (Multiply value in gold by 100, or in silver by 10.)")]
        [SerializeField] Money value;

        [SerializeField] ActivityItem activity;

        // The instances stored here are not for specific items, but are to be used as prototypes similar to prefabs 
        // in the editor.
        [SerializeField] ItemInWorld worldItem;
        [SerializeField] ItemEquipt equiptItem;

        // TODO: Add tool tip data for use with the UI

        public string ID { get => id; }
        public string Name { get => itemName; }
        public float Weight { get => weight; }
        public Money Value { get => value; }
        public ItemInWorld InWorld { get => worldItem; }
        public ItemEquipt EquiptItem { get => equiptItem; }
        public Sprite Icon { get => icon; }
        public bool IsStackable { get => isStackable; }
        public EItemType ItemType { get => itemType; }
        public EEquiptSlot EquiptType { get => equiptType; }
        public string Description { get => description; }
        public ActivityItem Activity { get => activity; }
        public IActivityObject IActivity { get => activity; }


        //-----------------------------------------------------------------------------------------------------------//
        //                                       UTILITY METHODS                                                     //
        //-----------------------------------------------------------------------------------------------------------//


        public bool FitsFlags(EEquiptSlotFlags flags)
        {
            return ((0x01 << (int)equiptType) & (int)flags) > 0;
        }


        public bool FitsSlot(EquipmentSlotUI slot)
        {
            return ((0x01 << (int)equiptType) & (int)slot.SlotType) > 0;
        }



        //-----------------------------------------------------------------------------------------------------------//
        //                                       INSTANCE FACTORIES                                                  //
        //-----------------------------------------------------------------------------------------------------------//



        public ItemStack ItemStackFactory(int number, int slot)
        {
            return new ItemStack(this, number, slot);
        }


        public ItemInWorld DropItemInWorld(Transform where, float distance, float force = 0.0f)
        {
            ItemInWorld dropped = worldItem.Spawn(where.position + (where.forward * distance));
            dropped.EnablePhysics();
            if (force == 0.0f) return dropped;
            dropped.ApplyImpulseForce(where.forward * force * Random.Range(0.95f, 1.05f));
            return dropped;
        }


        public ItemInWorld DropItemInFromHand(Transform where, float distance, float force = 0.0f)
        {
            ItemInWorld dropped = worldItem.Spawn(where.position + (where.forward * distance));
            dropped.gameObject.transform.rotation = where.rotation;
            dropped.EnablePhysics();
            if (force == 0.0f) return dropped;
            dropped.ApplyImpulseForce(where.forward * force * Random.Range(0.95f, 1.05f));
            return dropped;
        }



        //-----------------------------------------------------------------------------------------------------------//
        //                                       UTILITY AI SUPPORT                                                  //
        //-----------------------------------------------------------------------------------------------------------//


        public float GetItemUtility(EObjectActivity activityType, ITalkerAI ai)
        {
            switch (activityType)
            {
                case EObjectActivity.NONE: return 0.0f;
                case EObjectActivity.NEED_DISCRETE: return activity.GetUtility(ai);
                case EObjectActivity.NEED_CONTINUOUS: return activity.GetUtility(ai);
                case EObjectActivity.COMBAT_OPTION: throw new System.Exception("Trying to get item utility from non-item " + ID);
                case EObjectActivity.WEAPON: return GetWeaponUtility(ai);
                case EObjectActivity.SPELL: return GetScrollUtility(ai);
                case EObjectActivity.HEALTH_ITEM: return GetHealthUtility(ai);
                case EObjectActivity.SELF: return GetSpecialSelfUtility(ai);
                default: throw new System.Exception("Trying to get item utility from non-item " + ID);
            }
        }


        private float GetWeaponUtility(ITalkerAI ai)
        {
            throw new System.NotImplementedException();
        }


        private float GetScrollUtility(ITalkerAI ai)
        {
            throw new System.NotImplementedException();
        }


        private float GetHealthUtility(ITalkerAI ai)
        {
            throw new System.NotImplementedException();
        }


        private float GetSpecialSelfUtility(ITalkerAI ai)
        {
            throw new System.NotImplementedException();
        }
        

        




    }
        

  

}
