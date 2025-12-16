using System.Collections;
using Animancer;
using UnityEngine;


namespace kfutils.rpg
{

    public class ItemShield : ItemEquipt, IBlockItem
    {

        [SerializeField] ItemActions useAnimation;
        [SerializeField] AbstractAction bashAnimation;

        [SerializeField] float blockAmount;
        [SerializeField] float stability;
        [SerializeField] float parryWindow;
        [SerializeField] int bashCost;
        [SerializeField] DamageSource bashDamage;
        [SerializeField] AudioSource audioSource;

        private ICombatant holder;
        private BlockArea blockArea;
        private bool bashing;



        public delegate void EventAction();

        public AbstractAction UseAnimation => useAnimation.Secondary;

        public float BlockAmount => blockAmount;
        public float Stability => stability;
        public float ParryWindow => parryWindow;

        public int StaminaCost => 0;
        public int PowerAttackCost => 0;

        public bool Parriable => throw new System.NotImplementedException();


        protected void Awake()
        {
            bashAnimation = Instantiate(bashAnimation);
        }


        public void StartBlock()
        {
            bashing = false;
            if (SetBlockArea() != null)
            {
                blockArea = holder.GetBlockArea();
                PCActing pc = holder as PCActing;
                if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.low);
                PlayUseAnimation(holder);
            }
        }


        public void EndBlock()
        {
            holder.StopAction();
            PCActing pc = holder as PCActing;
            if (pc != null) StartCoroutine(ReturnArmsToNormal(pc));
        }


        public ClipTransition GetBlockAnimation() => useAnimation.Primary.anim;


        public void OnEquipt(IActor actor)
        {
            holder = actor as ICombatant;
            blockArea = holder.GetBlockArea();
            if (blockArea != null)
            {
                blockArea.blockItem = this;
            }
        }


        public BlockArea SetBlockArea()
        {
            blockArea = holder.GetBlockArea();
            return blockArea;
        }


        public void OnUnequipt()
        {
            //throw new System.NotImplementedException();
        }


        public void PlayEquipAnimation(IActor user)
        {/*
            if (user.ActionState.NormalizedTime >= 1)
            {
                user.PlayAction(useAnimation.mask, equiptAnim, OnEqipAnimationEnd, 0);
                user.ActionState.Events.OnEnd = OnEqipAnimationEnd;
            }
        */
        }


        public void PlayUseAnimation(IActor user)
        {
            user.PlayAction(useAnimation.Primary.mask, useAnimation.Primary.GetSequential(0));
        }


        public void OnEqipAnimationEnd()
        {
            EndBlock();
        }


        public void BeHit()
        {
            useAnimation.PrimarySound.Play(audioSource);
        }


        public void BeParried()
        {
            useAnimation.SecondarySound.Play(audioSource);
        }


        public IEnumerator ReturnArmsToNormal(PCActing pc)
        {
            yield return new WaitForSeconds(0.1f);
            pc.SetArmsPos(PCActing.ArmsPos.high);
        }


        /**************************************************************************************************************/
        /*                                            BASHING METHODS                                                 */
        /**************************************************************************************************************/


        public void OnUse(IActor actor)
        {
            ICombatant attacker = actor as ICombatant;
            if (!bashing && (attacker != null))
            {
                blockArea.LowerBlock();                
                AbstractAction action = useAnimation.Primary;
                AnimancerState attackState = attacker.PlayAction(useAnimation.Primary.mask, bashAnimation.anim);
             
                PCActing pc = attacker as PCActing;
                if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.high);
                attackState.Events.SetCallback(0, TryHit);
                attackState.Events.OnEnd = EndBash;
                bashing = true;            
            }
        }


        public void OnUseCharged(IActor actor)
        {
            OnUse(actor);
        }


        public int GetDamage() => bashDamage.BaseDamage;


        private void TryHit()
        {
            AimParams aim;
            holder.GetAimParams(out aim);
            RaycastHit target;
            if (Physics.Raycast(aim.from, aim.toward, out target, 1.25f, GameConstants.attackableLayer))
            {
                if (target.collider != null)
                {
                    GameObject hitObject = target.collider.gameObject;
                    BlockArea ba = hitObject.GetComponent<BlockArea>();
                    if (ba != null)
                    {
                        BeHit();
                        EndBash();
                        return;
                    }
                    IDamageable damageable = hitObject.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        if (damageable is EntityActing enemy) enemy.ToggleStagger(1.5f);
                        if (damageable is EntityHitbox hitbox)
                        {
                            if((hitbox.GetEntity is EntityActing actor) && (!actor.IsStunned()))
                            {
                                actor.ToggleStagger(1.5f);
                            }
                        }
                        bashDamage.DoDamage(holder, null, damageable);
                    }
                }
            }
        }


        private void EndBash()
        {
            bashing = false;
            if (holder.IsBlocking)
            {
                blockArea.RestoreBlock(this);
                StartBlock();
            }
        }


    }


}
