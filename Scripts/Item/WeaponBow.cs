using Unity.Entities.UniversalDelegates;
using UnityEngine;


namespace kfutils.rpg {

    public class WeaponBow : ItemEquipt, IWeapon
    {

        // Weapon Fields
        [SerializeField] protected float attackTime;
        [SerializeField] protected DamageSource damage;

        [SerializeField] protected ItemActions useAnimation; 
        [SerializeField] protected int drawCost;
        [SerializeField] protected int holdCost;
        [SerializeField] protected float lauchSpeed;

        [SerializeField] protected float noise = 4.0f; 

        public bool Parriable => false;
        public float MaxRange => lauchSpeed;
        public float MinRange => 3.0f;
        public AbstractAction UseAnimation => useAnimation.Primary;
        public int StaminaCost => drawCost;
        public int PowerAttackCost => holdCost;


        public void AttackMelee(ICombatant attacker) {}
        public void BeBlocked(ICombatant blocker, BlockArea blockArea) {}


        public void AttackRanged(ICombatant attacker, Vector3 direction)
        {
            // TODO!
            throw new System.NotImplementedException();
        }


        public float EstimateDamage(IDamageable victem)
        {
            throw new System.NotImplementedException();
        }


        public float GetAttackSpeed()
        {
            throw new System.NotImplementedException();
        }


        public int GetDamage()
        {
            throw new System.NotImplementedException();
        }


        public void OnEquipt(IActor actor)
        {
            throw new System.NotImplementedException();
        }


        public void OnUnequipt()
        {
            throw new System.NotImplementedException();
        }


        public void OnUse(IActor actor)
        {
            throw new System.NotImplementedException();
        }


        public void OnUseCharged(IActor actor)
        {
            throw new System.NotImplementedException();
        }


        public void PlayEquipAnimation(IActor actor)
        {
            throw new System.NotImplementedException();
        }


        public void PlayUseAnimation(IActor actor)
        {
            throw new System.NotImplementedException();
        }


        private DamageSource CombineDamage(ItemAmmo itemAmmo) => damage.Combine(itemAmmo.Damage);



    }

}
