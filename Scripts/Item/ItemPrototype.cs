using UnityEngine;


namespace kfutils.rpg {
    
    public class ItemPrototype : ScriptableObject {
        [SerializeField] string id;
        [SerializeField] ItemInInventory inventoryItem;
        [SerializeField] ItemInWorld worldItem;
        [SerializeField] ItemEquipt equiptItem;

        // TODO: Add tool tip data for use with the UI

        public string ID { get => id; }
        public ItemInInventory InInventory { get => inventoryItem; }
        public ItemInWorld InWorld { get => worldItem; }
        public ItemEquipt EquiptItem {get => equiptItem; }


        //-----------------------------------------------------------------------------------------------------------//
        //                                       INSTANCE FACTORIES                                                  //
        //-----------------------------------------------------------------------------------------------------------//



    
        
    }



}
