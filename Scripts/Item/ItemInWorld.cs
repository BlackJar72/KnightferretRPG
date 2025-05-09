using System;
using System.Collections;
using UnityEngine;


namespace kfutils.rpg {
    
    public class ItemInWorld : MonoBehaviour, IInteractable, ISerializationCallbackReceiver {
        
        [SerializeField] string id;
        public string ID => id;
        
        [SerializeField] ItemPrototype prototype;

        [SerializeField] Rigidbody physics;

        public ItemPrototype Prototype => prototype;

        private ChunkManager chunk;


        public void Use(GameObject from) {
            Inventory toInv = from.GetComponent<Inventory>();
            if(toInv != null) {
                if(prototype.IsStackable) toInv.AddToFirstEmptySlot(new ItemStack(prototype, 1, -1));
                else toInv.AddToFirstEmptySlot(new ItemStack(prototype, 1, -1, id));
                InventoryManagement.SignalInventoryUpdate(toInv);
                Destroy(gameObject);
            }
        }


        public ItemInWorld Spawn(Vector3 where) {
            ItemInWorld spawned = Instantiate(this);
            spawned.id = prototype.ID + Guid.NewGuid();
            spawned.gameObject.transform.position = where;
            chunk = WorldManagement.WorldLogic.GetChunk(where);
            return spawned;
        }


        public ItemInWorld SpawnWithID(string id, Vector3 where) {
            ItemInWorld spawned = Instantiate(this);
            spawned.gameObject.transform.position = where;
            chunk = WorldManagement.WorldLogic.GetChunk(where);
            spawned.id = id;
            return spawned;
        }


        public void EnablePhysics() {
            physics.isKinematic = false;
            StartCoroutine(PhysicsTimer());
            SendToCorrectChunk();
        }


        private void SendToCorrectChunk() {
            ChunkManager chunk = WorldManagement.GetChunkFromTransform(transform);
            if(chunk != null) transform.SetParent(chunk.LooseItems);
        }


        public void DisablePhysics() {
            physics.isKinematic = true;
            SendToCorrectChunk();
        }


        IEnumerator PhysicsTimer() {
            yield return new WaitForSeconds(5.0f);
            do {
                yield return new WaitForSeconds(1.0f);
            } while((physics.linearVelocity.magnitude > 0.01f) && (physics.angularVelocity.magnitude > 0.01f));
            physics.isKinematic = true;
        }


        public void ApplyImpulseForce(Vector3 force) {
            if(physics.isKinematic) return;
            physics.AddForce(force, ForceMode.Impulse);            
        }


        public void OnBeforeSerialize() {
            if((prototype != null) && string.IsNullOrEmpty(id)) id = prototype.ID + Guid.NewGuid();
        }
        public void OnAfterDeserialize() {/*Do Nothing*/}



    }


}
