using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg {


    [System.Serializable]
    public class SavedGame {

        [SerializeField] PCTalking playerCharacter;
        [SerializeField] Dictionary<string, ItemData> itemRegistry;
        [SerializeField] Dictionary<string, InventoryData> inventoryData;
        [SerializeField] Dictionary<string, EntityData> entityRegistry;


        public SavedGame() {
            playerCharacter = EntityManagement.playerCharacter;
            itemRegistry = ItemManagement.itemRegistry;
            inventoryData = InventoryManagement.inventoryData;
            entityRegistry = EntityManagement.EntityRegistry;
        }


        public void Save(/*TODO: Identifier for save file*/) {
            // TODO: Save tha game data as a file
        }


        public void Load(/*TODO: Identifier for save file*/) {
            // TODO: Load the game data
        }

    }

}