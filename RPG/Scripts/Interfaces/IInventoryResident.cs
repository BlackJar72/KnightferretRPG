using System;
using UnityEngine;
using UnityEngine.UI;



namespace kfutils.rpg {

    public interface IInventoryResident : IHaveStringID 
    {
        public Sprite Icon { get; }
        public int SlotNumber { get; }
        public int StackSize { get; set; }
        public EEquiptSlot EquiptType { get; }
        public float Weight { get; }
        public bool IsStackable { get; }
        


        public void BeAssignedRef(StackRef stackRef);
        public void UnasignRef(StackRef stackRef);
        public void Activate();
        
        
    }


}