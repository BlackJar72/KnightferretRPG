using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class WeaponMelee : ItemEquipt, IWeapon, IBlockItem
    {

        // Weapon Fields

        [SerializeField] float attackTime;
        [SerializeField] DamageSource damage;

        [SerializeField] ItemActions useAnimation;
        [SerializeField] ItemActions blockAnimation;
        [SerializeField] int attackCost;
        [SerializeField] int powerAttackCost;
        [SerializeField] bool parriable = true;
        [SerializeField] float minRange = 0.1f;
        [SerializeField] float maxRange = 1.5f;
        [SerializeField] float noise = 4.0f; 

        private ICombatant holder;
        private Collider hitCollider;

        private bool busy = false;
        private bool attacking = false;
        private bool queued = false;
        private int attack = 0;
        private AnimancerState attackState;

        // Blocking Fields

        [SerializeField] float blockAmount;
        [SerializeField] float stability;
        [SerializeField] float parryWindow;
        [SerializeField] AudioSource audioSource;

        private BlockArea blockArea;

        private bool blocking = false;

        private float damageFactor; // For normal vs power attacks


        public delegate void EventAction();


        public AbstractAction UseAnimation => useAnimation.Primary;

        public float AttackTime => attackTime;
        public int StaminaCost => attackCost;
        public int PowerAttackCost => powerAttackCost;
        public bool Parriable => parriable;

        public float BlockAmount => blockAmount;
        public float Stability => stability;
        public float ParryWindow => parryWindow;
        public DamageSource DamagerSrc => damage;


        /*******************************************************************************************************************************/
        /*                                     GENERAL / WEAPON / ATTACKING METHODS                                                    */
        /*******************************************************************************************************************************/


        protected void Awake()
        {
            useAnimation = Instantiate(useAnimation);
        }


        public void AttackMelee(ICombatant attacker)
        {
            attacking = true;
        }


        public void AttackRanged(ICombatant attacker, Vector3 direction)
        {
            Debug.LogError("Trying to perform raged attack with melee weapon " + prototype.Name);
            throw new System.NotImplementedException();
        }


        public float GetAttackSpeed()
        {
            return attackTime;
        }


        public int GetDamage()
        {
            return damage.BaseDamage;
        }


        void OnTriggerEnter(Collider other)
        {
#if UNITY_EDITOR
            //Debug.Log("Hit " + other.gameObject.name);
#endif
            GameObject hit = other.gameObject;
            IDamageable damageable = hit.GetComponent<IDamageable>();
#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
            if (attacking && (damageable != null) && (damageable.GetEntity != holder))
            {
                if (damageable.InParriedState()) damageFactor += 1.0f;
                if (damageable.IsSurprised(holder)) damageFactor += 1.5f;
                damage.DoDamage(holder, this, damageable, damageFactor);
                //attacking = false;
                OnAttackEnd();
            }
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
        }


        // FIXME: These should also have some reference to there user, I think
        //        But, should they use wrap attack, or should one or both be used to draw/sheath the weapon



        public void OnUse(IActor actor)
        {
            damageFactor = 1.0f;
            ICombatant attacker = actor as ICombatant;
            if (attacker != null)
            {
                if (busy) queued = true;
                else
                {
                    AttackMelee(attacker);
                    PlayUseAnimation(actor);
                }
            }
        }


        public void OnUseCharged(IActor actor)
        {
            damageFactor = 1.5f;
            ICombatant attacker = actor as ICombatant;
            if (attacker != null)
            {
                if (busy) queued = true;
                else
                {
                    AttackMelee(attacker);
                    PlayUseAnimation(actor);
                }
            }
        }


        public void PlayUseAnimation(IActor attacker)
        {
            if (!busy)
            {
                AbstractAction action;
                if (damageFactor > 1.1f)
                {
                    action = useAnimation.Secondary;
                    useAnimation.SecondarySound.Play(audioSource);
                }
                else
                {
                    action = useAnimation.Primary;
                    useAnimation.PrimarySound.Play(audioSource);

                }

                if (attacker is PCActing)
                {
                    attackState = attacker.PlayAction(useAnimation.Primary.mask, action.GetSequential(attack), OnUseAnimationEnd, 0, attackTime);
                }
                else
                {
                    attackState = attacker.PlayAction(useAnimation.Primary.mask, action.GetRandom(attack), OnUseAnimationEnd, 0, attackTime);
                }

                PCActing pc = attacker as PCActing;
                if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.high);
                attackState.Events.SetCallback(0, OnAttackStart);
                attackState.Events.SetCallback(1, OnAttackEnd);
                busy = true;
            }
        }


        public void OnUseAnimationEnd()
        {
            busy = false;
            //attacking = false;
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
                if(hitCollider != null) hitCollider.enabled = false;
            }
            OnAttackEnd();
        }


        public void OnEquipt(IActor actor)
        {
            holder = actor as ICombatant;
            hitCollider = GetComponent<Collider>();
            hitCollider.enabled = false;
            if (actor.ActionState != null) PlayEquipAnimation(actor);
            PCActing pc = actor as PCActing;
            if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.high);
        }


        public void OnAttackStart()
        {
            if(hitCollider != null) hitCollider.enabled = true;
        }


        public void OnAttackEnd()
        {
            if(hitCollider != null) hitCollider.enabled = false;
            // This must be done at the end of the attack, otherwise stealth/surprise attack would be impossible 
            if(attacking && (holder is ISoundSource soundSource)) soundSource.MakeSound(noise, SoundType.General);
            attacking = false;
        }


        public void OnUnequipt()
        {
            holder.RemoveEquiptAnimation();
            if (blocking) EndBlock();
        }


        public void PlayEquipAnimation(IActor user)
        {
            if (user.ActionState.NormalizedTime >= 1)
            {
                user.PlayAction(useAnimation.Primary.mask, equiptAnim, OnEqipAnimationEnd, 0);
                busy = true;
                attacking = false;
                user.ActionState.Events.OnEnd = OnEqipAnimationEnd;
            }
        }


        public void ReplayEquipAnimation()
        {
            if (holder == null) return;
            AnimancerLayer animancer = holder.ActionLayer;
            AnimancerState animState = holder.ActionState;
            if ((animState == null) || (animState.NormalizedTime >= 1))
            {
                animancer.SetMask(useAnimation.Primary.mask);
                animState = animancer.Play(equiptAnim);
                animState.NormalizedTime = 0;
                busy = false;
            }
        }


        public void OnEqipAnimationEnd()
        {
            busy = false;
        }


        public void BeBlocked(ICombatant blocker, BlockArea blockArea)
        {
#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
            if (attacking && (blocker != null) && (blocker.GetEntity != holder))
            {
                //Debug.Log("public void BeBlocked(ICombatant blocker, BlockArea blockArea)");
                hitCollider.enabled = false;
                DamageData dmg = damage.GetDamage(holder, this, blocker);
                blocker.BlockDamage(dmg, blockArea);
                //if (holder is EntityActing actor) actor.DelayFurtherAction(1.0f, false);
                //attacking = false;
                OnAttackEnd();
                PlayEquipAnimation(holder);
            }
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
        }


        /*******************************************************************************************************************************/
        /*                                     BLOCKING / PARRYING METHODS                                                             */
        /*******************************************************************************************************************************/


        public void StartBlock()
        {
            if (SetBlockArea() != null)
            {
                blocking = true;
                blockArea.blockItem = this;
                hitCollider.enabled = false; // Should already be disabled, but just in case
                holder.PlayAction(blockAnimation.Primary.mask, blockAnimation.Primary.GetSequential(0));
            }
        }


        public void EndBlock()
        {
            blocking = false;
            holder.StopAction();
        }


        public BlockArea SetBlockArea()
        {
            blockArea = holder.GetBlockArea();
            return blockArea;
        }


        public void BeHit()
        {
            blockAnimation.PrimarySound.Play(audioSource);
        }


        public void BeParried()
        {
            blockAnimation.SecondarySound.Play(audioSource);
        }


        public ClipTransition GetBlockAnimation() => blockAnimation.Primary.anim;


        /*******************************************************************************************************************************/
        /*                                     BLOCKING / PARRYING METHODS                                                             */
        /*******************************************************************************************************************************/


        public float MaxRange => maxRange;
        public float MinRange => minRange;

        public float EstimateDamage(IDamageable victem)
        {
            return damage.EstimateDamage(victem);
        }




    }


}
