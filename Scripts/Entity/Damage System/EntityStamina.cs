using System;
using kfutils.rpg;
using UnityEngine;


namespace kfutils {

    [Serializable]
    public class EntityStamina : IHaveStringID {

        public const float HEALING_PAUSE_TIME = 1.0f; 
        public const float BASE_REGEN_RATE = 2.5f;
        public const float BASE_REGEN_ADJUST = 0.025f;

        public float baseStamina;
        public float currentStamina;
        [SerializeField][HideInInspector] float buff;
        public float RelativeStamina { get => currentStamina / baseStamina; }

        [NonSerialized] private EntityLiving owner = null;
        public EntityLiving Owner { get => owner; }

        public float Stamina { get => currentStamina;  set { currentStamina = value; } }
        public float Buff { get => buff;  set { buff = value; MakeSane(); } }

        public float timeToHeal = float.MinValue;
        public bool CanHeal { get => timeToHeal < Time.time; }
        public bool HasStamina { get => currentStamina > 0; }

        public string ID { get => owner.ID; }
        

        public EntityStamina(float maxStamina)
        {
            currentStamina = baseStamina = maxStamina;
            buff = 0;
        }


        public EntityStamina Copy() {
            EntityStamina copy = new(baseStamina);
            copy.currentStamina = currentStamina;
            copy.timeToHeal = timeToHeal;
            copy.owner = null;
            return copy;
        }


        public void MakeSane() {
            baseStamina = Mathf.Min(baseStamina, baseStamina + buff);
            currentStamina = Mathf.Min(currentStamina, baseStamina + buff);
        }


        public void ChangeBaseStamina(float newStamina) {
            if(newStamina > baseStamina) {
                baseStamina = newStamina;
            } else {
                float relative = RelativeStamina;
                baseStamina = newStamina;
                currentStamina = baseStamina * relative;
            }
            timeToHeal = Time.time; // No pause when this happens
            EntityManagement.AddExhausted(this);
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


        /// <summary>
        /// This is to set the owner.  Should function, essentially, like a readonly value, but as it cannot 
        /// be set in a constructor, it is instead set up so that it can only be set once (though with  
        /// more flexibility as to when as is required).  This largely so to allow for acces to the owners ID 
        /// for use in (de)serialization.
        /// </summary>
        /// <param name="owner"></param>
        public void SetOnwer(EntityLiving owner) {
            // Only allow this to change if it has not yet been set.
            if(this.owner == null) this.owner = owner;
        }



    }


}