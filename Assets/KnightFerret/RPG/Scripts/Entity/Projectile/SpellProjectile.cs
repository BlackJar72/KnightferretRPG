using System;
using UnityEngine;


namespace kfutils.rpg {

    [RequireComponent(typeof(Rigidbody))]
    public class SpellProjectile : Projectile {
    
        protected Vector3 startPos;
        protected float sqrRange;


        public void SetRange(float range, Vector3 start) {
            sqrRange = range * range;
            startPos = start;
        }


        protected virtual void Update()
        {
            if ((startPos - gameObject.transform.position).sqrMagnitude > sqrRange)
            {
                if (impactPrefab != null)
                {
                    Instantiate(impactPrefab, transform.position, transform.rotation,
                                WorldManagement.GetChunkFromTransform(transform).transform);
                }
                Destroy(gameObject);
            }
        }


        protected override void OnCollisionEnter(Collision collision) {
            Debug.Log(collision.gameObject.name + " was hit!");
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if((damageable != null) && damageable.IsSpecifiedIdentity(sender)) return;
            base.OnCollisionEnter(collision);
        }




    }

}
