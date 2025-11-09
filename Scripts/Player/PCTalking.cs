using UnityEngine;
using UnityEngine.InputSystem;


namespace kfutils.rpg {

    public class PCTalking : PCActing
    {

        [SerializeField] string characterName;


        // Input System1
        protected InputAction questJournalAction;




        protected override void Awake()
        {
            EntityManagement.playerCharacter = this;
            base.Awake();
            InitInput();
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            base.Start();
        }


        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

        }


        #region Input

        private void InitInput()
        {
            activateObjectAction = input.actions["Interact"];
            questJournalAction = input.actions["OpenCloseInventory"];

        }


        protected override void EnableAction()
        {
            base.EnableAction();
            rightAttackAction.started += Dummy;
            activateObjectAction.started += Interact;
        }


        protected override void DisableAction()
        {
            base.DisableAction();
            rightAttackAction.started -= Dummy;
            activateObjectAction.started -= Interact;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            EnableAction();

        }


        protected override void OnDisable()
        {
            base.OnDisable();
            DisableAction();

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


#region Save Handling


        public PCData GetPCData()
        {
            PCData result = new();
            result = GetMovingData(result);

            return result;
        }


        public void SetPCData(PCData loaded)
        {
            SetFromMovingData(loaded);
            SetFromActiningData(loaded);
            SetFromTalkingData(loaded);
        } 


        /// <summary>
        /// This sets Talking data during loads; living is included since.
        /// </summary>
        /// <param name="data"></param>
        protected void SetFromTalkingData(PCData data)
        {
            // TODO
        }




#endregion



    }


}