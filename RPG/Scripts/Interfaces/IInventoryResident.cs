using System;
using UnityEngine;
using UnityEngine.UI;



namespace kfutils.rpg {

    public interface IInventoryResident
    {
        public Sprite Icon { get; }
        public int SlotNumber { get; }


        public void BeAssignedRef(StackRef stackRef);
        public void UnasignRef(StackRef stackRef);
        public void Activate();
        
        
    }


}