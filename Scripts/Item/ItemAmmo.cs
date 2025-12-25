using UnityEngine;


namespace kfutils.rpg
{


    public class ItemAmmo : ItemConsumable, IHaveStringID
    {
        [SerializeField] string ammoTypeID;
        [SerializeField] protected DamageSource damage;
        [SerializeField][Min(0.0f)] protected float rangePentalty = 0.0f;
        [SerializeField] Projectile projectilePrefab;
        [SerializeField] GameObject imagePrefab;

        public GameObject ImagePrefab => imagePrefab;
        public float RangePentalty => rangePentalty;
        public Projectile ShotProjectile => projectilePrefab;

        public DamageSource Damage => damage;
        public string ID => ammoTypeID;

        public ref DamageSource GetDamageRef() => ref damage;


        public override void OnUse(IActor actor) {}
        public override void PlayUseAnimation(IActor actor) {}


        public virtual void OnUseAsAmmo(IActor user)
        {
            DecrimentSlot();
        }


    }

}
