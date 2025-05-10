using System.Collections.Generic;
using UnityEngine;



namespace kfutils.rpg {

    public static class ItemManagement {
        
        private static readonly Dictionary<string, ItemPrototype> prototypeRegistry = new();
        
        public static readonly Dictionary<string, ItemData> itemRegistry = new();


        public static void AddItemPrototype(ItemPrototype prototype) {
            prototypeRegistry.Add(prototype.ID, prototype);
        }


        public static bool AddItem(ItemData item) {
            if(itemRegistry.ContainsKey(item.ID)) return false;
            itemRegistry.Add(item.ID, item);
            return true;
        }


        public static ItemData GetItem(string id) => itemRegistry.ContainsKey(id) ? itemRegistry[id] : null;




    }

}
