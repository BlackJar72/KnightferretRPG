using System.Collections;
using Animancer;
using UnityEngine;


namespace kfutils.rpg
{

    public class ItemShield : ItemEquipt, IBlockItem
    {

        [SerializeField] ItemActions useAnimation;

        [SerializeField] float blockAmount;
        [SerializeField] float stability;
        [SerializeField] float parryWindow;
        [SerializeField] AudioSource audioSource;

        private ICombatant holder;
        private BlockArea blockArea;

        private bool blocking = false;
        private float blockStart = float.NegativeInfinity;



        public delegate void EventAction();

        public AbstractAction UseAnimation => useAnimation.Secondary;

        public float BlockAmount => blockAmount;
        public float Stability => stability;
        public float ParryWindow => parryWindow;

        public int StaminaCost => 0;
        public int PowerAttackCost => 0;


        public void StartBlock()
        {
            if (SetBlockArea() != null)
            {
                blocking = true;
                blockArea = holder.GetBlockArea();
                blockStart = Time.time; // FIXME: Use session independent world time
                PCActing pc = holder as PCActing;
                if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.low);
                PlayUseAnimation(holder);
            }
        }


        public void EndBlock()
        {
            blocking = false;
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


        public void OnUse(IActor actor)
        {
            //throw new System.NotImplementedException();
        }


        public void OnUseCharged(IActor actor)
        {
            OnUse(actor);
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
        

    }


}
