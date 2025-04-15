using UnityEngine;

namespace kfutils.rpg {

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour {

        [SerializeField] GameObject impactPrefab;
        [SerializeField] DamageSource damage;
        [SerializeField] float speed;
        [SerializeField] Rigidbody rb;

        private IAttacker sender;


        void Awake() {
            if(rb == null) rb = gameObject.GetComponent<Rigidbody>();
        }


        public void Launch(IAttacker sender, Vector3 direction) {
            this.sender = sender;
            rb.linearVelocity = direction * speed;
        }


        void OnCollisionEnter(Collision collision) {
            GameObject impact;
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if(impactPrefab != null && (damageable != sender)) { 
                impact = Instantiate(impactPrefab, transform);
                impact.transform.SetParent(impact.transform.parent);
            }
            if((damageable != null) && (damageable != sender)) {
                damage.DoDamage(sender, damageable);
            }
            if(damageable != sender) Destroy(gameObject);
        }



   }


}