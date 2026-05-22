using UnityEngine;


namespace kfutils.rpg {


    public class StackRef
    {
        private IInventoryResident reference; // FIXME: This should be an interface, so spells can also be held!


        public IInventoryResident Item => reference;


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


        
        
    }


}