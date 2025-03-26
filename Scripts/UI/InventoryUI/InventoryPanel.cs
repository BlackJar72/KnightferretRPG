using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

namespace kfutils.rpg.ui {

    public class InventoryPanel : MonoBehaviour {

        [SerializeField] InventorySlotUI slotPrefab = null;
        [SerializeField] Inventory inventory;
        [SerializeField] int minRows = 4;
        [SerializeField] int columns = 8;

        private int slots, rows;

        private List<InventorySlotUI> inventorySlots = new();


        private void Awake() {
            inventory.inventoryUpdated += UpdateInventory;
            inventory.inventorySlotUpdated += UpdateSlot;
        }


        private void Start() {
            Redraw();
        }


        private void Redraw() {
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
                slotUI.item = inventory.GetItemInSlot(i);
                slotUI.icon.sprite = slotUI.item.item.Inv.icon;
                if(slotUI.item.item.Inv.IsStackable) {
                    slotUI.SetText(slotUI.item.stackSize);
                    slotUI.ShowText();
                } else {
                    slotUI.HideText();
                }
                inventorySlots.Add(slotUI);
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
                            && inventorySlots[slot].item.item.Inv.IsStackable) {
                    inventorySlots[slot].SetText(inventorySlots[slot].item.stackSize);
                }
            }
        }


    }


}