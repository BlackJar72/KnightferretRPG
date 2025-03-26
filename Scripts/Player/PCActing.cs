using UnityEngine;
using UnityEngine.InputSystem;


namespace kfutils.rpg {

    public class PCActing : PCMoving {



        // Input System
        protected InputAction rightAttackAction;
        protected InputAction leftBlcokAction;
        protected InputAction activateObjectAction;
        protected InputAction toggleInventoryAction;
        protected InputAction sheathWeaponAction;
        protected InputAction pauseAction;
        protected InputAction dragObjectAction;
        protected InputAction castSpellAction;
        protected InputAction throwItemAction;
        protected InputAction toggleViewModeAction;
        protected InputAction freeTPSAction;
        protected InputAction quickSlot1Action;
        protected InputAction quickSlot2Action;
        protected InputAction quickSlotAction;
        protected InputAction quickSlot4Action;
        protected InputAction quickSlot5Action;
        protected InputAction quickSlot6Action;
        protected InputAction quickSlot7Action;
        protected InputAction quickSlot8Action;
        protected InputAction quickSlot9Action;
        protected InputAction screenshot;



        protected override void Awake() {
            base.Awake();
            InitInput(); 
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start() {
            base.Start();
        }


        // Update is called once per frame
        protected override void Update() {
            base.Update();
            
        }


#region Input

        private void InitInput()
        {
            toggleInventoryAction = input.actions["OpenCloseInventory"];
            rightAttackAction = input.actions["RightUseAttack"];
            activateObjectAction = input.actions["Interact"];
            
        }


        protected override void OnEnable()
        {   
            base.OnEnable();
            EnableAction();  
            EnableUIActions();          
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            DisableAction();
            DisableUIActions();
        }


        protected void EnableUIActions() {
            toggleInventoryAction.started += ToggleCharacterSheet;
        }


        protected void DisableUIActions() {
            toggleInventoryAction.started -= ToggleCharacterSheet;            
        }


        protected override void EnableControls() {
            EnableMovement();
            EnableAction();
        }


        protected override void DisableControls() {
            DisableMovement();
            DisableAction();
        }


        protected void EnableAction() {
            rightAttackAction.started += Dummy;
            if(this is not PCTalking) activateObjectAction.started += Interact;        
        }


        protected virtual void DisableAction() {
            rightAttackAction.started -= Dummy;
            if(this is not PCTalking) activateObjectAction.started -= Interact;
        }


        // A temptoray dummy method for un-implemented actions
        protected void Dummy(InputAction.CallbackContext context) {/*DO NOTHING*/}


#endregion


        public void GetAimParams(out AimParams aim)
        {
            aim.from = playerCam.transform.position;
            aim.toward = playerCam.transform.forward;
        }


        protected virtual void Interact(InputAction.CallbackContext context)
        {
            AimParams aim;
            GetAimParams(out aim);
            RaycastHit hit;
            if (Physics.Raycast(aim.from, aim.toward, out hit, 2f))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null) interactable.Use();
            }
        }


        // FIXME??? Should this be in PCTalking, so as to also disable character interaction (probably)
        protected virtual void ToggleCharacterSheet(InputAction.CallbackContext context) {
            if(gameManager.UIManager.ToggleCharacterSheet()) {
                DisableMovement();
                DisableAction();
            } else {
                EnableMovement();
                EnableAction();
            }
        }

        




    }


}