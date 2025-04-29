using UnityEngine;



namespace kfutils.rpg {
    
    [System.Serializable]
    public class HotBar {

        [SerializeField] SlotData[] slots = new SlotData[9];


        public SlotData GetSlot(int index) => slots[index];


        /// <summary>
        /// This if either (or both) slots have been moved, this should 
        /// move them.
        /// </summary>
        /// <param name="slot1"></param>
        /// <param name="slot2"></param>
        public void OnSlotsSwapped(SlotData slot1, SlotData slot2) {
            int cur1 = -1, cur2 = -1;
            for(int i = 0; i < slots.Length; i++) {
                if(slots[i] == slot1) cur1 = i;
                if(slots[i] == slot2) cur2 = i;
            }
            if(cur1 > -1) slots[cur1] = slot2;
            if(cur2 > -1) slots[cur2] = slot1;
        }


        public bool OnSlotEmptied(SlotData slot) {
            for(int i = 0; i < slots.Length; i++) {
                if(slots[i] == slot) {
                    slots[i].filled = false;
                    return true;
                }
            }
            return false;
        }


        public void CleanUpDuplicates(int slotNumber, SlotData slot) {
            for(int i = 0; i < slots.Length; i++) {
                if((i != slotNumber) && (slots[i] == slot)) {
                    slots[i].filled = false;
                }
            }
        }





    }


}
