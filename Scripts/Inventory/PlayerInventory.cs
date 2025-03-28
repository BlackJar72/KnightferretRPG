using TMPro;
using UnityEngine;


namespace kfutils.rpg {
    
    public class PlayerInventory : Inventory {
        
        [SerializeField] EquiptmentSlots equiptment;
        [SerializeField] Money money;

        [SerializeField] TMP_Text moneyText;
        [SerializeField] TMP_Text weightText;





        private void OnEnable() {
            InventoryManager.inventoryUpdated += UpdateBottomBar;
            InventoryManager.inventorySlotUpdated += UpdateBottomBar;
            UpdateBottomBar(this);
        }


        private void  OnDisable() {
            //if (gameObject.activeInHierarchy) {}
            InventoryManager.inventoryUpdated -= UpdateBottomBar;
            InventoryManager.inventorySlotUpdated -= UpdateBottomBar;
        }


        private void UpdateBottomBar(IInventory inv, int slot) { UpdateBottomBar(inv); }
        private void UpdateBottomBar(IInventory inv) {
            if((inv == this) || (inv == equiptment)) {
                int weight = Mathf.RoundToInt(CalculateWeight() + equiptment.CalculateWeight());
                weightText.SetText("Weight: " + weight);
            }
            moneyText.SetText(money.GetSimpleMoneyString());
        }


        // TODO: Handle money changes


        


    }

}