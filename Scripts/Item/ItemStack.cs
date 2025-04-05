using System;
using UnityEngine;

namespace kfutils.rpg {

    [Serializable]
    public class ItemStack {

        [Serializable]
        public struct ProtoStack {
            public ItemPrototype item;
            public int stackSize;
            public ItemStack MakeStack(int slot = 0) => new ItemStack(item, stackSize, slot);
        }
    

        public ItemPrototype item;
        public int stackSize;

        public int slot; 

        
        public ItemStack(ItemPrototype item, int number, int slot = -1) {
            this.item = item;
            this.stackSize = number;
            this.slot = slot;
        }


        public ItemStack Copy() {
            return new ItemStack(item, stackSize, slot);
        }


        public ItemStack CopyInto(ItemStack other) {
            other.item = item;
            other.stackSize = stackSize;
            other.slot = slot;
            return other;
        }


        public override string ToString() {
            if(item == null) return  "[NULL (item): " + "; stackSize = " + stackSize + "; slot = " + slot + "] ";
            else return "[Item: " + item.ID + ", name = " + item.Name + "; stackSize = " + stackSize + "; slot = " + slot + "] ";
        }


    }


}
