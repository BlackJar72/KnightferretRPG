using System;
using kfutils.rpg;
using Unity.Entities.UniversalDelegates;
using UnityEngine;


namespace kfutils {

    [Serializable]
    public class EntityMana {

        public const float BASE_REGEN_RATE = 0.05f;
        public const float BASE_REGEN_ADJUST = 0.000833333333333f; // 1/1200f, or full regen in 20 minutes (minus effects of base regen rate)

        public float baseMana;
        public float currentMana;
        [SerializeField][HideInInspector] float buff;
        public float RelativeMana { get => currentMana / baseMana; }

        public float Mana { get => currentMana;  set { currentMana = value; } }
        public float Buff { get => buff;  set { buff = value; MakeSane(); } }

        public bool HasMana { get => currentMana > 0; }
        

        public EntityMana(float maxMana)
        {
            currentMana = baseMana = maxMana;
            buff = 0;
        }


        public void MakeSane() {
            baseMana = Mathf.Min(baseMana, baseMana + buff);
            currentMana = Mathf.Min(currentMana, baseMana + buff);
        }


        public void ChangeBaseMana(float newMana) {
            if(newMana > baseMana) {
                baseMana = newMana;
            } else {
                baseMana = newMana;
                currentMana = baseMana * RelativeMana;
            }
            MakeSane();
        }


        public bool UseMana(float amount)
        {
            bool result = currentMana >= amount;
            if (result) {
                currentMana -= amount;
                EntityManagement.AddManaExhausted(this);
            } else {
                currentMana = 0;
            }
            return result;
        } 


        public void HealFully() {
            currentMana = baseMana + buff;
        }


        public bool CanDoAction(float ManaCost) => currentMana > ManaCost;


        /// <summary>
        /// For Mana regeneration after being using.
        /// </summary>
        public bool NaturalRegen() {
            currentMana = Mathf.Min((currentMana + ((baseMana * BASE_REGEN_ADJUST) + BASE_REGEN_RATE) * Time.deltaTime), baseMana);
            return currentMana < baseMana;
        }



    }


}