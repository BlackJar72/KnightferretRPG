using UnityEngine;

namespace kfutils.rpg {

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour {

        [SerializeField] protected GameObject impactPrefab;
        [SerializeField] protected DamageSource damage;
        [SerializeField] protected float speed;
        [SerializeField] protected Rigidbody rb;

        private ICombatant sender;


        protected virtual void Awake() {
            if(rb == null) rb = gameObject.GetComponent<Rigidbody>();
        }


        public virtual void Launch(ICombatant sender, Vector3 direction) {
            this.sender = sender;
            rb.linearVelocity = direction * speed;
        }


        protected virtual void OnCollisionEnter(Collision collision) {
            GameObject impact;
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if(impactPrefab != null) {
                impact = Instantiate(impactPrefab, transform);
                impact.transform.SetParent(impact.transform.parent.root);
            }
            if((damageable != null) && (damageable != sender)) {
                damage.DoDamage(sender, damageable);
            }
            if(damageable != sender) Destroy(gameObject);
        }



   }


}