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
        [SerializeField] protected MovementSet movementSet;
        [SerializeField] protected Transform eyes;
        [SerializeField] protected NavMeshAgent navAgent;

        protected AnimancerState moveState;
        protected AnimancerLayer moveLayer;

        protected MixerTransition2D moveMixer;
        protected MixerParameterTweenVector2 moveTween;

        protected Vector3 destination;
        protected MoveType moveType;


        // This is to make sure this is never overriden into something harmful.
        protected sealed override void MakePC(string id) { base.MakePC(ID); }

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


        protected void SetDestination(Vector3 to)
        {
            destination = to;
            navAgent.SetDestination(destination);
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
