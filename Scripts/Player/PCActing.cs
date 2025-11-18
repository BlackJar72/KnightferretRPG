using System.Collections;
using Animancer;
using kfutils.rpg.ui;
using UnityEngine;
using UnityEngine.InputSystem;


namespace kfutils.rpg {

    public class PCActing : PCMoving, ICombatant
    {

        [SerializeField] protected PlayerInventory inventory;
        [SerializeField] protected Spellbook spellbook;
        [SerializeField] protected SpellEquiptSlot equiptSpell;
        [SerializeField] ParticleSystem magicSparkles;

        [SerializeField] protected CharacterEquipt itemLocations;
        [SerializeField] protected BlockArea blockArea;

        [Tooltip("There need to be three of these, for high, mid, and low (in that order)")]
        [SerializeField] Transform[] armsPositions;


        protected AnimancerLayer actionLayer;
        protected AnimancerLayer armsActionLayer;
        protected AnimancerState actionState;
        protected AnimancerState armsActionState;


        protected AnimancerLayer leftLayer;
        protected AnimancerLayer armsLeftLayer;
        protected AnimancerState leftState;
        protected AnimancerState armsLeftState;

        protected bool blocking;
        protected bool chargingAction;
        protected float chargeTimer;
        protected float castTimer;
        protected bool isCasting;

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
        protected InputAction quickSlot3Action;
        protected InputAction quickSlot4Action;
        protected InputAction quickSlot5Action;
        protected InputAction quickSlot6Action;
        protected InputAction quickSlot7Action;
        protected InputAction quickSlot8Action;
        protected InputAction quickSlot9Action;
        protected InputAction screenshot;
        protected InputAction togglePauseMenu;


        // Accessor Properties
        public PlayerInventory Inventory => inventory;
        public CharacterInventory CharInventory => Inventory;
        public Spellbook Spells => spellbook;
        public EquiptmentSlots EquiptItems => inventory.Equipt;
        public SpellEquiptSlot EquiptSpell => equiptSpell;
        public bool IsCasting => isCasting;

        public AnimancerLayer ActionLayer => actionLayer;
        public AnimancerState ActionState => actionState;
        public AnimancerState ArmsActionState => armsActionState;

        public Transform[] ArmsPositions => armsPositions;
        public bool IsBlocking => blocking;


        public enum ArmsPos
        {
            high = 0, mid = 1, low = 2
        }


        protected override void Awake()
        {
            inventory.SetOwner(this); 
            blockArea.SetOwner(this);
            InventoryPanel[] invPanels = (InventoryPanel[])FindObjectsByType(typeof(InventoryPanel), FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < invPanels.Length; i++) invPanels[i].InitHotbarActions();
            if (inventory == null) inventory = GetComponent<PlayerInventory>();
            if (spellbook == null) spellbook = GetComponent<Spellbook>();
            base.Awake();
            InitInput();
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            base.Start();
            equiptSpell.InitForPC();
            // Right / General
            actionLayer = animancer.Layers[1];
            armsActionLayer = arms.Layers[1];
            actionState = moveState;
            armsActionState = armsMoveState;
            // Left
            leftLayer = animancer.Layers[2];
            armsLeftLayer = arms.Layers[2];
            leftState = moveState;
            armsLeftState = armsMoveState;
        }


        public override void ResetCharacter()
        {
            base.ResetCharacter();
            equiptSpell.UnequiptSpell();
            inventory.Clear();
            spellbook.Clear();
            inventory.Equipt.Clear();
            inventory.Hotbar.Clear(); 
            blockArea.LowerBlock();
            SetArmsPos(ArmsPos.high);
            blocking = false;
            chargingAction = false;
            chargeTimer = 0.0f;
            castTimer = 0.0f;
            isCasting =  false;
        }


        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (chargingAction && (chargeTimer < Time.time)) UseChargedAction();
            else if (isCasting && (castTimer < Time.time)) CastSpell();
        }


        public void PreSaveEquipt()
        {
            inventory.Equipt.PreSave();
        }


        /// <summary>
        /// This sets Acting data during loads; living is included since.
        /// </summary>
        /// <param name="data"></param>
        protected void SetFromActiningData(PCData data)
        {
            // TODO
        }


