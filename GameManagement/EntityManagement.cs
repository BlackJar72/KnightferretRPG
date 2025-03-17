using System.Collections.Generic;
using UnityEngine;

namespace kfutils.rpg
{

    public static class EntityManagement {
            
            // Should this be static? A Singleton? Or just a regular MonoBehaviour?

            // Core entity lists
            static public List<Entity> allEntities;
            static public List<Entity> persistentEntities;
            static public List<Entity> spawnedEntities;
            static public List<Entity> activeEntities;

            // Special entity lists
            static public List<EntityHealth> healingEntities; // Entities that are currently healing naturally 
            static public List<EntityHealth> waitingToHeal; // Entities whose natural healing has not yet started (paused by recnently being damaged)
            static public List<EntityStamina> recoveringEntities; // Entities that are currently recovering stamina
            static public List<EntityStamina> waitingToRecover; // Entities whose stamina recovery has not yet started (paused by recnently using stamina)
            static public List<EntityMana> recoveringMana; // Entities that are currently recovering mana



            public static void Initialize() {
                allEntities = new List<Entity>();
                persistentEntities = new List<Entity>();
                spawnedEntities = new List<Entity>();
                activeEntities = new List<Entity>();

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

            


    }


}