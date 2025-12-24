using UnityEngine;

namespace kfutils.rpg {

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour {

        [SerializeField] protected GameObject impactPrefab;
        [SerializeField] protected DamageSource damage;
        [SerializeField] protected float speed;
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected bool stickyImpact;

        protected ICombatant sender;
        protected bool impactSpawned = false;

        public float Speed => speed;
        public bool StickyImpact => stickyImpact;


        protected virtual void Awake() {
            if(rb == null) rb = gameObject.GetComponent<Rigidbody>();
        }


        public virtual void Launch(ICombatant sender, Vector3 direction) {
            this.sender = sender;
            if(sender is EntityLiving living)
            {            
                Physics.IgnoreCollision(GetComponent<Collider>(), living.GetComponent<Collider>());
            }            
            rb.linearVelocity = direction * speed;
        }


        protected virtual void OnCollisionEnter(Collision collision) {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable == sender) return;
            if((impactPrefab != null) && !impactSpawned) {
                impactSpawned = true;
                GameObject impact;
                if (stickyImpact)
                {
                    impact = Instantiate(impactPrefab, transform.position, transform.rotation,
                                collision.gameObject.transform);
                }
                else
                {
                    impact = Instantiate(impactPrefab, transform.position, transform.rotation,
                                WorldManagement.GetChunkFromTransform(transform).transform);
                }
                WorldEffect effect = impact.GetComponent<WorldEffect>();
                if(effect != null) effect.Create();
            }
            if((damageable != null) && (damage.BaseDamage > 0)) {
                damage.DoDamage(sender, null, damageable);
            }
            Destroy(gameObject);
        }



   }


}