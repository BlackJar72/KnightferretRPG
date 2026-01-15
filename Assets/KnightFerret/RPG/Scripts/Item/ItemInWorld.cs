using System;
using System.Collections;
using UnityEngine;


namespace kfutils.rpg {
    
    public class ItemInWorld : MonoBehaviour, IInteractable {
        
        [SerializeField] string id;
        public string ID => id;
        
        [SerializeField] ItemPrototype prototype;

        [SerializeField] Rigidbody physics;

        public ItemPrototype Prototype => prototype;

        public ChunkManager chunk;


        public void Use(GameObject from) {
            Inventory toInv = from.GetComponent<Inventory>();
            if(toInv != null) {
                if(prototype.IsStackable) {
                    ItemStack stack = new ItemStack(prototype, 1, -1);
                    toInv.AddToFirstEmptySlot(stack);
                    ItemManagement.itemRegistry.Remove(id);
                    ItemManagement.AddItem(new ItemData(stack));
                }
                else toInv.AddToFirstEmptySlot(new ItemStack(prototype, 1, -1, id));
                InventoryManagement.SignalInventoryUpdate(toInv);
                chunk.Data.ItemsInChunkList.Remove(id);
                Destroy(gameObject);
            }
        }


        public void SetID(string ID) {
            id = ID;
        }


        public ItemInWorld Spawn(Vector3 where) {
            ItemInWorld spawned = Instantiate(this);
            spawned.id = prototype.ID + Guid.NewGuid();
            spawned.gameObject.transform.position = where;
            chunk = WorldManagement.WorldLogic.GetChunk(where);
            spawned.SendToCorrectChunk();
            spawned.chunk.Data.AddItem(spawned.id);
            if(ItemManagement.GetItem(spawned.id) == null) ItemManagement.AddItem(new ItemData(spawned));
            return spawned;
        }


        public ItemInWorld SpawnWithID(string id, Vector3 where) {
            ItemInWorld spawned = Instantiate(this);
            spawned.gameObject.transform.position = where;
            chunk = WorldManagement.WorldLogic.GetChunk(where);
            spawned.id = id;
            spawned.SendToCorrectChunk();
            spawned.chunk.Data.AddItem(id);
            if(ItemManagement.GetItem(id) == null) ItemManagement.AddItem(new ItemData(spawned));
            return spawned;
        }


        public void EnablePhysics() {
            ItemData data = ItemManagement.GetItem(id);
            if(data != null) data.physics = true;
            physics.isKinematic = false;
            StartCoroutine(PhysicsTimer());
            SendToCorrectChunk();
        }


        private void SendToCorrectChunk() {
            ChunkManager newchunk = WorldManagement.GetChunkFromTransform(transform);
            if(newchunk != null) transform.SetParent(newchunk.LooseItems);
            if(chunk != newchunk) {
                if(chunk != null) chunk.Data.ItemsInChunkList.Remove(id);
                chunk = newchunk;
                chunk.Data.AddItem(id);
            }
            ItemData data = ItemManagement.GetItem(id);
            if(data != null) data.transformData = transform.GetGlobalData();
        }


        public void DisablePhysics() {
            ItemData data = ItemManagement.GetItem(id);
            if(data != null) data.physics = false;
            physics.isKinematic = true;
            SendToCorrectChunk();
        }


        IEnumerator PhysicsTimer() {
            yield return new WaitForSeconds(5.0f);
            do {
                SendToCorrectChunk();
                yield return new WaitForSeconds(1.0f);
            } while((physics.linearVelocity.magnitude > 0.01f) && (physics.angularVelocity.magnitude > 0.01f));
            DisablePhysics();
        }


        public void ApplyImpulseForce(Vector3 force) {
            if(physics.isKinematic) return;
            physics.AddForce(force, ForceMode.Impulse);            
        }


        [ContextMenu("Create ID")]
        public void OnBeforeSerialize() {
            if((prototype != null) && string.IsNullOrEmpty(id)) id = prototype.ID + Guid.NewGuid();
        }
        public void OnAfterDeserialize() {/*Do Nothing*/}



    }


}
