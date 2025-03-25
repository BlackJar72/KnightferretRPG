using System.Collections.Generic;
using UnityEngine;



namespace kfutils.rpg {

    // Should this be static? A Singleton? Or just a regular MonoBehaviour?
    public static class ItemManagement {
        
        private static Dictionary<string, ItemPrototype> prototypeRegistry;


        /// <summary>
        /// Initializes the data for items.
        /// 
        /// THIS SHOULD ONLY BE CALLED WHEN STARTING OVER (i.e., at the beginning 
        /// of a game or when loading a save file).  All items must then be re-registered.
        /// 
        /// However, this must be called at least once before the item registries (or anything 
        /// relating the this class) can be used.
        /// </summary>
        public static void Initialize() {
            prototypeRegistry = new Dictionary<string, ItemPrototype>();
        }


        public static void AddItemPrototype(ItemPrototype prototype) {
            prototypeRegistry.Add(prototype.ID, prototype);
        }


        // TODO??? Should I include a way to just add all ItemPrototypes in a directory?  In assets as a whole?




    }

}
