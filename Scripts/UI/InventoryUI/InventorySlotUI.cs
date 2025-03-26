using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace kfutils.rpg.ui {

    public class InventorySlotUI : MonoBehaviour {

        
        public Image icon;
        public Inventory inventory;
        public int slotNumber;
        [SerializeField] TMP_Text numberText;
        
        public ItemStack item;


        public virtual void SwapWith(InventorySlotUI other) {
            if((other.item.item == item.item) && item.item.Inv.IsStackable) {
                other.item.stackSize += item.stackSize;
                inventory.RemoveItem(item);
            } else {
                item.slot = other.slotNumber;
                other.item.slot = slotNumber;
                if(other.inventory != inventory) {
                    Inventory originalInv = inventory;
                    inventory.RemoveItem(item);
                    other.inventory.AddItem(item);
                    inventory = other.inventory;
                    other.inventory.RemoveItem(item);
                    originalInv.AddItem(other.item);
                    other.inventory = originalInv;
                }
            }
            inventory.SignalUpdate();           
        }


        

        /// <summary>
        /// Remove a number of items from the given slot. Will never remove more
        /// that there are.
        /// </summary>
        public void RemoveFromSlot(int number) {
            number = Mathf.Min(number, item.stackSize);
            item.stackSize -= number;
            if(item.stackSize < 1) {
                inventory.RemoveItem(item);
                icon.sprite = null;
                icon.enabled = false;
            } else if(item.item.Inv.IsStackable && (numberText != null)) {
                numberText.SetText(item.stackSize.ToString());
            } 
        }


        public void HideText() {
            numberText.gameObject.SetActive(false);
        }


        public void ShowText() {
            numberText.gameObject.SetActive(true);
        }


        public void SetText(int number) {
            numberText.SetText(number.ToString());
        }





    }

}
