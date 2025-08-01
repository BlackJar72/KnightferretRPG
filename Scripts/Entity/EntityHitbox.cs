using UnityEngine;


namespace kfutils.rpg
{

    [RequireComponent(typeof(Rigidbody))]
    public class EntityHitbox : MonoBehaviour, IDamageable, IHittable
    {

        [Tooltip("The entity to which damage should be forwarded")]
        [SerializeField] EntityLiving owner;


        public int GetArmor() => owner.GetArmor();
        public void TakeDamage(Damages damage) => owner.TakeDamage(damage);
        public void TakeDamage(DamageData damage) => owner.TakeDamage(damage);
        public Collider GetCollider() => GetComponent<Collider>();
        public Vector3 GetCenter() => GetComponent<Collider>().bounds.center;
        public EntityLiving GetEntity => owner;

        

        

    }

}
