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


        public void Save(/*TODO: Identifier for save file*/) {
            // TODO: Save tha game data as a file
        }


        public void Load(/*TODO: Identifier for save file*/) {
            // TODO: Load the game data
        }

    }

}