using System;
using Animancer;
using UnityEngine;
using UnityEngine.AI;


namespace kfutils.rpg {


    [System.Serializable]
    public enum MoveType
    {
        idle = 0,
        crouch = 1,
        walk = 2,
        run = 3
    }


    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CharacterController))]
    public class EntityMoving : EntityLiving
    {
        [SerializeField] protected CharacterController controller;
        [SerializeField] protected MovementSet movementSetPrototype;
        [SerializeField] protected Transform eyes;
        [SerializeField] protected NavMeshAgent navAgent;

        protected MovementSet movementSet;

        protected AnimancerState moveState;
        protected AnimancerLayer moveLayer;

        protected MixerTransition2D moveMixer;
        protected MixerParameterTweenVector2 moveTween;

        [SerializeField] protected Vector3 destination;
        [SerializeField] protected MoveType moveType;


        // This is to make sure this is never overriden into something harmful.
        protected sealed override void MakePC(string id) { base.MakePC(ID); }


        protected override void Awake()
        {
            base.Awake();
            // This is required for the animations to work on multiple characters,
            // and no CreateInstance is not appropriate as I need to clone the give 
            // object, not create a totally new one.
            movementSet = Instantiate(movementSetPrototype);
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            base.Start();
            navAgent = GetComponent<NavMeshAgent>();
            moveMixer = movementSet.Walk;
            moveLayer = animancer.Layers[0];
            moveState = moveLayer.Play(moveMixer);
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


        protected override void Update()
        {
            if (alive) Move();
        }


        protected virtual void Move()
        {
            base.Update();
            if (ShouldStop())
            {
                SetDirectionalParameters(Vector2.zero);
            }
            else
            {
                SetDirectionalParameters(Vector2.up);
            }
        }


        protected bool ShouldStop()
        {
            return (moveType == MoveType.idle) || !navAgent.isActiveAndEnabled || navAgent.isStopped
                    || (navAgent.remainingDistance <= navAgent.stoppingDistance) || (navAgent.velocity.sqrMagnitude == 0);
        }


        protected override void Die()
        {
#if UNITY_EDITOR
            Debug.Log(GetPersonalName() + " (" + ID + ") " + "Died!");
#endif
            base.Die();
            navAgent.enabled = false;
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


        public void SetDestination(Vector3 to, float stopDist = 0.0f)
        {
            destination = to;
            navAgent.SetDestination(destination);
            navAgent.stoppingDistance = stopDist;
        }


        protected void SetMoveType(MoveType type)
        {
            moveType = type;
            switch (moveType)
            {
                case MoveType.idle:
                    navAgent.speed = 0;
                    moveMixer = movementSet.Walk;
                    SetDirectionalParameters(Vector2.zero);
                    break;
                case MoveType.crouch:
                    navAgent.speed = attributes.crouchSpeed;
                    moveMixer = movementSet.Crouch;
                    // TODO: Set DirectionalMixerState parameters
                    break;
                case MoveType.walk:
                    navAgent.speed = attributes.walkSpeed;
                    moveMixer = movementSet.Walk;
                    // TODO: Set DirectionalMixerState parameters
                    break;
                case MoveType.run:
                    navAgent.speed = attributes.crouchSpeed;
                    moveMixer = movementSet.Run;
                    // TODO: Set DirectionalMixerState parameters
                    break;
                default:
                    navAgent.speed = 0;
                    moveMixer = movementSet.Walk;
                    // TODO: Set DirectionalMixerState parameters
                    break;
            }
        }


        protected void SetDirectionalParameters(Vector2 movement)
        {
            if (moveMixer.State is DirectionalMixerState dms)
            {
                dms.Parameter = Vector2.MoveTowards(dms.Parameter, movement, 10 * Time.deltaTime);
            }
        }





    }


}
