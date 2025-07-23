using UnityEngine;


namespace kfutils.rpg {

    public interface IWeapon : IUsable 
    {
        int GetDamage();
        
        float GetAttackSpeed();

        void AttackMelee(ICombatant attacker);
        
        void AttackRanged(ICombatant attacker, Vector3 direction);

        void Sheath();
        
        void Draw();
        
    }

}
