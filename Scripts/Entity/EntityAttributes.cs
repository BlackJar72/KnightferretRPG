using System;
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
        [SerializeField] public float meleeDamageFactor = 1.0f;
        [SerializeField] public float maxEncumbrance = 120f;
        [SerializeField] public float halfEncumbrance = 60f;
        [SerializeField] public float runningCostFactor = 1.0f; // Mostly used for running, and perhaps other movement, not all stamina use
        [SerializeField] public float manaCostFactor = 1.0f; // Modifies the cost of casting spells
        [SerializeField] public int maxSpellDifficulty = 0;

        [SerializeField] public DamageAdjustType damageAdjuster = DamageAdjustType.NONE; // The type of natural damage adjuster this entity has
        [SerializeField] public DamageModifiers damageModifiers = new DamageModifiers(); // The damage modifiers the entity currently has (due to status effects)


        /// <summary>
        /// Used for characters of playable races (human or not); other creatures will have bespoke attribute assignments, or perhaps 
        /// different  specialized methods for special cases.
        /// </summary>
        /// <param name="health">Should be the EntityHealth field of the entity</param>
        /// <param name="stamina">Should be the EntityStamina field of the entity</param>
        /// <param name="mana">Should be the EntityMana field of the entity</param>
        public void DeriveAttributesForHuman(EntityHealth health, EntityStamina stamina, EntityMana mana) {
            #if FAST_ACTION
                crouchSpeed = 1.0f + (baseStats.Agility * 0.1f);
                walkSpeed = 4.5f + (baseStats.Agility * 0.05f);
                runSpeed  = walkSpeed + (baseStats.Agility * 0.25f);
            # else
                crouchSpeed = 0.1f + (baseStats.Agility * 0.1f);
                walkSpeed = 1.1f + (baseStats.Agility * 0.05f);
                runSpeed = walkSpeed + (baseStats.Agility * 0.25f);
            #endif
            jumpForce = Mathf.Clamp((baseStats.Strength * 0.05f) + (baseStats.Agility * 0.05f), 0.25f, 2.0f);
            naturalArmor = Mathf.Max(0, (baseStats.Agility / 2) - 5);
            meleeDamageBonus = Mathf.Max(0, (baseStats.Strength / 2) - 5);
            maxEncumbrance = (float)(20 + (10 * baseStats.Strength));
            halfEncumbrance = maxEncumbrance / 2f;
            maxSpellDifficulty = baseStats.Intelligence;
            runningCostFactor = 2.0f - ((float)baseStats.Endurance / (float)EntityBaseStats.MAX_SCORE);
            manaCostFactor = 1.5f - ((float)baseStats.Intelligence / (float)EntityBaseStats.MAX_SCORE);
            health.ChangeBaseHealth((20 + (baseStats.Vitality * 5)) * (0.9f + ((float)level * 0.1f)));
            stamina.ChangeBaseStamina((20 + (baseStats.Endurance * 5)) * (0.9f + ((float)level * 0.1f)));
            mana.ChangeBaseMana((20 + (baseStats.Spirit * 5)) * (0.9f + ((float)level * 0.1f)));
        }


        /// <summary>
        /// Used for characters of playable races (human or not); other creatures will have bespoke attribute assignments, or perhaps 
        /// different  specialized methods for special cases.
        /// </summary>
        /// <param name="health">Should be the EntityHealth field of the entity</param>
        /// <param name="stamina">Should be the EntityStamina field of the entity</param>
        /// <param name="mana">Should be the EntityMana field of the entity</param>
        public void DeriveAttributesForHuman(Skills skills, EntityHealth health, EntityStamina stamina, EntityMana mana) {
            #if FAST_ACTION
                walkSpeed = 4.5f + (baseStats.Agility * 0.05f);
                runSpeed  = walkSpeed + ((baseStats.Agility + (skills.Athletics.Adds * 0.5f)) * 0.25f);
                crouchSpeed = Mathf.Min(1.0f + (skills.Stealth * 0.1f, walkSpeed);
            # else
                walkSpeed = 1.1f + (baseStats.Agility * 0.05f);
                runSpeed = walkSpeed + ((baseStats.Agility + (skills.Athletics.Adds * 0.5f)) * 0.25f);
                crouchSpeed = Mathf.Min(0.1f + (skills.Stealth * 0.1f), walkSpeed);
            #endif
            jumpForce = Mathf.Clamp((baseStats.Strength * 0.04f) + (baseStats.Agility * 0.04f) + (skills.Acrobatics.Adds * 0.08f), 0.25f, 2.5f);
            naturalArmor = Mathf.Max(0, (baseStats.Agility / 2) - 5); 
            meleeDamageBonus = Mathf.Max(0, (baseStats.Strength / 2) - 5);
            meleeDamageFactor = skills.Melee.Total / (float)EntityBaseStats.DEFAULT_SCORE;
            maxEncumbrance = 20 + (10 * baseStats.Strength);
            halfEncumbrance = maxEncumbrance / 2f; 
            if((skills.Thaumaturgy.Adds > 0) || (skills.Spellcraft.Adds > 0)) maxSpellDifficulty = skills.Thaumaturgy;
            else maxSpellDifficulty = 0;
            runningCostFactor = 2.0f - ((float)skills.Athletics / EntityBaseStats.MAX_SCORE);
            manaCostFactor = 1.5f - ((float)skills.Spellcraft / (EntityBaseStats.MAX_SCORE + Skill.MAX_SCORE));
            health.ChangeBaseHealth((20 + (baseStats.Vitality * 5)) * (0.9f + (level * 0.1f)));
            stamina.ChangeBaseStamina((20 + (baseStats.Endurance * 5)) * (0.9f + (level * 0.1f)));
            mana.ChangeBaseMana((20 + (baseStats.Spirit * 5)) * (0.9f + (level * 0.1f)));
        }


        public EntityAttributes Copy() {
            EntityAttributes copy = new();
            copy.baseStats.CopyInto(baseStats);
            copy.level = level; // The level of the entity; only used for player character and NPCs of playable races
            copy.crouchSpeed = crouchSpeed; // Movement speed
            copy.walkSpeed = walkSpeed; // Movement speed
            copy.runSpeed = runSpeed; // Movement speed
            copy.jumpForce = jumpForce; //TODO / FIXME: Implement jumping, then determine what this should be:
            copy.naturalArmor = naturalArmor; // Natural, not derived from worn armor
            copy.meleeDamageBonus = meleeDamageBonus; // Bonus damage for melee attacks
            copy.maxEncumbrance = maxEncumbrance;
            copy.halfEncumbrance = halfEncumbrance;
            copy.runningCostFactor = runningCostFactor; // Mostly used for running, and perhaps other movement, not all stamina use
            copy.manaCostFactor = manaCostFactor; // Modifies the cost of casting spells
            copy.maxSpellDifficulty = maxSpellDifficulty;
            copy.damageAdjuster = damageAdjuster; // The type of natural damage adjuster this entity has
            copy.damageModifiers = damageModifiers.Copy();
            return copy;
        }


        public void CopyInto(EntityAttributes other) {
            baseStats.CopyInto(other.baseStats);
            level = other.level; // The level of the entity; only used for player character and NPCs of playable races
            crouchSpeed = other.crouchSpeed; // Movement speed
            walkSpeed = other.walkSpeed; // Movement speed
            runSpeed = other.runSpeed; // Movement speed
            jumpForce = other.jumpForce; //TODO / FIXME: Implement jumping, then determine what this should be:
            naturalArmor = other.naturalArmor; // Natural, not derived from worn armor
            meleeDamageBonus = other.meleeDamageBonus; // Bonus damage for melee attacks
            maxEncumbrance = other.maxEncumbrance;
            halfEncumbrance = other.halfEncumbrance;
            runningCostFactor = other.runningCostFactor; // Mostly used for running, and perhaps other movement, not all stamina use
            manaCostFactor = other.manaCostFactor; // Modifies the cost of casting spells
            maxSpellDifficulty = other.maxSpellDifficulty;
            damageAdjuster = other.damageAdjuster; // The type of natural damage adjuster this entity has
            damageModifiers = other.damageModifiers.Copy();
        }


        public void CopyIntoNew(EntityAttributes source, EntityHealth health, EntityStamina stamina, EntityMana mana) {
            level = 1; 
            baseStats.CopyInto(source.baseStats);
            DeriveAttributesForHuman(health, stamina, mana);
        }


        


    }


}
