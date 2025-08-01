using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class WeaponMelee : ItemEquipt, IWeapon
    {

        [SerializeField] float attackTime;
        [SerializeField] DamageSource damage;

        [SerializeField] ItemActions useAnimation;
        [SerializeField] int attackCost;

        private ICombatant holder;
        private Collider hitCollider;

        private bool busy = false;
        private bool attacking = false;
        private bool queued = false;
        private int attack = 0;
        AnimancerState attackState;

        public delegate void EventAction();


        public AbstractAction UseAnimation => useAnimation.Primary;

        public int StaminaCost => attackCost;


        public void AttackMelee(ICombatant attacker) {
            attacking = true;        
        }


        public void AttackRanged(ICombatant attacker, Vector3 direction) {
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
            if(attacking && (damageable != null) && (damageable.GetEntity != holder)) {
                damage.DoDamage(holder, damageable);
                attacking = false; 
            }
        }


        // FIXME: These should also have some reference to there user, I think
        //        But, should they use wrap attack, or should one or both be used to draw/sheath the weapon



        public void OnUse(IActor actor) {
            ICombatant attacker = actor as ICombatant;
            if(attacker != null) {
                if (busy) queued = true;
                else
                {
                    AttackMelee(attacker);
                    PlayUseAnimation(actor);
                }
            }
        }


        public void PlayUseAnimation(IActor attacker) {
            if (!busy)
            {
                if (attacker is PCActing)
                {
                    attackState = attacker.PlayAction(useAnimation.Primary.mask, useAnimation.Primary.GetSequential(attack), OnUseAnimationEnd, 0, attackTime);
                }
                else
                {
                    attackState = attacker.PlayAction(useAnimation.Primary.mask, useAnimation.Primary.GetRandom(attack), OnUseAnimationEnd, 0, attackTime);
                }
                attackState.Events.AddCallback(0, OnAttackStart);
                attackState.Events.AddCallback(1, OnAttackEnd);
                busy = true;
            }
        }


        public void OnUseAnimationEnd() {
            busy = false;
            attacking = false;
            if (queued && (hitCollider != null))
            {
                queued = false;
                attack = (++attack) % useAnimation.Primary.number;
                OnUse(holder);
            }
            else
            {
                attack = 0;
                ReplayEquipAnimation();
                hitCollider.enabled = false;
            }
            attackState.Events.RemoveCallback(0, OnAttackStart);
            attackState.Events.RemoveCallback(1, OnAttackEnd);
        }


        public void OnEquipt(IActor actor) {
            holder = actor as ICombatant;
            hitCollider = GetComponent<Collider>();
            hitCollider.enabled = false;
            if(actor.ActionState != null) PlayEquipAnimation(actor);
        }


        public void OnAttackStart()
        {
            hitCollider.enabled = true;  
        }


        public void OnAttackEnd()
        {
            hitCollider.enabled = false; 
        }


        public void OnUnequipt()
        {
            holder.RemoveEquiptAnimation();
            // TODO: Reset the holder's animation to default and do general clean-up
        }


        public void PlayEquipAnimation(IActor user) {
            if(user.ActionState.NormalizedTime >= 1) {
                user.PlayAction(useAnimation.Primary.mask, equiptAnim, OnEqipAnimationEnd, 0);
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
                animancer.SetMask(useAnimation.Primary.mask);
                animState = animancer.Play(equiptAnim);
                animState.NormalizedTime = 0; 
                busy = false;
            }
        }


        public void OnEqipAnimationEnd() {
            busy = false;
        }


        public void BeBlocked(ICombatant blocker, BlockArea blockArea)
        {
            if (attacking && (blocker != null) && (blocker.GetEntity != holder))
            {
                hitCollider.enabled = false;
                DamageData dmg = damage.GetDamage(holder, blocker);
                blocker.BlockDamage(dmg, blockArea);
                if (holder is EntityActing actor) actor.DelayFurtherAction(2.0f);
                attacking = false; 
                PlayEquipAnimation(holder);
            }
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
