using System;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

namespace kfutils.rpg.ui {

    public class InventoryPanel : MonoBehaviour {

        [SerializeField] InventorySlotUI slotPrefab = null;
        [SerializeField] Inventory inventory;
        [SerializeField] int minRows = 4;
        [SerializeField] int columns = 8;

        private int slots, rows;

        private bool shouldRedraw = false;

        private List<InventorySlotUI> inventorySlots = new();


        private void Awake() {
            inventory.inventoryUpdated += UpdateInventory;
            inventory.inventorySlotUpdated += UpdateSlot;
        }


        private void Start() {
            Redraw();
        }


        //FIXME: Created manager to handle this so all inventories in the world are not checking every frame
        void Update() {
            if(shouldRedraw) {
                DoRedraw();
                shouldRedraw = false;
            }
        }


        private void Redraw() {
            shouldRedraw = true;
        }


        private void DoRedraw() {
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
            for(int i = 0; i < inventory.inventory.Count; i++) {
                ItemStack stack = inventory.inventory[i];
                InventorySlotUI slotUI = inventorySlots[stack.slot];
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


        private void UpdateInventory(Inventory inv) {
            if(inv == inventory) Redraw();
        }


        private void GetInventorySize() {
            slots = inventory.GetLastSlot();
            rows = Mathf.Max(minRows, (slots / columns) + 2);
            slots = rows * columns;
        }


        private void UpdateSlot(Inventory inv, int slot) {
            if(inventory == inv) {
                if((slot < inventorySlots.Count) && (inventorySlots[slot] != null) 
                            && inventorySlots[slot].item.item.IsStackable) {
                    inventorySlots[slot].SetText(inventorySlots[slot].item.stackSize);
                }
            }
        }


    }


}