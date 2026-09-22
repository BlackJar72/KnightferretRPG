using System;
using UnityEngine;



namespace kfutils.rpg {

    [System.Serializable]
    public class HotBar
    {

        [SerializeField] ui.HotbarReference[] slots = new ui.HotbarReference[9];


        public void ActivateSlot(int slot) => slots[slot].Activate();

        internal void Clear()
        {
            for(int i = 0; i < slots.Length; i++) slots[i].ClearReferant();
        }


        public void MarkSelected(IInvReferant referant)
        {
            for(int i = 0; i < slots.Length; i++)
            {
                if(slots[i].HoldsReferant(referant)) slots[i].SetSelected(true);
            }
        }


        public void MarkUnselected(IInvReferant referant)
        {
            for(int i = 0; i < slots.Length; i++)
            {
                if(slots[i].HoldsReferant(referant)) slots[i].SetSelected(false);
            }
        }


        public void CopyInto(HotBar other)
        {
            for(int i = 0; i < slots.Length; i++) slots[i].SetReferant(other.slots[i].Item);            
        }


    }


}
