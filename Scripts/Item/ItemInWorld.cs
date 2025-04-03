using System.Collections;
using UnityEngine;


namespace kfutils.rpg {
    
    public class ItemInWorld : MonoBehaviour, IInteractable {
        
        [SerializeField] ItemPrototype prototype;

        [SerializeField] Rigidbody physics;


        public void Use(GameObject from) {
            Inventory toInv = from.GetComponent<Inventory>();
            if(toInv != null) {
                toInv.AddToFirstEmptySlot(new ItemStack(prototype, 1, -1));
                InventoryManager.SignalInventoryUpdate(toInv);
                GameObject.Destroy(gameObject);
            }
        }


        public void EnablePhysics() {
            physics.isKinematic = false;
            StartCoroutine(PhysicsTimer());
        }


        public void DisablePhysics() {
            physics.isKinematic = true;
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



    }


}
