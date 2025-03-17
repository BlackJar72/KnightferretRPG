using UnityEditor.UIElements;
using UnityEngine;


namespace kfutils.rpg
{

    public interface IWeaponUser : IAttacker
    {
        // TODO: Add relevant parameters to these methods, once we know what they are (i.e., have created entities and weapons)
       public void MeleeAttack();
       public void RangedAttack();
       public void SpecialAttack();
       public void Block();
    }

}