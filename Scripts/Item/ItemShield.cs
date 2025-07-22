using UnityEngine;


namespace kfutils.rpg
{

    public class ItemShield : ItemEquipt, IBlockItem
    {

        [SerializeField] AbstractAction useAnimation;

        [SerializeField] AbstractAction npcAnimation;

        [SerializeField] float blockAmount;
        [SerializeField] float stability;
        [SerializeField] float parryWindow;

        private IAttacker holder;
        private Collider blockCollider;

        private bool blocking = false;
        private float blockStart = float.NegativeInfinity;



        public delegate void EventAction();

        public AbstractAction UseAnimation => useAnimation;

        public float BlockAmount => blockAmount;
        public float Stability => stability;
        public float ParryWindow => parryWindow;

        public int StaminaCost => 0;


        public void StartBlock()
        {
            blocking = true;
            blockStart = Time.time; // FIXME: Use session independent world time
            // TODO: Actually setup blocks, importantly set up the block collider
        }


        public void EndBlock()
        {
            blocking = false;
            // TODO: Actually take down blocks, importantly deactivate the block collider
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


        public void PlayEquipAnimation(IActor user)
        {
            if (user.ActionState.NormalizedTime >= 1)
            {
                user.PlayAction(useAnimation.mask, equiptAnim, OnEqipAnimationEnd, 0);
                user.ActionState.Events.OnEnd = OnEqipAnimationEnd;
            }
        }


        public void PlayUseAnimation(IActor actor)
        {
            throw new System.NotImplementedException();
            // HELP!  I need to have the animation play, pause on block, and reverse when the block is ended.
        }


        public void OnEqipAnimationEnd() {
            EndBlock();
        }
        


    }


}
