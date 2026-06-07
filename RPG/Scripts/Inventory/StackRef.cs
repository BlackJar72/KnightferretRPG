using UnityEngine;


namespace kfutils.rpg {


    public class StackRef
    {
        private IInventoryResident reference; // FIXME: This should be an interface, so spells can also be held!
        private int localSlot;

        public IInventoryResident Item => reference;
        public int Slot { get => localSlot; set => localSlot = value; }
        public int RemoteSlot => reference.SlotNumber;
        public int StackSize { get => reference.StackSize; set => reference.StackSize = value; } 

        public void Assign(IInventoryResident item)
        {
            item?.UnasignRef(this);
            reference = item;
            reference.BeAssignedRef(this);
        }


        public void ClearRef() 
        {
            reference?.UnasignRef(this);
            reference = null;
        }


        public void CoAssign(StackRef item)
        {
            item.Assign(item.Item);
            // FIXME??? 
        } 


        
        
    }


}