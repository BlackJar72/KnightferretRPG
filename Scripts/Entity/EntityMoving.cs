using Animancer;
using UnityEngine;
using UnityEngine.AI;


namespace kfutils.rpg {


    [RequireComponent(typeof(NavMeshAgent))]
    public class EntityMoving : EntityLiving
    {
        
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
            moveState = moveLayer.Play(moveMixer);
            moveTween = new MixerParameterTweenVector2(moveMixer.State);
        }


        // Update is called once per frame
        /*protected override void Update()
        {
            
        }*/


        



    }


}
