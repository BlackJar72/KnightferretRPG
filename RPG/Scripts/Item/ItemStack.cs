using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace kfutils.rpg {

    [System.Serializable]
    public class ItemStack : IInventoryResident {

        [System.Serializable]
        public class ProtoStack {
            public ItemData item;
            public int stackSize;
            public bool equipt = false;
            public ItemStack MakeStack(int slot = 0)
            {
                ItemManagement.AddItem(item);
                return new ItemStack(item.Prototype, stackSize, slot, item.ID);
            }
        }


        public ItemPrototype item;
        [SerializeField] string id;
        public int stackSize;
        public int slot; 
        
        public string ID => id;

        public Sprite Icon => item.Icon;

        public int SlotNumber => slot;
        public int StackSize { get => stackSize; set => stackSize = value; }
        public EEquiptSlot EquiptType => item.EquiptType;
        public float Weight => item.Weight;
        public bool IsStackable => item.IsStackable;

        public List<StackRef> refs;

        
        public ItemStack(ItemPrototype item, int number, int slot = -1) {
            this.item = item;
            this.stackSize = number;
            this.slot = slot;
            if(item != null) id = item.ID + System.Guid.NewGuid();
            else id = "";
            refs = new();
        }

        
        public ItemStack(ItemPrototype item, int number, int slot, string ID) {
            this.item = item;
            this.stackSize = number;
            this.slot = slot;
            id = ID;
            refs = new();
        }


        public ItemStack Copy() {
            return new ItemStack(item, stackSize, slot, id);
        }


        public override string ToString() {
            if(item == null) return  "[NULL (item): " + "; stackSize = " + stackSize + "; slot = " + slot + "] ";
            else return "[Item: " + item.ID + ", name = " + item.Name + "; stackSize = " + stackSize + "; slot = " + slot + "] ";
        }


        public bool CanEquipt() {
            return (item != null) && (item.EquiptItem != null);
        }


        public ItemInWorld DropItemInWorld(Transform where, float distance, float force = 0.0f) {
            ItemInWorld dropped;
            if(item.IsStackable) dropped = item.InWorld.Spawn(where.position + (where.forward * distance));
            else dropped = item.InWorld.SpawnWithID(id, where.position + (where.forward * distance));
            dropped.EnablePhysics();
            if(force == 0.0f) return dropped;
            dropped.ApplyImpulseForce(where.forward * force * Random.Range(0.95f, 1.05f));
            return dropped;
        }



        //-----------------------------------------------------------------------------------------------------------//
        //                                       UTILITY AI SUPPORT                                                  //
        //-----------------------------------------------------------------------------------------------------------//


        public ActivityHolder GetItemUtility(EObjectActivity activityType, ITalkerAI ai)
        {
            return new ActivityHolder(item.Activity, item.GetItemUtility(activityType, ai), this);
        }



        //-----------------------------------------------------------------------------------------------------------//
        //                                      STACK REFENCE SUPPORT                                                //
        //-----------------------------------------------------------------------------------------------------------//


        public void BeAssignedRef(StackRef reference)
        {
            if(!refs.Contains(reference)) refs.Add(reference);
        } 


        public void UnasignRef(StackRef reference)
        {
            refs.Remove(reference);
        }


        /// <summary>
        /// Must be called with the item stack is removed from the inventory, whether by being moved to another inventory,  
        /// emptied and destroyed, or otherwise.  This will clean up its reference.
        /// </summary>
        public void BeRemoved()
        {
            foreach(StackRef reference in refs) reference.ClearRef();
            #if UNITY_EDITOR
            // Should I just log?  Or throw an exception?
            if(refs.Count > 0) Debug.LogError("References removed but list is not clear!  (" + item.ID + " : " + ID 
                                            + " of size " + stackSize + " in slot " + slot + ")");
            #endif
            refs.Clear();
        }


        /// <summary>
        /// When it is activated in the inventory UI (or by the hotbar);
        /// 
        /// This should generally EQUIP or UNEQUIP the item.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Activate()
        {
            throw new System.NotImplementedException();
        }


    }


}
