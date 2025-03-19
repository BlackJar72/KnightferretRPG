using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



namespace kfutils.rpg
{

    public static class EntityManagement {
            
            // Should this be static? A Singleton? Or just a regular MonoBehaviour?

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



            public static void Initialize() {
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





    }


}