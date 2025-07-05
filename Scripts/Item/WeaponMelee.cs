using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class WeaponMelee : ItemEquipt, IWeapon
    {

        [SerializeField] float attackTime;
        [SerializeField] DamageSource damage;

        [SerializeField] AbstractAction useAnimation;

        [SerializeField] AbstractAction npcAnimation;
        [SerializeField] int attackCost;

        private IAttacker holder;
        private Collider hitCollider;

        private bool busy = false;
        private bool attacking = false;
        private bool queued = false;
        private int attack = 0;

        public delegate void EventAction();


        public AbstractAction UseAnimation => useAnimation;

        public int StaminaCost => attackCost;


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
            return damage.BaseDamage;
        }


        void OnTriggerEnter(Collider other) {
            #if UNITY_EDITOR
            //Debug.Log("Hit " + other.gameObject.name);
            #endif
            GameObject hit = other.gameObject;
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if(attacking && (damageable != null) && (damageable != holder)) {
                damage.DoDamage(holder, damageable);
                attacking = false; 
            }
        }


        // FIXME: These should also have some reference to there user, I think
        //        But, should they use wrap attack, or should one or both be used to draw/sheath the weapon



        public void OnUse(IActor actor) {
            IAttacker attacker = actor as IAttacker;
            if(attacker != null) {
                if (busy) queued = true;
                else
                {
                    hitCollider.enabled = true;
                    AttackMelee(attacker);
                    PlayUseAnimation(actor);
                }
            }
        }


        public void PlayUseAnimation(IActor attacker) {
              if(!busy) {
                if (attacker is PCActing)
                {
                    attacker.PlayAction(useAnimation.mask, useAnimation.GetSequential(ref attack), OnUseAnimationEnd, 0, attackTime);
                }
                else
                {
                    attacker.PlayAction(useAnimation.mask, npcAnimation.GetSequential(ref attack), OnUseAnimationEnd, 0, attackTime);
                }
                busy = true;
            }
        }


        public void OnUseAnimationEnd() {
            busy = false;
            attacking = false;
            if (queued)
            {
                queued = false;
                attack++;
                OnUse(holder);
            }
            else
            {
                attack = 0;
                ReplayEquipAnimation();
                hitCollider.enabled = false;
            }
        }


        public void OnEquipt(IActor actor) {
            holder = actor as IAttacker;
            hitCollider = GetComponent<Collider>();
            hitCollider.enabled = false;
            if(actor.ActionState != null) PlayEquipAnimation(actor);
        }


        public void OnUnequipt() {
            holder.RemoveEquiptAnimation();
            // TODO: Reset the holder's animation to default and do general clean-up
        }


        public void PlayEquipAnimation(IActor user) {
            if(user.ActionState.NormalizedTime >= 1) {
                user.PlayAction(useAnimation.mask, equiptAnim, OnEqipAnimationEnd, 0);
                busy = true;
                attacking = false;
                user.ActionState.Events.OnEnd = OnEqipAnimationEnd;
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


        public void OnEqipAnimationEnd() {
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
