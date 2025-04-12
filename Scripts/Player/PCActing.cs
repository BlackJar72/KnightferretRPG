using Animancer;
using kfutils.rpg.ui;
using UnityEngine;
using UnityEngine.InputSystem;


namespace kfutils.rpg {

    public class PCActing : PCMoving, IAttacker {

        [SerializeField] PlayerInventory inventory;
        [SerializeField] Spellbook spellbook;
        [SerializeField] SpellEquiptSlot equiptSpell;

        [SerializeField] CharacterEquipt itemLocations;


        protected AnimancerLayer actionLayer;
        protected AnimancerState actionState;


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


        // Accessor Properties
        public PlayerInventory Inventory => inventory;
        public Spellbook Spells => spellbook;
        public EquiptmentSlots EquiptItems => inventory.Equipt;
        public SpellEquiptSlot EquiptSpell => equiptSpell;

        public AnimancerLayer ActionLayer => actionLayer;

        public AnimancerState ActionState => actionState;



        protected override void Awake() {
            if(inventory == null) inventory = GetComponent<PlayerInventory>();
            if(spellbook == null) spellbook = GetComponent<Spellbook>();
            inventory.SetOwner(this); // Just in case it wasn't set correctly
            base.Awake();
            InitInput(); 
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start() {
            base.Start();
            actionLayer = animancer.Layers[1];
            actionState = moveState;
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
            InventoryManager.toggleCharacterSheet += ToggleCharacterSheet;
        }


        protected void DisableUIActions() {
            toggleInventoryAction.started -= ToggleCharacterSheet;   
            InventoryManager.toggleCharacterSheet -= ToggleCharacterSheet;         
        }


        protected override void EnableControls() {
            EnableMovement();
            EnableAction();
        }


        protected override void DisableControls() {
            DisableMovement();
            DisableAction();
        }


        protected virtual void EnableAction() { 
            rightAttackAction.started += UseRightItem;
            if(this is not PCTalking) activateObjectAction.started += Interact;  
        }


        protected virtual void DisableAction() { 
            rightAttackAction.started -= UseRightItem;
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
                if (interactable != null) interactable.Use(gameObject);
            }
        }


        // FIXME??? Should this be in PCTalking, so as to also disable character interaction (probably)
        protected virtual void ToggleCharacterSheet(InputAction.CallbackContext context) {
            ToggleCharacterSheet();
        }


        // FIXME??? Should this be in PCTalking, so as to also disable character interaction (probably)
        public virtual void ToggleCharacterSheet() {
            if(gameManager.UIManager.ToggleCharacterSheet()) {
                DisableMovement();
                DisableAction();
            } else {
                EnableMovement();
                EnableAction();
            }
        }


        // FIXME??? Should this be in PCTalking, so as to also disable character interaction (probably)
        public virtual void AllowActions(bool allow) {
            if(!allow) {
                DisableMovement();
                DisableAction();
            } else {
                EnableMovement();
                EnableAction();
            }
        }

        
        public virtual void AddToMainInventory(ItemStack stack) {
            GetComponent<Inventory>().AddToFirstEmptySlot(stack);
        }


#region Equipment


        public virtual void EquiptItem(ItemStack item) {
            if(item != null) {
                ItemEquipt equipt = itemLocations.EquipItem(item);
                IUsable usable = equipt as IUsable;
                if(usable != null) {                     
                    usable.OnEquipt(this);              
                    //usable.PlayEquipAnimation(actionLayer, actionState);
                }            
            } 
        }


        public virtual void UnequiptItem(ItemStack item) {
            if(item != null) {
                itemLocations.UnequipItem(item);
            } 
        }


        public virtual void UnequiptItem(EEquiptSlot slot) {
            itemLocations.UnequipItem(slot);
        }


        public void UseRightItem(InputAction.CallbackContext context) {
            ItemEquipt requipt = itemLocations.GetRHandItem();
            // FIXME/TODO: This should call OnUse() of the item (if is IUsable)
            //               -- perhaps that also means OnUse() should take an AnimancerComponent?
            if(requipt != null) {
                IUsable usable = requipt as IUsable;
                if(usable != null) {
                    usable.OnUse(this);
                }
            }
        }


#region 


        public void MeleeAttack(IWeapon weapon)
        {
            throw new System.NotImplementedException();
        }

        public void RangedAttack(IWeapon weapon, Vector3 direction)
        {
            throw new System.NotImplementedException();
        }

        public void Block()
        {
            throw new System.NotImplementedException();
        }

        public void DrawWeapon(IWeapon weapon)
        {
            throw new System.NotImplementedException();
        }

        public void SheatheWeapon(IWeapon weapon)
        {
            throw new System.NotImplementedException();
        }

        public void SwitchWeapon(IWeapon currentWeapon, IWeapon newWeapon)
        {
            throw new System.NotImplementedException();
        }

        public void AttackBlocked()
        {
            throw new System.NotImplementedException();
        }


#endregion
#endregion



    }


}