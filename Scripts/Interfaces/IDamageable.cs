using Unity.Entities.UniversalDelegates;
using UnityEngine;


namespace kfutils.rpg
{

    public class DamageData {
        public Damages damage;
        public IAttacker attacker;
        public IWeapon weapon;
    }




    public interface IDamageable
    {
        
    }

}