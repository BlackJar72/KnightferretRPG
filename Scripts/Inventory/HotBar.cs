using System.Collections.Generic;
using UnityEngine;



namespace kfutils.rpg {

    [System.Serializable]
    public class HotBar
    {

        [SerializeField] SlotData[] slots = new SlotData[9];


        public SlotData GetSlot(int index) => slots[index];


        public List<SlotData> GetEquiptSlotsOverlapping(SlotData next)
        {
            ItemStack itemi;
            List<SlotData> equipt = new();
            List<(int, SlotData)> debugging = new();
            EquiptmentSlots pcslots = EntityManagement.playerCharacter.Inventory.Equipt;
            int nextMask = EntityManagement.playerCharacter.Inventory.GetItemInSlot(next.invSlot).item.GetSlotMask();
            Debug.Log("nextMask = " + nextMask);
            for(int i = 0; i < slots.Length; i++)
            {
                SlotData sloti = slots[i];
                if((sloti == next) || (!sloti.filled) || (sloti.inventory != InvType.EQUIPT)) continue;
                itemi = pcslots.GetItemInSlot(sloti.invSlot);
                if((itemi != null) && (itemi.item != null) && (itemi.item.GetSlotFlag() & nextMask) > 0) equipt.Add(sloti);
                // DEBUG BELOW
                if((itemi != null) && (itemi.item != null) && (itemi.item.GetSlotFlag() & nextMask) > 0) debugging.Add((i + 1, sloti));
                if((itemi != null) && (itemi.item != null) && (itemi.item.GetSlotFlag() & nextMask) > 0) Debug.Log("itemi.item.GetSlotFlag() = " + itemi.item.GetSlotFlag());
            }
            Debug.Log(debugging.ToMultiString());
            return equipt; 
        }



        public void CopyInto(HotBar other)
        {
            for (int i = 0; i < slots.Length; i++) slots[i] = other.slots[i];
        }


        /// <summary>
        /// This if either (or both) slots have been moved, this should 
        /// move them.
        /// </summary>
        /// <param name="slot1"></param>
        /// <param name="slot2"></param>
        public void OnSlotsSwapped(SlotData slot1, SlotData slot2)
        {
            Debug.Log("public void OnSlotsSwapped(SlotData slot1, SlotData slot2)");
            if((slot1 is null) || (slot2 is null)) return;
            bool tmp = slot1.filled;
            slot1.filled = slot2.filled;
            slot2.filled = tmp;
            int cur1 = -1, cur2 = -1;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == slot1) cur1 = i;
                if (slots[i] == slot2) cur2 = i;
            }
            if (cur1 > -1) slots[cur1] = slot2;
            if (cur2 > -1) slots[cur2] = slot1;
        }


        public bool OnSlotEmptied(SlotData slot)
        {
            Debug.Log("public bool OnSlotEmptied(SlotData slot)");
            if(slot is null) return false;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == slot)
                {
                    slots[i].filled = false;
                    return true;
                }
            }
            return false;
        }


        public void CleanUpDuplicates(int slotNumber, SlotData slot)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if ((i != slotNumber) && (slots[i] == slot))
                {
                    slots[i].inventory = InvType.NONE;
                    slots[i].invSlot = -1;
                    slots[i].filled = false;
                }
            }
        }





        public bool RemoveEquiptSlot(int slot)
        {
            //Debug.Log("public bool RemoveEquiptSlot(int slot)");
            bool changed = false;
            for (int i = 0; i < slots.Length; i++)
            {
                if ((slots[i].inventory == InvType.EQUIPT) && (slots[i].invSlot == slot))
                {
                    slots[i].inventory = InvType.NONE;
                    slots[i].invSlot = -1;
                    slots[i].filled = false;
                    changed = true;
                }
            }
            return changed;
        }


        public void Clear()
        {
            for(int i = 0; i < slots.Length; i++)
            {                
                slots[i].inventory = InvType.NONE;
                slots[i].invSlot = -1;
                slots[i].filled = false;
            }
        }


        public void UnequipSpell()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].inventory == InvType.SPELLS)
                {
                    slots[i].inventory = InvType.NONE;
                    slots[i].invSlot = -1;
                    slots[i].filled = false;
                }
            }
        }





    }


}
