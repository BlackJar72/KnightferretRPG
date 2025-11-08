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
        public void TakeDamageOverTime(Damages damage) => owner.TakeDamageOverTime(damage);
        public void TakeDamageOverTime(DamageData damage) => owner.TakeDamageOverTime(damage);
        public void TakeShockOverTime(Damages damage) => owner.TakeShockOverTime(damage);
        public void TakeShockOverTime(DamageData damage) => owner.TakeShockOverTime(damage);
        public Collider GetCollider() => GetComponent<Collider>();
        public Vector3 GetCenter() => GetComponent<Collider>().bounds.center;
        public EntityLiving GetEntity => owner;
        public bool IsStunned() => owner.IsStunned();
        public bool InParriedState() => owner.InParriedState();
        public void SetParried(bool parried = true) => owner.SetParried(parried);
        public bool IsSurprised(ICombatant attacker) => owner.IsSurprised(attacker);
        public Damages ApplyDamageAdjustment(Damages damage) => owner.ApplyDamageAdjustment(damage);
        public void HealDamage(float amount) => owner.HealDamage(amount);
        
     }

}
