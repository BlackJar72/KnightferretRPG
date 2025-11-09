using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace kfutils.rpg {


    [System.Serializable]
    public class SavedGame
    {

        public const string saveSubdir = "saves";
        public const string saveFileExtension = ".es3";

        // General world data
        [SerializeField] double time;
        // Player save data
        [SerializeField] PCData pcData = new();
        // Main Registries
        [SerializeField] Dictionary<string, ItemData> itemRegistry;
        [SerializeField] Dictionary<string, InventoryData> inventoryData;
        [SerializeField] Dictionary<string, SpellbookData> spellbookData;
        [SerializeField] Dictionary<string, EquiptmentSlots> equiptData;
        [SerializeField] Dictionary<string, Money> moneyData;
        [SerializeField] Dictionary<string, EntityData> entityRegistry;
        [SerializeField] Dictionary<string, ChunkData> chunkData;

        [SerializeField] string currentWorldspace;
        [SerializeField] HotBar hotbar;


        // Active Lists

        // Gameplay effect lists
        [SerializeField] List<EntityHealth> healingEntities; // Entities that are currently healing naturally 
        [SerializeField] List<EntityHealth> waitingToHeal; // Entities whose natural healing has not yet started (paused by recnently being damaged)
        [SerializeField] List<EntityStamina> recoveringEntities; // Entities that are currently recovering stamina
        [SerializeField] List<EntityStamina> waitingToRecover; // Entities whose stamina recovery has not yet started (paused by recnently using stamina)
        [SerializeField] List<EntityMana> recoveringMana; // Entities that are currently recovering mana


        public SavedGame()
        {
            time = WorldTime.time;
            pcData = EntityManagement.playerCharacter == null ? null : EntityManagement.playerCharacter.GetPCData();
            itemRegistry = ItemManagement.itemRegistry;
            inventoryData = InventoryManagement.inventoryData;
            spellbookData = InventoryManagement.spellbookData;
            equiptData = InventoryManagement.equiptData;
            moneyData = InventoryManagement.moneyData;
            hotbar = InventoryManagement.hotBar;
            entityRegistry = EntityManagement.EntityRegistry;
            chunkData = WorldManagement.ChunkDataRegistry;
            currentWorldspace = WorldManagement.GetCurrentWorldspaceID();

            // Active Lists

            // Gameplay effect lists
            healingEntities = EntityManagement.healingEntities;
            waitingToHeal = EntityManagement.waitingToHeal;
            recoveringEntities = EntityManagement.recoveringEntities;
            waitingToRecover = EntityManagement.waitingToRecover;
            recoveringMana = EntityManagement.recoveringMana;
        }
        

        public static string GetFullPathOfSave(string saveName) => saveSubdir + Path.DirectorySeparatorChar + saveName + saveFileExtension;


        public static void DeleteSave(string saveName)
        {
            string fileName = saveSubdir + Path.DirectorySeparatorChar + saveName + saveFileExtension;
            ES3.DeleteFile(fileName);
        }


        public static bool HasSave(string saveName)
        {
            string fileName = saveSubdir + Path.DirectorySeparatorChar + saveName + saveFileExtension;
            return ES3.FileExists(fileName);
        }


        public void Save(string saveName)
        {
            string fileName = saveSubdir + Path.DirectorySeparatorChar + saveName + saveFileExtension;
            EntityManagement.playerCharacter.StoreDataForSave();
            EntityManagement.playerCharacter.PreSaveEquipt();
            foreach (string id in entityRegistry.Keys)
            {
                IActor actor = entityRegistry[id] as IActor;
                if (actor != null) actor.PreSaveEquipt();
            }
            ES3.Save("Time", time, fileName);
            ES3.Save("PCData", pcData, fileName);
            ES3.Save("ItemRegistry", itemRegistry, fileName); 
            ES3.Save("InventoryData", inventoryData, fileName);
            ES3.Save("SpellbookData", spellbookData, fileName);
            ES3.Save("EquiptData", equiptData, fileName);
            ES3.Save("MoneyData", moneyData, fileName);
            ES3.Save("HotBar", hotbar, fileName);
            ES3.Save("EntityRegistry", entityRegistry, fileName);
            ES3.Save("ChunkData", chunkData, fileName);
            ES3.Save("CurrentWorldspace", currentWorldspace, fileName);
            // TODO: More, much, much more...
            ES3.Save("HealingEntities", EntityManagement.GetIDList(healingEntities.Cast<IHaveStringID>().ToList()), fileName);
            ES3.Save("WaitingToHeal", EntityManagement.GetIDList(waitingToHeal.Cast<IHaveStringID>().ToList()), fileName);
            ES3.Save("ReoveringEntities", EntityManagement.GetIDList(recoveringEntities.Cast<IHaveStringID>().ToList()), fileName);
            ES3.Save("WaitingToRecover", EntityManagement.GetIDList(waitingToRecover.Cast<IHaveStringID>().ToList()), fileName);
            ES3.Save("RecoveringMana", EntityManagement.GetIDList(recoveringMana.Cast<IHaveStringID>().ToList()), fileName);
        }


        /// <summary>
        /// Call this before the world scene is loaded, to setup shared (static) data.
        /// </summary>
        /// <param name="saveName"></param>
        public void LoadWorld(string saveName)
        {
            string fileName = saveSubdir + Path.DirectorySeparatorChar + saveName + saveFileExtension;
            // TODO: Load the game data
            time = ES3.Load("Time", fileName, time);
            itemRegistry = ES3.Load("ItemRegistry", fileName, itemRegistry);
            inventoryData = ES3.Load("InventoryData", fileName, inventoryData);
            spellbookData = ES3.Load("SpellbookData", fileName, spellbookData);
            equiptData = ES3.Load("EquiptData", fileName, equiptData);
            moneyData = ES3.Load("MoneyData", fileName, moneyData);
            hotbar = ES3.Load("HotBar", fileName, hotbar);
            entityRegistry = ES3.Load("EntityRegistry", fileName, entityRegistry);
            chunkData = ES3.Load("ChunkData", fileName, chunkData);
            currentWorldspace = ES3.Load("CurrentWorldspace", fileName, currentWorldspace);

            // Set runtime data
            WorldTime.SetTime(time);
            ItemManagement.SetItemData(itemRegistry);
            InventoryManagement.SetInventoryData(inventoryData);
            InventoryManagement.SetSpellbookData(spellbookData);
            InventoryManagement.SetEquiptData(equiptData);
            InventoryManagement.SetMoneyData(moneyData);
            InventoryManagement.hotBar.CopyInto(hotbar);
            EntityManagement.SetEntityRegistry(entityRegistry);
            WorldManagement.SetChunkData(chunkData);
            WorldManagement.LoadWSFromSave(currentWorldspace);

            // FIXME: Entities and their data are partially separated in this
            healingEntities = EntityManagement.RestoreHealing(LoadStringIDList("HealingEntities", fileName));
            waitingToHeal = EntityManagement.RestoreWaitingToHeal(LoadStringIDList("WaitingToHeal", fileName));
            recoveringEntities = EntityManagement.RestoreRecoving(LoadStringIDList("ReoveringEntities", fileName));
            waitingToRecover = EntityManagement.RestoreWaitingToRecover(LoadStringIDList("WaitingToRecover", fileName));
            recoveringMana = EntityManagement.RestoreRecovingMana(LoadStringIDList("RecoveringMana", fileName));
        }


        /// <summary>
        /// Call this from player character class (PCTaling and its ancestors) after the game object has been properly 
        /// loaded (during Start() at least for now) to load data that needs to be set.  This needed because loading to 
        /// early creates an error as the objects do not exist, while trying to cache the data into a static context crashes 
        /// the engine for some reason.
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="oldData"></param>
        /// <returns></returns>
        public PCData LoadPlayer(string saveName, PCData oldData)
        {
            string fileName = saveSubdir + Path.DirectorySeparatorChar + saveName + saveFileExtension;
            pcData = ES3.Load("PCData", fileName, oldData);
            return pcData;
        }


        public List<string> LoadStringIDList(string saveKey, string filePath)
        {
            object tmp = ES3.Load(saveKey, filePath);
            List<string> tmplist = tmp as List<string>;
            return tmplist;
        }
        


    }

}