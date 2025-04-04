using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;


namespace kfutils.rpg.ui {

    public class EquiptmentPanel : MonoBehaviour, IRedrawing {

        [Tooltip("This is the backing inventory, which must be an instace of type Equiptment slots.")]
        [SerializeField] EquiptmentSlots inventory;

        [Tooltip("This should hold all the actual slots that are childrn of this (but not empties used as place holder) exactly once.")]
        [SerializeField] EquipmentSlotUI[] slots = new EquipmentSlotUI[11];
        [Tooltip("The slots holding rings; these should also be in the slots array.")]
        [SerializeField] EquipmentSlotUI[] rings = new EquipmentSlotUI[2];
        [Tooltip("The slots represening the hands, for held items; these should also be in the slots array.")]
        [SerializeField] EquipmentSlotUI[] hands = new EquipmentSlotUI[2];
        [Tooltip("The slots slot for the currently equipt spell; this is not found in any other array.")]
        [SerializeField] SpellEquiptSlot spell;


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


        private void UpdateInventory(IInventory<ItemStack> inv) {
            if(inv == inventory) Redraw();
        }


        private void UpdateSlot(IInventory<ItemStack> inv, int slot) {
            if(inventory == inv) {
                if((slot < slots.Length) && (slot > 0) 
                            && slots[slot].item.item.IsStackable) {
                    slots[slot].SetText(slots[slot].item.stackSize);
                }
            }
        }


        public void EquipItemFromSlot(InventorySlotUI externalSlot) { 
            // Not for swapping within eqipt area; must be external (really, should be main inventory)
            if(externalSlot.inventory is EquiptmentPanel) return;
            if((externalSlot.item.item.EquiptType == EEquiptSlot.RRING) 
                            || (externalSlot.item.item.EquiptType == EEquiptSlot.LRING)) {
                if((rings[0].item == null) || (rings[0].item.item == null)) {
                    rings[0].SwapWith(externalSlot);
                } else {
                    rings[1].SwapWith(externalSlot);
                }
            } else if(externalSlot.item.item.EquiptType == EEquiptSlot.HANDS) {
                hands[0].SwapWith(externalSlot);
                InventorySlotUI extraSlot = externalSlot.inventoryPanel.GetSlotAt(externalSlot.inventory.FindFirstEmptySlot());
                if((hands[1] != null) && (hands[1].item != null) && (hands[1].item.item != null)) hands[1].SwapWith(extraSlot);
                // TODO/FIXME: Block off-hand while equipt; also needs to reverse effect when no two-handed weapon is equipt, 
                //             and to detect drag-based equipting.
            } else {
                for(int i = slots.Length - 1; i > -1; i--) { // Going backward to favor right hand (treated as dominant) 
                    if(externalSlot.item.item.FitsSlot(slots[i])) {
                        slots[i].SwapWith(externalSlot);
                        break;
                    }
                }
            }



        }



    }


}
