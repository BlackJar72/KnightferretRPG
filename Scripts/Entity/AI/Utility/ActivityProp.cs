using System.Runtime.CompilerServices;
using UnityEngine;


namespace kfutils.rpg
{

    public class ActivityProp : MonoBehaviour, IActivityObject
    {

        [SerializeField] ENeed theNeed;
        [Range(0.0f, 1.0f)][SerializeField] float desireabilityFactor = 1.0f;
        [SerializeField] float timeToDo;
        [SerializeField] Transform actorLocation;
        [SerializeField] bool shareable = false;

        public bool available = true;

        private float desireability;

        public ENeed TheNeed => theNeed;
        public float DesireabilityFactor => desireabilityFactor;
        public float TimeToDo => timeToDo;
        public Transform ActorLocation => actorLocation;
        public bool Shareable => shareable;



        public float GetUtility(ITalkerAI entity)
        {
            if (available || shareable)
            {
                desireability = desireabilityFactor + Mathf.Sqrt(desireabilityFactor / (timeToDo + 60f) + 1) - 1;
                desireability *= entity.GetNeed(theNeed).GetDrive();
                desireability /= Mathf.Sqrt((entity.GetTransform.position - actorLocation.position).magnitude) + 3;
                return desireability;
            }
            else
            {
                return 0.0f;
            }
        }





    }



}