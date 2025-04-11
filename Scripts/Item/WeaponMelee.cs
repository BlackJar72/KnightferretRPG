using System.Collections;
using Animancer;
using Unity.VisualScripting;
using UnityEngine;


namespace kfutils.rpg {

    public class WeaponMelee : ItemEquipt, IWeapon
    {

        [SerializeField] float attackTime;
        [SerializeField] int baseDamage;
        [SerializeField] BoxCollider attackCollider;

        [SerializeField] AbstractAction useAnimation;

        public IAttacker holder;

        public IActor Holder { get => holder; set {holder = value as IAttacker; } }

        private bool busy = false;


        public AbstractAction UseAnimation => useAnimation;

        // TODO: Some reference to the animation (Animancer; but if not labels for mechanim)


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


        public void OnUse(IActor actor) {
            IAttacker attacker = actor as IAttacker;
            if(attacker != null) AttackMelee(attacker);
        }


        public void PlayUseAnimation(AnimancerLayer animancer, AnimancerState animState) {
            if((animState == null) || (!busy)) {
                animancer.SetMask(useAnimation.mask);
                animState = animancer.Play(useAnimation.anim);
                animState.Time = 0; 
                busy = true;
                animState.Events.OnEnd = OnUseAnimationEnd;
            }
        }


        private void OnUseAnimationEnd() {
            busy = false;
            ReplayEquipAnimation();
        }


        public void PlayEquipAnimation(AnimancerLayer animancer, AnimancerState animState) {
            if((animState == null) || (animState.NormalizedTime >= 1)) {
                animancer.SetMask(useAnimation.mask);
                animState = animancer.Play(equiptAnim);
                animState.NormalizedTime = 0; 
                busy = true;
                animState.Events.OnEnd = OnEqipAnimationEnd;
            }
        }


        public void ReplayEquipAnimation() {
            AnimancerLayer animancer = holder.ActionLayer;
            AnimancerState animState = holder.ActionState;
            if((animState == null) || (animState.NormalizedTime >= 1)) {
                animancer.SetMask(useAnimation.mask);
                animState = animancer.Play(equiptAnim);
                animState.NormalizedTime = 0; 
                busy = false;
            }
        }


        private void OnEqipAnimationEnd() {
            busy = false;
        }


        public void Sheath() {
            // TODO:  Switch animation layer 1 to use moveState (or to have an empty mask?)
            //        Figure out the dynamic mixer so you can do this right.   
            throw new System.NotImplementedException();
        }


        public void Draw() {
            // TODO: Have the character draw and hold the weapon.
            //       Question -- do I need separate drawn and hold animations? Or just idle (holding), and the transition is the draw?
            throw new System.NotImplementedException();
        }


    }


}
