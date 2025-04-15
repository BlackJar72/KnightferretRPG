using Animancer;
using UnityEngine;
using UnityEngine.InputSystem;

namespace kfutils.rpg {

    [RequireComponent(typeof(PlayerInput))]
    public class PCMoving : EntityLiving {

        public enum MoveType {
            CROUCH = 0,
            NORMAL = 1,
            RUN = 2
        }

        public const string PC = "PLAYER_CHARACTER";

        [SerializeField] protected GameManager gameManager;

        [SerializeField] protected MovementSet movementSet;


        // Camera
        public GameObject camPivot;
        public Camera playerCam;

        // Movement and Position Data
        protected Vector3 movement;
        protected MoveType moveType = MoveType.NORMAL;
        protected float baseSpeed;
        protected Vector3 hVelocity;
        protected float vSpeed;
        protected Vector3 velocity;
        protected bool falling;
        protected bool onGround;
        protected Collider[] footContats;
        protected CharacterController characterController;
        protected AnimancerState moveState;
        protected AnimancerLayer moveLayer;
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
        protected bool shouldJump;
        protected bool hasJumped;
        protected bool shouldSprint;
        protected bool shouldCrouch;
        protected bool movementAllowed;
        protected float weightMovementFactor = 1.0f;

        protected float looky;
        protected Vector2[] moveIn = new Vector2[4];
        protected Vector2[] lookIn = new Vector2[4];

        protected sealed override void MakePC(string id) { base.MakePC(PC); }


        public void SetWeightForMovement(float weight) {
            weightMovementFactor = Mathf.Max(1.0f - (Mathf.Max((weight - attributes.halfEncumbrance), 0.0f) / attributes.halfEncumbrance), 0.0001f);
        }


        protected override void Awake() {
            base.Awake();
            MakePC(PC);
            hasJumped = false;
            InitInput(); 
        }
        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            // Temporary for Testing; TODO: Remove this! (It should be called elsewhere.)
            NewCharacterInit();

            // Normal stuff below
            characterController = GetComponent<CharacterController>();  
            if(gameManager == null) gameManager = FindFirstObjectByType<GameManager>();          
            Cursor.lockState = CursorLockMode.Locked;

            moveMixer = movementSet.Walk;
            moveLayer = animancer.Layers[0];
            moveState = moveLayer.Play(moveMixer);    
            moveTween = new MixerParameterTweenVector2(moveMixer.State);        
        }


        /// <summary>
        /// Initialize a new character, to be called after character creation but before 
        /// the start of the game.
        /// </summary>
        /// TODO / FIXME?  This may need to take a some character creation data as an argument later
        public virtual void NewCharacterInit() {
            // First, we need to handle the derived attribute (may be moved to more derived class later)
            attributes.baseStats.GenRandomHumanStats();
            attributes.DeriveAttributesForHuman(health, stamina, mana);

            // Make sure all the bars are full.
            health.HealFully();
            stamina.HealFully();
            mana.HealFully();
            // TODO ...?
        }


        // Update is called once per frame
        protected override void Update()
        {
            // FIXME? Move to fixed update???

            if(movementAllowed) {
                // Determine Movement type
                if(shouldCrouch) {
                    moveType = MoveType.CROUCH;
                    baseSpeed = attributes.crouchSpeed;                    
                    moveMixer = movementSet.Crouch;
                } else if (shouldSprint && stamina.HasStamina && (movement != Vector3.zero)) {
                    moveType = MoveType.RUN;
                    baseSpeed = attributes.runSpeed;                  
                    moveMixer = movementSet.Run;
                } else {
                    moveType = MoveType.NORMAL;
                    baseSpeed = attributes.walkSpeed;                  
                    moveMixer = movementSet.Walk;
                }
                // Do Movement
                AdjustHeading();
                Move();
            }
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
        }


        protected virtual void OnEnable() {
            EnableMovement();
        }


