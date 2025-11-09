using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



namespace kfutils.rpg {

    // Should this be static? A Singleton? Or just a regular MonoBehaviour?
    public static class EntityManagement {

        // A central registry for entities, which should work and not be too hard to do if using EasySave as it handles inheritance for us.
        static private Dictionary<string, EntityData> entityRegistry;
        static public PCTalking playerCharacter; 

        // Core entity lists
        static public List<EntityLiving> allEntities;
        static public List<EntityLiving> persistentEntities;
        static public List<EntityLiving> spawnedEntities;
        static public List<EntityLiving> activeEntities;

        // Special entity lists
        static public List<EntityHealth> healingEntities; // Entities that are currently healing naturally 
        static public List<EntityHealth> waitingToHeal; // Entities whose natural healing has not yet started (paused by recnently being damaged)
        static public List<EntityStamina> recoveringEntities; // Entities that are currently recovering stamina
        static public List<EntityStamina> waitingToRecover; // Entities whose stamina recovery has not yet started (paused by recnently using stamina)
        static public List<EntityMana> recoveringMana; // Entities that are currently recovering mana

        // Accessor Properties
        static public Dictionary<string, EntityData> EntityRegistry => entityRegistry;

        static public PCData pcData; 
        


        // TODO:  I *NEED* to create a proper entity registry, where copies or all entities can be kept and serialized, allowing all (other?) lists to be
        //        serialized and deserialized in terms of entity IDs through which references to the actual entity can be retrieved for the registry.
        //        Perhaps as a list, or perhaps as dictionary, or perhaps and combination of both.  (For this, entities need unique IDs, of course.)

        // FIXME: Need to remove entities that are unloaded.
        //  *OR*  Perhaps all remain loaded but become innactive, attached to a special scene that holds perpetual data and gameme management?  
        //          (How much data? -- not that much per entity, but how many entities might there be?  
        //          In the age of multi-gigabyte ram, probably not a lot relatively speaking.)  

        public static void Initialize()
        {
            entityRegistry = new Dictionary<string, EntityData>();

            allEntities = new List<EntityLiving>();
            persistentEntities = new List<EntityLiving>();
            spawnedEntities = new List<EntityLiving>();
            activeEntities = new List<EntityLiving>();

            healingEntities = new List<EntityHealth>();
            waitingToHeal = new List<EntityHealth>();
            recoveringEntities = new List<EntityStamina>();
            waitingToRecover = new List<EntityStamina>();
            recoveringMana = new List<EntityMana>();
        }


        public static void SetEntityRegistry(Dictionary<string, EntityData> loaded) {
            entityRegistry = loaded;
        }


        /// <summary>
        /// Add a living entity to the registry.  This is handled through a wrapper to 
        /// ensure the that the keys is always the ID held by the entity (they must match).
        /// </summary>
        /// <param name="entity"></param>
        public static void AddToRegistry(EntityData entity) {
            entityRegistry.Add(entity.ID, entity);
        }
        

        /// <summary>
        /// Add a living entity to the registry if and only if it is not already there. 
        /// This is similar to AddToRegistry(EntityData entity), but will not produce 
        /// an error if an entity with the same ID already exists, but instead quietyly 
        /// not add the entity.
        /// </summary>
        /// <param name="entity"></param>
        public static void AddToRegistrySafe(EntityData entity)
        {
            if (!entityRegistry.ContainsKey(entity.ID)) entityRegistry.Add(entity.ID, entity);

        }
        

        /// <summary>
        /// Add a living entity to the registry, replacing it if it exists. Technically, 
        /// it will simply try to remove the entity first, then added it regardless of 
        /// if it was previously there. 
        /// </summary>
        /// <param name="entity"></param>
        public static void AddToRegistryForce(EntityData entity)
        {
            entityRegistry.Remove(entity.ID);
            entityRegistry.Add(entity.ID, entity);

        }


        /// <summary>
        /// Retrieve and entity from the registry. This is wrapped, as the core registry 
        /// should not be exposed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EntityData GetFromRegistry(string id) {
            if(entityRegistry.ContainsKey(id)) return entityRegistry[id];
            else return null;
        }
        

        /*/// <summary>
        /// Removes and entity from the registry -- THIS SHOULD ONLY BE 
        /// DONE WITH TEMPORARY ENTITIES THAT ARE SPAWNED DURING GAMEPLAY!
        /// </summary>
        /// <param name="entity"></param>
        public static void RemoveFromRegistry(EntityData entity)
        {
            entityRegistry.Add(entity.ID, entity);
        }*/


        public static void Update() {
            // First, handle all the healing and recovery
            HealEntities();
            RecoverEntityStamina();
            RecoverEntityMana();            
        }


#region Healing and Recovery
        // Healing and Recovery

        static public void AddWounded(EntityHealth entity) {
            if(!waitingToHeal.Contains(entity)) {
                waitingToHeal.Add(entity);
                healingEntities.Remove(entity);
            }
        }


        static public void AddExhausted(EntityStamina entity) {
            if(!waitingToRecover.Contains(entity)) {
                waitingToRecover.Add(entity);
                recoveringEntities.Remove(entity);
            }
        }


        static public void AddManaExhausted(EntityMana entity) {
            if(!recoveringMana.Contains(entity)) recoveringMana.Add(entity);
        }


        static public void HealEntities() {
            for(int i = waitingToHeal.Count - 1; i > -1; i--) {
                if(waitingToHeal[i].CanHeal) {
                    healingEntities.Add(waitingToHeal[i]);
                    waitingToHeal.RemoveAt(i);
                }
            }
            for(int i = healingEntities.Count - 1; i > -1; i--) {
                if(!healingEntities[i].NaturalRegen()) {
                    healingEntities.RemoveAt(i);
                }
            }
        }


