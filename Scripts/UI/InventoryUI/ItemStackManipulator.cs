using TMPro;
using UnityEngine;
using UnityEngine.UI;



namespace kfutils.rpg.ui {

    /// <summary>
    /// Attaches to a gui ui panel on right-clicking an inventory slot, to allow for the item stack to be 
    /// split, and perhaps manipulated in oter ways.
    /// </summary>
    public class ItemStackManipulator : ShowOrHide {

        [SerializeField] GameObject menu;
        
        [SerializeField] GameObject spliting;
        [SerializeField] Slider slider;
        [SerializeField] TMP_InputField textInput;
        [SerializeField] Button splitButton;


        private ItemStack stack;
        private int newStackSize;
        private IInventory<ItemStack> inventory;


        protected void OnEnable() {
            InventoryManager.closeAllInvUI += Close;
            InventoryManager.closeStackMainUI += Close;
        }


        protected void OnDisable() {
            InventoryManager.closeAllInvUI -= Close;
            InventoryManager.closeStackMainUI -= Close;
        }


        public void Open(InventorySlotUI selected) {
            stack = selected.item;
            inventory = selected.inventory;
            UpdateSliderSettings();
            spliting.gameObject.SetActive(false);
            menu.gameObject.SetActive(true);
            UpdateSplittable();
            SetVisible();
        }


        public void Close() {
            spliting.gameObject.SetActive(false);
            menu.gameObject.SetActive(false);
            SetHidden();
        }


        private void UpdateSliderSettings() {
            slider.minValue = 1;
            slider.maxValue = stack.stackSize - 1;
            newStackSize = stack.stackSize / 2;
            slider.value = newStackSize;
            textInput.SetTextWithoutNotify(newStackSize.ToString());
        }


        private void UpdateSplittable() {
            splitButton.enabled = stack.stackSize > 1; 
            if(splitButton.enabled) {
                splitButton.GetComponent<Image>().color = Color.white;
            } else {
                splitButton.GetComponent<Image>().color = Color.gray;
            }
        }


        public void DropItem() {
            PCActing pc = EntityManagement.playerCharacter;
            stack.item.DropItemInWorld(pc.playerCam.transform, 0.5f);
            inventory.RemoveFromSlot(stack.slot, 1);
            UpdateSplittable();
            if(stack.stackSize < 1) Close();
        }


        public void ThrowItem() {
            PCActing pc = EntityManagement.playerCharacter;
            stack.item.DropItemInWorld(pc.playerCam.transform, 0.5f, 5.0f * (1  + pc.attributes.jumpForce) * Mathf.Sqrt(stack.item.Weight));
            inventory.RemoveFromSlot(stack.slot, 1);
            UpdateSplittable();
            if(stack.stackSize < 1) Close();
        }


        public void SwitchToSplit() {
            UpdateSliderSettings();
            menu.gameObject.SetActive(false);
            spliting.gameObject.SetActive(true);
        }


        public void SplitTextUpdate(string text) {
            int value;
            if(int.TryParse(text, out value)) {
                newStackSize = value;
                slider.value = newStackSize;
            } else {
                textInput.SetTextWithoutNotify(slider.value.ToString());
            }
        }


        public void SplitSliderUpdate(string text) {
            textInput.SetTextWithoutNotify(slider.value.ToString());
        }


        public void DoSplit() {
            ItemStack newStack = new ItemStack(stack.item, (int)slider.value);
            stack.stackSize -= newStack.stackSize;
            inventory.AddToFirstReallyEmptySlot(newStack);
            if(stack.stackSize < 2) Close();
            else {
                UpdateSliderSettings();
            }
        }


        /// <summary>
        ///  Will use or equipt the item, depending on type (and if using with equipting is allowed).
        /// </summary>
        public void Use() {
            // TODO!
        }





    }

}
