using UnityEngine;


namespace kfutils.rpg
{


    public class ItemAmmo : ItemConsumable
    {
        [SerializeField] protected DamageSource damage;


        public DamageSource Damage => damage;


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
