using System;
using UnityEngine;



namespace kfutils.rpg {


    
    [System.Serializable]
    public class HotBarData
    {
        public ui.HotbarRefData[] data;
        public HotBarData() {}
        public HotBarData(HotBar bar)
        {
            data = new ui.HotbarRefData[bar.Slots.Length];
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = new(bar.Slots[i]);
            }
        }
    }


    [System.Serializable]
    public class HotBar
    {

        [SerializeField] ui.HotbarReference[] slots = new ui.HotbarReference[9];


        public ui.HotbarReference[] Slots => slots;


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


        public HotBarData GetData() => new(this);


        public void ReconstructFromData(HotBarData data)
        {
            if(data.data.Length != slots.Length) throw 
                new SystemException("Size of saved hotbar did not match size of game hotbar!");
            int numslots = Mathf.Min(data.data.Length, slots.Length);
            for(int i = 0; i < numslots; i++)
            {
                switch(data.data[i].inventory)
                {
                    case ui.InventoryType.INVALID:
                            slots[i].ClearReferant();
                            Debug.LogError("Hotbar slot " + i + " contained invalid data in save file.");
                        break;
                    case ui.InventoryType.EMPTY:
                            slots[i].ClearReferant();
                        break;
                    case ui.InventoryType.MAIN:
                            slots[i].SetReferant(EntityManagement.playerCharacter
                                    .Inventory.GetItemInSlot(data.data[i].slot));
                        break;
                    case ui.InventoryType.EQUIPT:
                            slots[i].SetReferant(EntityManagement.playerCharacter
                                    .Inventory.Equipt.GetItemInSlot(data.data[i].slot));
                        break;
                    case ui.InventoryType.SPELL:
                            // TODO!
                        break;
                    default:
                            slots[i].ClearReferant();
                            Debug.LogError("Hotbar slot " + i + " contained invalid data in save file.");
                        break;
                }
            }
        }


    }


}
