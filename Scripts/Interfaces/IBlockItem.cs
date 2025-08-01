using UnityEngine;


namespace kfutils.rpg
{

    public interface IBlockItem : IUsable
    {

        public void StartBlock();
        public void EndBlock();

        public float BlockAmount { get; }
        public float Stability { get; }
        public float ParryWindow { get; }

        public void BeHit();   


    }

}
