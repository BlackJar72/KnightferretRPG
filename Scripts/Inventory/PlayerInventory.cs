using kfutils.rpg.ui;
using TMPro;
using UnityEngine;


namespace kfutils.rpg {

    public class PlayerInventory : CharacterInventory
    {

        [SerializeField] TMP_Text moneyText;
        [SerializeField] TMP_Text weightText;
        [SerializeField] HotbarUI hotbarUI;
        [SerializeField] PCActing pc;

        public HotbarUI Hotbar => hotbarUI;


        protected override void Awake()
        {
            base.Awake();
            if (owner == null) owner = pc;            
        }


        public override void OnEnable()
        {
            base.OnEnable();
            equipt.belongsToPC = true;
            InventoryManagement.inventoryUpdated += UpdateBottomBar;
            InventoryManagement.inventorySlotUpdated += UpdateBottomBar;
            UpdateBottomBar(this);
        }


        private void OnDisable()
        {
            InventoryManagement.inventoryUpdated -= UpdateBottomBar;
            InventoryManagement.inventorySlotUpdated -= UpdateBottomBar;
        }


        public int UpdateWeight()
        {
            float weight = Mathf.RoundToInt(CalculateWeight() + equipt.CalculateWeight());
            PCMoving pc = GetComponent<PCMoving>();
            if (pc != null)
            {
                pc.SetWeightForMovement(weight);
            }
            return Mathf.RoundToInt(weight);
        }


        private void UpdateBottomBar(IInventory<ItemStack> inv, int slot) { UpdateBottomBar(inv); }
        private void UpdateBottomBar(IInventory<ItemStack> inv)
        {
            if ((inv == this) || (inv == equipt))
            {
                weightText.SetText("Weight: " + UpdateWeight());
            }
            moneyText.SetText("Money: " + money.GetGoodMoneyString());
        }


        public override bool OwnedByPC => true;


        public void UseHotbar(int slotNumber)
        {
            SlotData slot = hotbarUI.GetSlot(slotNumber);
            if (slot.filled)
            {
                InventoryManagement.SigalHotbarActivated(slot);
            }
        }


        // TODO: Handle money changes
        


    }

}