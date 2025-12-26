using System.Collections.Generic;
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


        public void AddedStartingGear(InitialPCData initialData)
        {
            initialData.AddGearToInventory(this);
        }


        public int UpdateWeight()
        {
            float weight = Mathf.RoundToInt(CalculateWeight() + equipt.CalculateWeight());
            if (TryGetComponent<PCMoving>(out PCMoving pc))
            {
                pc.SetWeightForMovement(weight);
            }
            return Mathf.RoundToInt(weight);
        }


        private void UpdateBottomBar(IInventory<ItemStack> inv, int slot) { UpdateBottomBar(inv); }
        private void UpdateBottomBar(IInventory<ItemStack> inv)
        {
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if ((inv == this) || (inv == equipt))
            {
                weightText.SetText("Weight: " + UpdateWeight());
            }
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            moneyText.SetText("Money: " + money.GetGoodMoneyString());
        }


        public override bool OwnedByPC => true;


        public void UseHotbar(int slotNumber)
        {
            SlotData slot = hotbarUI.GetSlot(slotNumber);
            if (slot.filled && !pc.IsCasting)
            {
                // List<SlotData> overlap = hotbarUI.GetEquiptSlotsOverlapping(slot);
                // if(overlap != null) foreach(SlotData overlapping in overlap) 
                // {
                //     GameManager.Instance.UI.Inventory.RespondToHotbar(overlapping);
                // }
                InventoryManagement.SigalHotbarActivated(slot);
            }
        }


        // TODO: Handle money changes
        


    }

}