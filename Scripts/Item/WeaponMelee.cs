using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class WeaponMelee : ItemEquipt, IWeapon
    {

        [SerializeField] float attackTime;
        [SerializeField] int baseDamage;

        [SerializeField] AbstractAction useAnimation;

        private IAttacker holder;

        public bool busy = false;
        public bool attacking = false;
        public bool queued = false;


        public AbstractAction UseAnimation => useAnimation;


        public void AttackMelee(IAttacker attacker) {
            attacking = true;
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
            EntityLiving living = hit.GetComponent<EntityLiving>();
            if(attacking && (living != null) /*&& (holder != null)*/ && (living != holder)) {
                Debug.Log("Hit " + living.GetName());
                attacking = false; 
            }
            // TODO: Get health, calculate damage, apply modifiers, and apply damage to health (if not null)
            // Hit one enemy, and don't hit over and over if it moves
        }


        // FIXME: These should also have some reference to there user, I think
        //        But, should they use wrap attack, or should one or both be used to draw/sheath the weapon



        public void OnUse(IActor actor) {
            IAttacker attacker = actor as IAttacker;
            if(attacker != null) {
                if(busy) queued = true;
                else {
                    AttackMelee(attacker);
                    PlayUseAnimation(attacker.ActionLayer, attacker.ActionState);
                }
            }
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
            attacking = false;
            if(queued) {
                queued = false;
                OnUse(holder);
            }
            else ReplayEquipAnimation();
        }


        public void OnEquipt(IActor actor) {
            holder = actor as IAttacker;
            PlayEquipAnimation(actor.ActionLayer, actor.ActionState);
        }


        public void PlayEquipAnimation(AnimancerLayer animancer, AnimancerState animState) {
            if((animState == null) || (animState.NormalizedTime >= 1)) {
                animancer.SetMask(useAnimation.mask);
                animState = animancer.Play(equiptAnim);
                animState.NormalizedTime = 0; 
                animState.Events.OnEnd = OnEqipAnimationEnd;
                busy = true;
                attacking = false;
            }
        }


        public void ReplayEquipAnimation() {
            if(holder == null) return;
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
