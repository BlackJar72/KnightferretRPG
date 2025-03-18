using System;
using kfutils.rpg;
using Unity.Entities;
using UnityEngine;

namespace kfutils.rpg {

    [Serializable]
    public class EntityAttributes
    {
        // First, the the base stats
        [SerializeField] public EntityBaseStats baseStats;

        // Now, other core attributes
        [SerializeField] public int level = 1; // The level of the entity; only used for player character and NPCs of playable races
        [SerializeField] public float crouchSpeed = 2.0f; // Movement speed
        [SerializeField] public float walkSpeed = 5.0f; // Movement speed
        [SerializeField] public float runSpeed = 7.5f; // Movement speed
        [SerializeField] public float jumpForce = 1.0f; //TODO / FIXME: Implement jumping, then determine what this should be:
        [SerializeField] public int naturalArmor = 0; // Natural, not derived from worn armor
        [SerializeField] public int meleeDamageBonus = 0; // Bonus damage for melee attacks

        [SerializeField] public DamageAdjustType damageAdjuster = DamageAdjustType.NONE; // The type of natural damage adjuster this entity has
        [SerializeField] public DamageModifiers damageModifiers; // The damage modifiers the entity currently has (due to status effects)


        /// <summary>
        /// Used for characters of playable races (human or not); other creatures will have bespoke attribute assignments, or perhaps 
        /// different  specialized methods for special cases.
        /// </summary>
        /// <param name="health">Should be the EntityHealth field of the entity</param>
        /// <param name="stamina">Should be the EntityStamina field of the entity</param>
        /// <param name="mana">Should be the EntityMana field of the entity</param>
        public void DeriveAttributesForHuman(EntityHealth health, EntityStamina stamina, EntityMana mana) {
            crouchSpeed = 1.0f + (baseStats.Agility * 0.1f);
            walkSpeed = 4.5f + (baseStats.Agility * 0.05f);
            runSpeed  = walkSpeed + (baseStats.Agility * 0.25f);
            jumpForce = Mathf.Clamp((baseStats.Strength * 0.05f) + (baseStats.Agility * 0.05f), 0.25f, 1.75f);
            naturalArmor = Mathf.Max(0, baseStats.Agility / 2 - 10);
            meleeDamageBonus = Mathf.Max(0, baseStats.Strength / 2 - 10);
            // TODO: Add other effects on health, stamina, and mana
            health.ChangeBaseHealth((20 + (baseStats.Vitality * 5)) * (1.0f + ((float)level * 0.1f)));
            stamina.ChangeBaseStamina((20 + (baseStats.Endurance * 5)) * (1.0f + ((float)level * 0.1f)));
            mana.ChangeBaseMana((20 + (baseStats.Spirit * 5)) * (1.0f + ((float)level * 0.1f)));
        }


        public virtual int GetArmor() {
            return naturalArmor;
        }


    }


}