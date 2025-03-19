using Unity.Entities.UniversalDelegates;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Video;


namespace kfutils.rpg
{

    public interface IWeaponUser : IAttacker
    {
        // TODO: Add relevant parameters to these methods, once we know what they are (i.e., have created entities and weapons)
       public void MeleeAttack(IWeapon weapon);
       public void RangedAttack(IWeapon weapon, Vector3 direction);
       public void SpecialAttack(); // FIXME: Not sure I really need this; may be removed
       public void Block();

       public void DrawWeapon(IWeapon weapon);
       public void SheatheWeapon(IWeapon weapon);
       public void SwitchWeapon(IWeapon weapon);
    }

}