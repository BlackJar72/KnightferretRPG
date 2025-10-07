using UnityEngine;


namespace kfutils.rpg
{

    public interface IActivityProp : IHaveUseLocation 
    {
        public bool Available { get; set; }
    }


}
