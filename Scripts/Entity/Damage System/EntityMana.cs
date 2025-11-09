using System;
using kfutils.rpg;
using UnityEngine;


namespace kfutils {

    [Serializable]
    public class EntityMana : IHaveStringID {

        public const float BASE_REGEN_RATE = 0.05f;
        public const float BASE_REGEN_ADJUST = 0.000833333333333f; // 1/1200f, or full regen in 20 minutes (minus effects of base regen rate)

        public float baseMana;
        public float currentMana;
        [SerializeField][HideInInspector] float buff;
        public float RelativeMana { get => currentMana / baseMana; }

        [NonSerialized] private EntityLiving owner = null;
        public EntityLiving Owner { get => owner; }

        public float Mana { get => currentMana;  set { currentMana = value; } }
        public float Buff { get => buff;  set { buff = value; MakeSane(); } }

        public bool HasMana { get => currentMana > 0; }

        public string ID { get => owner.ID; }
        

        public EntityMana(float maxMana)
        {
            currentMana = baseMana = maxMana;
            buff = 0;
        }


        public EntityMana Copy() {
            EntityMana copy = new(baseMana);
            copy.currentMana = currentMana;
            owner = null;
            return copy;
        }


        public void CopyInto(EntityMana other) {
            baseMana = other.baseMana;
            currentMana = other.currentMana;
            buff = other.buff;
        }


        public void MakeSane() {
            baseMana = Mathf.Min(baseMana, baseMana + buff);
            currentMana = Mathf.Min(currentMana, baseMana + buff);
        }


        public void ChangeBaseMana(float newMana) {
            float relative = RelativeMana;
            baseMana = newMana;
            currentMana = baseMana * relative;
            EntityManagement.AddManaExhausted(this);
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


        // For a nights (or days) sleep; i.e., a sleep / rest option in hours, not a full day of rest
        // This is set up to ensure full recovery after a full 8 hours of sleep.
        public void RestAndHealHours(float hours) {
            if(hours < 1) return;
            // First we calculate the percentage of health to heal (as an actual percent for understandability)
            float amount = hours >= 8 ? 2 : 0;
            amount += Math.Clamp((hours - 8.0f) / 16.0f, 0.0f, 1.0f) * 5.0f;
            hours = Mathf.Min(hours, 8.0f);
            amount += (10.0f * ((hours * hours) / 64.0f)) + hours;
            // convert from a percentage to an actual amount of health, including form percent to decimal fraction
            float altAmount = Mathf.Min((currentMana + ((baseMana * BASE_REGEN_ADJUST) + BASE_REGEN_RATE) 
                                                        * hours * GameConstants.TIME_SCALE), baseMana);
            amount *= baseMana * 0.05f;
            amount = Mathf.Max(amount, altAmount);
            // Now apply it by adding and clamping between 0 and fully healed
            currentMana = Mathf.Clamp(currentMana + amount, 0, baseMana + buff);
        }


        public bool CanDoAction(float ManaCost) => currentMana > ManaCost;


        /// <summary>
        /// For Mana regeneration after being using.
        /// </summary>
        public bool NaturalRegen() {
            currentMana = Mathf.Min((currentMana + ((baseMana * BASE_REGEN_ADJUST) + BASE_REGEN_RATE) * Time.deltaTime), baseMana);
            return currentMana < baseMana;
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
        

        public EntityMana BeLoaded(EntityLiving owner)
        {
            if(owner != null) this.owner = owner;
            if (currentMana < baseMana) EntityManagement.AddManaExhausted(this);
            return this;
        }



    }


}