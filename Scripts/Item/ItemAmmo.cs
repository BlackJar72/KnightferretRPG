using UnityEngine;


namespace kfutils.rpg
{


    public class ItemAmmo : ItemConsumable, IHaveStringID
    {
        [SerializeField] string ammoTypeID;
        [SerializeField] protected DamageSource damage;
        [SerializeField][Min(0.0f)] protected float rangePentalty = 0.0f;

        public float RangePentalty => rangePentalty;


        public DamageSource Damage => damage;
        public string ID => ammoTypeID;


        public override void OnUse(IActor actor)
        {
            // TODO!
            throw new System.NotImplementedException();
        }


        public override void PlayUseAnimation(IActor actor)
        {
            // TODO!
            throw new System.NotImplementedException();
        }


    }

}
