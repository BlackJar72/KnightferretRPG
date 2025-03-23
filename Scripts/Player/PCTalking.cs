using UnityEngine;
using UnityEngine.InputSystem;


namespace kfutils.rpg {

    public class PCTalking : PCActing {

        [SerializeField] string characterName;


        // Input System1
        protected InputAction questJournalAction;

        


        protected override void Awake() {
            base.Awake();
            InitInput(); 
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start() {
            base.Start();
            EntityManagement.playerCharacter = this; 
        }


        // Update is called once per frame
        protected override void Update() {
            base.Update();
            
        }


#region Input

        private void InitInput()
        {   
            activateObjectAction = input.actions["Interact"];
            questJournalAction = input.actions["OpenCloseInventory"];
            
        }


        protected override void OnEnable()
        {   
            base.OnEnable();
            rightAttackAction.started += Dummy;
            activateObjectAction.started += Interact;
            
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            rightAttackAction.started -= Dummy; 
            activateObjectAction.started -= Interact;
            
        }


#endregion


        public override string GetPersonalName()
        {
            return characterName;
        }


        // FIXME: When this is fully implemented, it should not be called both here and from PCActing!
        protected override void Interact(InputAction.CallbackContext context)
        {
            // TODO: Check for characters to interact with first!
            base.Interact(context);
        }



    }


}