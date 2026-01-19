using System;
using UnityEngine;

namespace kfutils.rpg {

    public class ThrownWeapon : Projectile
    {
        [SerializeField] ItemPrototype item;
        [SerializeField] Vector3 spin;

        private bool hasDropped = false;

        public virtual void Launch(ICombatant sender, Vector3 direction) {
            this.sender = sender;
            Vector3 pos = rb.transform.position;
            rb.transform.position.Set(spin.x, spin.y, spin.z);
            rb.angularVelocity = new(speed, 0, 0);
            rb.linearVelocity = (direction * speed) + (Vector3.up * speed * 0.1f);
        }


        public virtual void SetItem(ItemPrototype prototype)
        {
            item = prototype;
        }


        protected override void OnCollisionEnter(Collision collision) {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if((damageable != null) && damageable.IsSpecifiedIdentity(sender)) return;
            if(!hasDropped) item.DropItemInWorld(transform, 0);
            hasDropped = true;
            base.OnCollisionEnter(collision);
        }


    }

}
