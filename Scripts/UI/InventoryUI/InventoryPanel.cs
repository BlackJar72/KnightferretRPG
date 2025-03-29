using System;
using System.Collections.Generic;
using UnityEngine;

namespace kfutils.rpg.ui {

    public class InventoryPanel : MonoBehaviour, IRedrawing  {

        [SerializeField] protected InventorySlotUI slotPrefab = null;
        [SerializeField] protected Inventory inventory;
        [SerializeField] protected int minRows = 4;
        [SerializeField] protected int columns = 8;

        protected int slots, rows;

        protected List<InventorySlotUI> inventorySlots = new();


        //private void Awake() {}
        //private void Start() {}


        protected virtual void OnEnable() {
            InventoryManager.inventoryUpdated += UpdateInventory;
            InventoryManager.inventorySlotUpdated += UpdateSlot;
            Redraw();
        }


        protected virtual void OnDisable() {
            InventoryManager.inventoryUpdated -= UpdateInventory;
            InventoryManager.inventorySlotUpdated -= UpdateSlot;
        }


        public void Redraw() {
            InventoryManager.AddRedraw(this);
        }


        public virtual void DoRedraw() {
            GetInventorySize();
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            inventorySlots.Clear();
            for(int i = 0; i < slots; i++)
            {
                InventorySlotUI slotUI = Instantiate(slotPrefab, transform);
                slotUI.inventory = inventory;
                slotUI.slotNumber = i;                
                slotUI.icon.gameObject.SetActive(false);
                slotUI.HideText();
                inventorySlots.Add(slotUI);
            }
            for(int i = 0; i < inventory.Count; i++) {
                ItemStack stack = inventory.GetByBackingIndex(i);
                InventorySlotUI slotUI = inventorySlots[stack.slot];
                if((stack == null) || (stack.item ==  null)) {
                    inventory.RemoveItem(stack);
                    continue;
                }
                slotUI.item = stack;
                if(slotUI.item != null) {
                    slotUI.icon.sprite = slotUI.item.item.Icon;
                    slotUI.icon.gameObject.SetActive(true);
                    if(slotUI.item.item.IsStackable) {
                        slotUI.SetText(slotUI.item.stackSize);
                        slotUI.ShowText();
                    } else {
                        slotUI.HideText();
                    }
                } 
            }
        }


        protected void UpdateInventory(IInventory inv) {
            if(inv == inventory) Redraw();
        }


        protected void GetInventorySize() {
            slots = inventory.GetLastSlot();
            rows = Mathf.Max(minRows, (slots / columns) + 2);
            slots = rows * columns;
        }


        protected void UpdateSlot(IInventory inv, int slot) {
            if(inventory == inv) {
                if((slot < inventorySlots.Count) && (inventorySlots[slot] != null) 
                            && inventorySlots[slot].item.item.IsStackable) {
                    inventorySlots[slot].SetText(inventorySlots[slot].item.stackSize);
                }
            }
        }


    }


}