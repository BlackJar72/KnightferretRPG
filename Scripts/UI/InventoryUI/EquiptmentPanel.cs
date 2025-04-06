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



        private void Awake() {
            inventory = mainInventory.Equipt;
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


        public EquipmentSlotUI GetSlotForEquipt(EEquiptSlot equiptType) {
            switch(equiptType) {
                case EEquiptSlot.HEAD: return slots[0];
                case EEquiptSlot.BODY: return slots[1];
                case EEquiptSlot.ARMS: return slots[2];
                case EEquiptSlot.LEGS: return slots[8];
                case EEquiptSlot.FEET:  return slots[9];
                case EEquiptSlot.RHAND: return slots[4];
                case EEquiptSlot.LHAND: return slots[3];
                case EEquiptSlot.HANDS: return slots[4];
                case EEquiptSlot.RRING: return slots[6];
                case EEquiptSlot.LRING: return slots[5];
                case EEquiptSlot.NECK: return slots[10];
                case EEquiptSlot.BELT: return slots[7];
                default: return null;
            }
        }


        public void ForwardUpdate() {
            mainInventory.SignalUpdate();
        }


        // TODO: Add a method to get for a given EEquiptSlot and return is, so that the swap method can 
        //       be called on it with the clicked InventorySlotUI as the argument.  This will emulate 
        //       dragging, as a drag will end up calling the EquiptSlotUI through its OnPointerDrop method.
        //       Two-handing can then be handled on the inventory back-end and done strictly to work with 
        //       drag-and-drop and clicking will excatly immitate dragging and dropping on the relevant 
        //       UI slot.  In contrast, ring selection can be done in the context here in the context of 
        //       selecting a slot to return, as rings are not two handed by simply need use the first 
        //       available slot, or (probably) the second if both are full.  (Two handed items may still 
        //       be a pain to work out.)  This should work better and be far simpler than trying to handle 
        //       the swap here while also dealing with two handing at the same time.
        //
        //       This could be accomplished either by a switch statement selecting hand choosen slots, or by 
        //       iterating (backward) through the array to find the first valid slot.  Pros and cons to both 
        //       approachnes: Iteration ensure the slots will not deviate from those defined in the editor 
        //       due to changing the prefabs, while a switch (or similar) based aproach would make handling 
        //       special cases (rings) a lot simpler to handle.



    }


}
