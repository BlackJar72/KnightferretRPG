using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using kfutils.rpg;


namespace kfutils {

    [Serializable]
    public class EntityHealth {
        public const float HEALING_PAUSE_TIME = 5.0f; // in seconds
        public const float BASE_REGEN_RATE = 1.0f;
        public const float BASE_REGEN_ADJUST = 0.01f;
        public static readonly DefaultDamageAdjuster defaultDamageAdjuster = new DefaultDamageAdjuster();

        [SerializeField] float baseHealth;

        [SerializeField] float wound;
        [SerializeField] float shock;

        [SerializeField][HideInInspector] float buff;

        public int   BaseHealth { get => (int)baseHealth;  }
        public float RelativeWound { get => wound / baseHealth; }
        public float RelativeShock { get => shock / baseHealth; }

        public float Health { get => wound;  set { wound = value; } }
        public float Shock { get => shock;  set { shock = value; } }
        public float Buff { get => buff;  set { buff = value; MakeSane(); } }

        public float HP => Mathf.Min(wound, shock);

        public bool ShouldDie { get => ((wound < 1) || (shock < 1)); }

        public float timeLastDamaged = float.MinValue;
        public bool CanHeal { get => (timeLastDamaged + HEALING_PAUSE_TIME) > Time.time; }

        //Tried to fix BTree error, didn't work.
        //There really should be no conversion as errors found by the IDE help find places that need to be edited.
        //public static implicit operator float(EntityHealth h) => Mathf.Min(h.shock, h.wound);

        public EntityHealth(float baseHealth) {
            shock = wound = this.baseHealth = baseHealth;
            buff = 0;
        }


        public void MakeSane() {
            wound = Mathf.Min(wound, baseHealth + buff);
            shock = Mathf.Min(shock, baseHealth + buff);
        }


        public void ChangeBaseHealth(float newHealth) {
            if(newHealth > baseHealth) {
                baseHealth = newHealth;
            } else {
                float woundDiff = RelativeWound;
                float shockDiff = RelativeShock;
                baseHealth = newHealth;
                wound = baseHealth * woundDiff;
                shock = shock * shockDiff;
            }
            MakeSane();
        }


        public void ChangeBaseHealthBy(float amount) {
            ChangeBaseHealth(baseHealth + amount);
        }


        public void TakeDamage(Damages damage) {            
            shock -= damage.shock;
            wound -= damage.wound;
            timeLastDamaged = Time.time;
            EntityManagement.AddWounded(this);
        }


        public void Heal(float amount) {
            wound = Mathf.Clamp(wound + amount, 0, baseHealth + buff);
            shock = Mathf.Clamp(shock + amount, 0, baseHealth + buff);
        }


        public void HealShock(float amount) {;
            shock = Mathf.Clamp(shock + amount, 0, baseHealth + buff);
        }


        // For a nights (or days) sleep; i.e., a sleep / rest option in hours, not a full day of rest
        public void RestAndHealHours(float hours) {
            if(hours < 1) return;
            // First we calculate the percentage of health to heal (as an actual percent for understandability)
            float amount = hours >= 8 ? 2 : 0;
            amount += Math.Clamp((hours - 8.0f) / 16.0f, 0.0f, 1.0f) * 5.0f;
            hours = Mathf.Min(hours, 8.0f);
            amount += (10.0f * ((hours * hours) / 64.0f)) + hours;
            // convert from a percentage to an actual amount of health, including form percent to decimal fraction
            amount *= baseHealth * 0.01f;
            // Now apply it by adding and clamping between 0 and fully healed
            wound = Mathf.Clamp(wound + amount, 0, baseHealth + buff);
            shock = baseHealth + buff; // Shock should always be fully healed;
        }


        // For downtime, taken in full days
        public void RestAndHealDays(int days) {
            if(days < 1) return;
            // 25% of health per day; not realistic but enough, and who wants to be laid-up injured for months in a game?!
            float amount = (float)days * 0.25f * baseHealth;
            wound = Mathf.Clamp(wound + amount, 0, baseHealth + buff);
            shock = baseHealth + buff; // Shock should always be fully healed;
        }


        public void HealWound(float amount) {
            wound = Mathf.Clamp(wound + amount, 0, baseHealth + buff);
        }


        public void HealFully() {
            shock = wound = baseHealth + buff;
        }


        public void MakeDead() {
            shock = wound = -1f;
        }


        /// <summary>
        /// For shock regeneration after being wounded.
        /// </summary>
        public bool NaturalRegen() {
            shock = Mathf.Min((shock + ((baseHealth * BASE_REGEN_ADJUST) + BASE_REGEN_RATE) * Time.deltaTime), baseHealth);
            return shock < baseHealth;
        }


        //public static EntityHealth operator +(EntityHealth a) => a;


    }


    public class DefaultDamageAdjuster : IDamageAdjuster {
        public Damages Apply(kfutils.Damages damage) {
            return default(Damages);
        }
    }
}