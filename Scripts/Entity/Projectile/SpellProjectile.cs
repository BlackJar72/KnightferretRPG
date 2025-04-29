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


        protected virtual void Update() {
            if((startPos - gameObject.transform.position).sqrMagnitude > sqrRange) {
                if(impactPrefab != null) {
                    GameObject impact;
                    impact = Instantiate(impactPrefab, transform);
                    impact.transform.SetParent(impact.transform.parent.root);
                }
                Destroy(gameObject);
            }
        }

    }

}
