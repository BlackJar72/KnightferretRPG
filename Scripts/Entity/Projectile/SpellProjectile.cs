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


        /*protected override void OnCollisionEnter(Collision collision)
        {
                Debug.Log(collision.gameObject.name);
                Debug.Log(((startPos - gameObject.transform.position).sqrMagnitude) 
                        + " (" + ((startPos - gameObject.transform.position).magnitude) + ") "
                        + " > " + sqrRange + " (" + Mathf.Sqrt(sqrRange) + ") ");
            base.OnCollisionEnter(collision);
        }*/




    }

}
