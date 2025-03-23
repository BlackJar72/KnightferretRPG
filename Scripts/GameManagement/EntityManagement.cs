using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;



namespace kfutils.rpg {

    // Should this be static? A Singleton? Or just a regular MonoBehaviour?
    public static class EntityManagement {

            // A central registry for entities, which should work and not be too hard to do if using EasySave as it handles inheritance for us.
            static private Dictionary<string, EntityLiving> entityRegistry;
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

            // TODO:  I *NEED* to create a proper entity registry, where copies or all entities can be kept and serialized, allowing all (other?) lists to be
            //        serialized and deserialized in terms of entity IDs through which references to the actual entity can be retrieved for the registry.
            //        Perhaps as a list, or perhaps as dictionary, or perhaps and combination of both.  (For this, entities need unique IDs, of course.)

            // FIXME: Need to remove entities that are unloaded.
            //  *OR*  Perhaps all remain loaded but become innactive, attached to a special scene that holds perpetual data and gameme management?  
            //          (How much data? -- not that much per entity, but how many entities might there be?  
            //          In the age of multi-gigabyte ram, probably not a lot relatively speaking.)  

            public static void Initialize() {
                entityRegistry = new Dictionary<string, EntityLiving>();

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


            /// <summary>
            /// Add a living entity to the registry.  This is handled through a wrapper to 
            /// ensure the that the keys is always the ID held by the entity (they must match).
            /// </summary>
            /// <param name="entity"></param>
            public static void AddToRegistry(EntityLiving entity) {
                entityRegistry.Add(entity.ID, entity);
            }


            /// <summary>
            /// Retrieve and entity from the registry. This is wrapped, as the core registry 
            /// should not be exposed.
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static EntityLiving GetFromRegistry(string id) => entityRegistry[id];


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
#endregion


#region Serialization Helpers

        /// <summary>
        /// Get an list of ID that can be serialized.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<string> GetIDList(this List<IHaveStringID> list) {
            List<string> output = new List<string>(list.Count); 
            foreach(IHaveStringID element in list) output.Add(element.ID); 
            return output;
        }


        public static List<EntityHealth> RebuildHealthList(List<string> serialized) {
            List<EntityHealth> output = new List<EntityHealth>();
            foreach(string id in serialized) output.Add(entityRegistry[id].health);
            return output;
        }


        public static List<EntityStamina> RebuildStaminahList(List<string> serialized) {
            List<EntityStamina> output = new List<EntityStamina>();
            foreach(string id in serialized) output.Add(entityRegistry[id].stamina);
            return output;
        }


        public static List<EntityMana> RebuildHelthList(List<string> serialized) {
            List<EntityMana> output = new List<EntityMana>();
            foreach(string id in serialized) output.Add(entityRegistry[id].mana);
            return output;
        }


        public static List<EntityLiving> RebuildEntityList(List<string> serialized) {
            List<EntityLiving> output = new List<EntityLiving>();
            foreach(string id in serialized) output.Add(entityRegistry[id]);
            return output;
        }


#endregion




    }


}
