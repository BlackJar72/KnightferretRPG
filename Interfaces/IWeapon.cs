using Unity.Entities.UniversalDelegates;
using UnityEngine;


namespace kfutils.rpg
{

    public interface IWeapon
    {
        int GetDamage();
        float GetAttackSpeed();

        void AttackMelee(IWeaponUser attacker);
        
        void AttackRanged(IWeaponUser attacker, Vector3 direction);

        
    }

}
