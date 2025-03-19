using Unity.Entities.UniversalDelegates;
using UnityEngine;


namespace kfutils.rpg
{

    public interface IAttacker : IHaveName
    {
        // TODO: Add relevant parameters to these methods, once we know what they are (i.e., have created entities)
        public void UnarmedAttack(IWeapon weapon); // Unarmed attacks will be treated as "weapons" for the purpose of defining properties


        /// <summary>
        /// Called when this attacker's melee attack is blocked by a target.
        /// </summary>
       public void AttackBlocked(); 

    }

}