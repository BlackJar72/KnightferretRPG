using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace kfutils.rpg.ui {

    public class HotbarUI : MonoBehaviour
    {

        [SerializeField] HotBar hotBar;


        public HotBar SlotData => hotBar;


        public SlotData GetSlot(int index) => hotBar.GetSlot(index);


        public List<SlotData> GetEquiptSlotsOverlapping(SlotData next)
        {
            if((next.filled) && (next.inventory == InvType.MAIN)) {
                ItemStack stack = EntityManagement.playerCharacter.Inventory.GetItemInSlot(next.invSlot);
                if((stack != null) && (stack.item != null)) return hotBar.GetEquiptSlotsOverlapping(next);
            }
            return null;
        }


        private void OnEnable()
        {
            InventoryManagement.hotBar = hotBar;
            InventoryManagement.slotsSwappedEvent += OnSlotsSwapped;
            InventoryManagement.slotEmptiedEvent += OnSlotEmptied;
            InventoryManagement.HotbarActivatedEvent += RespondToHotbar;
            InventoryManagement.HotbarUpdateEvent += RespondToChanges;
            WorldManagement.GameReloaded += OnGameReloaded;
        }


        private void OnDisable()
        {
            InventoryManagement.slotsSwappedEvent -= OnSlotsSwapped;
            InventoryManagement.slotEmptiedEvent -= OnSlotEmptied;
            InventoryManagement.HotbarActivatedEvent -= RespondToHotbar;
            InventoryManagement.HotbarUpdateEvent -= RespondToChanges;
            WorldManagement.GameReloaded -= OnGameReloaded;
        }


        public void OnSlotsSwapped(SlotData slot1, SlotData slot2)
        {
            hotBar.OnSlotsSwapped(slot1, slot2);
        }


        public void OnSlotEmptied(SlotData slot)
        {
            if (hotBar.OnSlotEmptied(slot)) Redraw();
        }


        public void Redraw()
        {
            HotbarSlotUI[] slots = GetComponentsInChildren<HotbarSlotUI>();
            for (int i = 0; i < slots.Length; i++) slots[i].Redraw();
        }


        public void RedrawForLoad()
        {
            HotbarSlotUI[] slots = GetComponentsInChildren<HotbarSlotUI>();
            for (int i = 0; i < slots.Length; i++) slots[i].RedrawForLoad(this);
        }


        public void OnGameReloaded()
        {
            if (InventoryManagement.hotBar != null)
            {
                hotBar.CopyInto(InventoryManagement.hotBar);
                InventoryManagement.hotBar = hotBar;
                RedrawForLoad();
                StartCoroutine(DelayedRedraw());
            }
        }


        IEnumerator DelayedRedraw()
        {
            yield return null;
            Redraw();
        }


        public void RemoveEquiptSlot(int slot)
        {
            if (hotBar.RemoveEquiptSlot(slot)) Redraw();
        }


        public void Clear()
        {
            hotBar.Clear();
            Redraw(); 
        }


        private void RespondToHotbar(SlotData slotData) => RespondToChanges();


        private void RespondToChanges()
        {
            HotbarSlotUI[] slots = GetComponentsInChildren<HotbarSlotUI>();
            for (int i = 0; i < slots.Length; i++) slots[i].RedrawForLoad(this);
        }




    }


}