        static public void RecoverEntityStamina() {
            for(int i = waitingToRecover.Count - 1; i > -1; i--) {
                if(waitingToRecover[i].CanHeal) {
                    recoveringEntities.Add(waitingToRecover[i]);
                    waitingToRecover.RemoveAt(i);
                }
            }
            for(int i = recoveringEntities.Count - 1; i > -1; i--) {
                if(!recoveringEntities[i].NaturalRegen()) {
                    recoveringEntities.RemoveAt(i);
                }
            }
        }


        static public void RecoverEntityMana() {
            for(int i = recoveringMana.Count - 1; i > -1; i--) {
                if(!recoveringMana[i].NaturalRegen()) {
                    recoveringMana.RemoveAt(i);
                }
            }
        }


        static public void RemoveDead(EntityLiving entity)
        {
            healingEntities.Remove(entity.health);
            waitingToHeal.Remove(entity.health);
            recoveringEntities.Remove(entity.stamina);
            waitingToRecover.Remove(entity.stamina);
            recoveringMana.Remove(entity.mana);
        }


        #endregion


        #region Serialization Helpers


        public static List<string> GetIDList(List<IHaveStringID> entities)
        {
            List<string> result = new();
            for (int i = 0; i < entities.Count; i++)
            {
                result.Add(entities[i].ID);
            }
            return result;
        }


        public static List<EntityData> RestoreListFromIDs(List<string> IDs) {
            List<EntityData> result = new();
            for(int i = 0; i < IDs.Count; i++) {
                result.Add(entityRegistry[IDs[i]]);
            }
            return result;
        }


        public static void RestoreListFromIDs(List<string> IDs, List<EntityData> entites) {
            entites.Clear();
            for(int i = 0; i < IDs.Count; i++) {
                // TODO: Recreate entities from data in registry!
                // TODo / FIXME: Replace entity lists with EntityData lists!!!
                entites.Add(entityRegistry[IDs[i]]);
            }    
        }


        public static List<EntityHealth> RestoreHealing(List<string> IDs)
        {
            healingEntities.Clear();
            for (int i = 0; i < IDs.Count; i++)
            {
                    if (entityRegistry.ContainsKey(IDs[i]))
                    {
                        healingEntities.Add(entityRegistry[IDs[i]].livingData.health);
                    }
            }
            return healingEntities;
        }


        public static List<EntityHealth> RestoreWaitingToHeal(List<string> IDs)
        {
            waitingToHeal.Clear();
            for (int i = 0; i < IDs.Count; i++)
            {
                if(entityRegistry.ContainsKey(IDs[i])) waitingToHeal.Add(entityRegistry[IDs[i]].livingData.health);
            }
            return waitingToHeal;
        }


        public static List<EntityStamina> RestoreRecoving(List<string> IDs)
        {
            recoveringEntities.Clear();
            for (int i = 0; i < IDs.Count; i++)
            {
                if(entityRegistry.ContainsKey(IDs[i])) recoveringEntities.Add(entityRegistry[IDs[i]].livingData.stamina);
            }
            return recoveringEntities;
        }


        public static List<EntityStamina> RestoreWaitingToRecover(List<string> IDs)
        {
            waitingToRecover.Clear();
            for (int i = 0; i < IDs.Count; i++)
            {
                if(entityRegistry.ContainsKey(IDs[i])) waitingToRecover.Add(entityRegistry[IDs[i]].livingData.stamina);
            }
            return waitingToRecover;
        }


        public static List<EntityMana> RestoreRecovingMana(List<string> IDs)
        {
            recoveringMana.Clear();
            for (int i = 0; i < IDs.Count; i++)
            {
                if(entityRegistry.ContainsKey(IDs[i])) recoveringMana.Add(entityRegistry[IDs[i]].livingData.mana);
            }
            return recoveringMana;
        }
    



#region Health, stamina, mana
        public static List<EntityHealth> RestoreHealthFromIDs(List<string> IDs)
        {
            List<EntityHealth> result = new();
            for (int i = 0; i < IDs.Count; i++)
            {
                result.Add(entityRegistry[IDs[i]].livingData.health);
            }
            return result;
        }


    public static void RestoreHealthFromIDs(List<string> IDs, List<EntityHealth> entites) {
        entites.Clear();
        for(int i = 0; i < IDs.Count; i++) {
            entites.Add(entityRegistry[IDs[i]].livingData.health);
        }
    }


    public static List<EntityStamina> RestoreStaminaFromIDs(List<string> IDs) {
        List<EntityStamina> result = new();
        for(int i = 0; i < IDs.Count; i++) {
            result.Add(entityRegistry[IDs[i]].livingData.stamina);
        }
        return result;
    }


    public static void RestoreHealthStaminaDs(List<string> IDs, List<EntityStamina> entites) {
        entites.Clear();
        for(int i = 0; i < IDs.Count; i++) {
            entites.Add(entityRegistry[IDs[i]].livingData.stamina);
        }
    }


    public static List<EntityMana> RestoreManaFromIDs(List<string> IDs) {
        List<EntityMana> result = new();
        for(int i = 0; i < IDs.Count; i++) {
            result.Add(entityRegistry[IDs[i]].livingData.mana);
        }
        return result;
    }


    public static void RestoreHealthManaIDs(List<string> IDs, List<EntityMana> entites) {
        entites.Clear();
        for(int i = 0; i < IDs.Count; i++) {
            entites.Add(entityRegistry[IDs[i]].livingData.mana);
        }
    }
    #endregion




#endregion




    }


}
