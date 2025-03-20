using kfutils.rpg;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;

namespace kfutils.rpg {

    public class PCMoving : EntityLiving {

        public enum MoveType {
            CROUCH = 0,
            NORMAL = 1,
            RUN = 2
        }

        // Movement and Position Data
        public Vector3 movement;
        public MoveType moveType = MoveType.NORMAL;
        private float baseSpeed;
        private Vector3 hVelocity;
        private float vSpeed;
        private Vector3 velocity;
        private bool falling;
        private bool onGround;
        private Collider[] footContats;
        private CharacterController characterController;

        // Input System
        private PlayerInput input;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction sprintAction;
        private InputAction sprintToggle;
        private InputAction crouchAction;
        private InputAction crouchToggle;
        private bool shouldJump;
        private bool hasJumped;
        private bool shouldSprint;
        private bool shouldCrouch;

        private float looky;
        private Vector2[] moveIn = new Vector2[4];
        private Vector2[] lookIn = new Vector2[4];
        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            hasJumped = false;
            InitInput(); // FIXME: Move to OnEnable(), with an OnDisable() to remover the bindings.
            characterController = GetComponent<CharacterController>();            
            Cursor.lockState = CursorLockMode.Locked;
        }


        // Update is called once per frame
        protected override void Update()
        {
            // FIXME? Move to another update???

            // Determine Movement type
            if(shouldCrouch) {
                moveType = MoveType.CROUCH;
                baseSpeed = attributes.crouchSpeed;
            } else if (shouldSprint && stamina.HasStamina && (movement != Vector3.zero)) {
                moveType = MoveType.RUN;
                baseSpeed = attributes.runSpeed;
            } else {
                moveType = MoveType.NORMAL;
                baseSpeed = attributes.walkSpeed;
            }
            // Do Movement
            AdjustHeading();
            Move();
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
            jumpAction.started += TriggerJump;
            sprintAction.started += StartSprint;
            sprintAction.canceled += StopSprint;
            //sprintToggle.started += ToggleSprint;
            crouchAction.started += StartCrouch;
            crouchAction.canceled += StopCrouch;
            //crouchToggle.started += ToggleCrouch;
        }


        private void GetLookInput()
        {
            lookIn[0] = lookIn[1]; lookIn[1] = lookIn[2];
            lookIn[2] = lookAction.ReadValue<Vector2>(); // * Options.lookSensitivity;
            lookIn[3] = ((lookIn[0] + lookIn[1] + lookIn[2]) / 3f);
        }


        private void GetMoveInput()
        {
            moveIn[0] = moveIn[1]; moveIn[1] = moveIn[2];
            moveIn[2] = moveAction.ReadValue<Vector2>() ; // * Options.moveSensitivity;
            moveIn[3] = ((moveIn[0] + moveIn[1] + moveIn[2]) / 3f);
        }


        private void TriggerJump(InputAction.CallbackContext context)
        {
            shouldJump = true;
        }


        private void StartSprint(InputAction.CallbackContext context)
        {
            shouldSprint = true;
        }


        private void StopSprint(InputAction.CallbackContext context)
        {
            shouldSprint = false;
        }


        private void ToggleSprint(InputAction.CallbackContext context)
        {
            shouldSprint = !shouldSprint;
        }


        private void StartCrouch(InputAction.CallbackContext context)
        {
            shouldCrouch = true;
            shouldSprint = false;
        }


        private void StopCrouch(InputAction.CallbackContext context)
        {
            shouldCrouch = false;
        }


        private void ToggleCrouch(InputAction.CallbackContext context)
        {
            shouldCrouch = !shouldCrouch;
            shouldSprint = false;
        }

#endregion


        private void AdjustHeading()
        {
            GetLookInput();

            transform.rotation = Quaternion.Euler(
                    transform.eulerAngles.x,
                    transform.eulerAngles.y + lookIn[3].x,
                    transform.eulerAngles.z
            );

            looky = Mathf.Clamp(looky - lookIn[3].y, -70f, 70f);
            //camPivot.transform.rotation = Quaternion.Euler(looky / 2, transform.eulerAngles.y, 0).normalized;
            //playerCam.transform.localRotation = Quaternion.Euler(looky / 2, camPivot.transform.eulerAngles.z, 0).normalized;
        }


        private void Move() {
            GetMoveInput();
            movement.Set(moveIn[3].x, 0, moveIn[3].y);
            Vector3 newVelocity = new Vector3(0, velocity.y, 0);

            if (movement.magnitude > 0) {
                if (movement.magnitude > 1) {
                    newVelocity += transform.rotation * (movement.normalized * baseSpeed);
                } else {
                    newVelocity += transform.rotation * (movement * baseSpeed);
                }
                if(moveType == MoveType.RUN) {
                    stamina.UseStamina(Time.deltaTime * attributes.runningCostFactor);
                }
            }

            hVelocity = newVelocity;

            onGround = characterController.isGrounded;
            falling = falling && !onGround;

            if (onGround) {
                if (shouldJump && stamina.CanDoAction(10f)) {
                    vSpeed = Mathf.Sqrt(attributes.jumpForce * GameConstants.GRAVITY);;
                    stamina.UseStamina(10f);                    
                } 
            }
            vSpeed -= GameConstants.GRAVITY * Time.deltaTime;
            if (!falling && (velocity.y < -5)) {
                falling = true;
            }

            velocity.Set(hVelocity.x, vSpeed, hVelocity.z);
            characterController.Move(velocity * Time.deltaTime);
            shouldJump = false;
        }
        


    }

}