        #region Input

        private void InitInput()
        {
            toggleInventoryAction = input.actions["OpenCloseInventory"];
            rightAttackAction = input.actions["RightUseAttack"];
            leftBlcokAction = input.actions["LeftUseBlock"];
            activateObjectAction = input.actions["Interact"];
            castSpellAction = input.actions["CastSpell"];
            pauseAction = input.actions["Pause"];
            togglePauseMenu = input.actions["TogglePauseMenu"];
            // Hotbar Quickslots
            quickSlot1Action = input.actions["Hotbar1"];
            quickSlot2Action = input.actions["Hotbar2"];
            quickSlot3Action = input.actions["Hotbar3"];
            quickSlot4Action = input.actions["Hotbar4"];
            quickSlot5Action = input.actions["Hotbar5"];
            quickSlot6Action = input.actions["Hotbar6"];
            quickSlot7Action = input.actions["Hotbar7"];
            quickSlot8Action = input.actions["Hotbar8"];
            quickSlot9Action = input.actions["Hotbar9"];

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


        protected override void Die()
        {
            if (alive)
            {
                DisableAction();
                DisableUIActions();
                StopAction();
                base.Die();
            }
        }


        protected override void MakeAlive()
        {
            base.MakeAlive();
            EnableAction();
            EnableUIActions();
        }


        protected void EnableUIActions()
        {
            toggleInventoryAction.started += ToggleCharacterSheet;
            InventoryManagement.toggleCharacterSheet += ToggleCharacterSheet;
            togglePauseMenu.started += TogglePauseMenu;
            pauseAction.started += ApplyPauseButton;
        }


        protected void DisableUIActions()
        {
            toggleInventoryAction.started -= ToggleCharacterSheet;
            InventoryManagement.toggleCharacterSheet -= ToggleCharacterSheet;
            togglePauseMenu.started -= TogglePauseMenu;
            pauseAction.started -= ApplyPauseButton;
        }


        protected override void EnableControls()
        {
            EnableMovement();
            EnableAction();
        }


        protected override void DisableControls()
        {
            DisableMovement();
            DisableAction();
        }


        protected virtual void EnableAction()
        {
            rightAttackAction.started += StartChargeRightItem;
            rightAttackAction.canceled += UseRightItem;
            leftBlcokAction.started += BlockLeftItem;
            leftBlcokAction.canceled += UseLeftItem;
            if (this is not PCTalking) activateObjectAction.started += Interact;
            castSpellAction.started += StartCastSpell;
            castSpellAction.canceled += StopCastSpell;
            // Hotbar Quickslots
            quickSlot1Action.canceled += QuickSlot1;
            quickSlot2Action.canceled += QuickSlot2;
            quickSlot3Action.canceled += QuickSlot3;
            quickSlot4Action.canceled += QuickSlot4;
            quickSlot5Action.canceled += QuickSlot5;
            quickSlot6Action.canceled += QuickSlot6;
            quickSlot7Action.canceled += QuickSlot7;
            quickSlot8Action.canceled += QuickSlot8;
            quickSlot9Action.canceled += QuickSlot9;
            chargingAction = false;
        }


        protected virtual void DisableAction()
        {
            rightAttackAction.started -= StartChargeRightItem;
            rightAttackAction.canceled -= UseRightItem;
            leftBlcokAction.started -= BlockLeftItem;
            leftBlcokAction.canceled -= UseLeftItem;
            if (this is not PCTalking) activateObjectAction.started -= Interact;
            castSpellAction.started -= StartCastSpell;
            castSpellAction.canceled -= StopCastSpell;
            // Hotbar Quickslots
            quickSlot1Action.canceled -= QuickSlot1;
            quickSlot2Action.canceled -= QuickSlot2;
            quickSlot3Action.canceled -= QuickSlot3;
            quickSlot4Action.canceled -= QuickSlot4;
            quickSlot5Action.canceled -= QuickSlot5;
            quickSlot6Action.canceled -= QuickSlot6;
            quickSlot7Action.canceled -= QuickSlot7;
            quickSlot8Action.canceled -= QuickSlot8;
            quickSlot9Action.canceled -= QuickSlot9;
            chargingAction = false;
        }


        // A temptoray dummy method for un-implemented actions
        protected void Dummy(InputAction.CallbackContext context) {/*DO NOTHING*/}


        #endregion

        #region QuickSlots

        public void QuickSlot1(InputAction.CallbackContext context) => inventory.UseHotbar(0);
        public void QuickSlot2(InputAction.CallbackContext context) => inventory.UseHotbar(1);
        public void QuickSlot3(InputAction.CallbackContext context) => inventory.UseHotbar(2);
        public void QuickSlot4(InputAction.CallbackContext context) => inventory.UseHotbar(3);
        public void QuickSlot5(InputAction.CallbackContext context) => inventory.UseHotbar(4);
        public void QuickSlot6(InputAction.CallbackContext context) => inventory.UseHotbar(5);
        public void QuickSlot7(InputAction.CallbackContext context) => inventory.UseHotbar(6);
        public void QuickSlot8(InputAction.CallbackContext context) => inventory.UseHotbar(7);
        public void QuickSlot9(InputAction.CallbackContext context) => inventory.UseHotbar(8);


        #endregion


        #region Saving / Loading


        protected PCData GetActionData(PCData data)
        {
            return data;
        }


        protected void TogglePauseMenu(InputAction.CallbackContext context)
        {
            gameManager.UI.TooglePauseMenu();
        }


        protected void ApplyPauseButton(InputAction.CallbackContext context)
        {
            gameManager.UI.PauseButtonHit();
        }


        #endregion


        public void GetAimParams(out AimParams aim)
        {
            aim.from = playerCam.transform.position;
            aim.toward = playerCam.transform.forward;
        }


        public AnimancerState PlayAction(AvatarMask mask, ITransition animation, float time = 0)
        {
            actionLayer.SetMask(mask);
            actionState = actionLayer.Play(animation);
            actionState.Time = time;
            armsActionLayer.SetMask(mask);
            armsActionState = armsActionLayer.Play(animation);
            armsActionState.Time = time;
            return actionState;
        }


        public AnimancerState PlayAction(AvatarMask mask, ITransition animation, System.Action onEnd, float time = 0.0f, float delay = 1.0f)
        {
            actionLayer.SetMask(mask);
            actionState = actionLayer.Play(animation);
            actionState.Time = time;
            armsActionLayer.SetMask(mask);
            armsActionState = armsActionLayer.Play(animation);
            armsActionState.Time = time;
            StartCoroutine(DoPostActionCode(onEnd, delay));
            return actionState;
        }


        public void StopAction()
        {
            actionLayer.StartFade(0, 0.1f);
            armsActionLayer.StartFade(0, 0.1f);
        }


        public IEnumerator DoPostActionCode(System.Action onEnd, float delay = 1.0f)
        {
            yield return new WaitForSeconds(delay);
            onEnd();
        }


        protected virtual void Interact(InputAction.CallbackContext context)
        {
            AimParams aim;
            GetAimParams(out aim);
            RaycastHit hit;
            IInteractable interactable = null;
            // First we use a raycast to prioritize objects directly aimed at, and avoid the problem
            // from Thief 3 where objects to side are grabbed because they were closer.
            if (Physics.Raycast(aim.from, aim.toward, out hit, 2f, GameConstants.interactable))
            {
                interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Use(gameObject);
                }
            }
            // If this fails, try a fairly narrow sphere cast to decrease the required percision so it doesn't feel like 
            // the interaction has to be "pixel perfect."
            if ((interactable == null) && Physics.SphereCast(aim.from, 0.1f, aim.toward, out hit, 2f, GameConstants.interactable))
            {
                interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Use(gameObject);
                }
            }
        }


        protected void StartCastSpell(InputAction.CallbackContext context)
        {
            if (EquiptSpell.currentSpell == null) return;
            chargingAction = false;
            isCasting = true;
            castTimer = Time.time + EquiptSpell.currentSpell.CastTime;
            magicSparkles.Play();
        }


        protected void CastSpell()
        {
            if (equiptSpell.currentSpell != null)
            {
                float cost = equiptSpell.currentSpell.ManaCost * attributes.manaCostFactor;
                if (mana.CanDoAction(cost))
                {
                    mana.UseMana(cost);
                    equiptSpell.currentSpell.SpellEffect.Cast(this);
                    AudioSource audio = GetComponent<AudioSource>();
                    if ((audio != null) && (equiptSpell.currentSpell.CastSound != null))
                    {
                        equiptSpell.CurrentSpell.CastSound.Play(audio);
                    }
                    if (equiptSpell.CurrentSpell.CastParticles != null)
                    {
                        // FIXME/TODO: Determine where they should be based on spell type (etc.)
                        Instantiate(equiptSpell.currentSpell.CastParticles, transform);
                    }
                    castTimer = Time.time + EquiptSpell.currentSpell.CastTime;
                }
                else StopCastSpell();
            }
            else StopCastSpell();
        }


        protected void StopCastSpell(InputAction.CallbackContext context) => StopCastSpell();
        protected void StopCastSpell()
        {
            isCasting = false;
            magicSparkles.Stop();
            StopAction();
        }


        // FIXME??? Should this be in PCTalking, so as to also disable character interaction (probably)
        protected virtual void ToggleCharacterSheet(InputAction.CallbackContext context)
        {
            ToggleCharacterSheet();
        }


        // FIXME??? Should this be in PCTalking, so as to also disable character interaction (probably)
        public virtual void ToggleCharacterSheet()
        {
            AllowActions(!(gameManager.UI.ToggleCharacterSheet() || GameManager.Instance.UI.PauseMenuVisible));
        }


        // FIXME??? Should this be in PCTalking, so as to also disable character interaction (probably)
        public virtual void AllowActions(bool allow)
        {
            if (!allow)
            {
                DisableMovement();
                DisableAction();
                moveLayer.Speed = 0;
                actionLayer.Speed = 0;
            }
            else
            {
                EnableMovement();
                EnableAction();
                moveLayer.Speed = 1;
                actionLayer.Speed = 1;
            }
            chargingAction = false;
        }


        public virtual void AddToMainInventory(ItemStack stack)
        {
            GetComponent<Inventory>().AddToFirstEmptySlot(stack);
        }


        #region Equipment


        public virtual void EquiptItemToBody(ItemStack item)
        {
            if (item != null)
            {
                ItemEquipt equipt = itemLocations.EquipItem(item);
                IUsable usable = equipt as IUsable;
                if (usable != null)
                {
                    usable.OnEquipt(this);
                }
            }
        }


        public void RemoveEquiptAnimation()
        {
            actionLayer.StartFade(0);
        }


        public virtual void UnequiptItemFromBody(ItemStack item)
        {
            if (item != null)
            {
                itemLocations.UnequipItem(item);
            }
        }


        public virtual void UnequiptItemFromBody(EEquiptSlot slot)
        {
            itemLocations.UnequipItem(slot);
        }


        public void StartChargeRightItem(InputAction.CallbackContext contex)
        {
            if (isCasting) return;
            chargingAction = true;
            chargeTimer = Time.time + GameConstants.TIME_TO_CHARGE_ACTIONS;
        }


        public void UseRightItem(InputAction.CallbackContext context)
        {
            if (isCasting) return;
            ItemEquipt requipt = itemLocations.GetRHandItem();
            ItemShield shield = itemLocations.GetLHandItem() as ItemShield;
            if (chargingAction && (requipt != null))
            {
                IUsable usable = requipt as IUsable;
                if ((usable != null) && !blocking)
                {
                    if (stamina.UseStamina(usable.StaminaCost))
                    {
                        usable.OnUse(this);
                    }
                }
                else if (blocking && (shield != null) && stamina.UseStamina(shield.StaminaCost))
                {
                    shield.OnUse(this);
                }
            }
            chargingAction = false;
        }


        public void UseChargedAction()
        {
            chargingAction = false;
            ItemEquipt requipt = itemLocations.GetRHandItem();
            ItemShield shield = itemLocations.GetLHandItem() as ItemShield;
            if (requipt)
            {
                IUsable usable = requipt as IUsable;
                if ((usable != null) && !blocking)
                {
                    if (stamina.UseStamina(usable.PowerAttackCost))
                    {
                        usable.OnUseCharged(this);
                    }
                }
                else if (blocking && (shield != null) && stamina.UseStamina(shield.StaminaCost))
                {
                    shield.OnUse(this);
                }
            }
        }


        public void BlockLeftItem(InputAction.CallbackContext context)
        {
            ItemEquipt lequipt = itemLocations.GetLHandItem();
            ItemEquipt requipt = itemLocations.GetRHandItem();

            //Debug.Log("BlockLeftItem(InputAction.CallbackContext context)");
            blocking = false;
            if ((lequipt != null) && (lequipt is IBlockItem))
            {
                blocking = true;
                Block(lequipt);
            }
            else if ((requipt != null) && (requipt is IBlockItem))
            {
                blocking = true;
                Block(requipt);
            }
        }


        public void UseLeftItem(InputAction.CallbackContext context)
        {
            ItemEquipt lequipt = itemLocations.GetLHandItem();
            ItemEquipt requipt = itemLocations.GetRHandItem();
            if (blocking)
            {
                blocking = false;
                if ((lequipt != null) && (lequipt is IBlockItem)) EndBlock(lequipt);
                if ((requipt != null) && (requipt is IBlockItem)) EndBlock(requipt);

            }
            else if ((lequipt != null) && (lequipt is IUsable usable))
            {
                if (stamina.UseStamina(usable.StaminaCost)) usable.OnUse(this);
            }
        }


        public void SetArmsPos(ArmsPos armsPos)
        {
            arms.transform.localPosition = armsPositions[(int)armsPos].localPosition;
        }


        #region 



        public void Block(ItemEquipt item)
        {
            if (item is IBlockItem blocker)
            {
                blocker.StartBlock();
                blockArea.RaiseBlock(blocker);
                blocking = true;
            }
        }


        public void EndBlock(ItemEquipt item)
        {
            if (item is IBlockItem blocker)
            {
                blocker.EndBlock();
                blockArea.LowerBlock();
                blocking = false;
            }
        }


        public BlockArea GetBlockArea() => blockArea;


        public void BreakBlock(IBlockItem blocker)
        {
            blockArea.LowerBlock();
            if (blocker != null)
            {
                blocker.EndBlock();
            }
            blocking = false;
            // TODO?? Stagger when block is dropped due to stamina depletion.          
        }


        private Damages BlockDamageHelper(Damages damage, BlockArea blockArea)
        {
            //Debug.Log("private Damages BlockDamageHelper(Damages damage, BlockArea blockArea)");
            //Debug.Log("Damege = " + damage);
            float shock = damage.shock;
            float reduction = damage.shock * blockArea.blockItem.BlockAmount;
            float cost = reduction * (1.0f - blockArea.blockItem.Stability);
            float paid = Mathf.Min(cost, stamina.currentStamina);
            reduction *= (paid / cost);
            stamina.UseStamina(paid);
            damage *= (shock - reduction) / shock;
            //Debug.Log("Damege = " + damage);
            if (stamina.currentStamina < 1) BreakBlock(blockArea.blockItem);
            return damage;
        }


        public void BlockDamage(Damages damage, BlockArea blockArea)
        {
            if (Time.time > (blockArea.BlockTime + blockArea.blockItem.ParryWindow))
            {
                damage = BlockDamageHelper(damage, blockArea);
                TakeDamage(damage);
            }
        }


        public void BlockDamage(DamageData damage, BlockArea blockArea)
        {
            // FIXME: CHeck if attack is parryable
            if (Time.time > (blockArea.BlockTime + blockArea.blockItem.ParryWindow))
            {
                damage.damage = BlockDamageHelper(damage.damage, blockArea);
                TakeDamage(damage);
                blockArea.blockItem.BeHit();
            }
            else if ((damage.weapon != null) && damage.weapon.Parriable)
            {
                //Debug.Log("Parry!");
                if (damage.attacker is EntityActing enemyActor)
                {
                    enemyActor.Stagger(2.5f);
                    enemyActor.SetParried(true);
                    blockArea.blockItem.BeParried();
                }
            }
        }


        public void RangedAttack(IWeapon weapon, Vector3 direction)
        {
            throw new System.NotImplementedException();
        }
        


        #endregion
        #endregion



    }


}