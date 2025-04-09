using System.Collections;
using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class WeaponMelee : ItemEquipt, IWeapon
    {

        [SerializeField] float attackTime;
        [SerializeField] int baseDamage;
        [SerializeField] BoxCollider attackCollider;

        [SerializeField] AnimationClip useAnimation;

        public AnimationClip UseAnimation => useAnimation;

        // TODO: Some reference to the animation (Animancer; but if not labels for mechanim)

        private AnimancerState animState;


        public void AttackMelee(IAttacker attacker) {
            attackCollider.enabled = true;
            // TODO: Initiate attack animation            
        }


        public void AttackRanged(IAttacker attacker, Vector3 direction) {
            Debug.Log("Trying to perform raged attack with melee weapon " + prototype.Name);
            throw new System.NotImplementedException();
        }


        public float GetAttackSpeed() {
            return attackTime;
        }


        public int GetDamage() {
            return baseDamage;
        }


        void OnTriggerEnter(Collider other) {
            GameObject hit = other.gameObject;
            // TODO: Get health, calculate damage, apply modifiers, and apply damage to health (if not null)
            attackCollider.enabled = false; // Hit one enemy, and don't hit over and over if it moves
        }


        // FIXME: These should also have some reference to there user, I think
        //        But, should they use wrap attack, or should one or both be used to draw/sheath the weapon


        public void OnActivate() {
            throw new System.NotImplementedException();
        }


        public void OnUse() {
            throw new System.NotImplementedException();
        }


        public void PlayeUseAnimation(AnimancerComponent animancer) {
            if((animState == null) || (animState.NormalizedTime >= 1)) {
                animState = animancer.Play(useAnimation);
                animState.Time = 0; 
            }    
        }


    }

}
