using UnityEngine;


namespace kfutils.rpg {

    public interface IWeapon : IUsable 
    {
        int GetDamage();
        float GetAttackSpeed();

        void AttackMelee(IAttacker attacker);
        
        void AttackRanged(IAttacker attacker, Vector3 direction);

        void Sheath();
        
        void Draw();

        
    }

}
