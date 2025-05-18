using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace kfutils.rpg {


    [System.Serializable]
    public class SavedGame
    {

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


        public SavedGame()
        {
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
            ES3.Save("HealingEntities", EntityManagement.GetIDList(healingEntities.Cast<IHaveStringID>().ToList()), "TestSave.es3");
            ES3.Save("WaitingToHeal", EntityManagement.GetIDList(waitingToHeal.Cast<IHaveStringID>().ToList()), "TestSave.es3");
            ES3.Save("ReoveringEntities", EntityManagement.GetIDList(waitingToRecover.Cast<IHaveStringID>().ToList()), "TestSave.es3");
            ES3.Save("WaitingToRecover", EntityManagement.GetIDList(waitingToRecover.Cast<IHaveStringID>().ToList()), "TestSave.es3");
            ES3.Save("RecoveringMana", EntityManagement.GetIDList(recoveringMana.Cast<IHaveStringID>().ToList()), "TestSave.es3");
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

            // FIXME: Entities and there data are partially separated in this
            healingEntities = EntityManagement.RestoreHealing(LoadStringIDList("HealingEntities", "TestSave.es3"));
            waitingToHeal = EntityManagement.RestoreWaitingToHeal(LoadStringIDList("WaitingToHeal", "TestSave.es3"));
            recoveringEntities = EntityManagement.RestoreRecoving(LoadStringIDList("ReoveringEntities", "TestSave.es3"));
            waitingToRecover = EntityManagement.RestoreWaitingToRecover(LoadStringIDList("WaitingToRecover", "TestSave.es3"));
            recoveringMana = EntityManagement.RestoreRecovingMana(LoadStringIDList("RecoveringMana", "TestSave.es3"));
        }


        public List<string> LoadStringIDList(string saveKey, string filePath)
        {
            object tmp = ES3.Load(saveKey, filePath);
            List<string> tmplist = tmp as List<string>;
            return tmplist;
        }
        


    }

}