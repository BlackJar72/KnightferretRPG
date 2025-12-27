using UnityEngine;

namespace kfutils.rpg {

    public class ArrowProjectile : Projectile
    {
        [Tooltip ("Multiplied by speed to determine impact damage; "
                 + "If arrows are slow (less than 30.0) this should be 1.0f. "
                 + "If arrows are fast (greater than 40.0f) this should be 0.5f (or similar). "
                 + "(This refers to what is typical for the game. )")]
        [SerializeField] float speedDamageFactor = 0.5f; 


        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }


        public ref DamageSource GetDamage() => ref damage;


        public void ApplyBowDamage(DamageSource bowDamage)
        {
            damage = bowDamage;
        }


        private void FixedUpdate()
        {
            transform.LookAt(transform.position + rb.linearVelocity);
        }


        protected override void OnCollisionEnter(Collision collision) {
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
                damage.SetBaseDamage(damage.BaseDamage + Mathf.RoundToInt(rb.linearVelocity.magnitude * speedDamageFactor));
                damage.DoDamage(sender, null, damageable);
            }
            Destroy(gameObject);
        }
    }

}