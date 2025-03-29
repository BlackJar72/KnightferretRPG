using kfutils.rpg.ui;
using UnityEngine;


namespace kfutils.rpg {

    public class EquiptmentPanel : MonoBehaviour, IRedrawing {

        [Tooltip("This is the backing inventory, which must be an instace of type Equiptment slots.")]
        [SerializeField] EquiptmentSlots inventory;

        [Tooltip("This should hold all the actual slots that are childrn of this (but not empties used as place holder) exactly once.")]
        [SerializeField] EquipmentSlotUI[] slots = new EquipmentSlotUI[12];


        private void Awake() {
            for(int i = 0; i < slots.Length; i++) {
                slots[i].inventory = inventory;
                slots[i].slotNumber = i;
            }
        }
        
        
        // private void Start() {}


        private void OnEnable() {
            InventoryManager.inventoryUpdated += UpdateInventory;
            InventoryManager.inventorySlotUpdated += UpdateSlot;
            Redraw();
        }


        private void OnDisable() {
            InventoryManager.inventoryUpdated -= UpdateInventory;
            InventoryManager.inventorySlotUpdated -= UpdateSlot;
        }


        public void Redraw()
        {
            InventoryManager.AddRedraw(this);
        }


        public void DoRedraw()
        {
            for(int i = 0; i < slots.Length; i++) {
                slots[i].item = inventory.GetItem(i);
                slots[i].inventory = inventory;
                slots[i].slotNumber = i;
                if((slots[i].item.item == null) || (slots[i].item.item.Icon == null)) {
                    slots[i].icon.gameObject.SetActive(false);
                    slots[i].icon.sprite = null;
                    slots[i].HideText();
                } else {
                    slots[i].icon.gameObject.SetActive(true);
                    slots[i].icon.sprite = slots[i].item.item.Icon;
                    slots[i].SetText(slots[i].item.stackSize);
                    if(slots[i].item.item.IsStackable) {
                        slots[i].ShowText();
                    } else {
                        slots[i].HideText();
                    }
                }
            }
        }


        private void UpdateInventory(IInventory inv) {
            if(inv == inventory) Redraw();
        }


        private void UpdateSlot(IInventory inv, int slot) {
            if(inventory == inv) {
                if((slot < slots.Length) && (slot > 0) 
                            && slots[slot].item.item.IsStackable) {
                    slots[slot].SetText(slots[slot].item.stackSize);
                }
            }
        }



    }


}
