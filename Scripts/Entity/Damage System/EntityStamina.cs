using System;
using kfutils.rpg;
using Unity.Entities.UniversalDelegates;
using UnityEngine;


namespace kfutils {

    [Serializable]
    public class EntityStamina {

        public const float HEALING_PAUSE_TIME = 1.0f; 
        public const float BASE_REGEN_RATE = 1.0f;
        public const float BASE_REGEN_ADJUST = 0.01f;

        public float baseStamina;
        public float currentStamina;
        [SerializeField][HideInInspector] float buff;
        public float RelativeStamina { get => currentStamina / baseStamina; }

        public float Stamina { get => currentStamina;  set { currentStamina = value; } }
        public float Buff { get => buff;  set { buff = value; MakeSane(); } }

        public float timeToHeal = float.MinValue;
        public bool CanHeal { get => timeToHeal < Time.time; }
        public bool HasStamina { get => currentStamina > 0; }
        

        public EntityStamina(float maxStamina)
        {
            currentStamina = baseStamina = maxStamina;
            buff = 0;
        }


        public void MakeSane() {
            baseStamina = Mathf.Min(baseStamina, baseStamina + buff);
            currentStamina = Mathf.Min(currentStamina, baseStamina + buff);
        }


        public void ChangeBaseStamina(float newStamina) {
            if(newStamina > baseStamina) {
                baseStamina = newStamina;
            } else {
                baseStamina = newStamina;
                currentStamina = baseStamina * RelativeStamina;
                timeToHeal = Time.time; // No pause when this happens
                EntityManagement.AddExhausted(this);
            }
            MakeSane();
        }


        public bool UseStamina(float amount)
        {
            bool result = currentStamina >= amount;
            if (result) {
                currentStamina -= amount;
                timeToHeal = Time.time + HEALING_PAUSE_TIME;
                EntityManagement.AddExhausted(this);
            } else {
                currentStamina = 0;
            }
            return result;
        } 


        public bool CanDoAction(float staminaCost) => currentStamina > staminaCost;


        /// <summary>
        /// For stamina regeneration after being using.
        /// </summary>
        public bool NaturalRegen() {
            currentStamina = Mathf.Min((currentStamina + ((baseStamina * BASE_REGEN_ADJUST) + BASE_REGEN_RATE) * Time.deltaTime), baseStamina);
            return currentStamina < baseStamina;
        }


        public void HealFully() {
            currentStamina = baseStamina + buff;
        }



    }


}