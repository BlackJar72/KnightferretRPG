using UnityEngine;

namespace kfutils.rpg {

    [System.Serializable]
    public class ItemStack {

        [System.Serializable]
        public struct ProtoStack {
            public ItemPrototype item;
            public int stackSize;
            public ItemStack MakeStack(int slot = 0) => new ItemStack(item, stackSize, slot);
        }
    

        public ItemPrototype item;
        [SerializeField][HideInInspector] string id;
        public string ID => id;
        public int stackSize;
        public int slot; 

        
        public ItemStack(ItemPrototype item, int number, int slot = -1) {
            this.item = item;
            this.stackSize = number;
            this.slot = slot;
            if(item != null) id = item.ID + System.Guid.NewGuid();
            else id = "";
        }

        
        public ItemStack(ItemPrototype item, int number, int slot, string ID) {
            this.item = item;
            this.stackSize = number;
            this.slot = slot;
            id = ID;
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
            if(item.IsStackable) dropped = item.InWorld.Spawn();
            else dropped = item.InWorld.SpawnWithID(id);
            dropped.transform.position = where.position + (where.forward * distance);
            dropped.EnablePhysics();
            if(force == 0.0f) return dropped;
            dropped.ApplyImpulseForce(where.forward * force * Random.Range(0.95f, 1.05f));
            return dropped;
        }


    }


}
