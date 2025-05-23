using System.Collections;
using UnityEngine;



namespace kfutils.rpg.ui {

    public class HotbarUI : MonoBehaviour
    {

        [SerializeField] HotBar hotBar;


        public HotBar SlotData => hotBar;


        public SlotData GetSlot(int index) => hotBar.GetSlot(index);


        private void OnEnable()
        {
            InventoryManagement.hotBar = hotBar;
            InventoryManagement.slotsSwappedEvent += OnSlotsSwapped;
            InventoryManagement.slotEmptiedEvent += OnSlotEmptied;
            WorldManagement.GameReloaded += OnGameReloaded;

        }


        private void OnDisable()
        {
            InventoryManagement.slotsSwappedEvent -= OnSlotsSwapped;
            InventoryManagement.slotEmptiedEvent -= OnSlotEmptied;
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
            Debug.Log("Event Recieved: OnGameReloaded()");
            hotBar.CopyInto(InventoryManagement.hotBar);
            InventoryManagement.hotBar = hotBar;
            RedrawForLoad();
            StartCoroutine(DelayedRedraw());
        }


        IEnumerator DelayedRedraw()
        {
            yield return new WaitForEndOfFrame();
            Redraw();
        }




    }


}