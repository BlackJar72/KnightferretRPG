using System;
using UnityEngine;

namespace kfutils.rpg {

    public class ThrownItem : Projectile
    {
        [SerializeField] protected ItemPrototype item;
        [SerializeField] protected float strengthFactor = 1.0f;
        

        private bool hasDropped = false;

        public override void Launch(ICombatant sender, Vector3 direction) {
            this.sender = sender;
            if(sender is EntityLiving living)
            {            
                Physics.IgnoreCollision(GetComponent<Collider>(), living.GetComponent<Collider>());
                rb.linearVelocity = direction * Mathf.Max((speed 
                        + ((living.attributes.baseStats.Strength - EntityBaseStats.DEFAULT_SCORE) * strengthFactor)), speed / 2.0f);
            }
            else rb.linearVelocity = direction * speed;
        }


        public virtual void SetItem(ItemPrototype prototype)
        {
            item = prototype;
        }


        protected override void OnCollisionEnter(Collision collision) {
            if((!hasDropped) && (item != null)) item.DropItemInWorld(transform, 0);
            hasDropped = true;
            base.OnCollisionEnter(collision);
        }


    }

}
