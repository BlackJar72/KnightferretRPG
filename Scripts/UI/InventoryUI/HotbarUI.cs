using UnityEngine;



namespace kfutils.rpg.ui {
    
    public class HotbarUI : MonoBehaviour {

        [SerializeField] HotBar hotBar;

        public SlotData GetSlot(int index) => hotBar.GetSlot(index);


        private void OnEnable() {
            InventoryManagement.slotsSwappedEvent += OnSlotsSwapped;
        }


        private void OnDisable() {
            InventoryManagement.slotsSwappedEvent -= OnSlotsSwapped;
        }


        public void OnSlotsSwapped(SlotData slot1, SlotData slot2) {
            hotBar.OnSlotsSwapped(slot1, slot2);
            Redraw();
        }


        public void Redraw() {

        }




    }


}