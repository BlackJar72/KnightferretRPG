using UnityEngine;

namespace kfutils.rpg {

    [RequireComponent(typeof(Rigidbody))]
    public class KineProjectile : MonoBehaviour {

        [SerializeField] GameObject impactPrefab;
        [SerializeField] DamageSource damage;
        [SerializeField] float speed;

        private ICombatant sender;
        private Vector3 velocity;
        private Rigidbody rb;


        void Start() {
            rb = GetComponent<Rigidbody>();
        }


        public void Launch(ICombatant sender, Vector3 direction) {
            this.sender = sender;
            this.velocity = direction * speed;
        }


        // Update is called once per frame
        void FixedUpdate() {
            rb.MovePosition(velocity);
        }


        void OnCollisionEnter(Collision collision) {
            GameObject impact;
            if(impactPrefab != null) { 
                impact = Instantiate(impactPrefab, transform);
                impact.transform.SetParent(impact.transform.parent);
            }
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if(damageable != null) {
                damage.DoDamage(sender, null, damageable);
            }
            Destroy(this);
        }



    }


}