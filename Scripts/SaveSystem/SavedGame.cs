using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg {


    [System.Serializable]
    public class SavedGame {

        // Player save data
        [SerializeField] PCTalking playerCharacter;
        // Main Registries
        [SerializeField] Dictionary<string, ItemData> itemRegistry;
        [SerializeField] Dictionary<string, InventoryData> inventoryData;
        [SerializeField] Dictionary<string, EntityData> entityRegistry;
        [SerializeField] Dictionary<string, ChunkData> chunkData;

        
        // Active Lists

        // Gameplay effect lists
        [SerializeField] List<EntityHealth> healingEntities; // Entities that are currently healing naturally 
        [SerializeField] List<EntityHealth> waitingToHeal; // Entities whose natural healing has not yet started (paused by recnently being damaged)
        [SerializeField] List<EntityStamina> recoveringEntities; // Entities that are currently recovering stamina
        [SerializeField] List<EntityStamina> waitingToRecover; // Entities whose stamina recovery has not yet started (paused by recnently using stamina)
        [SerializeField] List<EntityMana> recoveringMana; // Entities that are currently recovering mana


        public SavedGame() {
            playerCharacter = EntityManagement.playerCharacter;
            itemRegistry = ItemManagement.itemRegistry;
            inventoryData = InventoryManagement.inventoryData;
            entityRegistry = EntityManagement.EntityRegistry;
            // TODO: Get world space to save
            chunkData = WorldManagement.ChunkDataRegistry;

            // Active Lists

            // Gameplay effect lists
            healingEntities = EntityManagement.healingEntities; 
            waitingToHeal = EntityManagement.waitingToHeal; 
            recoveringEntities = EntityManagement.recoveringEntities; 
            waitingToRecover = EntityManagement.waitingToRecover; 
            recoveringMana = EntityManagement.recoveringMana; 
        }


        public void Save(/*TODO: Identifier for save file*/)
        {
            // TODO: Save tha game data as a file
            ES3.Save("PlaterData", playerCharacter, "TestSave.es3");
            ES3.Save("ItemRegistry", itemRegistry, "TestSave.es3");
            ES3.Save("InventoryData", inventoryData, "TestSave.es3");
            ES3.Save("EntityRegistry", entityRegistry, "TestSave.es3");
            ES3.Save("ChunkData", chunkData, "TestSave.es3");
            // TODO: More, much, much more...
        }


        public void Load(/*TODO: Identifier for save file*/)
        {
            // TODO: Load the game data
            itemRegistry = ES3.Load("ItemRegistry", "TestSave.es3", itemRegistry);
            inventoryData = ES3.Load("InventoryData", "TestSave.es3", inventoryData);
            entityRegistry = ES3.Load("EntityRegistry", "TestSave.es3", entityRegistry);
            chunkData = ES3.Load("ChunkData", "TestSave.es3", chunkData);

            // Set runtime data
            ItemManagement.SetItemData(itemRegistry);
            InventoryManagement.SetInventoryData(inventoryData);
            EntityManagement.SetEntityRegistry(entityRegistry);
            WorldManagement.SetChunkData(chunkData);
        }

    }

}