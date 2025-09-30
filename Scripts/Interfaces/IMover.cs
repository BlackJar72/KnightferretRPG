using UnityEngine;



namespace kfutils.rpg
{

    public interface IMover
    {
        public Transform GetTransform { get; }
        public bool AtLocation(Transform location);



    }


}
