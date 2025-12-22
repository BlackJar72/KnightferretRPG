using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace kfutils.rpg.ui {

    public class EquiptmentPanel : MonoBehaviour, IRedrawing {

        [Tooltip("This is the backing inventory, which must be an instace of type Equiptment slots.")]
        [SerializeField] CharacterInventory mainInventory;
        private EquiptmentSlots inventory;

        [Tooltip("This should hold all the actual slots that are childrn of this (but not empties used as place holder) exactly once.")]
        [SerializeField] EquipmentSlotUI[] slots = new EquipmentSlotUI[11];
        [Tooltip("The slots holding rings; these should also be in the slots array.")]
        [SerializeField] SpellEquiptSlot spell;
        [SerializeField] InventoryPanel mainInventoryPanel;

        public InventoryPanel MainInventoryPanel => mainInventoryPanel;



        private void Awake() {
            inventory = mainInventory.Equipt;
            for(int i = 0; i < slots.Length; i++) {
                slots[i].inventory = inventory;
                slots[i].slotNumber = i;
                slots[i].equiptPanel = this;
            }
        }
        
        
        // private void Start() {}


        private void OnEnable() {
            InventoryManagement.inventoryUpdated += UpdateInventory;
            InventoryManagement.inventorySlotUpdated += UpdateSlot;
            InventoryManagement.inventoryUpdatedAll += UpdateInventoryAll;
            Redraw();
        }


        private void OnDestroy() {
            InventoryManagement.inventoryUpdated -= UpdateInventory;
            InventoryManagement.inventorySlotUpdated -= UpdateSlot;
            InventoryManagement.inventoryUpdatedAll -= UpdateInventoryAll;
        }


        public void Redraw()
        {
            InventoryManagement.AddRedraw(this);
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


        private void UpdateInventoryAll() {
            Redraw();
        }


        private void UpdateSlot(IInventory<ItemStack> inv, int slot) {
            if(inventory == inv) {
                if((slot < slots.Length) && (slot > 0) 
                            && slots[slot].item.item.IsStackable) {
                    slots[slot].SetText(slots[slot].item.stackSize);
                }
            }
        }


        public EquipmentSlotUI GetSlotForEquipt(ItemPrototype item) {
            EEquiptSlot  equiptType = item.EquiptType;
            switch(equiptType) {
                case EEquiptSlot.HEAD: return slots[0];
                case EEquiptSlot.BODY: return slots[1];
                case EEquiptSlot.ARMS: return slots[2];
                case EEquiptSlot.LEGS: return slots[8];
                case EEquiptSlot.FEET:  return slots[9];
                case EEquiptSlot.RHAND: return slots[4];
                case EEquiptSlot.LHAND: return slots[3];
                case EEquiptSlot.HANDS: return slots[4];
                case EEquiptSlot.RING: {
                    if(slots[6].item.item == null) return slots[6];
                    else return slots[5];
                }
                case EEquiptSlot.NECK: return slots[10];
                case EEquiptSlot.AMMO: return slots[7];
                default: return null;
            }
        }


        public void ForwardUpdate() {
            mainInventory.SignalUpdate();
        }


        public void RespondToHotbar(SlotData slot) {
            slots[slot.invSlot].EquipItem();
        }



    }


}
