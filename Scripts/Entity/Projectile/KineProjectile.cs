using UnityEngine;

namespace kfutils.rpg {

    [RequireComponent(typeof(Rigidbody))]
    public class KineProjectile : MonoBehaviour {

        [SerializeField] GameObject impactPrefab;
        Vector3 velocity;
        Rigidbody rb;


        void Start() {
            rb = GetComponent<Rigidbody>();
        }


        // Update is called once per frame
        void FixedUpdate() {
            rb.MovePosition(velocity);
        }


        void OnCollisionEnter(Collision collision) {
            GameObject impact = Instantiate(impactPrefab, transform);
            impact.transform.SetParent(impact.transform.parent);
            // TODO: Effect of impact (damage)
            Destroy(this);
        }



    }


}