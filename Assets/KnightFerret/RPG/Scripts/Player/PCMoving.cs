using System;
using System.Collections;
using Animancer;
using UnityEngine;
using UnityEngine.InputSystem;

namespace kfutils.rpg {

    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(CharacterController))]
    public class PCMoving : EntityLiving, IMover, ISoundSource 
    {

        public enum MoveType
        {
            CROUCH = 0,
            NORMAL = 1,
            RUN = 2
        }

        public enum MoveMethod
        {
            LAND = 0,
            WATER = 1
        }

        public const string PC = "PLAYER_CHARACTER";

        public const float MAX_WATER_V = 2.5f;

        [SerializeField] protected GameManager gameManager;

        [SerializeField] protected MovementSet movementSet;
        [SerializeField] protected AnimancerComponent arms; // TODO: Animate arms, once there are arms to animate

        [SerializeField] protected Transform eyeY;

        // Camera
        [SerializeField] protected GameObject camPivot;
        [SerializeField] protected Camera playerCam;
        [SerializeField] protected GameObject followCam;
        [SerializeField] protected Transform swimpoint;

        // Movement and Position Data
        protected Vector3 movement;
        protected MoveType moveType = MoveType.NORMAL;
        protected float baseSpeed;
        protected Vector3 hVelocity;
        protected float vSpeed;
        protected Vector3 velocity;
        protected bool falling;
        protected bool onGround;
        protected CharacterController characterController;
        protected AnimancerState moveState;
        protected AnimancerState armsMoveState;
        protected AnimancerLayer moveLayer;
        protected AnimancerLayer armsMoveLayer;
        protected MixerTransition2D moveMixer;
        protected MixerParameterTweenVector2 moveTween;

        // Input System
        protected PlayerInput input;
        protected InputAction moveAction;
        protected InputAction lookAction;
        protected InputAction jumpAction;
        protected InputAction sprintAction;
        protected InputAction sprintToggle;
        protected InputAction crouchAction;
        protected InputAction crouchToggle;
        protected InputAction swimAction;
        protected bool shouldJump;
        protected bool hasJumped;
        protected bool shouldSprint;
        protected bool shouldCrouch;
        protected bool movementAllowed;
        protected float weightMovementFactor = 1.0f;
        protected float weightBoyancyFactor = 1.0f;

        protected float looky;
        protected Vector2[] moveIn = new Vector2[4];
        protected Vector2[] lookIn = new Vector2[4];

        protected sealed override void MakePC(string id) { base.MakePC(PC); }

        protected delegate void Movement();
        protected Movement Move;

        private readonly Vector3 standingCenter = new Vector3(0f, 0.9f, 0f);
        private readonly Vector3 crouchingCenter = new Vector3(0f, 0.6f, 0f);

        public Transform GetTransform => transform;

        public GameObject CamPivot => camPivot;
        public Camera PlayerCam => playerCam;
        public GameObject FollowCam => followCam;
        public Transform Swimpoint => swimpoint;



        public void SetWeightForMovement(float weight)
        {
            weightMovementFactor = Mathf.Max(1.0f - (Mathf.Max((weight - attributes.halfEncumbrance), 0.0f) / attributes.halfEncumbrance), 0.0001f);
            weightBoyancyFactor = 1.0f - (Mathf.Max((weight - (attributes.halfEncumbrance * 0.5f)), 0.0f) / (attributes.halfEncumbrance * 0.5f));
        }


        protected override void Awake()
        {
            characterController = GetComponent<CharacterController>();
            base.Awake();
            MakePC(PC);
            Move = LandMove;
            hasJumped = false;
            InitInput();
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {    
            Cursor.lockState = CursorLockMode.Locked;
            movementSet = Instantiate(movementSet);
            moveMixer = movementSet.Walk;
            moveLayer = animancer.Layers[0];
            armsMoveLayer = arms.Layers[0];
            moveState = moveLayer.Play(moveMixer);
            armsMoveState = armsMoveLayer.Play(moveMixer);
            moveTween = new MixerParameterTweenVector2(moveMixer.State);
        }


        public virtual void ResetCharacter()
        {
            data = new(PC);
            MakeAlive();
            alertness = Alertness.Alerted;
            movement = Vector3.zero;
            moveType = MoveType.NORMAL;
            UpdateMoveType();
            hVelocity = Vector3.zero;
            vSpeed = 0.0f;
            velocity = Vector3.zero;
            falling = false;
            onGround = true;
            shouldJump = false;
            hasJumped = false;
            shouldSprint = false;
            shouldCrouch = false;
            EnableMovement();
            weightMovementFactor = 1.0f;
            weightBoyancyFactor = 1.0f;            
        }


        /// <summary>
        /// Initialize a new character, to be called after character creation but before 
        /// the start of the game.
        /// </summary>
        /// TODO / FIXME?  This may need to take a some character creation data as an argument later
        public virtual void NewCharacterInit()
        {
            FirstPerson();
            health.HealFully();
            stamina.HealFully();
            mana.HealFully();
        }


        public virtual void SetName(string name)
        {
            entityName = name;
        }


        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            Move();
        }


        protected override void Die()
        {
            if (alive)
            {
                alive = false;
                if (hitbox != null) hitbox.gameObject.SetActive(false);
                EntityManagement.RemoveDead(this);
                moveState = moveLayer.Play(deathAnimation.anim);
                DisableMovement();
                Move = LieDead;
                ThirdPerson();
                StartCoroutine(DeathHelper());
            }
        }


        private IEnumerator DeathHelper()
        {
            yield return new WaitForSeconds(1.5f);
            GameManager.Instance.UI.ShowDeathMessage();
        }


        protected virtual void MakeAlive()
        {
            alive = true;
            if (hitbox != null) hitbox.gameObject.SetActive(true);
            EnableMovement();
            FirstPerson();
            Start();
        }


        protected void FirstPerson()
        {
            playerCam.enabled = true;
            playerCam.gameObject.GetComponent<AudioListener>().enabled = true;
            followCam.SetActive(false);
        }


        protected void ThirdPerson()
        {
            playerCam.enabled = false; 
            playerCam.gameObject.GetComponent<AudioListener>().enabled = false;
            followCam.SetActive(true);
        }


        public void MakeSound(float loudness, SoundType soundType = SoundType.General)
        {
            SoundManagement.SoundMadeAtByPlayer(new WorldSound(transform.position, loudness, soundType, this), this as PCTalking);
        }


        public bool AtLocation(Transform location)
        {
            return (transform.position - location.position).magnitude < 0.025f;
        }


        public bool AtDestination() => false;


        protected void UpdateMoveType()
        {
            if (shouldCrouch)
            {
                moveType = MoveType.CROUCH;
                baseSpeed = attributes.crouchSpeed;
                moveMixer = movementSet.Crouch;
                characterController.height = 1.2f;
            }
            else if (shouldSprint && stamina.HasStamina && (movement != Vector3.zero))
            {
                moveType = MoveType.RUN;
                baseSpeed = attributes.runSpeed;
                moveMixer = movementSet.Run;
                characterController.height = 1.8f;
            }
            else
            {
                moveType = MoveType.NORMAL;
                baseSpeed = attributes.walkSpeed;
                moveMixer = movementSet.Walk;
                characterController.height = 1.8f;
            }
        }


        public void Teleport(TransformData destination)
        {
            vSpeed = 0;
            hVelocity = Vector3.zero;
            characterController.enabled = false;
            transform.position = destination.position;
            transform.rotation = destination.rotation;
            transform.localScale = destination.scale;
            characterController.enabled = true;
        }


        public void Teleport(Transform destination)
        {
            vSpeed = 0;
            hVelocity = Vector3.zero;
            characterController.enabled = false;
            transform.position = destination.position;
            transform.rotation = destination.rotation;
            characterController.enabled = true;
        }


        public override bool CanBeSeenFrom(Transform from, float rangeSqr)
        {
            Vector3 toOther = characterController.bounds.center - from.position;
            float dist = toOther.sqrMagnitude;
            return ((dist < rangeSqr)
            && (Vector3.Dot(from.forward, toOther) > 0)
            && !Physics.Linecast(from.position, characterController.bounds.center, GameConstants.LevelMask));
        }


#region Input


        private void InitInput()
        {
            input = GetComponent<PlayerInput>();
            moveAction = input.actions["Move"];
            lookAction = input.actions["Look"];
            jumpAction = input.actions["Jump"];
            sprintAction = input.actions["Sprint"];
            //sprintToggle = input.actions["Toggle Sprint"];
            crouchAction = input.actions["Crouch"];
            //crouchToggle = input.actions["Toggle Crouch"];
            swimAction = input.actions["SwimUp"];
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            EnableMovement();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            DisableMovement();
        }


        protected override void StoreData()
        {
            base.StoreData();
            data.movingData ??= new();
            data.movingData.movement = velocity;
            data.movingData.heading = movement;
            data.movingData.rotation = transform.rotation;
            data.movingData.lastPos = transform.position;
            data.movingData.speed = baseSpeed;
            data.movingData.hVelocity = hVelocity;
            data.movingData.vSpeed = vSpeed;
            data.movingData.velocity = velocity;
            data.movingData.falling = falling;
            data.movingData.onGround = onGround;
            data.movingData.navSeekerPos = new TransformData(transform.position, transform.rotation, transform.localScale);;
        }


        protected override void LoadData()
        {
            base.LoadData();
            characterController.enabled = false;
            velocity = data.movingData.movement;
            movement = data.movingData.heading;
            transform.rotation = data.movingData.rotation;
            transform.position = data.movingData.lastPos;
            baseSpeed = data.movingData.speed;
            hVelocity = data.movingData.hVelocity;
            vSpeed = data.movingData.vSpeed;
            velocity = data.movingData.velocity;
            falling = data.movingData.falling;
            onGround = data.movingData.onGround;
            characterController.enabled = true;
        }


        protected virtual void EnableControls()
        {
            EnableMovement();
        }


        protected virtual void DisableControls()
        {
            DisableMovement();
        }


        protected void EnableMovement()
        {
            if (alive)
            {
                jumpAction.started += TriggerJump;
                sprintAction.started += StartSprint;
                sprintAction.canceled += StopSprint;
                //sprintToggle.started += ToggleSprint;
                crouchAction.started += StartCrouch;
                crouchAction.canceled += StopCrouch;
                //crouchToggle.started += ToggleCrouch; 
                movementAllowed = true;
            }
        }


        protected virtual void DisableMovement()
        {
            jumpAction.started -= TriggerJump;
            sprintAction.started -= StartSprint;
            sprintAction.canceled -= StopSprint;
            //sprintToggle.started -= ToggleSprint;
            crouchAction.started -= StartCrouch;
            crouchAction.canceled -= StopCrouch;
            //crouchToggle.started -= ToggleCrouch;
            movementAllowed = false;
            hVelocity = Vector3.zero;
        }


        protected void GetLookInput()
        {
            Vector3 eyePos = new Vector3(camPivot.transform.position.x,
                    eyeY.position.y, camPivot.transform.position.z);
            camPivot.transform.position = eyePos;
            lookIn[0] = lookIn[1]; lookIn[1] = lookIn[2];
            lookIn[2] = lookAction.ReadValue<Vector2>(); // * Options.lookSensitivity;
            lookIn[3] = ((lookIn[0] + lookIn[1] + lookIn[2]) / 3f);
        }


        protected void GetMoveInput()
        {
            moveIn[0] = moveIn[1]; moveIn[1] = moveIn[2];
            moveIn[2] = moveAction.ReadValue<Vector2>(); // * Options.moveSensitivity;
            moveIn[3] = ((moveIn[0] + moveIn[1] + moveIn[2]) / 3f);
        }


        protected void TriggerJump(InputAction.CallbackContext context)
        {
            shouldJump = true;
        }


        protected void StartSprint(InputAction.CallbackContext context)
        {
            shouldSprint = true;
        }


        protected void StopSprint(InputAction.CallbackContext context)
        {
            shouldSprint = false;
        }


        protected void ToggleSprint(InputAction.CallbackContext context)
        {
            shouldSprint = !shouldSprint;
        }


        protected void StartCrouch(InputAction.CallbackContext context)
        {
            shouldCrouch = true;
            shouldSprint = false;
            CharacterController controller = GetComponent<CharacterController>();
        }


        protected void StopCrouch(InputAction.CallbackContext context)
        {
            shouldCrouch = false;
        }


        protected void ToggleCrouch(InputAction.CallbackContext context)
        {
            shouldCrouch = !shouldCrouch;
            shouldSprint = false;
        }

#endregion


#region Saving / Loading


        protected PCData GetMovingData(PCData data)
        {
            data.entityData = this.data;
            data.location = gameObject.transform.GetGlobalData();
            data.moveMethod = GetMoveMethod();
            data.movement = movement;
            data.moveType = moveType;
            data.baseSpeed = baseSpeed;
            data.hVelocity = hVelocity;
            data.vSpeed = vSpeed;
            data.velocity = velocity;
            data.falling = falling;
            data.onGround = onGround;
            data.shouldJump = shouldJump;
            data.hasJumped = hasJumped;
            data.shouldSprint = shouldSprint;
            data.shouldCrouch = shouldCrouch;
            data.weightMovementFactor = weightMovementFactor;
            data.weightBoyancyFactor = weightBoyancyFactor;
            data.looky = looky;

            return data;
        }


        public MoveMethod GetMoveMethod()
        {
            if (Move == WaterMove) return MoveMethod.WATER;
            return MoveMethod.LAND;
        }


        public void SetMoveMethod(MoveMethod method)
        {
            switch (method)
            {
                case MoveMethod.LAND:
                    Move = LandMove;
                    return;
                case MoveMethod.WATER:
                    Move = WaterMove;
                    return;
                default:
                    Move = LandMove;
                    return;
            }
        }


        /// <summary>
        /// This sets Living and Moving data during loads; living is included since 
        /// there is no PCLiving but instead entity living is inherited directly.
        /// </summary>
        /// <param name="data"></param>
        protected void SetFromMovingData(PCData data)
        {
            FirstPerson();
            characterController.enabled = false;
            entityName = data.entityData.livingData.entityName;
            attributes.CopyInto(data.entityData.livingData.attributes);
            health.CopyInto(data.entityData.livingData.health);
            stamina.CopyInto(data.entityData.livingData.stamina);
            mana.CopyInto(data.entityData.livingData.mana);
            health.BeLoaded(this);
            stamina.BeLoaded(this);
            mana.BeLoaded(this);
            statusEffects = data.entityData.livingData.statusEffects;
            EntityManagement.AddToRegistryForce(this.data);
            gameObject.transform.SetPositionAndRotation(data.location.position, data.location.rotation);
            gameObject.transform.localScale = data.location.scale;
            SetMoveMethod(data.moveMethod);
            movement = data.movement;
            moveType = data.moveType;
            baseSpeed = data.baseSpeed;
            hVelocity = data.hVelocity;
            vSpeed = data.vSpeed;
            velocity = data.velocity;
            falling = data.falling;
            onGround = data.onGround;
            shouldJump = data.shouldJump;
            hasJumped = data.hasJumped;
            shouldSprint = data.shouldSprint;
            shouldCrouch = data.shouldCrouch;
            weightMovementFactor = data.weightMovementFactor;
            weightBoyancyFactor = data.weightBoyancyFactor;
            looky = data.looky;
            characterController.enabled = true;
            MakeAlive();
        }





#endregion


#region Core Movement


        public void SetSwimming(bool swimming)
        {
            if (swimming)
            {
                Move = WaterMove;
                shouldCrouch = false;
            }
            else Move = LandMove;
        }


        protected void AdjustHeading()
        {
            GetLookInput();

            transform.rotation = Quaternion.Euler(
                    transform.eulerAngles.x,
                    transform.eulerAngles.y + lookIn[3].x,
                    transform.eulerAngles.z
            );

            looky = Mathf.Clamp(looky - lookIn[3].y, -70f, 70f);
            camPivot.transform.rotation = Quaternion.Euler(looky / 2, transform.eulerAngles.y, 0).normalized;
            playerCam.transform.localRotation = Quaternion.Euler(looky / 2, camPivot.transform.eulerAngles.z, 0).normalized;
        }


        protected void LandMove()
        {
            movement.Set(moveIn[3].x, 0, moveIn[3].y);
            Vector3 newVelocity = Vector3.zero;

            if (movementAllowed)
            {
                UpdateMoveType();
                AdjustHeading();
                GetMoveInput();

                if (movement.magnitude > 0)
                {
                    if (movement.magnitude > 1)
                    {
                        newVelocity += transform.rotation * (movement.normalized * baseSpeed * weightMovementFactor);
                    }
                    else
                    {
                        newVelocity += transform.rotation * (movement * baseSpeed * weightMovementFactor);
                    }
                    if (moveType == MoveType.RUN)
                    {
                        stamina.UseStamina(Time.deltaTime * attributes.runningCostFactor / weightMovementFactor);
                    }
                    float noise = (float)moveType + 1;
                    noise = noise * noise; // TODO: Factor in stealth skill and armor worn
                    MakeSound(noise);
                }

                DirectionalMixerState dms = moveMixer.State as DirectionalMixerState;
                if (dms != null)
                {
                    dms.Parameter = Vector2.MoveTowards(dms.Parameter, new Vector2(movement.x, movement.z), 10 * Time.deltaTime);
                }

                hVelocity = newVelocity;
            }

            onGround = characterController.isGrounded;

            if (falling && onGround) health.TakeDamage(Functions.CalcFallDamage(vSpeed, attributes.naturalArmor));

            falling = falling && !onGround;

            if (onGround)
            {
                if (shouldJump && stamina.CanDoAction(10f) && movementAllowed)
                {
                    vSpeed = Mathf.Sqrt(attributes.jumpForce * weightMovementFactor * GameConstants.GRAVITY);
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
            characterController.Move(velocity * Time.deltaTime);
            shouldJump = false;
            SetSwimming(swimpoint.position.y < (WorldManagement.SeaLevel + 0.5f));
        }


        protected void WaterMove()
        {
            Vector3 newVelocity = Vector3.zero;

            if (movementAllowed && stamina.CanDoAction(1.0f))
            {
                moveType = MoveType.NORMAL;
                AdjustHeading();
                GetMoveInput();
                movement.Set(moveIn[3].x, 0, moveIn[3].y);
                bool moved = false;

                if (movement.magnitude > 0)
                {
                    if (movement.magnitude > 1)
                    {
                        newVelocity += transform.rotation * (movement.normalized * attributes.crouchSpeed * weightMovementFactor);
                    }
                    else
                    {
                        newVelocity += transform.rotation * (movement * attributes.crouchSpeed * weightMovementFactor);
                    }
                    moved = true;
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    newVelocity.y = attributes.crouchSpeed * Mathf.Max(weightBoyancyFactor, 0);
                    moved = true;
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    newVelocity.y = -attributes.crouchSpeed;
                    moved = true;
                }
                if (moved) stamina.UseStamina(Time.deltaTime * attributes.runningCostFactor / weightMovementFactor);
            }

            if (newVelocity.magnitude > MAX_WATER_V) newVelocity = newVelocity.normalized * MAX_WATER_V;

            DirectionalMixerState dms = moveMixer.State as DirectionalMixerState;
            if (dms != null)
            {
                dms.Parameter = Vector2.MoveTowards(dms.Parameter, new Vector2(movement.x, movement.z), 10 * Time.deltaTime);
            }

            vSpeed += (1.0f - ((1.0f - weightBoyancyFactor) * 2.0f)) * Time.deltaTime; // Byancy
            if (vSpeed > -MAX_WATER_V) vSpeed = Mathf.Min(MAX_WATER_V, vSpeed + newVelocity.y);
            vSpeed -= vSpeed * Time.deltaTime; // Drag

            hVelocity = newVelocity;
            hVelocity.y = 0;

            if (camPivot.transform.position.y > (WorldManagement.SeaLevel + 0.25f)) vSpeed = Mathf.Min(vSpeed, 0);

            onGround = characterController.isGrounded;

            if (falling && onGround) health.TakeDamage(Functions.CalcFallDamage(vSpeed, attributes.naturalArmor));

            falling = (vSpeed < -MAX_WATER_V) && falling && !onGround;

            velocity.Set(hVelocity.x, vSpeed, hVelocity.z);
            characterController.Move(velocity * Time.deltaTime);
            shouldJump = false;
            SetSwimming(swimpoint.position.y < (WorldManagement.SeaLevel + 0.5f));
        }


        protected void LieDead()
        {
            onGround = characterController.isGrounded;
            if (onGround)
            {
                vSpeed = -GameConstants.GRAVITY3;
            }
            else
            {
                vSpeed -= GameConstants.GRAVITY * Time.deltaTime;
                vSpeed = Math.Max(vSpeed, GameConstants.TERMINAL_VELOCITY);
            }
            velocity.Set(0, vSpeed, 0);
            characterController.Move(velocity * Time.deltaTime);
            shouldJump = false;
        }




        #endregion




    }

}
