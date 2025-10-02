using UnityEngine;



namespace kfutils.rpg
{

    public interface IMover
    {
        public Transform GetTransform { get; }
        public bool AtLocation(Transform location);



    }


    public interface IMoverAI : IMover
    {
        public void SetDestination(Vector3 to, float stopDist = 0.0f);



    }


}