        protected virtual void OnDisable() {
            DisableMovement();
        }


        protected virtual void EnableControls() {
            EnableMovement();
        }


        protected virtual void DisableControls() {
            DisableMovement();
        }


        protected void EnableMovement() {
            jumpAction.started += TriggerJump;
            sprintAction.started += StartSprint;
            sprintAction.canceled += StopSprint;
            //sprintToggle.started += ToggleSprint;
            crouchAction.started += StartCrouch;
            crouchAction.canceled += StopCrouch;
            //crouchToggle.started += ToggleCrouch; 
            movementAllowed = true;       
        }


        protected virtual void DisableMovement() {
            jumpAction.started -= TriggerJump;
            sprintAction.started -= StartSprint;
            sprintAction.canceled -= StopSprint;
            //sprintToggle.started -= ToggleSprint;
            crouchAction.started -= StartCrouch;
            crouchAction.canceled -= StopCrouch;
            //crouchToggle.started -= ToggleCrouch;
            movementAllowed = false;
        }


        protected void GetLookInput()
        {
            lookIn[0] = lookIn[1]; lookIn[1] = lookIn[2];
            lookIn[2] = lookAction.ReadValue<Vector2>(); // * Options.lookSensitivity;
            lookIn[3] = ((lookIn[0] + lookIn[1] + lookIn[2]) / 3f);
        }


        protected void GetMoveInput()
        {
            moveIn[0] = moveIn[1]; moveIn[1] = moveIn[2];
            moveIn[2] = moveAction.ReadValue<Vector2>() ; // * Options.moveSensitivity;
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
            //Debug.Log("Stop Crouch");
            shouldCrouch = false;
        }


        protected void ToggleCrouch(InputAction.CallbackContext context)
        {
            shouldCrouch = !shouldCrouch;
            shouldSprint = false;
        }

#endregion


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


        protected void Move() {
            GetMoveInput();
            Vector3 oldMove = movement;
            movement.Set(moveIn[3].x, 0, moveIn[3].y);
            Vector3 newVelocity = new Vector3(0, velocity.y, 0);

            if (movement.magnitude > 0) {
                if (movement.magnitude > 1) {
                    newVelocity += transform.rotation * (movement.normalized * baseSpeed * weightMovementFactor);
                } else {
                    newVelocity += transform.rotation * (movement * baseSpeed * weightMovementFactor);
                }
                if(moveType == MoveType.RUN) {
                    stamina.UseStamina(Time.deltaTime * attributes.runningCostFactor / weightMovementFactor);
                }
            }

            DirectionalMixerState dms = moveMixer.State as DirectionalMixerState;
            if(dms != null) { 
                dms.Parameter = Vector2.MoveTowards(dms.Parameter, new Vector2(movement.x, movement.z), 10 * Time.deltaTime);
            } 

            hVelocity = newVelocity;

            onGround = characterController.isGrounded;

            if(falling && onGround) health.TakeDamage(Functions.CalcFallDamage(vSpeed, attributes.naturalArmor));

            falling = falling && !onGround;

            if (onGround) {
                if (shouldJump && stamina.CanDoAction(10f)) {
                    vSpeed = Mathf.Sqrt(attributes.jumpForce * weightMovementFactor * GameConstants.GRAVITY);;
                    stamina.UseStamina(10f);  
                    moveState = moveLayer.Play(movementSet.Jump);                  
                } else {
                    vSpeed = -GameConstants.GRAVITY3;
                    moveState = moveLayer.Play(moveMixer); 
                }
            } else {
                vSpeed -= GameConstants.GRAVITY * Time.deltaTime;
                if (!falling && (velocity.y < -10)) {
                    falling = true;
                    moveState = moveLayer.Play(movementSet.Fall);
                }
            }

            velocity.Set(hVelocity.x, vSpeed, hVelocity.z);
            characterController.Move(velocity * Time.deltaTime);
            shouldJump = false;
        }
        


    }

}