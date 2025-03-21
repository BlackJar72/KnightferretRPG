using UnityEngine;
using UnityEngine.InputSystem;


namespace kfutils.rpg {

    public class PCActing : PCMoving {



        // Input System
        private PlayerInput input;
        private InputAction rightAttackAction;
        private InputAction leftBlcokAction;
        private InputAction activateObjectAction;
        private InputAction toggleInventoryAction;
        private InputAction sheathWeaponAction;
        private InputAction questJournalAction;
        private InputAction pauseAction;
        private InputAction dragObjectAction;
        private InputAction castSpellAction;
        private InputAction throwItemAction;
        private InputAction toggleViewModeAction;
        private InputAction freeTPSAction;
        private InputAction quickSlot1Action;
        private InputAction quickSlot2Action;
        private InputAction quickSlotAction;
        private InputAction quickSlot4Action;
        private InputAction quickSlot5Action;
        private InputAction quickSlot6Action;
        private InputAction quickSlot7Action;
        private InputAction quickSlot8Action;
        private InputAction quickSlot9Action;
        private InputAction screenshot;


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
            input = GetComponent<PlayerInput>();

            rightAttackAction = input.actions["RightUseAttack"];
            activateObjectAction = input.actions["Interact"];
            
        }


        protected override void OnEnable()
        {   
            base.OnEnable();
            rightAttackAction.started += Dummy;
            activateObjectAction.started += UseObject;
            
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            rightAttackAction.started -= Dummy;
            activateObjectAction.started -= UseObject;
            
        }


        // A temptoray dummy method for un-implemented actions
        private void Dummy(InputAction.CallbackContext context) {/*DO NOTHING*/}


#endregion


        public void GetAimParams(out AimParams aim)
        {
            aim.from = playerCam.transform.position;
            aim.toward = playerCam.transform.forward;
        }


        private void UseObject(InputAction.CallbackContext context)
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

        




    }


}