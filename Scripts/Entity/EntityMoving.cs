using Animancer;
using UnityEngine;
using UnityEngine.AI;


namespace kfutils.rpg {


    [RequireComponent(typeof(NavMeshAgent))][RequireComponent(typeof(CharacterController))]
    public class EntityMoving : EntityLiving
    {
        [SerializeField] protected CharacterController controller;
        [SerializeField] protected MovementSet movementSet;
        [SerializeField] protected Transform eyes;

        protected AnimancerState moveState;

        protected AnimancerLayer moveLayer;

        protected MixerTransition2D moveMixer;
        protected MixerParameterTweenVector2 moveTween;


        // This is to make sure this is never overriden into something harmful.
        protected sealed override void MakePC(string id) { base.MakePC(ID); }

        public NavMeshAgent navAgent;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            base.Start();
            navAgent = GetComponent<NavMeshAgent>();
            moveMixer = movementSet.Walk;
            moveLayer = animancer.Layers[0];
            if (alive)
            {
                moveState = moveLayer.Play(moveMixer);
                moveTween = new MixerParameterTweenVector2(moveMixer.State);
            }
            else
            {
                Die(); // FIXME: Save and restore death animation state
            }
        }


        protected override void Die()
        {  
#if UNITY_EDITOR          
            Debug.Log(GetPersonalName() + " (" + ID + ") " + "Died!");
#endif
            base.Die();
            controller.enabled = false;
            moveLayer = animancer.Layers[0]; // This may not be defined when reloading a scene
            moveLayer.SetMask(deathAnimation.mask);
            moveState = moveLayer.Play(deathAnimation.anim);
            moveState.Time = 0;
        } 


        // Update is called once per frame
        /*protected override void Update()
        {
            
        }*/






    }


}
