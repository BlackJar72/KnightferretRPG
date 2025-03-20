using Unity.Entities.UniversalDelegates;
using UnityEngine;


namespace kfutils.rpg
{

    public interface IAttacker : IHaveName
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
        public void Block();

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
        /// Called when this attacker's melee attack is blocked by a target.
        /// </summary>
       public void AttackBlocked(); 

    }

}