using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace kfutils.rpg.ui {

    public class InventoryPanel : MonoBehaviour, IRedrawing  {

        [SerializeField] protected InventorySlotUI slotPrefab = null;
        [SerializeField] protected Inventory inventory;
        [SerializeField] protected int minRows = 4;
        [SerializeField] protected int columns = 8;
        [SerializeField] EquiptmentPanel equiptPanel;
        [SerializeField] protected ScrollRect scrollRect;

        protected int slots, rows;

        protected List<InventorySlotUI> inventorySlots = new();

        public EquiptmentPanel EquiptPanel => equiptPanel;


        //private void Awake() {}
        //private void Start() {}

        protected virtual void Start() {
            if(inventory.GetType() == typeof(PlayerInventory)) InventoryManagement.HotbarActivatedEvent += RespondToHotbar;
        }


        protected virtual void OnEnable() {
            InventoryManagement.inventoryUpdated += UpdateInventory;
            InventoryManagement.inventorySlotUpdated += UpdateSlot;
            InventoryManagement.inventoryUpdatedAll += UpdateInventoryAll;
            scrollRect.verticalNormalizedPosition = 1.0f;
            Redraw();
        }


        protected virtual void OnDisable() {}


        void OnDestroy() {   
            InventoryManagement.inventoryUpdated -= UpdateInventory;
            InventoryManagement.inventorySlotUpdated -= UpdateSlot;         
            InventoryManagement.HotbarActivatedEvent -= RespondToHotbar;
            InventoryManagement.inventoryUpdatedAll -= UpdateInventoryAll;
        }


        public void ToggleCharacterSheet() {
            InventoryManagement.SignalToggleCharacterSheet();
        }


        public void Redraw() {
            InventoryManagement.AddRedraw(this);
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
                slotUI.inventoryPanel = this;
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


        public InventorySlotUI GetSlotAt(int index) => inventorySlots[index];

        public InventorySlotUI GetFirstEmptrySlot() {
            for(int i = 0; i < inventorySlots.Count; i++) {
                InventorySlotUI tmp = inventorySlots[i]; 
                if((tmp.item == null) || (tmp.item.item == null)) {
                    return tmp;
                }
            }
            return null;
        }


        protected void UpdateInventory(IInventory<ItemStack> inv) {
            if(inv == inventory) Redraw();
        }


        protected void UpdateInventoryAll() {
            Redraw();
        }


        protected void GetInventorySize() {
            RectTransform rt = GetComponent<RectTransform>();
            int lowestVisible = Mathf.CeilToInt(rt.localPosition.y / 128) + minRows;
            try{
                slots = inventory.GetLastSlot();
            } catch (Exception e) {
                Debug.Log(this.GetType() + ": " + gameObject.name);
                Debug.Log(inventory);
                throw e;
            }
            rows = Mathf.Min(Mathf.Max(lowestVisible, (slots / columns) + 2), 64);
            slots = rows * columns;
        }


        protected void UpdateSlot(IInventory<ItemStack> inv, int slot) {
            if(inventory == inv) {
                if((slot < inventorySlots.Count) && (inventorySlots[slot] != null) 
                            && inventorySlots[slot].item.item.IsStackable) {
                    inventorySlots[slot].SetText(inventorySlots[slot].item.stackSize);
                }
            }
        }


        public void RespondToHotbar(SlotData slot) {
            if(slot.inventory == InvType.MAIN) {
                for(int i = 0; i < inventorySlots.Count; i++) {
                    if(inventorySlots[i].item.slot == slot.invSlot) {
                        inventorySlots[i].EquipItem();
                        return;
                    }
                }
            }
            else if(slot.inventory == InvType.EQUIPT) {
                equiptPanel.RespondToHotbar(slot);
            }
        }


    }


}