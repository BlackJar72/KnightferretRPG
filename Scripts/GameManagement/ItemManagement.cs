using System.Collections.Generic;
using UnityEngine;



namespace kfutils.rpg {

    public static class ItemManagement
    {

        private static readonly Dictionary<string, ItemPrototype> prototypeRegistry = new();

        public static Dictionary<string, ItemData> itemRegistry = new();


        public static void AddItemPrototype(ItemPrototype prototype) {
            if(!prototypeRegistry.ContainsKey(prototype.ID)) prototypeRegistry.Add(prototype.ID, prototype);
        }


        public static bool AddItem(ItemData item)
        {
            if (itemRegistry.ContainsKey(item.ID)) return false;
            itemRegistry.Add(item.ID, item);
            return true;
        }


        public static ItemData GetItem(string id) => itemRegistry.ContainsKey(id) ? itemRegistry[id] : null;


        public static ItemPrototype GetPrototype(string id) => prototypeRegistry.ContainsKey(id) ? prototypeRegistry[id] : null;


        public static void SetItemData(Dictionary<string, ItemData> loaded) {
            itemRegistry = loaded;
        }


        public static void NewGame()
        {
            itemRegistry = new();
        }

 

    }

}
