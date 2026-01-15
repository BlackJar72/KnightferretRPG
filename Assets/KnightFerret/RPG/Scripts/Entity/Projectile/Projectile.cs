using UnityEngine;

namespace kfutils.rpg {

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour {

        [SerializeField] protected GameObject impactPrefab;
        [SerializeField] protected DamageSource damage;
        [SerializeField] protected float speed;
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected bool stickyImpact;

        private protected ICombatant sender;


        protected virtual void Awake() {
            if(rb == null) rb = gameObject.GetComponent<Rigidbody>();
        }


        public virtual void Launch(ICombatant sender, Vector3 direction) {
            this.sender = sender;
            rb.linearVelocity = direction * speed;
        }


        protected virtual void OnCollisionEnter(Collision collision) {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable == sender) return;
            if(impactPrefab != null) {
                if (stickyImpact)
                {
                    Instantiate(impactPrefab, transform.position, transform.rotation,
                                collision.gameObject.transform);
                }
                else
                {
                    Instantiate(impactPrefab, transform.position, transform.rotation,
                                WorldManagement.GetChunkFromTransform(transform).transform);
                }
            }
            if(damageable != null) {
                damage.DoDamage(sender, null, damageable);
            }
            Destroy(gameObject);
        }



   }


}