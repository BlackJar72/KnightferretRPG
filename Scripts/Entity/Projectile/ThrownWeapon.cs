using System;
using UnityEngine;

namespace kfutils.rpg {

    public class ThrownWeapon : Projectile
    {
        [SerializeField] ItemPrototype item;
        

        private bool hasDropped = false;

        public override void Launch(ICombatant sender, Vector3 direction) {
            this.sender = sender;
            if(sender is EntityLiving living)
            {            
                Physics.IgnoreCollision(GetComponent<Collider>(), living.GetComponent<Collider>());
                rb.linearVelocity = direction * (speed - 10 + living.attributes.baseStats.Strength);
            }
            else rb.linearVelocity = direction * speed;
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
