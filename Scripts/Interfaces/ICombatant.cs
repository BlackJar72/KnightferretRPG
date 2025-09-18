using Unity.Entities.UniversalDelegates;
using UnityEngine;


namespace kfutils.rpg
{

    public interface ICombatant : IHaveName, IActor, IDamageable
    {

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
        /// Returns the BlockArea of the Combatant
        /// </summary>
        public BlockArea GetBlockArea();

        /// <summary>
        /// Is the combatant currently blocking?
        /// </summary>
        public bool IsBlocking { get;  }

    }

}