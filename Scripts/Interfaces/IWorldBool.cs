using UnityEngine;

namespace kfutils.rpg {


    public interface IWorldBool : IHaveStringID
    {
        public bool GetWorldState { get; }
    }

}
