using Unity.Entities.UniversalDelegates;
using UnityEngine;


namespace kfutils.rpg
{

    public interface ICombatant : IHaveName, IActor, IDamageable
    {
        // TODO: Add relevant parameters to these methods, once we know what they are (i.e., have created entities)
        /// <summary>
        /// Called when attacking with the weapon at close range.
        /// </summary>
        /// <param name="weapon"></param>
        public void MeleeAttack(IWeapon weapon);

        // TODO: Add other relevant parameters
        /// <summary>
        /// Called when shooting a ranged weapon (or using a ranged special attack?)
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="direction"></param>
        public void RangedAttack(IWeapon weapon, Vector3 direction);

        /// <summary>
        /// Called when blocking
        /// </summary>
        public void Block(ItemEquipt item);

        /// <summary>
        /// Called when blocking
        /// </summary>
        public void EndBlock(ItemEquipt item);

        /// <summary>
        /// Called when shiled or other blocking items is hit.
        /// Handles damage in place of TakeDamage, amoung other things.
        /// </summary>
        /// <param name="damage"></param>
        public void BlockDamage(Damages damage, BlockArea blockArea);

        /// <summary>
        /// Called when shiled or other blocking items is hit.
        /// Handles damage in place of TakeDamage, amoung other things.
        /// </summary>
        /// <param name="damage"></param>
        public void BlockDamage(DamageData damage, BlockArea blockArea);

        /// <summary>
        /// Ready the weapon for combat
        /// </summary>
        /// <param name="weapon"></param>
        public void DrawWeapon(IWeapon weapon);

        /// <summary>
        /// Put away the weapon (or return to non-fighting stance for natural weapons)
        /// </summary>
        /// <param name="weapon"></param>
        public void SheatheWeapon(IWeapon weapon);

        /// <summary>
        /// Switch to a differet weapon.  Usually this will be a sequence of sheathing current weapon and drawing new weapon.
        /// </summary>
        /// <param name="weapon"></param>
        public void SwitchWeapon(IWeapon currentWeapon, IWeapon newWeapon);

        /// <summary>
        /// Returns the BlockArea of the Combatant
        /// </summary>
        public BlockArea GetBlockArea();

    }

}