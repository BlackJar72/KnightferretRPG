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


    [RequireComponent(typeof(CharacterController))]
    public class EntityMoving : EntityLiving, IMover
    {
        [SerializeField] protected CharacterController controller;
        [SerializeField] protected MovementSet movementSetPrototype;
        [SerializeField] protected Transform eyes;
        [SerializeField] protected NavSeeker navSeeker;

        protected MovementSet movementSet;

        protected AnimancerState moveState;
        protected AnimancerLayer moveLayer;

        protected MixerTransition2D moveMixer;
        protected MixerParameterTweenVector2 moveTween;

        protected Vector3 movement;
        protected Vector3 heading;
        protected Quaternion rotation;
        protected Vector3 lastPos;
        protected float speed;
        protected Vector3 hVelocity;
        protected float vSpeed;
        protected Vector3 velocity;
        protected bool falling;
        protected bool onGround;

        [SerializeField] protected Vector3 destination;
        [SerializeField] protected MoveType moveType;

        public Transform GetTransform => transform;


        // This is to make sure this is never overriden into something harmful.
        protected sealed override void MakePC(string id) { base.MakePC(ID); }


        protected override void Awake()
        {
            base.Awake();
            // This is required for the animations to work on multiple characters,
            // and no CreateInstance is not appropriate as I need to clone the give 
            // object, not create a totally new one.
            movementSet = Instantiate(movementSetPrototype);
            rotation = new Quaternion();
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            base.Start();
            moveMixer = movementSet.Walk;
            moveLayer = animancer.Layers[0];
            moveState = moveLayer.Play(moveMixer);
            lastPos = transform.position;
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


        protected override void OnEnable()
        {
            base.OnEnable();

        }


        protected override void OnDisable()
        {
            base.OnDisable();
        }


        protected override void StoreData()
        {
            base.StoreData();
            data.movingData = new();
            data.movingData.movement = movement;
            data.movingData.heading = heading;
            data.movingData.rotation = rotation;
            data.movingData.lastPos = lastPos;
            data.movingData.speed = speed;
            data.movingData.hVelocity = hVelocity;
            data.movingData.vSpeed = vSpeed;
            data.movingData.velocity = velocity;
            data.movingData.falling = falling;
            data.movingData.onGround = onGround;
        }


        protected override void LoadData()
        {
            base.LoadData();
            movement = data.movingData.movement;
            heading = data.movingData.heading;
            rotation = data.movingData.rotation;
            lastPos = data.movingData.lastPos;
            speed = data.movingData.speed;
            hVelocity = data.movingData.hVelocity;
            vSpeed = data.movingData.vSpeed;
            velocity = data.movingData.velocity;
            falling = data.movingData.falling;
            onGround = data.movingData.onGround;
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
            LandMove();
        }


        protected bool ShouldStop()
        {
            return (moveType == MoveType.idle) || !navSeeker.Agent.isActiveAndEnabled
                || (navSeeker.Agent.remainingDistance <= navSeeker.Agent.stoppingDistance)
                || (navSeeker.Agent.velocity.sqrMagnitude == 0); ;
        }


        public bool AtLocation(Transform location)
        {
            return (transform.position - location.position).magnitude < 0.025f;
        }


        protected override void Die()
        {
            base.Die();
            navSeeker.Agent.enabled = false;
            navSeeker.gameObject.SetActive(false);
            controller.enabled = false;
            moveLayer = animancer.Layers[0]; // This may not be defined when reloading a scene
            moveLayer.SetMask(deathAnimation.mask);
            moveState = moveLayer.Play(deathAnimation.anim);
            moveState.Time = 0;
        }


        public void SetDestination(Vector3 to, float stopDist = 0.0f)
        {
            destination = to;
            navSeeker.Agent.SetDestination(destination);
            navSeeker.Agent.stoppingDistance = stopDist;
            navSeeker.stopped = false;
        }


        public void StartMoving()
        {
            navSeeker.stopped = false;
        }


        public void StopMoving()
        {
            navSeeker.stopped = true;
        }


        protected void SetMoveType(MoveType type)
        {
            moveType = type;
            switch (moveType)
            {
                case MoveType.idle:
                    navSeeker.Agent.speed = speed = 0;
                    moveMixer = movementSet.Walk;
                    SetDirectionalParameters(Vector2.zero);
                    break;
                case MoveType.crouch:
                    navSeeker.Agent.speed = speed = attributes.crouchSpeed;
                    moveMixer = movementSet.Crouch;
                    // TODO: Set DirectionalMixerState parameters
                    break;
                case MoveType.walk:
                    navSeeker.Agent.speed = speed = attributes.walkSpeed;
                    moveMixer = movementSet.Walk;
                    // TODO: Set DirectionalMixerState parameters
                    break;
                case MoveType.run:
                    navSeeker.Agent.speed = speed = attributes.crouchSpeed;
                    moveMixer = movementSet.Run;
                    // TODO: Set DirectionalMixerState parameters
                    break;
                default:
                    navSeeker.Agent.speed = speed = 0;
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





        protected void LandMove()
        {
            if (Time.deltaTime == 0) return;
            movement = navSeeker.transform.position - transform.position;
            heading.Set(movement.x, 0, movement.z);
            Vector3 newVelocity = Vector3.zero;

            if (heading.magnitude > 0.1)
            {
                heading.Normalize();
                rotation.SetLookRotation(heading, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.125f);
                newVelocity += heading * speed;
                if (moveType == MoveType.run)
                {
                    stamina.UseStamina(Time.deltaTime * attributes.runningCostFactor);
                    if (!stamina.HasStamina) SetMoveType(MoveType.walk);
                }

                DirectionalMixerState dms = moveMixer.State as DirectionalMixerState;
                if (dms != null)
                {
                    Vector3 motion = transform.position - lastPos;
                    motion.y = 0;
                    float moveSpeed = (motion.magnitude / Time.deltaTime) / speed;
                    dms.Parameter = Vector2.MoveTowards(dms.Parameter, new Vector2(0, moveSpeed), 10 * Time.deltaTime);
                    lastPos = transform.position;
                }
            }
            else
            {
                DirectionalMixerState dms = moveMixer.State as DirectionalMixerState;
                if (dms != null)
                {
                    dms.Parameter = Vector2.MoveTowards(dms.Parameter, new Vector2(0, 0), 10 * Time.deltaTime);
                    lastPos = transform.position;
                }
            }

            hVelocity = newVelocity;

            onGround = controller.isGrounded;

            if (falling && onGround) health.TakeDamage(Functions.CalcFallDamage(vSpeed, attributes.naturalArmor));

            falling = falling && !onGround;

            if (onGround)
            {
                if ((movement.y > 0.5f) && stamina.CanDoAction(10f))
                {
                    vSpeed = Mathf.Sqrt(attributes.jumpForce * GameConstants.GRAVITY);
                    stamina.UseStamina(10f);
                    moveState = moveLayer.Play(movementSet.Jump);
                }
                else
                {
                    vSpeed = -GameConstants.GRAVITY3;
                    moveState = moveLayer.Play(moveMixer);
                }
            }
            else
            {
                vSpeed -= GameConstants.GRAVITY * Time.deltaTime;
                vSpeed = Math.Max(vSpeed, GameConstants.TERMINAL_VELOCITY);
                if (!falling && (velocity.y < -10))
                {
                    falling = true;
                    moveState = moveLayer.Play(movementSet.Fall);
                }
            }

            velocity.Set(hVelocity.x, vSpeed, hVelocity.z);
            controller.Move(velocity * Time.deltaTime);
            //SetSwimming(camPivot.transform.position.y < (WorldManagement.SeaLevel + 0.5f));
        }
        


    }


}
