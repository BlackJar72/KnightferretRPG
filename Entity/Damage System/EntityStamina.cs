using kfutils.rpg;
using Unity.Entities.UniversalDelegates;
using UnityEngine;


namespace kfutils {

    [SerializeField]
    public class EntityStamina {

        public const float HEALING_PAUSE_TIME = 0.5f; 
        public const float BASE_REGEN_RATE = 2.5f;
        public const float BASE_REGEN_ADJUST = 0.025f;

        public float baseStamina;
        public float currentStamina;
        [SerializeField][HideInInspector] float buff;
        public float RelativeStamina { get => currentStamina / baseStamina; }

        public float Stamina { get => currentStamina;  set { currentStamina = value; } }
        public float Buff { get => buff;  set { buff = value; MakeSane(); } }

        public float timeLastTired = float.MinValue;
        public bool CanHeal { get => (timeLastTired + HEALING_PAUSE_TIME) > Time.time; }
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
            }
            MakeSane();
        }


        public bool UseStamina(float amount)
        {
            bool result = currentStamina >= amount;
            if (result) {
                currentStamina -= amount;
                timeLastTired = Time.time;
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



    }


}