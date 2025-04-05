using TMPro;
using UnityEngine;


namespace kfutils.rpg {
    
    public class PlayerInventory : CharacterInventory {
        
        [SerializeField] Money money;

        [SerializeField] TMP_Text moneyText;
        [SerializeField] TMP_Text weightText;





        private void OnEnable() {
            InventoryManager.inventoryUpdated += UpdateBottomBar;
            InventoryManager.inventorySlotUpdated += UpdateBottomBar;
            UpdateBottomBar(this);
        }


        private void  OnDisable() {
            InventoryManager.inventoryUpdated -= UpdateBottomBar;
            InventoryManager.inventorySlotUpdated -= UpdateBottomBar;
        }


        public int UpdateWeight() {
            float weight = Mathf.RoundToInt(CalculateWeight() + equipt.CalculateWeight());
            PCMoving pc = GetComponent<PCMoving>();
            if(pc != null) {
                pc.SetWeightForMovement(weight);
            }
            return Mathf.RoundToInt(weight);
        }


        private void UpdateBottomBar(IInventory<ItemStack> inv, int slot) { UpdateBottomBar(inv); }
        private void UpdateBottomBar(IInventory<ItemStack> inv) {
            if((inv == this) || (inv == equipt)) {
                weightText.SetText("Weight: " + UpdateWeight());                
            }
            moneyText.SetText("Money: " + money.GetGoodMoneyString());
        }


        // TODO: Handle money changes


        


    }

}